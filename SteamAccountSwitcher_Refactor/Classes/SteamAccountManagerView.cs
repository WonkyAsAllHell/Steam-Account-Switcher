using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml;
using WonkyUtils;

namespace SteamAccountSwitcher_Refactor
{
	public class SteamAccountView : INotifyPropertyChanged
	{
		public string AvatarUri {
			get => _avatarUri;
			set {
				_avatarUri = value;
				NotifyPropertyChanged();
			} 
		}
		
		public string SteamID { get; set; }
		public string PersonaName { get; set; }
		public int MostRecent { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		private string _avatarUri;
		private string _cacheDir = Path.Join(WorkingPath.GetWorkingPath("SteamAccountSwitcher", WorkingPathOpt.user), @"Cache\");
		private string _hashFile = Path.Join(WorkingPath.GetWorkingPath("SteamAccountSwitcher", WorkingPathOpt.user), @"hash.json");
		private Dictionary<string, byte[]> hashes;

		public SteamAccountView(string SteamID, string PersonaName, int MostRecent)
		{
			this.SteamID = SteamID;
			this.PersonaName = PersonaName;
			this.MostRecent = MostRecent;

			CreateFolders();
			UpdateProfilePicture();

			string jsonString = JsonConvert.SerializeObject(hashes);
			File.WriteAllText(_hashFile, jsonString);
		}

		private void CreateFolders()
		{
			if(!Directory.Exists(_cacheDir))
				Directory.CreateDirectory(_cacheDir);

			if(!File.Exists(_hashFile))
				File.WriteAllText(_hashFile, "");

			hashes = JsonConvert.DeserializeObject<Dictionary<string, byte[]>>(File.ReadAllText(_hashFile));
		}

		private void UpdateProfilePicture()
		{
			DirectoryInfo cacheInfo = new DirectoryInfo(_cacheDir);

			try { AvatarUri = cacheInfo.GetFiles($"{SteamID}*.*").OrderByDescending(f => f.LastWriteTime).First().FullName; }
			catch (InvalidOperationException) { DownloadImageAsync(); }

			if(string.IsNullOrEmpty(AvatarUri))
				return;

			TimeSpan fileAge = DateTime.Now - File.GetLastWriteTime(AvatarUri);

			if(fileAge.Days > 1)
				DownloadImageAsync();
		}

		private async void DownloadImageAsync()
		{
			string filePath = await Task.Run(() => {
				XmlDocument profileXml = new XmlDocument();
				profileXml.Load($"https://steamcommunity.com/profiles/{SteamID}?xml=1");

				if(profileXml.DocumentElement == null)
					return "";

				XmlNode urlXml = profileXml.DocumentElement.SelectSingleNode("/profile/avatarFull");

				if(urlXml == null)
					return "";

				string url = profileXml.DocumentElement.SelectSingleNode("/profile/avatarFull").InnerText;

				//create filename
				int counter = 0;
				string ext = Path.GetExtension(url);
				string filename = SteamID + ext;
				string fullpath = Path.Join(_cacheDir, filename);

				while(File.Exists(fullpath))
					fullpath = Path.Join(_cacheDir, $"{SteamID}_{++counter}{ext}");

				using(WebClient client = new WebClient()) {
					client.DownloadFile(new Uri(url), fullpath);
				}

				//Calculate Hash
				byte[] fileData = File.ReadAllBytes(fullpath);
				byte[] newHash = MD5.Create().ComputeHash(fileData);

				if(hashes != null && hashes.ContainsKey(SteamID)) {
					if(hashes[SteamID] == newHash) {
						File.Delete(fullpath);
						return "";
					}
				} else {
					hashes = new Dictionary<string, byte[]>();
				}

				hashes[SteamID] = newHash;

				return fullpath;
			});

			if(filePath != "")
				AvatarUri = filePath;
		}

		protected void NotifyPropertyChanged([CallerMemberName] string propertyname = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
		}
	}

	public class SteamAccountManagerView
	{
		public ObservableCollection<SteamAccountView> Children { get; set; }

		private SteamAccountManager _steamAccountManager;

		public SteamAccountManagerView()
		{
			_steamAccountManager = new SteamAccountManager();
			Children = new ObservableCollection<SteamAccountView>();

			foreach(KeyValuePair<string, SteamAccountVDF> steamAccountPair in _steamAccountManager.SteamAccounts) {
				Children.Add(new SteamAccountView(steamAccountPair.Key, steamAccountPair.Value.PersonaName, steamAccountPair.Value.MostRecent));
			}

		}

		public void SwitchSteamAccount(string steamID)
		{
			_steamAccountManager.ChangeSteamAccount(steamID);
		}
	}
}

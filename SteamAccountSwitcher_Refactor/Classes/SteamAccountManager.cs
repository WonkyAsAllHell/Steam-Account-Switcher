using Gameloop.Vdf;
using Gameloop.Vdf.JsonConverter;
using Gameloop.Vdf.Linq;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using WonkyUtils;

namespace SteamAccountSwitcher_Refactor
{
	public class SteamAccountVDF
	{
		public string AccountName { get; set; }
		public string PersonaName { get; set; }
		public int RememberPassword { get; set; }
		public int MostRecent { get; set; }
		public int WantsOffline { get; set; }
		public string TimeStamp { get; set; }
	}

	public class SteamAccountManager
	{
		public Dictionary<string, SteamAccountVDF> SteamAccounts { get; set; }

		private string _loginUsersPath = @"C:\Program Files (x86)\Steam\config\loginusers.vdf";
		private string _workingPath = WorkingPath.GetWorkingPath("SteamAccountSwitcher", WorkingPathOpt.user);

		public SteamAccountManager()
		{
			GetData();
		}

		public void ChangeSteamAccount(string steamID)
		{
			KillSteam();
			UpdateLoginUsers(steamID);
			UpdateRegistry(steamID);
			StartSteam();
		}

		private void KillSteam()
		{
			foreach(Process process in Process.GetProcessesByName("steam"))
				process.Kill();
		}

		private void UpdateLoginUsers(string steamID)
		{
			BackupLoginUsers();

			foreach(KeyValuePair<string, SteamAccountVDF> steamAccount in SteamAccounts) {
				steamAccount.Value.MostRecent = steamAccount.Key == steamID ? 1 : 0;
				steamAccount.Value.RememberPassword = 1;
				steamAccount.Value.TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
			}

			File.WriteAllText(_loginUsersPath, @"""users""" + Environment.NewLine + JObject.FromObject(SteamAccounts).ToVdf());
		}

		private void BackupLoginUsers()
		{
			int counter = 0;
			string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");

			string filename = "loginusers" + timestamp;
			string fullPath = Path.Join(_workingPath, $"{filename}.vdf");

			while(File.Exists(fullPath))
				fullPath = Path.Join(_workingPath, $"{filename}_{++counter}.vdf");

			File.Copy(_loginUsersPath, fullPath);
		}

		private void UpdateRegistry(string steamID)
		{
			SteamAccountVDF steamAccount = SteamAccounts[steamID];
			RegistryKey steamRegistryKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Valve\Steam");

			if(steamRegistryKey != null) {
				steamRegistryKey.SetValue("AutoLoginUser", steamAccount.AccountName);
				steamRegistryKey.SetValue("LastGameNameUsed", steamAccount.PersonaName);
				steamRegistryKey.SetValue("RememberPassword", 1);
			}
		}

		private void StartSteam()
		{
			Process.Start(@"C:\Program Files (x86)\Steam\Steam.exe");
		}

		private void GetData()
		{
			VProperty loginUsersVDF = VdfConvert.Deserialize(File.ReadAllText(_loginUsersPath));
			SteamAccounts = loginUsersVDF.ToJson().First.ToObject<Dictionary<string, SteamAccountVDF>>();
		}
	}
}

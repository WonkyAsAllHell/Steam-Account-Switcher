using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WonkyUtils
{
	public enum WorkingPathOpt
	{
		user,
		global
	}

    public class WorkingPath
    {
		private static string _workingPath = "";

		public static string GetWorkingPath()
		{
			if (_workingPath != "")
				return _workingPath;

			string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			_workingPath = Path.Combine(appDataPath, @"WonkyApps\");

			if (!Directory.Exists(_workingPath))
				Directory.CreateDirectory(_workingPath);

			return _workingPath;
		}

		public static string GetWorkingPath(string appName, WorkingPathOpt option)
		{
			string parentDirectory;

			switch(option) {
				case WorkingPathOpt.user:
					parentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
					break;

				case WorkingPathOpt.global:
					parentDirectory = @"C:\Program Files\";
					break;

				default:
					parentDirectory = "";
					break;
			}

			string workingPath = Path.Combine(parentDirectory, @"WonkyApps\", appName);

			if(!Directory.Exists(workingPath))
				Directory.CreateDirectory(workingPath);

			return workingPath;
		}

		public static void SetWorkingPath()
        {
			Environment.CurrentDirectory = _workingPath;
        }

		public static void SetWorkingPath(string appName, WorkingPathOpt option)
		{
			_workingPath = GetWorkingPath(appName, option);
			Environment.CurrentDirectory = _workingPath;
		}
	}
}

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using IWshRuntimeLibrary;
using Microsoft.WindowsAPICodePack.Shell;
using File = System.IO.File;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace SaveDataRelocator {
	class Program {

		public static string LocalDirectory{
			get{
				return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			}
		}

		public static string Instructions =
@" -- SaveDataRelocator -- 
Copy data from local to remote location and then starts and application.
When the application shuts down the data will be copied back to local.


Usage:
	Place SaveDataRelocator.exe next to the game you are running.
	This is crucial since this folder will implicitly be the ""Local"" folder.

	drag and drop the game exe onto SaveDataRelocator.exe and you will be
	asked to pick a directory. Find the games save data directory.

	a new shortcut will be created for you, which
	you can copy to your desktop.


Try searching theese folders:
	AppData/Roaming
	AppData/Local
	AppData/LocalLow
	Documents/
	Documents/My Games


Command line arguments:

	[0] = Executebale to run
	[1...] = Remote location to manage data (you can input many parameters)
";

		[STAThread]
		private static void Main(string[] args){
			if (args.Length == 1){
				var exePath = args[0];
				if (File.Exists(exePath) == false)
					exePath += ".exe";
				CreateShortcut(exePath);
				return;
			}
			if (args.Length < 2){
				Console.WriteLine(Instructions);
				Console.ReadLine();
				return;
			}
			for (var i = 0; i < args.Length; i++)
				args[i] = Environment.ExpandEnvironmentVariables(args[i]);

			var startExe = args[0];

			if (File.Exists(startExe) == false)
				startExe = Path.Combine(LocalDirectory, startExe);

			if (File.Exists(startExe) == false)
				startExe += ".exe";

			if (File.Exists(startExe) == false) {
				Console.WriteLine("Executeable not found: " + startExe);
				Console.ReadLine();
				return;
			}
			
			for (var i = 1; i < args.Length; i++){
				var remoteDir = args[i];
				var localDir = Path.Combine(LocalDirectory, Path.GetFileName(remoteDir));
				Console.WriteLine(Directory.Exists(localDir) ? "Copying to remote directory" : "No local data found");

				if (Directory.Exists(remoteDir) && Directory.Exists(localDir))
					Directory.Delete(remoteDir, true);
				if (Directory.Exists(localDir))
					DirectoryCopy(localDir, remoteDir);
			}

			Console.WriteLine("\n\nStarting process");
			var process = new Process {
				StartInfo = {
					FileName = startExe,
					WorkingDirectory = LocalDirectory,
					Domain = LocalDirectory,
					UseShellExecute = true
				}
			};
			process.Start();
			process.WaitForExit();
			Console.WriteLine("Proccess exited\nCopying to local directory\n\n");

			for (var i = 1; i < args.Length; i ++) {
				var remoteDir = args[i];
				var localDir = Path.Combine(LocalDirectory, Path.GetFileName(remoteDir));
				if (Directory.Exists(localDir))
					Directory.Delete(localDir, true);
				DirectoryCopy(remoteDir, localDir);
				if (Directory.Exists(remoteDir))
					Directory.Delete(remoteDir, true);
			}
		}

		private static void CreateShortcut(string exePath) {
			var dialog = new CommonOpenFileDialog{
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				Multiselect = false,
				IsFolderPicker = true,
				Title = "Pick a folder",
			};
			var specialFolderPath = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\..");
			var documentsPath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents");
			dialog.AddPlace(LocalDirectory, FileDialogAddPlaceLocation.Top);
			dialog.AddPlace(specialFolderPath, FileDialogAddPlaceLocation.Top);
			dialog.AddPlace(Path.Combine(specialFolderPath, "Roaming"), FileDialogAddPlaceLocation.Top);
			dialog.AddPlace(Path.Combine(specialFolderPath, "Local"), FileDialogAddPlaceLocation.Top);
			dialog.AddPlace(Path.Combine(specialFolderPath, "LocalLow"), FileDialogAddPlaceLocation.Top);
			dialog.AddPlace(documentsPath, FileDialogAddPlaceLocation.Top);
			dialog.AddPlace(Path.Combine(documentsPath, "My Games"), FileDialogAddPlaceLocation.Top);
			string dialogFileName;
			if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
				dialogFileName = dialog.FileName;
			else return;

			var shell = new WshShell();
			var fullShortcutPath = Path.GetFileName(exePath);
			var shortcut = (IWshShortcut)shell.CreateShortcut(Path.GetFileNameWithoutExtension(exePath) + ".lnk");
			shortcut.IconLocation = Path.GetFullPath(exePath);
			shortcut.Description = "New shortcut for " + fullShortcutPath;
			shortcut.TargetPath = Assembly.GetExecutingAssembly().Location;
			shortcut.Arguments = string.Format("\"{0}\" \"{1}\" ", Path.GetFileName(exePath), dialogFileName.Replace(specialFolderPath, "%appdata%\\.."));
			shortcut.Save();
		}

		private static void DirectoryCopy(string sourceDirName, string destDirName, int depth = 1) {
			var dir = new DirectoryInfo(sourceDirName);
			if (dir.Exists == false)
				return;

			var dirs = dir.GetDirectories();
			Console.WriteLine(dir.Name.PadLeft(depth + dir.Name.Length, '\t') + "\\");
			if (Directory.Exists(destDirName) == false) {
				Directory.CreateDirectory(destDirName);
			}

			var files = dir.GetFiles();
			foreach (var file in files) {
				var temppath = Path.Combine(destDirName, file.Name);
				Console.WriteLine(file.Name.PadLeft(depth + file.Name.Length+1, '\t'));
				file.CopyTo(temppath, false);
			}

			foreach (var subdir in dirs) {
				var temppath = Path.Combine(destDirName, subdir.Name);
				DirectoryCopy(subdir.FullName, temppath, depth+1);
			}
		}
	}
}
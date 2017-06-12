using System;
using System.IO;
using System.Reflection;
using IWshRuntimeLibrary;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Shell;

namespace SaveDataRelocator {
	public static class ShortcutCreator {
		public static void CreateShortcut(string exePath) {
			var dialog = new CommonOpenFileDialog {
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				Multiselect = false,
				IsFolderPicker = true,
				Title = "Pick a folder",
			};
			var specialFolderPath = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\..");
			var userProfilePath = Environment.ExpandEnvironmentVariables("%userprofile%");
			var documentsPath = Path.Combine(userProfilePath, "Documents");

			Action<string> addPlace = p => {
				if (Directory.Exists(p))
					dialog.AddPlace(p, FileDialogAddPlaceLocation.Top);
			};

			addPlace(Globals.LocalDirectory);
			addPlace(specialFolderPath);
			addPlace(Path.Combine(specialFolderPath, "Roaming"));
			addPlace(Path.Combine(specialFolderPath, "Local"));
			addPlace(Path.Combine(specialFolderPath, "LocalLow"));
			addPlace(documentsPath);
			addPlace(Path.Combine(documentsPath, "My Games"));
			addPlace(Path.Combine(userProfilePath, "Saved Games"));

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
			shortcut.Arguments = string.Format("\"{0}\" \"{1}\" ", Path.GetFileName(exePath), dialogFileName
				.Replace(specialFolderPath, "%appdata%\\..")
				.Replace(userProfilePath, "%userprofile%")
			);
			shortcut.Save();
		}
	}
}

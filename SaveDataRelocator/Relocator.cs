using System;
using System.Diagnostics;
using System.IO;

namespace SaveDataRelocator {
	public class Relocator {

		public Relocator(string startExe, params string[] paths){
			_startExe = startExe;
			_paths = paths;
		}

		private const string LocalIdentifier = "_Relocated";

		private readonly string _startExe;
		private readonly string[] _paths;

		public void CopyToRemote(){
			foreach (var remoteDir in _paths){
				var localDir = Path.Combine(Globals.LocalDirectory, Path.GetFileName(remoteDir) + LocalIdentifier);
				Console.WriteLine(Directory.Exists(localDir) ? "Copying to remote directory" : "No local data found");

				if (Directory.Exists(remoteDir) && Directory.Exists(localDir))
					Directory.Delete(remoteDir, true);

				if (Directory.Exists(localDir))
					Utils.DirectoryCopy(localDir, remoteDir.Replace(LocalIdentifier, ""));
			}
		}

		public void CopyToLocal(){
			foreach (var remoteDir in _paths){
				var localDir = Path.Combine(Globals.LocalDirectory, Path.GetFileName(remoteDir) + LocalIdentifier);
				if (Directory.Exists(localDir))
					Directory.Delete(localDir, true);
				Utils.DirectoryCopy(remoteDir, localDir);
				if (Directory.Exists(remoteDir))
					Directory.Delete(remoteDir, true);
			}
		}

		public void WaitForProcess(){
			Console.WriteLine("\n\nStarting process");
			var process = new Process {
				StartInfo = {
					FileName = _startExe,
					WorkingDirectory = Globals.LocalDirectory,
					Domain = Globals.LocalDirectory,
					UseShellExecute = true
				}
			};
			process.Start();
			process.WaitForExit();
			Console.WriteLine("Proccess exited\nCopying to local directory\n\n");
		}
	}
}

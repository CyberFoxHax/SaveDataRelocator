using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using File = System.IO.File;

namespace SaveDataRelocator {
	class Program {
		[STAThread]
		private static void Main(string[] args){
			if (args.Length == 1){
				var exePath = args[0];
				if (File.Exists(exePath) == false)
					exePath += ".exe";
				ShortcutCreator.CreateShortcut(exePath);
				return;
			}
			if (args.Length < 2){
				Console.WriteLine(Globals.Instructions);
				Console.ReadLine();
				return;
			}
			for (var i = 0; i < args.Length; i++)
				args[i] = Environment.ExpandEnvironmentVariables(args[i]);

			var startExe = args[0];

			if (File.Exists(startExe) == false)
				startExe = Path.Combine(Globals.LocalDirectory, startExe);

			if (File.Exists(startExe) == false)
				startExe += ".exe";

			if (File.Exists(startExe) == false) {
				Console.WriteLine("Executeable not found: " + startExe);
				Console.ReadLine();
				return;
			}

			var relocator = new Relocator(startExe, args.Skip(1).ToArray());
			relocator.CopyToRemote();
			relocator.WaitForProcess();
			relocator.CopyToLocal();
		}
	}
}
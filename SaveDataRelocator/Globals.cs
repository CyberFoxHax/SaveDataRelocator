using System.IO;
using System.Reflection;

namespace SaveDataRelocator {
	public class Globals {

		public static string LocalDirectory {
			get {
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
	#User#/Saved Games


Command line arguments:

	[0] = Executebale to run
	[1...] = Remote location to manage data (you can input many parameters)
";
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveDataRelocator {
	public static class Utils {
		public static void DirectoryCopy(string sourceDirName, string destDirName, int depth = 1) {
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
				Console.WriteLine(file.Name.PadLeft(depth + file.Name.Length + 1, '\t'));
				file.CopyTo(temppath, false);
			}

			foreach (var subdir in dirs) {
				var temppath = Path.Combine(destDirName, subdir.Name);
				DirectoryCopy(subdir.FullName, temppath, depth + 1);
			}
		}
	}
}

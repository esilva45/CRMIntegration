using System;
using System.IO;

namespace CRMIntegration.Utils {
    class Util {

        public static string FindDirectory(string dnowner, string party_dn, string start_time) {
            string patch = Properties.Settings.Default.PathFile;
            string result = dnowner + "/file%20not%20found";

            try {
                DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(patch + dnowner);
                FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles("*" + dnowner + "*" + party_dn + "*" + start_time + "*.wav");

                foreach (FileInfo foundFile in filesInDir) {
                    result = dnowner + "/" + foundFile.Name.Replace(" ", "%20");
                }
            }
            catch (Exception ex) {
                Log(ex.ToString());
            }

            return result;
        }

        public static void VerifyDir(string path) {
            try {
                DirectoryInfo dir = new DirectoryInfo(path);

                if (!dir.Exists) {
                    dir.Create();
                }
            }
            catch { }
        }

        public static void Log(string lines) {
            string path = Properties.Settings.Default.PathLog;
            VerifyDir(path);

            string fileName = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "_Logs.txt";

            try {
                StreamWriter file = new StreamWriter(path + fileName, true);
                file.WriteLine(DateTime.Now.ToString() + ": " + lines);
                file.Close();
            }
            catch (Exception) { }
        }
    }
}

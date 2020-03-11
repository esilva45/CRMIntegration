using Npgsql;
using System;

namespace CRMIntegration.Utils {
    class Connection {
        private static NpgsqlConnection conn = null;

        private Connection() { }

        public static NpgsqlConnection GetConnection() {
            try {
                string DBHost = Properties.Settings.Default.DBHost;
                string DBPort = Properties.Settings.Default.DBPort;
                string DBUser = Properties.Settings.Default.DBUser;
                string DBPass = SecurePassword.Decrypt(Properties.Settings.Default.DBPass);
                string DBBase = Properties.Settings.Default.DBBase;
                string connString = "Server=" + DBHost + ";Port=" + DBPort + ";User Id=" + DBUser + "; Password=" + DBPass + "; Database=" + DBBase + ";";

                conn = new NpgsqlConnection(connString);
                conn.Open();
            }
            catch (Exception ex) {
                Util.Log(ex.ToString());
            }

            return conn;
        }

        public static void CloseConnection() {
            try {
                if (conn != null) {
                    conn.Close();
                }
            }
            catch (Exception ex) {
                Util.Log(ex.ToString());
            }
        }
    }
}

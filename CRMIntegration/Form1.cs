using CRMIntegration.Utils;
using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace CRMIntegration {
    public partial class Form1 : Form {
        System.Timers.Timer timer = new System.Timers.Timer(10 * 1000);

        public Form1() {
            InitializeComponent();

            AddApplicationToStartup();

            int check = 0;

            if (Properties.Settings.Default.DBBase != "") {
                txt_database.Text = Properties.Settings.Default.DBBase;
                check++;
            }

            if (Properties.Settings.Default.DBHost != "") {
                txt_host.Text = Properties.Settings.Default.DBHost;
                check++;
            }

            if (Properties.Settings.Default.DBPort != "") {
                txt_port.Text = Properties.Settings.Default.DBPort;
                check++;
            }

            if (Properties.Settings.Default.DBUser != "") {
                txt_user.Text = Properties.Settings.Default.DBUser;
                check++;
            }

            if (Properties.Settings.Default.DBPass != "") {
                txt_pass.Text = SecurePassword.Decrypt(Properties.Settings.Default.DBPass);
                check++;
            }

            if (Properties.Settings.Default.UrlLigacao != "") {
                txt_urlfile.Text = Properties.Settings.Default.UrlLigacao;
                check++;
            }

            if (Properties.Settings.Default.PathLog != "") {
                txt_log.Text = Properties.Settings.Default.PathLog;
                check++;
            }

            if (Properties.Settings.Default.PathFile != "") {
                txt_dirfile.Text = Properties.Settings.Default.PathFile;
                check++;
            }

            if (Properties.Settings.Default.UrlService != "") {
                txt_service.Text = Properties.Settings.Default.UrlService;
                check++;
            }

            if (check < 9) {
                MessageBox.Show("Complete as configurações e salve, a aplicação será reiniciada", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else {
                ProcessorManager();
            }
        }

        private void ProcessorManager() {
            try {
                timer.AutoReset = true;
                timer.Enabled = true;
                timer.Elapsed += timer_Elapsed;
                timer.Start();
            }
            catch (Exception ex) {
                Util.Log(ex.ToString());
            }
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            timer.Stop();
            SendMessage.Message();
            timer.Start();
        }

        private void btn_save_Click(object sender, EventArgs e) {
            try {
                Properties.Settings.Default["DBBase"] = txt_database.Text;
                Properties.Settings.Default["DBHost"] = txt_host.Text;
                Properties.Settings.Default["DBPort"] = txt_port.Text;
                Properties.Settings.Default["DBUser"] = txt_user.Text;
                Properties.Settings.Default["DBPass"] = SecurePassword.Encrypt(txt_pass.Text);
                Properties.Settings.Default["UrlLigacao"] = txt_urlfile.Text;
                Properties.Settings.Default["PathLog"] = txt_log.Text;
                Properties.Settings.Default["PathFile"] = txt_dirfile.Text;
                Properties.Settings.Default["UrlService"] = txt_service.Text;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Upgrade();

                MessageBox.Show("Salvo com sucesso", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Restart();
            }
            catch (Exception ex) {
                MessageBox.Show("Ocorreu um erro", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Util.Log(ex.ToString());
            }
        }

        private void Form1_Resize(object sender, EventArgs e) {
            if (WindowState == FormWindowState.Minimized) {
                Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e) {
            Show();
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }
    }
}

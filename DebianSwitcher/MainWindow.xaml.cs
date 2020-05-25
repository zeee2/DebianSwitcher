using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using WinHttp;

namespace DebianSwitcher
{
    public partial class MainWindow : Window
    {
        ServerSwitcher serverSwitcher;
        CertificateManager certificateManager;

        string ver = "2.1.0.4";
        string a = "";

        public MainWindow()
        {
            InitializeComponent();
            version(ver);
            alertcheck(a);
        }

        private string php(string url)
        {
            WinHttpRequest win = new WinHttpRequest();
            try
            {
                win.Open("GET", url);
                win.Send();
                win.WaitForResponse();
            }
            catch { MessageBox.Show(this, " Server Connection Error", " Debian "); Environment.Exit(0); }
            return win.ResponseText;
        }

        private void version(string version)
        {

            string result = php("https://debian.moe/static/switcher/switcherver.txt");
            if (result != version)
            {
                update frm = new update();
                Hide();
                ShowInTaskbar = false;
                frm.Show();
            }
            if (result == version)
            {
                Dispatcher.UnhandledException += Dispatcher_UnhandledException;
                certificateManager = new CertificateManager();
                switchButton.Content = "Retrieving IP address ...";
                DisableSwitching();
                Switching();
            }
        }

        private void alertcheck(string a)
        {
            string result = php("https://debian.moe/static/switcher/alert.txt");
            if (result != a)
            {
                MessageBox.Show(result, "Debian");
            }
        }

        public void changefilename()
        {
            string filename = Path.GetFileName(Assembly.GetEntryAssembly().Location);
            string kakao = "DebianSwitcher.exe";
            if (filename != kakao)
            {
                string path = Environment.CurrentDirectory;
                FileInfo fileRename = new FileInfo(path + @"\" + kakao);
                try
                {
                    Process[] processList = Process.GetProcessesByName("DebianSwitcher");
                    if (processList.Length > 0)
                    {
                        processList[0].Kill();
                    }
                    fileRename.Delete();
                    FileInfo fi = new FileInfo(path + @"\" + filename);
                    fi.MoveTo(path + @"\" + kakao);
                }
                catch { }
            }
        }

        private void DisableSwitching()
        {
            switchButton.IsEnabled = false;
        }

        private async void Switching()
        {
            await ChkCertificate();
            var serverIp = await GeneralHelper.GetIPAsync();

            serverSwitcher = new ServerSwitcher(serverIp);

            await ChkServer();
        }

        private async Task ChkCertificate()
        {
            switchButton.IsEnabled = false;
            var certificateStatus = await certificateManager.GetStatusAsync();
            switchButton.IsEnabled = true;
        }

        private async Task ChkServer()
        {
            switchButton.IsEnabled = false;
            var currentServer = await serverSwitcher.GetCurrentServerAsync();
            switchButton.Content = (currentServer == Server.Bancho) ? Constants.DebianConnect : Constants.BanchoConnect;
            switchButton.IsEnabled = true;
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void switchButton_Click(object sender, RoutedEventArgs e)
        {
            var serv = await serverSwitcher.GetCurrentServerAsync();
            var status = await certificateManager.GetStatusAsync();

            try
            {
                if (status)
                {
                    serverSwitcher.SwitchBancho();
                    try
                    {
                        certificateManager.Uninstall();
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }
                else
                {
                    serverSwitcher.SwitchDebian();
                    try
                    {
                        certificateManager.Install();
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Switcher Error!" + string.Format("\r\n\r\nDetails:\r\n{0}", ex.Message));
                Logger.Log(ex);
            }

            await ChkCertificate();
            await ChkServer();
        }

        private void websiteText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://debian.moe");
        }

        void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Fatal(e.Exception);
        }

        private void titleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
    }
}

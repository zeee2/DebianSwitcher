using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows;
using WinHttp;

namespace DebianSwitcher
{
    public partial class update : Window
    {
        public update()
        {
            InitializeComponent();
        }

        string ver = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        private void updateStart()
        {
            try
            {
                string http = "https://debian.moe/static/switcher/switcherdl.txt";
                string referer = "https://debian.moe/";
                string result = winhttp(http, referer);
                WebClient webClient = new WebClient();
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                string path = Environment.CurrentDirectory;
                string path2 = path + @"\" + ver + ".exe";
                webClient.DownloadFileAsync(new Uri(result), path2);
            }
            catch { MessageBox.Show("Update Failled"); Environment.Exit(0); }
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressbar1.Value = e.ProgressPercentage;
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            MessageBox.Show(this, "\rUpdate Successful", " Debian ");
            try
            {
                string path = Environment.CurrentDirectory;
                string path2 = path + @"\" + ver + ".exe";
                Process ps = new Process();
                ps.StartInfo.FileName = ver + ".exe";
                ps.StartInfo.WorkingDirectory = path;
                ps.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                ps.Start();
            }
            catch { }
            Application.Current.Shutdown();
            Environment.Exit(0);
        }

        private string winhttp(string http, string referer)
        {
            WinHttpRequest win = new WinHttpRequest();
            try
            {
                win.Open("GET", http, true);
                win.SetRequestHeader("Referer", referer);
                win.Send();
                win.WaitForResponse();
            }
            catch { MessageBox.Show(this, "\rServer Connection Error", " Debian "); Environment.Exit(0); }
            string result = Encoding.UTF8.GetString(win.ResponseBody);
            return result;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process[] processList = Process.GetProcessesByName(ver);
            if (processList.Length > 0)
            {
                processList[0].Kill();
            }
            updateStart();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void websiteText_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://debian.moe");
        }

        private void titleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
    }
}

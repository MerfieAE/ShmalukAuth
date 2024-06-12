using System;
using System.Diagnostics;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace SmalukAuth
{
    public partial class Form1 : Form
    {
        private ChromiumWebBrowser chromiumWebBrowser1;

        public Form1()
        {
            InitializeComponent();
            InitializeChromium();
            this.Load += MainForm_Load;
        }

        private void InitializeChromium()
        {
            CefSettings settings = new CefSettings();
            settings.CefCommandLineArgs.Add("disable-web-security", "2");
            settings.CefCommandLineArgs.Add("allow-running-insecure-content", "1");
            Cef.Initialize(settings);

            // Создаем экземпляр ChromiumWebBrowser и добавляем его на форму
            chromiumWebBrowser1 = new ChromiumWebBrowser("https://www.example.com");
            chromiumWebBrowser1.Dock = DockStyle.Fill;
            this.Controls.Add(chromiumWebBrowser1);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ShowCloudSelectionDialog();
        }

        private void ShowCloudSelectionDialog()
        {
            var cloudSelectionForm = new CloudSelectionForm();
            if (cloudSelectionForm.ShowDialog() == DialogResult.OK)
            {
                switch (cloudSelectionForm.SelectedCloud)
                {
                    case "Yandex":
                        StartYandexAuthentication();
                        break;
                    case "Google Drive":
                        StartGoogleDriveAuthentication();
                        break;
                    case "Bybit":
                        StartBybitAuthentication();
                        break;
                }
            }
        }

        private void StartYandexAuthentication()
        {
            string clientId = "8a26c7ca388b4824a3a5c0da97319c08";
            string redirectUri = "https://disk.yandex.ru/client/disk";
            string authUrl = $"https://oauth.yandex.ru/authorize?response_type=token&client_id={clientId}&redirect_uri={redirectUri}";
            chromiumWebBrowser1.Load(authUrl);
        }

        private void StartGoogleDriveAuthentication()
        {
            string clientId = "641622121895-0brc5pf9sdmjaksp7c49qhoptatg8j4i.apps.googleusercontent.com";
            string redirectUri = "https://www.google.ru/drive/";
            string authUrl = $"https://accounts.google.com/o/oauth2/auth?response_type=token&client_id={clientId}&redirect_uri={redirectUri}&scope=https://www.googleapis.com/auth/drive.readonly";
            Process.Start(new ProcessStartInfo
            {
                FileName = authUrl,
                UseShellExecute = true
            });
        }

        private void StartBybitAuthentication()
        {
            string clientId = "PE3R5KecSgJuutJH6U";
            string redirectUri = "https://www.bybit.com";
            string authUrl = $"https://www.bybit.com/login?client_id={clientId}&response_type=token&redirect_uri={redirectUri}&scope=cloud_api:disk.create,cloud_api:disk.read,cloud_api:disk.write";
            chromiumWebBrowser1.Load(authUrl);
        }

        private void ChromeBrowser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            if (e.Url.Contains("access_token="))
            {
                string token = ExtractAccessToken(e.Url);
                if (!string.IsNullOrEmpty(token))
                {
                    if (e.Url.StartsWith("https://oauth.yandex.ru"))
                    {
                        LoadYandexDisk(token);
                    }
                    else if (e.Url.StartsWith("https://accounts.google.com"))
                    {
                        LoadGoogleDrive(token);
                    }
                    else if (e.Url.StartsWith("https://www.bybit.com"))
                    {
                        LoadBybitDrive(token);
                    }
                }
            }
        }

        private string ExtractAccessToken(string url)
        {
            var tokenIndex = url.IndexOf("access_token=");
            if (tokenIndex != -1)
            {
                var token = url.Substring(tokenIndex + "access_token=".Length);
                return token.Split('&')[0];
            }
            return string.Empty;
        }

        private void LoadYandexDisk(string accessToken)
        {
            string apiRequest = "https://cloud-api.yandex.net/v1/disk/resources/files?oauth_token=" + accessToken;
            chromiumWebBrowser1.Load(apiRequest);
        }

        private void LoadGoogleDrive(string accessToken)
        {
            string apiRequest = "https://www.googleapis.com/drive/v3/files?access_token=" + accessToken;
            chromiumWebBrowser1.Load(apiRequest);
        }

        private void LoadBybitDrive(string accessToken)
        {
            string apiRequest = "https://www.bybit.com?access_token=" + accessToken;
            chromiumWebBrowser1.Load(apiRequest);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Cef.Shutdown();
            base.OnFormClosing(e);
        }

        private void chromiumWebBrowser1_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
        }
    }
}
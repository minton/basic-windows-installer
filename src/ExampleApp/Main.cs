using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonMark;

namespace ExampleApp
{
    public partial class Main : Form
    {
        const string GitHubRepository = "https://github.com/minton/basic-windows-installer";
        const string GitHubReadMe = "https://raw.githubusercontent.com/minton/basic-windows-installer/master/README.md";

        public Main()
        {
            InitializeComponent();
            btnExit.Click += (_, __) => Application.Exit();
            lnkGithub.LinkClicked += (_, __) => Process.Start(GitHubRepository);
            lblVersion.Text = String.Format(lblVersion.Text, Assembly.GetExecutingAssembly().GetName().Version);
            Task.Run(() =>
            {
                using (var client = new WebClient())
                {
                    client.DownloadStringCompleted += (_, e) => PopulateReadMe(e.Result);
                    client.DownloadStringAsync(new Uri(GitHubReadMe));
                }
            });
        }

        void PopulateReadMe(string readme)
        {
            var html = CommonMarkConverter.Convert(readme);
            browser.DocumentText = html;
        }
    }
}

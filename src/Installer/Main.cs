using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BasicWindowsInstaller.Installer
{
    public partial class Main : Form
    {        
        public Main()
        {
            InitializeComponent();
            pb.Image = Utility.GetLoadingGraphic();
            WireMouseEvents();
            tmrDelay.Tick += (s, e) =>
            {
                tmrDelay.Stop();
                bgw.RunWorkerAsync();
            };
        }     

        #region mouse movement

        public const int HT_CAPTION = 0x2;
        public const int WM_NCLBUTTONDOWN = 0xA1;
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        void WireMouseEvents()
        {
            MouseDown += HandleMouseMove;
            pb.MouseDown += HandleMouseMove;
            lblStatus.MouseDown += HandleMouseMove;
        }

        void HandleMouseMove(object s, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        #endregion

        void Install(object s, DoWorkEventArgs e)
        {
            var result = new InstallResult{Success = true};
            try
            {
                do
                {
                    if (Utility.IsDotNet452() == false)
                        if (Install(".NET 4.5.2...", "dotnet.exe", result, "/passive /norestart", Path.GetTempPath(), true, true) == false) break;
                    if (ExtractFile("CommonMark...", "CommonMark.dll", result, Utility.GetInstallPath("ExampleApp")).Success == false) break;
                    if (ExtractFile("ExampleApp...", "ExampleApp.exe.config", result, Utility.GetInstallPath("ExampleApp")).Success == false) break;
                    if (Install("ExampleApp...", "ExampleApp.exe", result, "", Utility.GetInstallPath("ExampleApp"), false, false) == false) break;
                    Utility.CreateShortcut(string.Format(@"{0}\ExampleApp.exe", Utility.GetInstallPath("ExampleApp")));
                } while (false);

                e.Result = result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                e.Result = result;
            }
        }

        bool Install(string status, string resource, InstallResult install, string arguments, string directory, bool wait, bool hidden)
        {
            var result = ExtractFile(status, resource, install, directory);
            return result.Success && Utility.StartProcess(result.Location, arguments, wait, hidden);
        }

        ResourceExtractionResult ExtractFile(string status, string resource, InstallResult install, string directory)
        {
            bgw.ReportProgress(0, status);
            var result = Utility.ExtractResource(resource, directory);
            if (result.Success == false)
            {
                bgw.ReportProgress(0, result.Error);
                install.Success = false;
                install.Message = result.Error;
                return result;
            }
            return result;
        }

        void StatusChanged(object s, ProgressChangedEventArgs e)
        {
            lblStatus.Text = e.UserState as string;
        }

        void Finished(object s, RunWorkerCompletedEventArgs e)
        {
            var result = e.Result as InstallResult;
            if (result == null) throw new NullReferenceException();

            if (result.Success == false)
            {
                MessageBox.Show(result.Message, "Install Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Application.Exit();
        }
    }
}

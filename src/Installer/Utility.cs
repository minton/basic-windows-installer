using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;
using Microsoft.Win32;

namespace BasicWindowsInstaller.Installer
{
    public static class Utility
    {
        public static void CreateShortcut(string installPath)
        {
            ShellLink sl;
            var dir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var file = Path.Combine(dir, "ExampleApp.lnk");
            File.Delete(file);

            sl = new ShellLink
            {
                Target = installPath,
                IconPath = installPath,
                IconIndex = 0,
                WorkingDirectory = Path.GetDirectoryName(installPath),
                Description = String.Format("ExampleApp v{0}", Assembly.GetExecutingAssembly().GetName().Version)
            };

            sl.SetAppUserModelId("io.minton.blog");
            sl.Save(file);
        }

        public static bool IsDotNet452()
        {
            var keyValue = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\", "Release", null);
            return CheckForDotNet452(keyValue);
        }

        static bool CheckForDotNet452(object releaseKey)
        {
            if (releaseKey == null) return false;
            int version;
            if (Int32.TryParse(releaseKey.ToString(), out version) == false) return false;

            if (version >= 393295)
            {
                return true;
            }
            if ((version >= 379893))
            {
                return true;
            }
            if ((version >= 378675))
            {
                return false;
            }
            if ((version >= 378389))
            {
                return false;
            }
            return false;
        }        

        public static bool StartProcess(string path, string arguments, bool wait, bool hidden)
        {
            var pi = new ProcessStartInfo(path, arguments);
            if (hidden)
            {
                pi.WindowStyle = ProcessWindowStyle.Hidden;
                pi.CreateNoWindow = true;
            }
            var process = Process.Start(pi);
            if (wait) process.WaitForExit(600000);
            return true;
        }

        public static ResourceExtractionResult ExtractResource(string name, string directory)
        {
            var result = new ResourceExtractionResult();
            try
            {
                if (Directory.Exists(directory) == false) Directory.CreateDirectory(directory);

                var filePath = Path.Combine(directory, name);
                var assembly = Assembly.GetCallingAssembly();

                using (var s = assembly.GetManifestResourceStream("BasicWindowsInstaller.Installer." + name))
                {
                    if (s == null)
                    {
                        result.Success = false;
                        result.Error = String.Format("Resource not found: `{0}`", name);
                        return result;
                    }

                    using (var r = new BinaryReader(s))
                    using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
                    using (var w = new BinaryWriter(fs))
                    {
                        w.Write(r.ReadBytes((int)s.Length));
                    }
                }
                result.Location = filePath;
                result.Success = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex.Message;
                throw;
            }
        }

        public static Image GetLoadingGraphic()
        {            
            var loader = Assembly.GetExecutingAssembly().GetManifestResourceStream("BasicWindowsInstaller.Installer.Resources.loading.gif");
            if (loader == null) throw new MissingManifestResourceException("loader not found");
            return Image.FromStream(loader);
        }

        public static string GetInstallPath(string directory)
        {
            var userLocal = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(userLocal, directory);
        }
    }

    public class ResourceExtractionResult
    {
        public bool Success { get; set; }
        public string Location { get; set; }
        public string Error { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SKInjector
{
    class Utils
    {
        public static FileInfo getOwnPath()
        {
            return new FileInfo(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory));
        }
        internal static void Exit()
        {
            // System.Windows.Application.Exit();
            var currentP = Process.GetCurrentProcess();
            currentP.Kill();
        }
        internal static bool IsAdmin()
        {
            bool isAdmin;
            try {
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            } catch (UnauthorizedAccessException ex) {
                isAdmin = false;
            } catch (Exception ex) {
                isAdmin = false;
            }
            return isAdmin;
        }
        public static void RestartAsAdmin(string[] args)
        {
            if (IsAdmin()) return;
            ProcessStartInfo proc = new ProcessStartInfo();
            proc.UseShellExecute = true;
            proc.WorkingDirectory = Environment.CurrentDirectory;
            proc.FileName = Assembly.GetEntryAssembly().CodeBase;
            proc.Arguments += string.Join(" ", args);
            proc.Verb = "runas";
            try
            {
                Process.Start(proc);
                Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to restart as admin for you. Please do this manually now!", "Can't restart as admin", MessageBoxButton.OK);
                Exit();
            }
        }
        public static string Base64Encode(string plainText) {
          var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
          return Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData) {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public static FileInfo DownloadFile(string url, DirectoryInfo destinationPath, string fileName = null) {
            if (fileName == null) fileName = url.Split('/').Last();
            // Main.webClient.DownloadFile(url, Path.Combine(destinationPath.FullName, fileName));
            return new FileInfo(Path.Combine(destinationPath.FullName, fileName));
        }
        public static void ShowFileInExplorer(FileInfo file) {
            StartProcess("explorer.exe", null, "/select, "+file.FullName.Quote());
        }
        public static Process StartProcess(FileInfo file, params string[] args) => StartProcess(file.FullName, file.DirectoryName, args);
        public static Process StartProcess(string file, string workDir = null, params string[] args) {
            ProcessStartInfo proc = new ProcessStartInfo();
            proc.FileName = file;
            proc.Arguments = string.Join(" ", args);
            Logger.Debug(proc.FileName, proc.Arguments);
            if (workDir != null) {
                proc.WorkingDirectory = workDir;
                Logger.Debug("WorkingDirectory:", proc.WorkingDirectory);
            }
            return Process.Start(proc);
        }
    }
}

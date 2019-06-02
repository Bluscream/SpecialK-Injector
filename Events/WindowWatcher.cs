using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKInjector.Events
{
    class WindowWatcher {
        const bool learning = true;
        static WindowHookNet.WindowHookNet hook;
        const string WhitelistFile = "whitelist_window_title.txt";
        static List<string> Whitelist = new List<string> {  }; // "", "Default IME", "AXWIN Frame Window"
        public static void Init()
        {
            if (!File.Exists(WhitelistFile)) {
                var file = File.Create(WhitelistFile); file.Close();
            }
            var logFile = File.ReadAllLines(WhitelistFile);
            Whitelist = new List<string>(logFile);
            var hook = WindowHookNet.WindowHookNet.Instance;
            hook.WindowCreated += onWindowCreated;
        }
 
        static void onWindowCreated(object sender,WindowHookNet.WindowHookEventArgs e) 
        {
            if (Whitelist.Contains(e.WindowTitle)) return;
            if (learning) Whitelist.Add(e.WindowTitle);
            GetWindowThreadProcessId(e.Handle, out uint pID);
            var pString = string.Format("[{0}] \"{1}\" => ", pID, e.WindowTitle);
            Logger.Warn(pString);
            try {
                var proc = Process.GetProcessById(Convert.ToInt32(pID));
                try { Console.WriteLine("Modules: {0}", proc.Modules.Join(" ", false));
                } catch (System.ComponentModel.Win32Exception) {}
                /*List<Module> modules;
                try { modules = Program.CollectModules(proc);
                } catch (System.InvalidOperationException ex) { return; }
                Console.WriteLine(pString+"Modules: {0}", modules.Select(p => p.ModuleName).Join(" "));*/
            }
            catch (System.ArgumentException ex) {
                return;
            }
        }
        static internal void Dispose()
        {
            if (hook != null)
                hook.WindowCreated -= onWindowCreated;
            if (learning) {
                using(TextWriter tw = new StreamWriter(WhitelistFile)) {
                    foreach (string s in Whitelist) tw.WriteLine(s);
                }
            }
        }

        [DllImport("user32.dll", SetLastError=true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
    }
}

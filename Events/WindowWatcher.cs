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
        static List<string> Whitelist = new List<string> { "", "Default IME", "AXWIN Frame Window"  };
        public static void Init()
        {
            var hook = WindowHookNet.WindowHookNet.Instance;
            hook.WindowCreated += onWindowCreated;
        }
 
        static void onWindowCreated(object sender,WindowHookNet.WindowHookEventArgs e) 
        {
            try
            {
                if (Whitelist.Contains(e.WindowTitle)) return;
                if (learning) Whitelist.Add(e.WindowTitle);
                GetWindowThreadProcessId(e.Handle, out uint pID);
                var pString = string.Format("[{0}] \"{1}\" =>", pID, e.WindowTitle);
                Logger.Warn(pString, "On Window Created: ", e.WindowClass);
                var proc = Process.GetProcessById(Convert.ToInt32(pID));
                ProcessHandler.HandleProcess(proc);
            } catch (Exception ex) {
                Logger.Error(ex.GetType(), "while processing", e, ":", ex.Message);
            }
        }
        static internal void Dispose()
        {
            if (hook != null)
                hook.WindowCreated -= onWindowCreated;
        }

        [DllImport("user32.dll", SetLastError=true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
    }
}

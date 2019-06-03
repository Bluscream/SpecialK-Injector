using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SKInjector.Events
{
    class WindowWatcher {
        const bool learning = true;
        static Lib.WindowHookNet hook;
        static List<string> Whitelist = new List<string> { "", "Default IME", "AXWIN Frame Window"  };
        public static void Init()
        {
            hook = Lib.WindowHookNet.Instance;
            hook.WindowCreated += onWindowCreated;
        }
 
        static void onWindowCreated(object sender, Lib.WindowHookEventArgs e) 
        {
            try {
                if (Whitelist.Contains(e.WindowTitle)) return;
                if (learning) Whitelist.Add(e.WindowTitle);
                GetWindowThreadProcessId(e.Handle, out uint pID);
                var pString = string.Format("[{0}] \"{1}\" =>", pID, e.WindowTitle);
                Logger.Trace(pString, "On Window Created: ", e.WindowClass);
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

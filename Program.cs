using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKInjector
{
    partial class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            Logger.Init();
            Logger.Trace("START");
            if (!Utils.IsAdmin()) { Utils.RestartAsAdmin(args); return; }
            Events.WindowWatcher.Init();
            // ProcessWatcher.Init();
            Console.ReadLine();
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            Logger.Log("Exiting...");
            // ProcessWatcher.Dispose();
            Events.WindowWatcher.Dispose();
        }
    }
}

using System;
using IniParser;
using IniParser.Model;

namespace SKInjector
{
    partial class Program
    {
        public static IniData config;
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            Logger.Init();
            Logger.Trace("START");
            // if (!Utils.IsAdmin()) { Utils.RestartAsAdmin(args); return; }
            config = Config.Load();
            Logger.Debug(config.Sections.ToJson());
            ProcessHandler.HandleAllProcesses();
            Events.WindowWatcher.Init();
            // Events.ProcessWatcher.Init();
            Console.ReadLine();
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            Logger.Log("Exiting...");
            // Events.ProcessWatcher.Dispose();
            Events.WindowWatcher.Dispose();
        }
    }
}

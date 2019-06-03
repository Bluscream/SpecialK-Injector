using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SKInjector
{
    class ProcessHandler
    {
        private static int lastPID = 0;
        static List<string> Whitelist = new List<string> { "", "system", "idle", "svchost", "explorer", "lghub", "lghub_agent", "chrome", "firefox", "steamwebhelper", "avastui", "startmenuexperiencehost", "notepad++", "shellexperiencehost", "searchui", "scriptedsandbox64", "nvcontainer", "sharex", "runtimebroker", "windowsinternal.composableshell.experiences.textinput.inputapp" };
        static List<string> GameDLLs = new List<string> { "dxgi.dll", "d3d8.dll", "d3d9.dll", "d3d9ex.dll", "opengl32.dll", "d3d11.dll", "dxcore.dll" };
        // static List<string> SKDLLs = new List<string> { "specialk.dll", "specialk32.dll", "specialk64.dll" };
        public static void HandleProcess(Process process)
        {
            if (process.Id == lastPID) return;
            if (Whitelist.Contains(process.ProcessName.ToLower())) return;
            if (Program.config.Sections.ContainsSection(process.ProcessName)) {
                var section = Program.config[process.ProcessName];
                if (section.hasTrueKey("ignore")) {
                    Logger.Log(process.ProcessName, "is set to ignore by config!");
                    return;
                }
                if (section.ContainsKey("delay")) {
                    Logger.Log(process.ProcessName, "is set to delay by config (", section["delay"], "ms)!");
                    System.Threading.Thread.Sleep(System.Convert.ToInt32(section["delay"]));
                }
            }
            lastPID = process.Id;
            var isGame = false;
            var modules = new List<string>();
            foreach (ProcessModule module in process.Modules)
            {
                var name = module.ModuleName.ToLower();
                modules.Add(name);
                if (GameDLLs.Contains(name)) { isGame = true; break; }
            }
            if (!isGame) return;
            Logger.Log(process.ProcessName, "is Game!");
            if (modules.ContainsAny("specialk.dll", "specialk32.dll", "specialk64.dll")) { Logger.Log("SpecialK already injected!"); return; }
            Injector.Inject(process);
        }
        public static void HandleAllProcesses() {
            foreach (var process in Process.GetProcesses())
            {
                try { HandleProcess(process); }
                catch (Exception ex) { Logger.Error("Error while trying to handle", process.ProcessName.Quote(), ":", ex.Message); }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKInjector
{
    class ProcessHandler
    {
        private static int lastPID = 0;
        static List<string> Whitelist = new List<string> { "", "svchost"  };
        static List<string> GameDLLs = new List<string> { "dxgi.dll", "d3d8.dll", "d3d9.dll", "d3d9ex.dll", "opengl32.dll", "d3d11.dll", "dxcore.dll" };
        // static List<string> SKDLLs = new List<string> { "specialk.dll", "specialk32.dll", "specialk64.dll" };
        public static void HandleProcess(Process process)
        {
            if (process.Id == lastPID) return;
            if (Whitelist.Contains(process.ProcessName)) return;
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
            if (modules.ContainsAny("specialk.dll", "specialk32.dll", "specialk64.dll")) { Logger.Log("SpecialK already injected!"); return;
            }
        }
    }
}

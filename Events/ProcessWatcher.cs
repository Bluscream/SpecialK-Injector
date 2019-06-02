using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SKInjector.Events
{
    class ProcessWatcher
    {
        static internal ManagementEventWatcher managementEventWatcher;
        static internal void Init()
        {
            managementEventWatcher = new ManagementEventWatcher( new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            managementEventWatcher.EventArrived += new EventArrivedEventHandler(OnNewProcess);
            managementEventWatcher.Start();
        }
        static internal void Dispose()
        {
            if (managementEventWatcher != null)
                managementEventWatcher.EventArrived -= OnNewProcess;
        }

        static void OnNewProcess(object sender, EventArrivedEventArgs e)
        {
            var pID = Convert.ToInt32(e.NewEvent.Properties["ProcessID"].Value);
            var pName = e.NewEvent.Properties["ProcessName"].Value;
            var pString = string.Format("[{0}] \"{1}\" => ", pID, pName);
            Console.WriteLine(pString+"New Process Started");
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                // Thread.Sleep(60000);
                try {
                    var proc = Process.GetProcessById(pID);
                    try { Console.WriteLine(pString+"Modules (Same Arch): {0}", proc.Modules.Join(" ", false));
                    } catch (System.ComponentModel.Win32Exception) {}
                    List<Module> modules;
                    try { modules = Program.CollectModules(proc);
                    } catch (System.InvalidOperationException ex) { return; }
                    Console.WriteLine(pString+"Modules: {0}", modules.Select(p => p.ModuleName).Join(" "));
                }
                catch (System.ArgumentException ex) {
                    return;
                }
            }).Start();
        }
    }
}

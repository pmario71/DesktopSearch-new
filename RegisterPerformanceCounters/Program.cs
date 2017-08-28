using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using DesktopSearch.Core.Utils;

namespace RegisterPerformanceCounters.exe
{
    class Program
    {
        static void Main(string[] args)
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            bool isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);

            if (!isElevated)
            {
                Console.WriteLine("Registering PerformanceCounters requires admin rights! Please start from admin shell!");
                return;
            }

            Performance.RegisterPerformanceCounters();
        }
    }
}

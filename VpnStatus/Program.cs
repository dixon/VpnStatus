using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace VpnStatus
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var icon = new SystemTrayIcon())
            using (ListenForVpnStatusChanges(icon))
            {
                Application.Run();
            }
        }

        private static IDisposable ListenForVpnStatusChanges(SystemTrayIcon i)
        {
            var t = new System.Timers.Timer();

            t.Elapsed += (sender, args) => i.Update();
            t.Interval = Settings.IntervalToPollMs;
            t.Enabled = true;

            return t;
        }

        public static bool IsVpnConnected => VpnInterface?.OperationalStatus == OperationalStatus.Up;

        private static NetworkInterface VpnInterface => 
            NetworkInterface.GetAllNetworkInterfaces()
                            .SingleOrDefault(i => (i.Name ?? "").Equals(Settings.VpnName, StringComparison.InvariantCultureIgnoreCase));
        
    }
}

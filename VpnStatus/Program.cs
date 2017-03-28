using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
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

            t.Elapsed += (sender, args) => i.UpdateStatusIcon();
            t.Interval = IntervalToPollMs;
            t.Enabled = true;

            return t;
        }

        internal static string VpnName => ConfigurationManager.AppSettings[nameof(VpnName)];

        internal static int IntervalToPollMs => int.Parse(ConfigurationManager.AppSettings[nameof(IntervalToPollMs)]);

        internal static bool IsVpnConnected =>
            NetworkInterface.GetAllNetworkInterfaces()
                            .Any(i => i.OperationalStatus == OperationalStatus.Up 
                                   && VpnName.Equals(i.Name, StringComparison.InvariantCultureIgnoreCase));
    }
}

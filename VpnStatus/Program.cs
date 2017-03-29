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

            t.Elapsed += (sender, args) => i.UpdateStatusIcon();
            t.Interval = Settings.IntervalToPollMs;
            t.Enabled = true;

            return t;
        }

        public static bool IsVpnConnected =>
            NetworkInterface.GetAllNetworkInterfaces()
                            .Any(i => i.OperationalStatus == OperationalStatus.Up 
                                   && Settings.VpnName.Equals(i.Name, StringComparison.InvariantCultureIgnoreCase));

        /// <summary>
        /// Understands parsing values from the App.config file's &lt;appSettings&gt; items.
        /// </summary>
        internal static class Settings
        {
            /// <summary>
            /// Returns the VPN name as configured in Control Panel\Network and Internet\Network Connections
            /// </summary>
            /// <remarks>For quick access to Network Connections, run ncpa.cpl</remarks>
            public static string VpnName => Get<string>();

            /// <summary>
            /// How often to query all network connections to determine vpn status.
            /// </summary>
            public static int IntervalToPollMs => Get<int>();

            /// <summary>
            /// Returns value of the appSetting under key (where appSettings key should match callers' name).
            /// </summary>
            private static T Get<T>([CallerMemberName]string key = null)
            {
                if (key.IsNullOrWhiteSpace())
                    throw new ArgumentNullException(nameof(key));

                string value = ConfigurationManager.AppSettings[key];
                if (value.IsNullOrWhiteSpace())
                    throw new Exception($"<appSettings> item with key {key} needs a value.");

                return (T)Convert.ChangeType(value, typeof(T));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace VpnStatus
{
    internal enum Status
    {
        Connected,
        Disconnected
    }

    internal static class StatusExtensionMethods
    {
        public static bool IsConnected(this Status s) => s == Status.Connected;
    }

    internal static class StatusInquiry
    {
        public static Status CurrentStatus => Vpn?.OperationalStatus == OperationalStatus.Up ? Status.Connected : Status.Disconnected;

        private static NetworkInterface Vpn => 
            UpInterfaces.SingleOrDefault(i => (i.Name ?? "").Equals(Settings.VpnName, StringComparison.InvariantCultureIgnoreCase));
            
        private static IEnumerable<NetworkInterface> UpInterfaces =>
            NetworkInterface.GetAllNetworkInterfaces().Where(i => i.OperationalStatus == OperationalStatus.Up);
    }
}

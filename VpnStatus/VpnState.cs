using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace VpnStatus
{
    internal enum VpnState
    {
        Connected,
        Disconnected
    }

    internal static class VpnStateHelper
    {
        public static bool IsConnected(this VpnState s) => s == VpnState.Connected;

        public static VpnState CurrentState => Vpn?.OperationalStatus == OperationalStatus.Up ? VpnState.Connected : VpnState.Disconnected;

        private static NetworkInterface Vpn => 
            UpInterfaces.SingleOrDefault(i => (i.Name ?? "").Equals(Settings.VpnName, StringComparison.InvariantCultureIgnoreCase));
            
        private static IEnumerable<NetworkInterface> UpInterfaces =>
            NetworkInterface.GetAllNetworkInterfaces().Where(i => i.OperationalStatus == OperationalStatus.Up);

        public static void ToggleVpn()
        {
            var psi = new ProcessStartInfo
            {
                FileName = "rasdial",
                Arguments = $"\"{Settings.VpnName}\" {(CurrentState.IsConnected() ? "/disconnect" : "")}"
            };

            using (var p = Process.Start(psi))
            {
                p.WaitForExit();
            }
        }
    }
}

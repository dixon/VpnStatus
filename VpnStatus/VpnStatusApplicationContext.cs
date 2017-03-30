using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VpnStatus
{
    internal class VpnStatusApplicationContext : TrayIconApplicationContext
    {
        public VpnStatusApplicationContext()
        {
            InitContextMenu();
            InitPolling();
        }

        private Timer _pollNetworkInterfaces; // Winforms Timer allows polling on main thread, so we can easily update UI components
        private Status? _lastState;

        private ToolStripItem ToggleStatusMenuItem => 
            ContextMenu?.Items.Cast<ToolStripItem>().SingleOrDefault(i => i.Name == nameof(ToggleStatusMenuItem));

        protected override void OnApplicationExit(EventArgs args)
        {
            base.OnApplicationExit(args);

            _pollNetworkInterfaces?.Dispose();
        }

        protected override void OnTrayIconClick(MouseEventArgs args)
        {
            base.OnTrayIconClick(args);
            
            if (args.Button == MouseButtons.Left)
            {
                Process.Start("ncpa.cpl");
            }
        }

        private void InitContextMenu()
        {
            // toggle text will be replaced in UpdateUi() with either "Connect" or "Disconnect"
            ContextMenu.Items.Add(nameof(ToggleStatusMenuItem), null, (o, e) => ToggleStatus()).Name = nameof(ToggleStatusMenuItem);
            ContextMenu.Items.Add("Exit", null, (o, e) => ExitThread());
        }

        private void PausePolling() => _pollNetworkInterfaces?.Stop();
        private void StartPolling() => _pollNetworkInterfaces?.Start();

        private void InitPolling()
        {
            _pollNetworkInterfaces = new Timer { Interval = Settings.IntervalToPollMs };
            _pollNetworkInterfaces.Tick += PollForChanges;

            PollForChanges();
        }

        private void PollForChanges(object sender = null, EventArgs args = null)
        {
            PausePolling();

            var currentState = StatusInquiry.CurrentStatus;
            if (currentState != _lastState)
            {
                _lastState = currentState;
                UpdateUi(currentState);
            }

            StartPolling();
        }

        private void ToggleStatus()
        {
            PausePolling();

            // shell out to rasdial, which seems to be the more sure-fire way to manipulate vpn connections
            var psi = new ProcessStartInfo
            {
                FileName = "rasdial",
                Arguments = $"\"{Settings.VpnName}\" {(StatusInquiry.CurrentStatus.IsConnected() ? "/disconnect" : "")}"
            };
            using (var p = Process.Start(psi))
            {
                p.WaitForExit();
            }
            
            _lastState = StatusInquiry.CurrentStatus;
            UpdateUi(_lastState.Value);

            StartPolling();
        }

        private void UpdateUi(Status state)
        {
            TrayIcon.Icon = state.IsConnected() ? Resources.icon_connected : Resources.icon_disconnected;
            TrayIcon.Text = $"VPN Status: {Settings.VpnName}: {state}";

            ToggleStatusMenuItem.Text = state.IsConnected() ? "Disconnect" : "Connect";
            ToggleStatusMenuItem.Image = (state.IsConnected() ? Resources.icon_disconnected : Resources.icon_connected).ToBitmap();
        }
        
    }
}

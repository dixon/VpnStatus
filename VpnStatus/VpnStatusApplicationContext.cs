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
        private VpnState? _lastState;

        private ToolStripItem ToggleVpnMenuItem => 
            ContextMenu?.Items.Cast<ToolStripItem>().SingleOrDefault(i => i.Name == nameof(ToggleVpnMenuItem));

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
            // "Toggle" text will be replaced in UpdateUi() with either "Connect" or "Disconnect"
            ContextMenu.Items.Add("Toggle", null, (o, e) => ToggleVpnState()).Name = nameof(ToggleVpnMenuItem);
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

            var currentState = VpnStateHelper.CurrentState;
            if (currentState != _lastState)
            {
                _lastState = currentState;
                UpdateUi(currentState);
            }

            StartPolling();
        }

        private void ToggleVpnState()
        {
            PausePolling();

            VpnStateHelper.ToggleVpn();
            _lastState = VpnStateHelper.CurrentState;
            UpdateUi(_lastState.Value);

            StartPolling();
        }

        private void UpdateUi(VpnState state)
        {
            TrayIcon.Icon = state.IsConnected() ? Resources.icon_connected : Resources.icon_disconnected;
            TrayIcon.Text = $"VPN Status: {Settings.VpnName}: {state}";

            ToggleVpnMenuItem.Text = state.IsConnected() ? "Disconnect" : "Connect";
            ToggleVpnMenuItem.Image = (state.IsConnected() ? Resources.icon_disconnected : Resources.icon_connected).ToBitmap();
        }
        
    }
}

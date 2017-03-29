using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VpnStatus
{
    internal class SystemTrayIcon : IDisposable
    {
        private NotifyIcon _notifyIcon;
        private ContextMenuStrip _rightClickMenuStrip;

        public SystemTrayIcon()
        {
            InitNotifyIcon();
            InitRightClickMenuStrip();
        }

        public void Dispose()
        {
            _rightClickMenuStrip?.Dispose();
            _notifyIcon?.Dispose();
        }

        public void UpdateStatusIcon() => _notifyIcon.Icon = StatusIcon;

        private void InitNotifyIcon()
        {
            _notifyIcon = new NotifyIcon
            {
                Text = "VPN Status for " + Program.Settings.VpnName,
                Visible = true
            };

            // left-clicking icon in tray should open Control Panel > Network Connections
            _notifyIcon.MouseClick += (o, args) => { if (args.Button == MouseButtons.Left) { Process.Start("ncpa.cpl"); } };

            UpdateStatusIcon();
        }

        private void InitRightClickMenuStrip()
        {
            _rightClickMenuStrip = new ContextMenuStrip();

            var item = new ToolStripMenuItem("Exit");
            item.Click += (sender, args) => Application.Exit();
            _rightClickMenuStrip.Items.Add(item);

            _notifyIcon.ContextMenuStrip = _rightClickMenuStrip;
        }

        private static Icon StatusIcon => Program.IsVpnConnected ? Resources.icon_connected : Resources.icon_disconnected;
    }
}

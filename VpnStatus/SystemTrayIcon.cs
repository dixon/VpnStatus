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

        public void Update()
        {
            var isConnected = Program.IsVpnConnected;
            _notifyIcon.Icon = isConnected ? Resources.icon_connected : Resources.icon_disconnected;;
            _notifyIcon.Text = $"VPN Status: {Settings.VpnName}: {(isConnected ? "Connected" : "Disconnected")}";
        }

        private void InitNotifyIcon()
        {
            _notifyIcon = new NotifyIcon();

            // left-clicking icon in tray should open Control Panel > Network Connections
            _notifyIcon.MouseClick += (o, args) => { if (args.Button == MouseButtons.Left) { Process.Start("ncpa.cpl"); } };

            // set icon and text
            Update();

            _notifyIcon.Visible = true;
        }

        private void InitRightClickMenuStrip()
        {
            _rightClickMenuStrip = _notifyIcon.ContextMenuStrip = new ContextMenuStrip();

            var item = new ToolStripMenuItem("Exit");
            item.Click += (sender, args) => Application.Exit();
            _rightClickMenuStrip.Items.Add(item);
        }
    }
}

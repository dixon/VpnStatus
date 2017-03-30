using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace VpnStatus
{
    /// <summary>
    /// Allows a long-running app with system tray icon + right-click context menu, but no initial Form.
    /// </summary>
    /// <remarks>
    /// Taken from https://www.cyotek.com/blog/creating-long-running-windows-forms-applications-without-a-start-up-form
    /// </remarks>
    internal abstract class TrayIconApplicationContext : ApplicationContext
    {
        protected TrayIconApplicationContext()
        {
            Application.ApplicationExit += (sender, args) => this.OnApplicationExit(args);

            ContextMenu = new ContextMenuStrip();

            TrayIcon = new NotifyIcon { Visible = true }; // won't be really visible until its Icon is set
            TrayIcon.MouseClick += (sender, args) => this.OnTrayIconClick(args);
            TrayIcon.ContextMenuStrip = ContextMenu;
        }

        protected NotifyIcon TrayIcon { get; }
        protected ContextMenuStrip ContextMenu { get; }

        protected virtual void OnApplicationExit(EventArgs args)
        {
            if (TrayIcon != null)
            {
                TrayIcon.Visible = false; // prevents "phantom" icon from staying around in the system tray
                TrayIcon.Dispose();
            }
            
            ContextMenu?.Dispose();
        }

        protected virtual void OnTrayIconClick(MouseEventArgs args) { }
    }
}

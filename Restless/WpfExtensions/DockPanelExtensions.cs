using System.Windows;
using System.Windows.Controls;

namespace Restless.WpfExtensions
{
    public static class DockPanelExtensions
    {
        public static void Add(this DockPanel dockPanel, UIElement child, Dock dock)
        {
            DockPanel.SetDock(child, dock);
            dockPanel.Children.Add(child);
        }

        public static void Add(this DockPanel dockPanel, UIElement lastChild)
        {
            dockPanel.LastChildFill = true;
            dockPanel.Children.Add(lastChild);
        }
    }
}
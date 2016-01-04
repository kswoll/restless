using System.Windows.Controls;

namespace Restless.WpfExtensions
{
    public static class MenuExtensions
    {
        public static MenuItem Add(this Menu menu, string title)
        {
            var menuItem = new MenuItem { Header = title };
            menu.Items.Add(menuItem);
            return menuItem;
        }

        public static MenuItem Add(this MenuItem menu, string title)
        {
            var menuItem = new MenuItem { Header = title };
            menu.Items.Add(menuItem);
            return menuItem;
        }
    }
}
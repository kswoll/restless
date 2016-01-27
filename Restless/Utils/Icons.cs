using System;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Restless.Utils
{
    public class Icons
    {
        internal static Viewbox Get(string data, int width = 20, int height = 20)
        {
            var viewbox = (Viewbox)XamlReader.Parse(data);
            viewbox.Width = width;
            viewbox.Height = height;
            return viewbox;
        }
    }
}
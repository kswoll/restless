using System.Windows;

namespace Restless.Styles
{
    public class GlobalStyles
    {
        public static void RegisterStyles(ResourceDictionary resources)
        {
            ButtonStyle.Register(resources);
            ToggleButtonStyle.Register(resources);
        }
    }
}
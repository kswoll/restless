using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Restless.Templates;

namespace Restless.Styles
{
    public class ToggleButtonStyle
    {
        public static void Register(ResourceDictionary resources)
        {
            var buttonTemplate = new FrameworkElementFactory(typeof(ButtonTemplate));

            var controlTemplate = new ControlTemplate(typeof(ToggleButton));
            controlTemplate.VisualTree = buttonTemplate;

            var style = new Style(typeof(ToggleButton));
            style.Setters.Add(new Setter(Control.TemplateProperty, controlTemplate));

            resources.Add(typeof(ToggleButton), style);
        } 
         
    }
}
using System.Windows;
using System.Windows.Controls;
using Restless.Templates;

namespace Restless.Styles
{
    public class ButtonStyle
    {
        public static void Register(ResourceDictionary resources)
        {
            var buttonTemplate = new FrameworkElementFactory(typeof(ButtonTemplate));

            var controlTemplate = new ControlTemplate(typeof(Button));
            controlTemplate.VisualTree = buttonTemplate;

            var style = new Style(typeof(Button));
            style.Setters.Add(new Setter(Control.TemplateProperty, controlTemplate));

            resources.Add(typeof(Button), style);
        } 
    }
}
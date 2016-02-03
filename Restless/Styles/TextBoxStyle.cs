using System.Windows;
using System.Windows.Controls;
using Restless.Templates;

namespace Restless.Styles
{
    public class TextBoxStyle
    {
        public static void Register(ResourceDictionary resources)
        {
            var textBoxTemplate = new FrameworkElementFactory(typeof(TextBoxTemplate));
            textBoxTemplate.AppendChild(new FrameworkElementFactory(typeof(ScrollViewer), "PART_ContentHost"));

            var controlTemplate = new ControlTemplate(typeof(TextBox));
            controlTemplate.VisualTree = textBoxTemplate;

            var style = new Style(typeof(TextBox));
            style.Setters.Add(new Setter(Control.TemplateProperty, controlTemplate));

            resources.Add(typeof(TextBox), style);
        }          
    }
}
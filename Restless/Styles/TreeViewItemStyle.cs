using System.Windows;
using System.Windows.Controls;
using Restless.Templates;

namespace Restless.Styles
{
    public class TreeViewItemStyle
    {
        public static void Register(ResourceDictionary resources)
        {
            var treeViewItemTemplate = new FrameworkElementFactory(typeof(TreeViewItemTemplate));

            var controlTemplate = new ControlTemplate(typeof(TreeViewItem));
            controlTemplate.VisualTree = treeViewItemTemplate;

            var style = new Style(typeof(TreeViewItem));
            style.Setters.Add(new Setter(Control.TemplateProperty, controlTemplate));

            resources.Add(typeof(TreeViewItem), style);
        } 
         
    }
}
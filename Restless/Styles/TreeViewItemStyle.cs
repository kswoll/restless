using System.Windows;
using System.Windows.Controls;
using Restless.Templates;

namespace Restless.Styles
{
    public class TreeViewItemStyle : Style
    {
        public TreeViewItemStyle() : base(typeof(TreeViewItem))
        {
            var treeViewItemTemplate = new FrameworkElementFactory(typeof(TreeViewItemTemplate));
            treeViewItemTemplate.AppendChild(new FrameworkElementFactory(typeof(ItemsPresenter), "ItemsHost"));
            treeViewItemTemplate.AppendChild(new FrameworkElementFactory(typeof(ContentPresenter), "PART_Header"));

            var controlTemplate = new ControlTemplate(typeof(TreeViewItem));
            controlTemplate.VisualTree = treeViewItemTemplate;

            Setters.Add(new Setter(Control.TemplateProperty, controlTemplate));
        }

        public static void Register(ResourceDictionary resources)
        {
            resources.Add(typeof(TreeViewItem), new TreeViewItemStyle());
        } 
    }
}
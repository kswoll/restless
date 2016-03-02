using System.Windows;
using System.Windows.Controls;
using Restless.Templates;

namespace Restless.Styles
{
    public class TreeViewItemStyle : TypedStyle<TreeViewItem>
    {
        public ControlTemplate Template { get; }
        public FrameworkElementFactory VisualTree { get; }

        public TreeViewItemStyle() 
        {
            VisualTree = new FrameworkElementFactory(typeof(TreeViewItemTemplate));
            VisualTree.AppendChild(new FrameworkElementFactory(typeof(ItemsPresenter), "ItemsHost"));
            VisualTree.AppendChild(new FrameworkElementFactory(typeof(ContentPresenter), "PART_Header"));

            Template = new ControlTemplate(typeof(TreeViewItem));
            Template.VisualTree = VisualTree;

            Setters.Add(new Setter(Control.TemplateProperty, Template));
        }

        public static void Register(ResourceDictionary resources)
        {
            resources.Add(typeof(TreeViewItem), new TreeViewItemStyle());
        } 
    }
}
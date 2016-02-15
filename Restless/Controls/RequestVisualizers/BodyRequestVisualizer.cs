using System.Windows.Controls;
using Restless.Models;
using SexyReact.Views;

namespace Restless.Controls.RequestVisualizers
{
    public class BodyRequestVisualizer : RequestVisualizer
    {
        public override string Title => "Body";

        public BodyRequestVisualizer()
        {
            var apiBodyTextBox = new TextBox();
            Children.Add(apiBodyTextBox);

            this.Bind(x => x.Model.Body).Mate(apiBodyTextBox);
        }

        public override void InitializeTab(TabItem item)
        {
            base.InitializeTab(item);

            this.Bind(x => x.Model.Method).To(x => item.Visibility = x.IsBodyAllowed() ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed);
        }
    }
}
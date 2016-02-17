using System.Windows;
using System.Windows.Controls;
using Restless.ViewModels;
using Restless.WpfExtensions;
using SexyReact.Views;

namespace Restless.Controls
{
    public class ApiCollectionPanel : RxGrid<ApiCollectionModel>
    {
        public ApiCollectionPanel()
        {
            var namePanel = NameValuePanel.Create("Title", new TextBox());

            this.AddRow(GridLength.Auto);
            this.Add(namePanel, 0, 0);

            this.Bind(x => x.Model.Title).Mate(namePanel.Value);
        }
    }
}
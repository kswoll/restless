using System.Text;
using System.Windows;
using System.Windows.Controls;
using Restless.Utils;
using Restless.ViewModels;
using Restless.WpfExtensions;
using SexyReact.Views;

namespace Restless.Controls.ResponseVisualizers
{
    public class TextBodyVisualizer : RxDockPanel<ApiResponseModel>, IResponseVisualizer
    {
        public bool IsThisPrimary(IResponseVisualizer other) => false;
        public int CompareTo(IResponseVisualizer other) => other is SummaryResponseVisualizer || other is HeadersResponseVisualizer ? -1 : 1;
        public string Header => "Body";

        [ResponseVisualizerPredicate]
        public static bool IsVisualizerVisible(ApiResponseModel response)
        {
            return ContentTypes.IsText(response.ContentType);
        }

        public TextBodyVisualizer()
        {
            var bodyText = new TextBox();
            bodyText.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            bodyText.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            bodyText.IsReadOnly = true;

            this.Add(bodyText);

            this.Bind(x => x.Response).To(x =>
            {
                Visibility = x == null ? Visibility.Hidden : Visibility.Visible;
                bodyText.Text = x == null ? "" : Encoding.UTF8.GetString(x);
            });
        }
    }
}
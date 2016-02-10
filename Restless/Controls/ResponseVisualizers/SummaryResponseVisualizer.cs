using System.Windows.Controls;
using Restless.Models;
using Restless.Utils;
using Restless.ViewModels;
using Restless.WpfExtensions;
using SexyReact.Views;

namespace Restless.Controls.ResponseVisualizers
{
    public class SummaryResponseVisualizer : RxStackPanel<ApiResponseModel>, IResponseVisualizer
    {
        public bool IsThisPrimary(IResponseVisualizer other) => false;
        public int CompareTo(IResponseVisualizer other) => 1;
        public string Header => "Summary";

        [ResponseVisualizerPredicate]
        public static bool IsVisualizerVisible(ApiResponseModel response)
        {
            return ContentTypes.IsText(response.ContentType);
        }

        public SummaryResponseVisualizer()
        {
            var elapsedPanel = NameValuePanel.Create("Elapsed", new Label());
            Children.Add(elapsedPanel);

            var contentLengthPanel = NameValuePanel.Create("Content Length", new Label());
            Children.Add(contentLengthPanel);

            this.Bind(x => x.ContentLength).To(x => contentLengthPanel.Value.Content = x);
            this.Bind(x => x.Elapsed).To(x => elapsedPanel.Value.Content = x);
        }
    }
}
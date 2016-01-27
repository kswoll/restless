using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Restless.Properties;
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

        public TextBodyVisualizer()
        {
            var bodyText = new TextBox();

            var wordWrapButton = new ToggleButton
            {
                Content = Icons.Get(IconResources.WordWrap),
                Focusable = false,
                ToolTip = TextResources.WordWrapToolTip
            };
            wordWrapButton.Checked += (sender, args) => bodyText.TextWrapping = TextWrapping.Wrap;
            wordWrapButton.Unchecked += (sender, args) => bodyText.TextWrapping = TextWrapping.NoWrap;

            var prettyPrintButton = new ToggleButton
            {
                Content = Icons.Get(IconResources.PrettyPrint),
                Focusable = false,
                IsChecked = true,
                ToolTip = TextResources.PrettyPrintToolTip
            };
            prettyPrintButton.Unchecked += (sender, args) => bodyText.Text = Model.StringResponse;
            prettyPrintButton.Checked += (sender, args) =>
            {
                if (Model.JsonResponse != null)
                {
                    bodyText.Text = Model.JsonResponse.ToString(Formatting.Indented);
                }
            };

            var toolBar = new StackPanel { Orientation = Orientation.Horizontal };
            toolBar.Children.Add(wordWrapButton);
            toolBar.Children.Add(prettyPrintButton);
            this.Add(toolBar, Dock.Top);

            bodyText.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            bodyText.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            bodyText.IsReadOnly = true;

            this.Add(bodyText);

            this.Bind(x => x.Response).To(x =>
            {
                Visibility = x == null ? Visibility.Hidden : Visibility.Visible;
                bodyText.Text = Model.JsonResponse != null ? Model.JsonResponse.ToString(Formatting.Indented) : x == null ? "" : Encoding.UTF8.GetString(x);
                prettyPrintButton.Visibility = Model.JsonResponse != null ? Visibility.Visible : Visibility.Collapsed;
            });
        }

        [ResponseVisualizerPredicate]
        public static bool IsVisualizerVisible(ApiResponseModel response)
        {
            return ContentTypes.IsText(response.ContentType);
        }
    }
}
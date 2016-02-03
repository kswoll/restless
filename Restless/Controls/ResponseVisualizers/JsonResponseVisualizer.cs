using System;
using System.Reactive.Linq;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using Restless.Properties;
using Restless.Utils;
using Restless.ViewModels;
using Restless.WpfExtensions;
using SexyReact.Views;
using SexyReact;
using SexyReact.Utils;

namespace Restless.Controls.ResponseVisualizers
{
    [ResponseVisualizer(ContentTypes.ApplicationJson), Rx]
    public class JsonResponseVisualizer : RxDockPanel<ApiResponseModel>, IResponseVisualizer
    {
        public string Header => "Json";
        public int CompareTo(IResponseVisualizer other) => 0;
        public bool IsThisPrimary(IResponseVisualizer other) => false;
        public string FilterText { get; set; }

        private readonly TreeView treeView;
        private readonly TextBox filterTextBox = new TextBox();
        private bool isFiltered;

        public JsonResponseVisualizer()
        {
            treeView = new TreeView();

            filterTextBox.TextChanged += (sender, args) => FilterText = filterTextBox.Text;
            this.ObserveProperty(x => x.FilterText).Throttle(TimeSpan.FromSeconds(.3)).SubscribeOnUiThread(_ => Filter());

            var clearFilterButton = new Button
            {
                Content = Icons.Get(IconResources.RemoveFilter),
                Focusable = false
            };
            clearFilterButton.Click += (sender, args) => ClearFilter();

            var toolBar = new DockPanel();
            toolBar.Add(clearFilterButton, Dock.Right);
            toolBar.Add(filterTextBox);
            this.Add(toolBar, Dock.Top);

            this.Add(treeView);

            this.Bind(x => x.Response).To(response =>
            {
                if (response != null)
                {
                    AddRootToken(Model.JsonResponse);
                }
            });
        }

        private void Filter()
        {
            if (!string.IsNullOrEmpty(FilterText))
            {
                treeView.Filter(x => ((string)x.Header).ToUpper().Contains(FilterText.ToUpper()));
                isFiltered = true;                
            }
            else if (isFiltered)
            {
                ClearFilter();
            }
        }

        private void ClearFilter()
        {
            filterTextBox.Text = "";
            treeView.ClearFilter();
            isFiltered = false;
        }

        private void AddRootToken(JToken token)
        {
            var childNode = CreateChildNode(token);
            treeView.Items.Add(childNode);
        }

        private TreeViewItem CreateChildNode(JToken token)
        {
            if (token is JValue)
                return CreateChildNode((JValue)token);
            else if (token is JArray)
                return CreateChildNode((JArray)token);
            else if (token is JObject)
                return CreateChildNode((JObject)token);
            else
                throw new Exception($"Unexpected JSON token: {token}");
        }

        private TreeViewItem CreateChildNode(JValue value)
        {
            return new TreeViewItem { Header = value.ToString() };
        }

        private TreeViewItem CreateChildNode(JObject obj)
        {
            var objectNode = new TreeViewItem
            {
                Header = $"{{object with {obj.Count} propert{(obj.Count == 1 ? "y" : "ies")}}}",
                IsExpanded = true
            };
            foreach (var token in obj)
            {
                var propertyNode = CreateChildNode(token.Value);
                propertyNode.Header = $"{token.Key} = {propertyNode.Header}";
                objectNode.Items.Add(propertyNode);
            }
            return objectNode;
        }

        private TreeViewItem CreateChildNode(JArray array)
        {
            var arrayNode = new TreeViewItem
            {
                Header = $"{{array with {array.Count} element{(array.Count != 1 ? "s" : "")}}}",
                IsExpanded = true
            };
            for (var i = 0; i < array.Count; i++)
            {
                var token = array[i];
                var elementNode = CreateChildNode(token);
                elementNode.Header = $"[{i}] = {elementNode.Header}";
                arrayNode.Items.Add(elementNode);
            }
            return arrayNode;
        }
    }
}
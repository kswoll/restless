using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;
using Newtonsoft.Json.Linq;
using Restless.Utils;
using Restless.ViewModels;
using Restless.WpfExtensions;
using SexyReact.Views;

namespace Restless.Controls.ResponseVisualizers
{
    [ResponseVisualizer(ContentTypes.ApplicationJson)]
    public class JsonResponseVisualizer : RxDockPanel<ApiResponseModel>, IResponseVisualizer
    {
        public string Header => "Json";
        public int CompareTo(IResponseVisualizer other) => 0;
        public bool IsThisPrimary(IResponseVisualizer other) => false;

        private readonly TreeView treeView;

        public JsonResponseVisualizer()
        {
            treeView = new TreeView();

            this.Add(treeView);

            this.Bind(x => x.Response).To(response =>
            {
                if (response != null)
                {
                    AddRootToken(Model.JsonResponse);
                    var writer = XmlWriter.Create(@"c:\temp\treeviewitem.xaml", new XmlWriterSettings { Indent = true });
                    XamlWriter.Save(((TreeViewItem)treeView.Items[0]).Template, writer);
//                    File.WriteAllText(@"c:\temp\treeviewitem.xaml", s);

//                    treeView.Filter(x => ((string)x.Header).Contains("12"));
                }
            });
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
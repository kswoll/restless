﻿using System;
using System.Text;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using Restless.ViewModels;
using Restless.WpfExtensions;
using SexyReact.Views;

namespace Restless.Controls.ResponseVisualizers
{
    [ResponseVisualizer("application/json")]
    public class JsonResponseVisualizer : RxDockPanel<ApiResponseModel>, IResponseVisualizer
    {
        public string Header => "Json";

        private TreeView treeView;

        public JsonResponseVisualizer()
        {
            treeView = new TreeView();

            this.Add(treeView);

            this.Bind(x => x.Response).To(x =>
            {
                if (x != null)
                {
                    var s = Encoding.UTF8.GetString(x);
                    var json = JToken.Parse(s);

                    AddRootToken(json);                    
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
            var objectNode = new TreeViewItem { Header = $"{{object with {obj.Count} properties}}" };
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
            var arrayNode = new TreeViewItem { Header = $"{{array with {array.Count} elements}}" };
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
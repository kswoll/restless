using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using Restless.Models;
using Restless.Utils;
using Restless.ViewModels;
using Restless.WpfExtensions;
using SexyReact.Views;

namespace Restless.Controls.ResponseVisualizers
{
    [ResponseVisualizer(ContentTypes.TextHtml)]
    public class HtmlResponseVisualizer : RxDockPanel<ApiResponseModel>, IResponseVisualizer
    {
        public HtmlResponseVisualizer()
        {
            var browser = new WebBrowser();

            this.Add(browser);

            this.Bind(x => x.Response).To(x =>
            {
                if (x != null)
                {
                    var s = Encoding.UTF8.GetString(x);
                    var url = new Uri(Model.Api.Model.Url).AbsoluteUri;
                    var baseUrl = Model.Api.Model.Url.Substring(0, url.Length - new Uri(url).PathAndQuery.Length);
                    browser.NavigateToString(InsertBaseRef(s, baseUrl));
                }
            });
        }

        public string Header => "Html";
        public bool IsThisPrimary(IResponseVisualizer other) => false;
        public int CompareTo(IResponseVisualizer other) => 0;

        /// <summary>
        ///     Insert a base href tag into the header part of the HTML
        ///     If a head tag cannot be found, it is simply inserted at the beginning
        /// </summary>
        /// <param name="input_html">The HTML to process</param>
        /// <param name="url">URL for the base href tag</param>
        /// <returns>The processed HTML</returns>
        private static string InsertBaseRef(string input_html, string url)
        {
            var base_tag = "<base href=\"" + url + "\" />"; //  target=\"" + url + "\" />";
            Regex ItemRegex = new Regex(@"<head\s*>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            Match match = ItemRegex.Match(input_html);
            if (match.Success)
            {
                // only replace the first match
                return ItemRegex.Replace(input_html, match.Value + base_tag, 1);
            }

            // not found, so insert the base tag at the beginning
            return base_tag + input_html;
        }
    }
}
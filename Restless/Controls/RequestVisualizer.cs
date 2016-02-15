using System.Windows.Controls;
using Restless.ViewModels;
using SexyReact.Views;

namespace Restless.Controls
{
    public abstract class RequestVisualizer : RxGrid<ApiModel>
    {
        public abstract string Title { get; }

        protected RequestVisualizer()
        {
            RowDefinitions.Add(new RowDefinition { SharedSizeGroup = "apiTabs" });
        }

        /// <summary>
        /// Called when a new API is created.
        /// </summary>
        public virtual void InitializeNew()
        {
        }

        public virtual void InitializeTab(TabItem item)
        {
        }
    }
}
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Restless.Controls.ResponseActions;
using Restless.Controls.ResponseVisualizers;
using Restless.Utils;
using Restless.ViewModels;
using Restless.WpfExtensions;
using SexyReact.Views;

namespace Restless.Controls
{
    public class ApiResponsePanel : RxDockPanel<ApiResponseModel>
    {
        public ApiResponsePanel()
        {
            var tabControl = new TabControl
            {
                Padding = new Thickness(0)
            };

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Visibility = Visibility.Collapsed
            };

            this.Add(buttonPanel, Dock.Bottom);
            this.Add(tabControl);

            this.Bind(x => x).To(model =>
            {
                Visibility = model?.Status != null ? Visibility.Visible : Visibility.Hidden;
                foreach (TabItem tabItem in tabControl.Items)
                {
                    tabItem.Template = null;
                }
                tabControl.Items.Clear();
                buttonPanel.Children.Clear();

                if (model?.Status != null)
                {
                    var visualizers = ResponseVisualizerRegistry.GetVisualizers(Model).ToList();
                    visualizers.Sort(x => x);
                    foreach (var visualizer in visualizers)
                    {
                        tabControl.Items.Add(new TabItem
                        {
                            Header = visualizer.Header,
                            Content = visualizer
                        });
                    }
                    var actions = ResponseActionRegistry.GetActions(Model).ToList();
                    if (actions.Any())
                    {
                        buttonPanel.Visibility = Visibility.Visible;

                        actions.Sort(x => x.Item1);
                        foreach (var item in actions)
                        {
                            var action = item.Item1;
                            var state = item.Item2;
                            if (state == ResponseActionState.Hidden)
                                continue;

                            var button = new Button
                            {
                                Content = action.Header,
                                ToolTip = action.ToolTip,
                                Focusable = false,
                                IsEnabled = state == ResponseActionState.Enabled
                            };
                            button.Click += async (sender, args) =>
                            {
                                var selectedTab = ((TabItem)tabControl.SelectedItem).Header;
                                await action.PerformAction(Model);
                                var recoveredTab = tabControl.Items.Cast<TabItem>().SingleOrDefault(x => x.Header == selectedTab);
                                if (recoveredTab != null)
                                    tabControl.SelectedItem = recoveredTab;
                            };
                            buttonPanel.Children.Add(button);
                        }
                    }
                }
            });
        }
    }
}
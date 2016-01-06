using System.Windows;
using System.Windows.Controls;

namespace Restless.WpfExtensions
{
    public class NameValuePanel
    {
        public static NameValuePanel<T> Create<T>(string name, T value)
            where T : FrameworkElement
        {
            return new NameValuePanel<T>(name, value);
        }
    }

    public class NameValuePanel<T> : Grid
        where T : FrameworkElement
    {
        public Label NameLabel { get; }
        public T Value { get; }

        public NameValuePanel(string name, T value)
        {
            this.AddColumn(1, GridUnitType.Star);
            this.AddRow(GridLength.Auto);
            this.AddRow(GridLength.Auto);

            NameLabel = new Label
            {
                Content = name
            };
            this.Add(NameLabel, 0, 0);

            Value = value;
            Value.Margin = new Thickness(5, 0, 5, 0);
            this.Add(Value, 1, 0);
        }
    }
}
using System.Windows;
using System.Windows.Controls;

namespace Restless.WpfExtensions
{
    public static class GridExtensions
    {
        public static void SetGridColumn(this UIElement element, int column)
        {
            Grid.SetColumn(element, column);
        }

        public static void SetGridRow(this UIElement element, int row)
        {
            Grid.SetRow(element, row);
        }

        public static void SetGridLocation(this UIElement element, int row, int column)
        {
            Grid.SetRow(element, row);
            Grid.SetColumn(element, column);
        }

        public static void Add(this Grid grid, UIElement child, int row, int column)
        {
            Grid.SetRow(child, row);
            Grid.SetColumn(child, column);
            grid.Children.Add(child);
        }

        public static ColumnDefinition AddColumn(this Grid grid, GridLength width)
        {
            var column = new ColumnDefinition
            {
                Width = width
            };
            grid.ColumnDefinitions.Add(column);
            return column;
        }

        public static ColumnDefinition AddColumn(this Grid grid, double width, GridUnitType unit = GridUnitType.Pixel)
        {
            var column = new ColumnDefinition
            {
                Width = new GridLength(width, unit)
            };
            grid.ColumnDefinitions.Add(column);
            return column;
        }

        public static RowDefinition AddRow(this Grid grid, double height, GridUnitType unit = GridUnitType.Pixel)
        {
            var row = new RowDefinition
            {
                Height = new GridLength(height, unit)
            };
            grid.RowDefinitions.Add(row);
            return row;
        }

        public static RowDefinition AddRow(this Grid grid, GridLength height)
        {
            var row = new RowDefinition
            {
                Height = height
            };
            grid.RowDefinitions.Add(row);
            return row;
        }
    }
}
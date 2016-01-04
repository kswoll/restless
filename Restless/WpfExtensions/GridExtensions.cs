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

        public static void AddColumn(this Grid grid, GridLength width)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = width
            });
        }

        public static void AddColumn(this Grid grid, int width, GridUnitType unit = GridUnitType.Pixel)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(width, unit)
            });
        }

        public static void AddRow(this Grid grid, int height, GridUnitType unit = GridUnitType.Pixel)
        {
            grid.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(height, unit)
            });
        }

        public static void AddRow(this Grid grid, GridLength height)
        {
            grid.RowDefinitions.Add(new RowDefinition
            {
                Height = height
            });
        }
    }
}
using System;
using System.Windows;
using System.Windows.Controls;

namespace Restless.WpfExtensions
{
    public static class GridSplitterExtensions
    {
        public static GridSplitter AddVerticalSplitter(this Grid grid, int minLeftWidth, int minRightWidth)
        {
            var splitter = grid.AddVerticalSplitter();

            grid.ColumnDefinitions[0].MinWidth = minLeftWidth;
            grid.SizeChanged += (sender, args) =>
            {
                grid.ColumnDefinitions[0].MaxWidth = args.NewSize.Width - grid.ColumnDefinitions[1].ActualWidth - minRightWidth;
            };

            return splitter;
        }

        public static GridSplitter AddVerticalSplitter(this Grid grid)
        {
            if (grid.ColumnDefinitions.Count != 3)
                throw new ArgumentException("A vertical splitter can only be added to a grid with three column definitions.");
            if (!grid.ColumnDefinitions[1].Width.IsAbsolute)
                throw new ArgumentException("Splitter column must be set to an absolute width");

            var splitter = new GridSplitter
            {
                Width = grid.ColumnDefinitions[1].Width.Value,
                ResizeDirection = GridResizeDirection.Columns,
                ResizeBehavior = GridResizeBehavior.PreviousAndNext,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            grid.Add(splitter, 0, 1);
            return splitter;
        }

        public static GridSplitter AddHorizontalSplitter(this Grid grid, int minTopHeight, int minBottomHeight)
        {
            var splitter = grid.AddHorizontalSplitter();

            grid.RowDefinitions[0].MinHeight = minTopHeight;
            grid.SizeChanged += (sender, args) =>
            {
                grid.RowDefinitions[0].MaxHeight = args.NewSize.Height - grid.RowDefinitions[1].ActualHeight - minBottomHeight;
            };

            return splitter;
        }

        public static GridSplitter AddHorizontalSplitter(this Grid grid)
        {
            if (grid.RowDefinitions.Count != 3)
                throw new ArgumentException("A vertical splitter can only be added to a grid with three row definitions.");
            if (!grid.RowDefinitions[1].Height.IsAbsolute)
                throw new ArgumentException("Splitter column must be set to an absolute height");

            var splitter = new GridSplitter
            {
                Height = grid.RowDefinitions[1].Height.Value,
                ResizeDirection = GridResizeDirection.Rows,
                ResizeBehavior = GridResizeBehavior.PreviousAndNext,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            grid.Add(splitter, 1, 0);
            return splitter;
        }
    }
}
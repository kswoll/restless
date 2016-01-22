using System.Windows;
using System.Windows.Controls;

namespace Restless.WpfExtensions
{
    public static class GridSplitterExtensions
    {
        public static GridSplitter AddVerticalSplitter(this Grid grid, int row, int column, int width = 4)
        {
            var splitter = new GridSplitter
            {
                Width = width,
                ResizeDirection = GridResizeDirection.Columns,
                ResizeBehavior = GridResizeBehavior.BasedOnAlignment,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            grid.Add(splitter, row, column);
            return splitter;
        }
    }
}
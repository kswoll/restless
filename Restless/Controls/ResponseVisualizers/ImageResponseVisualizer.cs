using System;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Restless.ViewModels;
using Restless.WpfExtensions;
using SexyReact.Views;

namespace Restless.Controls.ResponseVisualizers
{
    [ResponseVisualizer("image/jpg", "image/jpeg", "image/png", "image/gif")]
    public class ImageResponseVisualizer : RxDockPanel<ApiResponseModel>, IResponseVisualizer
    {
        public string Header => "Image";
        public int CompareTo(IResponseVisualizer other) => 0;
        public bool IsThisPrimary(IResponseVisualizer other) => false;

        public ImageResponseVisualizer()
        {
            var image = new Image();

            this.Add(image);

            this.Bind(x => x.Response).To(x =>
            {
                if (x != null)
                {
                    image.Source = BitmapFrame.Create(new MemoryStream(x));
                }
            });
        }
    }
}
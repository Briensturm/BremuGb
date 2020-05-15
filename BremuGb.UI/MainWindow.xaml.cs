using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BremuGb.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            FillImage();
        }

        private void FillImage()
        {
            int width = 640, height = 480, bytesperpixel = 4;
            int stride = width * bytesperpixel;
            byte[] imgdata = new byte[width * height * bytesperpixel];

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    // BGRA
                    imgdata[row * stride + col * 4 + 0] = 0;
                    imgdata[row * stride + col * 4 + 1] = Convert.ToByte((1 - (col / (float)width)) * 0xff);
                    imgdata[row * stride + col * 4 + 2] = Convert.ToByte((col / (float)width) * 0xff);
                    imgdata[row * stride + col * 4 + 3] = Convert.ToByte((row / (float)height) * 0xff);
                }
            }
            var gradient = BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgra32, null, imgdata, stride);

            MyImage.Source = gradient;
        }
    }
}

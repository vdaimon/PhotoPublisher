using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PhotoPublisher
{
    public class Window1DataContext:INPC
    {
        private BitmapImage _img;
        public BitmapImage Img { get => _img; set => Set(ref _img, value); }
        public double Height { get; }
        public double Width { get; }

        public Window1DataContext(Bitmap img)
        {
            Img = ToBitmapImage(img);
            Height = img.Height;
            Width = img.Width;
        }
        private BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
    }

    public partial class Window1 : Window
    {
        private Window1DataContext _datacontext;
        public Window1(Bitmap img)
        {
            InitializeComponent();
            _datacontext = new Window1DataContext(img);
            DataContext = _datacontext;
        }
    }
}

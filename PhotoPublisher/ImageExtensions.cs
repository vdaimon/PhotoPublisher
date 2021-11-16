using System.IO;
using System.Windows.Media.Imaging;
using SixLabors.ImageSharp;

namespace PhotoPublisher
{
    internal static class ImageExtensions
    {
        public static BitmapImage GetBitmapImage(this Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.SaveAsPng(stream);
                stream.Seek(0, SeekOrigin.Begin);

                BitmapImage bitmap = new BitmapImage();

                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();

                return bitmap;
            }

        }
    }
}

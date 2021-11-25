using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace PhotoPublisher
{
    public class ImageContainer:INPC
    {
        private double _currentHeight;
        private double _currentWidth;
        private System.Windows.Size _currentSize;
        private BitmapImage _originalImage;

        public double CurrentHeight { get => _currentHeight; set => Set(ref _currentHeight, value, () => SetCurrentSize()); }
        public double CurrentWidth { get => _currentWidth; set => Set(ref _currentWidth, value, () => SetCurrentSize()); }
        public System.Windows.Size CurrentSize { get => _currentSize; set => Set(ref _currentSize, value, () => SetCurrentSize(true)); }
        public BitmapImage OriginalImage { get => _originalImage; set => Set(ref _originalImage, value); }
        public Bitmap BitmapOriginalImage { get => BitmapImage2Bitmap(OriginalImage); }

        public ImageContainer(BitmapImage img)
        {
            OriginalImage = img;
            CurrentHeight = OriginalImage.Height;
            CurrentWidth = OriginalImage.Width;
        }

        private void SetCurrentSize(bool isSizeChanged = false)
        {
            if (isSizeChanged)
            {
                _currentHeight = _currentSize.Height;
                _currentWidth = _currentSize.Width;
            }
            else  _currentSize = new System.Windows.Size(CurrentWidth, CurrentHeight);
        }

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }
    }
}

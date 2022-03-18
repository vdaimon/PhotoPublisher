using System.Windows.Media.Imaging;

namespace PhotoPublisher
{
    public class ImageContainer:INPC
    {
        private double _currentHeight;
        private double _currentWidth;
        private System.Windows.Size _currentSize;
        private BitmapImage _bitmapImage;

        public double CurrentHeight { get => _currentHeight; set => Set(ref _currentHeight, value, () => SetCurrentSize()); }
        public double CurrentWidth { get => _currentWidth; set => Set(ref _currentWidth, value, () => SetCurrentSize()); }
        public System.Windows.Size CurrentSize { get => _currentSize; set => Set(ref _currentSize, value, () => SetCurrentSize(true)); }
        public BitmapImage BitmapImage { get => _bitmapImage; set => Set(ref _bitmapImage, value); }
        public SixLabors.ImageSharp.Image SixLaborsImage { get;}

        public ImageContainer(SixLabors.ImageSharp.Image original)
        {
            _bitmapImage = original.GetBitmapImage();
            _currentHeight = BitmapImage.Height;
            _currentWidth = BitmapImage.Width;
            SixLaborsImage = original;
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

    }
}

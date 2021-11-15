using System.Windows.Media.Imaging;

namespace PhotoPublisher
{
    public class ImageContainer:INPC
    {
        private readonly double _originalHeight;
        private readonly double _originalWidth;
        private double _currentHeight;
        private double _currentWidth;
        private double _currentSize;

        public BitmapImage OriginalImage { get; }
        public double CurrentHeight { get => _currentHeight; set => Set(ref _currentHeight, value); }
        public double CurrentWidth { get => _currentWidth; set => Set(ref _currentWidth, value); }

        public double CurrentSize { get => _currentSize; set => Set(ref _currentSize, value, ()=> Resize(value)); }
        public double OriginalHeight { get => _originalHeight; }
        public double OriginalWidth { get => _originalWidth; }
        public ImageContainer(BitmapImage img)
        {
            OriginalImage = img;
            _originalHeight = img.Height;
            _originalWidth = img.Width;
            CurrentSize = 100;
        }

        public void Resize(double percent)
        {
            CurrentHeight = _originalHeight/100*percent;
            CurrentWidth = _originalWidth/100*percent;
        }
    }
}

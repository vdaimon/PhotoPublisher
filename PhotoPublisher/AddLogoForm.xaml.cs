

using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Drawing;
using System.Drawing.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace PhotoPublisher
{
    public class AddLogoFormDataContext : INPC
    {
        private ImageContainer _logoImg = null;
        private bool _isMouseLeftButtonDown;
        private bool _isMouseInsideLogo;
        private bool _isParameterFormEnable;
        private System.Windows.Point _mouseLocation;
        private System.Windows.Point _currentLocation = new System.Windows.Point(0, 0);
        private System.Windows.Size _previousBaseSize;
        private double _currentBaseResizeHeightCoeff = 1;
        private double _currentBaseResizeWidthCoeff = 1;
        private double _originalBaseResizeHeightCoeff;
        private double _originalBaseResizeWidthCoeff;
        private double _originalSizeLogoCoeff;
        private double _sliderResizeLogoCoeff = 0.5;
        private double _logoOpacity = 1;


        public ImageContainer LogoImg { get => _logoImg; set => Set(ref _logoImg, value); }
        public ImageContainer BaseImg { get; }
        public System.Windows.Point CurrentLocation { get => _currentLocation; set => Set(ref _currentLocation, value); }
        public System.Windows.Point MouseLocation { get => _mouseLocation; set => Set(ref _mouseLocation, value); }
        public bool IsMouseLeftButtonDown { get => _isMouseLeftButtonDown; set => Set(ref _isMouseLeftButtonDown, value); }
        public bool IsMouseInsideLogo { get => _isMouseInsideLogo; set => Set(ref _isMouseInsideLogo, value); }
        public bool IsParameterFormEnable { get => _isParameterFormEnable; set => Set(ref _isParameterFormEnable, value); }
        public double SliderResizeLogoCoeff { get => _sliderResizeLogoCoeff; set => Set(ref _sliderResizeLogoCoeff, value, () => ResizeLogo()); }
        public double LogoOpacity { get => _logoOpacity; set => Set(ref _logoOpacity, value); }




        public AddLogoFormDataContext(ImageContainer baseImg)
        {
            BaseImg = baseImg;
        }

        public void ResizeBase(System.Windows.Size newSize)
        {
            if (_previousBaseSize != null)
                _previousBaseSize = new System.Windows.Size(BaseImg.CurrentWidth, BaseImg.CurrentHeight);

            if (newSize != null)
                BaseImg.CurrentSize = newSize;

            _currentBaseResizeHeightCoeff = BaseImg.CurrentHeight / _previousBaseSize.Height;
            _currentBaseResizeWidthCoeff = BaseImg.CurrentWidth / _previousBaseSize.Width;

            _originalBaseResizeHeightCoeff = BaseImg.CurrentHeight / BaseImg.BitmapImage.Height;
            _originalBaseResizeWidthCoeff = BaseImg.CurrentWidth / BaseImg.BitmapImage.Width;

            ResizeLogo(true);
        }
        private void ResizeLogo(bool reposition = false)
        {
            if (LogoImg != null)
            {
                LogoImg.CurrentHeight = LogoImg.BitmapImage.Height * _originalBaseResizeHeightCoeff * _originalSizeLogoCoeff * _sliderResizeLogoCoeff;
                LogoImg.CurrentWidth = LogoImg.BitmapImage.Width * _originalBaseResizeWidthCoeff * _originalSizeLogoCoeff * _sliderResizeLogoCoeff;

                if (reposition)
                {
                    SetCurrentLogoLocation(new System.Windows.Point(CurrentLocation.X * _currentBaseResizeWidthCoeff, CurrentLocation.Y * _currentBaseResizeHeightCoeff));
                    return;
                }

                var widthDiff = CurrentLocation.X + LogoImg.CurrentWidth - BaseImg.CurrentWidth;
                var heigthDiff = CurrentLocation.Y + LogoImg.CurrentHeight - BaseImg.CurrentHeight;

                if (widthDiff > 0)
                    SetCurrentLogoLocation(new System.Windows.Point(CurrentLocation.X - widthDiff, CurrentLocation.Y));
                if (heigthDiff > 0)
                    SetCurrentLogoLocation(new System.Windows.Point(CurrentLocation.X, CurrentLocation.Y - heigthDiff));
            }
        }

        public void SetCurrentLogoLocation(System.Windows.Point point)
        {
            if (point.X > BaseImg.CurrentWidth - LogoImg.CurrentWidth)
                point.X = BaseImg.CurrentWidth - LogoImg.CurrentWidth;
            if (point.Y > BaseImg.CurrentHeight - LogoImg.CurrentHeight)
                point.Y = BaseImg.CurrentHeight - LogoImg.CurrentHeight;
            if (point.X < 0)
                point.X = 0;
            if (point.Y < 0)
                point.Y = 0;

            CurrentLocation = point;
        }

        public void SetLogoImage(string logoPath)
        {
            IsParameterFormEnable = true;

            SixLabors.ImageSharp.Image logo = SixLabors.ImageSharp.Image.Load(logoPath);
            LogoImg = new ImageContainer(logo);

            _originalSizeLogoCoeff = Math.Min(BaseImg.BitmapImage.Height / LogoImg.BitmapImage.Height, BaseImg.BitmapImage.Width / LogoImg.BitmapImage.Width);

            ResizeLogo();
        }

        public void SaveResultImage()
        {
            var hCoeff  = BaseImg.SixLaborsImage.Height/BaseImg.CurrentHeight;
            var wCoeff = BaseImg.SixLaborsImage.Width/BaseImg.CurrentWidth;

            var x = CurrentLocation.X * wCoeff;
            var y = CurrentLocation.Y * hCoeff;
                    
            using (var newLogo = LogoImg.SixLaborsImage.Clone(ctx => ctx.Resize((int)Math.Round(LogoImg.CurrentWidth * wCoeff), (int)Math.Round(LogoImg.CurrentHeight * hCoeff))))
            using (var newImg = BaseImg.SixLaborsImage.Clone(ctx => ctx.DrawImage(newLogo, new SixLabors.ImageSharp.Point((int)Math.Round(x), (int)Math.Round(y)), (float)_logoOpacity)))
            {
                newImg.SaveAsJpeg("c:\\Temp\\resImg.jpg");

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "JPEG Image|*.jpg|Bitmap Image|*.bmp|PNG Image|*.png";
                saveFileDialog.ShowDialog();

                string path = saveFileDialog.FileName;
                string ext = System.IO.Path.GetExtension(path);

                switch (ext)
                {
                    case ".jpg":
                        newImg.SaveAsJpeg(path);
                        break;
                    case ".bmp":
                        newImg.SaveAsBmp(path);
                        break;
                    case ".png":
                        newImg.SaveAsPng(path);
                        break;

                }
            }
        }
    }

    public partial class AddLogoForm : Window
    {
        private AddLogoFormDataContext _dataContext;
        public AddLogoForm(ImageContainer baseImg)
        {
            _dataContext = new AddLogoFormDataContext(baseImg);
            DataContext = _dataContext;

            InitializeComponent();

        }

        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_dataContext.IsMouseInsideLogo)
            {
                _dataContext.IsMouseLeftButtonDown = true;
                _dataContext.MouseLocation = e.GetPosition((Canvas)sender);
            }
        }

        private void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _dataContext.IsMouseLeftButtonDown = false;
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            if (_dataContext.IsMouseLeftButtonDown)
            {
                var location = e.GetPosition((Canvas)sender);
                var diff = location - _dataContext.MouseLocation;
                _dataContext.SetCurrentLogoLocation(_dataContext.CurrentLocation + diff);
            }
        }

        private void LogoMouseEnter(object sender, MouseEventArgs e)
        {
            _dataContext.IsMouseInsideLogo = true;
        }

        private void LogoMouseLeave(object sender, MouseEventArgs e)
        {
            _dataContext.IsMouseInsideLogo = false;
            _dataContext.IsMouseLeftButtonDown = false;
        }

        private void LogoMouseMove(object sender, MouseEventArgs e)
        {
            _dataContext.MouseLocation = e.GetPosition(canvas);
        }

        private void OpenLogoButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                _dataContext.SetLogoImage(Path.GetFullPath(openFileDialog.FileName));
            }
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            _dataContext.SaveResultImage();
        }

        private void BaseSizeChanged(object sender, RoutedEventArgs e)
        {
            var img = (System.Windows.Controls.Image)sender;
            _dataContext.ResizeBase(img.RenderSize);
        }
    }
}

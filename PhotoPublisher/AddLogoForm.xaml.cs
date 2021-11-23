using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;

namespace PhotoPublisher
{
    public class AddLogoFormDataContext : INPC
    {
        private ImageContainer _logoImg = null;
        private bool _isMouseLeftButtonDown;
        private bool _isMouseInsideLogo;
        private System.Windows.Point _logoLocation;
        private System.Windows.Point _mouseLocation;
        private System.Windows.Size _currentBaseSize;
        private System.Windows.Point _currentLocation;
        private System.Windows.Size _previousBaseSize;
        private System.Windows.Size _logoSize;
        private double _baseHeightCoeff;
        private double _baseWidthCoeff;
        private double _logoHeightCoeff;
        private double _logoWidthCoeff;
        private double _resizeLogoCoeff;

        public ImageContainer LogoImg { get => _logoImg; set => Set(ref _logoImg, value); }
        public ImageContainer BaseImg { get; }
        public System.Windows.Point CurrentLocation { get => _currentLocation; set => Set(ref _currentLocation, value); }
        public System.Windows.Point MouseLocation { get => _mouseLocation; set => Set(ref _mouseLocation, value); }
        public bool IsMouseLeftButtonDown { get => _isMouseLeftButtonDown; set => Set(ref _isMouseLeftButtonDown, value); }
        public bool IsMouseInsideLogo { get => _isMouseInsideLogo; set => Set(ref _isMouseInsideLogo, value); }
        public System.Windows.Size LogoSize { get => _logoSize; set => Set(ref _logoSize, value, () => SetLogoCoeff()); }
        public double ResizeLogoCoeff { get => _resizeLogoCoeff; set => Set(ref _resizeLogoCoeff, value, () => ResizeLogo()); }
        
        private void DebugInfo(string msg)
        {
            System.Diagnostics.Debug.WriteLine(msg);
        }
        public AddLogoFormDataContext(ImageContainer baseImg)
        {
            _isMouseLeftButtonDown = false;
            BaseImg = baseImg;
            CurrentLocation = new System.Windows.Point(0, 0);
            ResizeLogoCoeff = 1;
        }

        private void SetLogoCoeff()
        {
            _logoHeightCoeff = _currentBaseSize.Height / _logoSize.Height;
            _logoWidthCoeff = _currentBaseSize.Width / _logoSize.Width;
        }
        public void ResizeBase(System.Windows.Size newSize)
        {
            if (_previousBaseSize != null)
                _previousBaseSize = _currentBaseSize;
                
            if (newSize != null)
                _currentBaseSize = newSize;

            _baseHeightCoeff = _currentBaseSize.Height / _previousBaseSize.Height;
            _baseWidthCoeff = _currentBaseSize.Width / _previousBaseSize.Width;

            ResizeLogo();
        }
        private void ResizeLogo()
        {
            if (LogoImg != null)
            {
                LogoImg.CurrentHeight = LogoSize.Height * _baseHeightCoeff * _logoHeightCoeff * _resizeLogoCoeff;
                LogoImg.CurrentWidth = LogoSize.Width * _baseWidthCoeff * _logoWidthCoeff * ResizeLogoCoeff;

                DebugInfo($"x-{CurrentLocation.X}, y-{CurrentLocation.Y}");
                DebugInfo($"heightCoeff-{_baseHeightCoeff}, widthCoeff-{_baseWidthCoeff}");

                if (_baseWidthCoeff > double.MaxValue || _baseWidthCoeff < double.MinValue)
                    return;

                CurrentLocation = new System.Windows.Point(CurrentLocation.X * _baseWidthCoeff, CurrentLocation.Y * _baseHeightCoeff);
            }
        }

        public void SetLogoImage(string logoPath)
        {
            using (SixLabors.ImageSharp.Image logo = SixLabors.ImageSharp.Image.Load(logoPath))
            {
                LogoImg = new ImageContainer(logo.GetBitmapImage());
            }
            LogoImg.CurrentHeight = LogoImg.OriginalImage.Height * BaseImg.OriginalImage.Height / BaseImg.CurrentHeight;
            LogoImg.CurrentWidth = LogoImg.OriginalImage.Width * BaseImg.OriginalImage.Width / BaseImg.CurrentWidth;
        }

        public Bitmap SaveResultImage()
        {
            var basePic = BaseImg.BitmapOriginalImage;
            var logoPic = LogoImg.BitmapOriginalImage;
            logoPic.MakeTransparent();
            Bitmap res = new Bitmap(basePic.Width,basePic.Height, PixelFormat.Format32bppArgb);

            var x = _logoLocation.X * basePic.Width/ _currentBaseSize.Width;
            var y = _logoLocation.Y * basePic.Height / _currentBaseSize.Height; 

            System.Diagnostics.Debug.WriteLine(x.ToString());
            System.Diagnostics.Debug.WriteLine(y.ToString());

            Graphics g = Graphics.FromImage(res);

            g.DrawImage(basePic, 0, 0, (Single)basePic.Width, (Single)basePic.Height);
            g.Flush();
            g.DrawImage(logoPic, (Single)x, (Single)y, (Single)logoPic.Width, (Single)logoPic.Height);
            g.Flush();
            return res;
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

        private void MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_dataContext.IsMouseInsideLogo)
            {
                _dataContext.IsMouseLeftButtonDown = true;
                _dataContext.MouseLocation = e.GetPosition((Canvas)sender);
            }
        }

        private void MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _dataContext.IsMouseLeftButtonDown = false;
        }

        private void MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_dataContext.IsMouseLeftButtonDown)
            {
                var location = e.GetPosition((Canvas)sender);
                var diff = location - _dataContext.MouseLocation;
                _dataContext.CurrentLocation += diff;
            }
        }

        private void Canvas_MouseEnter(object sender, MouseEventArgs e)
        {
            _dataContext.IsMouseInsideLogo = true;
        }

        private void Canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            _dataContext.IsMouseInsideLogo = false;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
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
            new Window1(_dataContext.SaveResultImage()).ShowDialog();
        }

        private void BaseSizeChanged(object sender, RoutedEventArgs e)
        {
            var img = (System.Windows.Controls.Image)sender;
            _dataContext.ResizeBase(img.RenderSize);
        }
        private void LogoSizeChanged(object sender, RoutedEventArgs e)
        {
                var img = (System.Windows.Controls.Image)sender;
                _dataContext.LogoSize = img.RenderSize;
        }
    }
}

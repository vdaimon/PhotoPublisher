using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Drawing;
using System.Drawing.Imaging;

namespace PhotoPublisher
{
    public class AddLogoFormDataContext : INPC
    {
        private ImageContainer _logoImg = null;
        private bool _isMouseLeftButtonDown;
        private bool _isMouseInsideLogo;
        private System.Windows.Point _mouseLocation;
        private System.Windows.Size _currentBaseSize;
        private System.Windows.Point _currentLocation;
        private System.Windows.Size _previousBaseSize;
        private double _baseHeightCoeff;
        private double _baseWidthCoeff;
        private double _logoHeightCoeff;
        private double _logoWidthCoeff;
        private double _resizeLogoCoeff;
        private double _maxLogoResizeCoeff;

        public ImageContainer LogoImg { get => _logoImg; set => Set(ref _logoImg, value); }
        public ImageContainer BaseImg { get; }
        public System.Windows.Point CurrentLocation { get => _currentLocation; set => Set(ref _currentLocation, value); }
        public System.Windows.Point MouseLocation { get => _mouseLocation; set => Set(ref _mouseLocation, value); }
        public bool IsMouseLeftButtonDown { get => _isMouseLeftButtonDown; set => Set(ref _isMouseLeftButtonDown, value); }
        public bool IsMouseInsideLogo { get => _isMouseInsideLogo; set => Set(ref _isMouseInsideLogo, value); }
        public double ResizeLogoCoeff { get => _resizeLogoCoeff; set => Set(ref _resizeLogoCoeff, value, () => ResizeLogo()); }

        public double MaxLogoResizeCoeff { get => _maxLogoResizeCoeff; set => Set(ref _maxLogoResizeCoeff, value); }




        private void DebugInfo(string msg)
        {
            System.Diagnostics.Debug.WriteLine(msg);
        }
        public AddLogoFormDataContext(ImageContainer baseImg)
        {
            _isMouseLeftButtonDown = false;
            BaseImg = baseImg;
            CurrentLocation = new System.Windows.Point(0, 0);
            _baseHeightCoeff = 1;
            _baseWidthCoeff = 1;
            ResizeLogoCoeff = 0.5;
            MaxLogoResizeCoeff = 1;
        }


        public void ResizeBase(System.Windows.Size newSize)
        {
            if (_previousBaseSize != null)
                _previousBaseSize = _currentBaseSize;

            if (newSize != null)
                _currentBaseSize = newSize;

            _baseHeightCoeff = _currentBaseSize.Height / _previousBaseSize.Height;
            _baseWidthCoeff = _currentBaseSize.Width / _previousBaseSize.Width;

            ResizeLogo(true);
        }
        private void ResizeLogo(bool reposition = false)
        {
            if (LogoImg != null)
            {
                LogoImg.CurrentHeight = LogoImg.OriginalImage.Height * _baseHeightCoeff * _logoHeightCoeff * _resizeLogoCoeff;
                LogoImg.CurrentWidth = LogoImg.OriginalImage.Width * _baseWidthCoeff * _logoWidthCoeff * _resizeLogoCoeff;

                if (reposition)
                    SetCurrentLogoLocation(new System.Windows.Point(CurrentLocation.X * _baseWidthCoeff, CurrentLocation.Y * _baseHeightCoeff));


                var widthDiff = CurrentLocation.X + LogoImg.CurrentWidth - _currentBaseSize.Width;
                var heigthDiff = CurrentLocation.Y + LogoImg.CurrentHeight - _currentBaseSize.Height;

                if (widthDiff > 0)
                    SetCurrentLogoLocation(new System.Windows.Point(CurrentLocation.X - widthDiff, CurrentLocation.Y));
                if (heigthDiff > 0)
                    SetCurrentLogoLocation(new System.Windows.Point(CurrentLocation.X, CurrentLocation.Y - heigthDiff));
            }
        }

        public void SetCurrentLogoLocation(System.Windows.Point point)
        {
            if (point.X > _currentBaseSize.Width - LogoImg.CurrentWidth)
                point.X = _currentBaseSize.Width - LogoImg.CurrentWidth;
            if (point.Y > _currentBaseSize.Height - LogoImg.CurrentHeight)
                point.Y = _currentBaseSize.Height - LogoImg.CurrentHeight;
            if (point.X < 0)
                point.X = 0;
            if (point.Y < 0)
                point.Y = 0;

            CurrentLocation = point;
            MaxLogoResizeCoeff = Math.Min(LogoImg.CurrentHeight/(_currentBaseSize.Height + CurrentLocation.Y), LogoImg.CurrentWidth/(_currentBaseSize.Width + CurrentLocation.X));

        }

        public void SetLogoImage(string logoPath)
        {
            using (SixLabors.ImageSharp.Image logo = SixLabors.ImageSharp.Image.Load(logoPath))
            {
                LogoImg = new ImageContainer(logo.GetBitmapImage());
            }

            var c = Math.Min(BaseImg.OriginalImage.Height / LogoImg.OriginalImage.Height, BaseImg.OriginalImage.Width / LogoImg.OriginalImage.Width);

            _logoHeightCoeff = _currentBaseSize.Height / BaseImg.OriginalImage.Height * c;
            _logoWidthCoeff = _currentBaseSize.Width / BaseImg.OriginalImage.Width * c;

            ResizeLogo();
        }

        public Bitmap SaveResultImage()
        {
            var basePic = BaseImg.BitmapOriginalImage;
            var logoPic = LogoImg.BitmapOriginalImage;
            logoPic.MakeTransparent();
            Bitmap res = new Bitmap(basePic.Width,basePic.Height, PixelFormat.Format32bppArgb);

            var x = CurrentLocation.X * basePic.Width/ _currentBaseSize.Width;
            var y = CurrentLocation.Y * basePic.Height / _currentBaseSize.Height; 

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
                _dataContext.SetCurrentLogoLocation(_dataContext.CurrentLocation + diff);
            }
        }

        private void Canvas_MouseEnter(object sender, MouseEventArgs e)
        {
            _dataContext.IsMouseInsideLogo = true;
        }

        private void Canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            _dataContext.IsMouseInsideLogo = false;
            _dataContext.IsMouseLeftButtonDown = false;
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
    }
}

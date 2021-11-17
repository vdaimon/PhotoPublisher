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
        public ImageContainer LogoImg { get => _logoImg; set => Set(ref _logoImg, value); }
        public ImageContainer BaseImg { get; }

        private double _maxChangesSize = 0;
        private bool _isMouseLeftButtonDown;
        private bool _isMouseInsideLogo;
        private System.Windows.Point _logoLocation;
        private System.Windows.Point _mouseLocation;
        private System.Windows.Size _imageSize;

        public System.Windows.Point LogoLocation { get => _logoLocation; set => Set(ref _logoLocation, value); }
        public bool IsMouseLeftButtonDown { get => _isMouseLeftButtonDown; set => Set(ref _isMouseLeftButtonDown, value); }
        public bool IsMouseInsideLogo { get => _isMouseInsideLogo; set => Set(ref _isMouseInsideLogo, value); }
        public System.Windows.Point MouseLocation { get => _mouseLocation; set => Set(ref _mouseLocation, value); }
        public System.Windows.Size ImageSize { get => _imageSize; set => Set(ref _imageSize, value); }

        public double MaxChangesSize
        {
            get => _maxChangesSize;
            set
            {
                Set(ref _maxChangesSize,
                    Math.Min(BaseImg.CurrentWidth / LogoImg.OriginalImage.Width * 100, BaseImg.CurrentHeight / LogoImg.OriginalImage.Height * 100));
            }
        }

        public AddLogoFormDataContext(ImageContainer baseImg)
        {
            _isMouseLeftButtonDown = false;
            LogoLocation = new System.Windows.Point(0, 0);
            BaseImg = baseImg;
        }

        public void SetLogoImage(string logoPath)
        {
            using (SixLabors.ImageSharp.Image logo = SixLabors.ImageSharp.Image.Load(logoPath))
            {
                LogoImg = new ImageContainer(logo.GetBitmapImage());
            }
            MaxChangesSize = 0;
        }

        public void SaveToFile()
        {
        }
        public Bitmap SaveResultImage()
        {
            var basePic = BaseImg.BitmapOriginalImage;
            var logoPic = LogoImg.BitmapOriginalImage;
            logoPic.MakeTransparent();
            Bitmap res = new Bitmap(basePic.Width,basePic.Height, PixelFormat.Format32bppArgb);

            var x = LogoLocation.X * basePic.HorizontalResolution/ ImageSize.Width;
            var y = LogoLocation.Y * basePic.VerticalResolution / ImageSize.Height; 

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
                _dataContext.LogoLocation += diff;
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
            _dataContext.MouseLocation = e.GetPosition(Owner);
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
            _dataContext.SaveToFile();
            new Window1(_dataContext.SaveResultImage()).ShowDialog();
        }

        private void BaseSizeChanged(object sender, RoutedEventArgs e)
        {
            var img = (System.Windows.Controls.Image)sender;
            _dataContext.ImageSize = img.RenderSize;
        }
    }
}

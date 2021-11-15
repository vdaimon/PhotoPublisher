using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using SixLabors.ImageSharp;

namespace PhotoPublisher
{
    public class MainWindowDataContext : INPC
    {
        private ImageContainer _logoImg = null;
        private ImageContainer _baseImg = null;
        public ImageContainer BaseImg { get => _baseImg; set => Set(ref _baseImg, value); }
        public ImageContainer LogoImg { get => _logoImg; set => Set(ref _logoImg, value); }

        
        private bool _isMouseLeftButtonDown;
        private bool _isMouseInsideLogo;
        private System.Windows.Point _logoLocation;
        private System.Windows.Point _mouseLocation;

        public System.Windows.Point LogoLocation { get => _logoLocation; set => Set(ref _logoLocation, value); }
        public bool IsMouseLeftButtonDown { get => _isMouseLeftButtonDown; set => Set(ref _isMouseLeftButtonDown, value); }
        public bool IsMouseInsideLogo { get => _isMouseInsideLogo; set => Set(ref _isMouseInsideLogo, value); }
        public System.Windows.Point MouseLocation { get => _mouseLocation; set => Set(ref _mouseLocation, value); }

        public double MaxChangesSize
        {
            get
            {
                double h = BaseImg.CurrentHeight / LogoImg.OriginalHeight *100;
                double w = BaseImg.CurrentWidth / LogoImg.OriginalWidth *100;
                return Math.Min(h, w);
            }

        }


        public MainWindowDataContext(string basePath, string logoPath)
        {
            _isMouseLeftButtonDown = false;
            LogoLocation = new System.Windows.Point(0, 0);

            using (SixLabors.ImageSharp.Image img = SixLabors.ImageSharp.Image.Load(basePath))
            {
                BaseImg = new ImageContainer(img.GetBitmapImage());
            }
            using (SixLabors.ImageSharp.Image logo = SixLabors.ImageSharp.Image.Load(logoPath))
            {
                LogoImg = new ImageContainer(logo.GetBitmapImage());
            }
        }
    }
    public partial class MainWindow : Window
    {
        private MainWindowDataContext _dataContext = new MainWindowDataContext(@"c:\temp\TestImg.jpg", @"c:\temp\TestLogo.png");
        public MainWindow()
        {
            InitializeComponent();

            DataContext = _dataContext;
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
                _dataContext.LogoLocation += diff;// new System.Windows.Point(_dataContext.LogoLocation.X + diff.X, _dataContext.LogoLocation.Y + diff.Y);
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
    }
}

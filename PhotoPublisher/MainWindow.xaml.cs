using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using SixLabors.ImageSharp;

namespace PhotoPublisher
{
    public class MainWindowDataContext : INPC
    {
        private ImageContainer _baseImg = null;
        public ImageContainer BaseImg { get => _baseImg; set => Set(ref _baseImg, value); }

        public MainWindowDataContext()
        {

        }

        public void SetBaseImage(string basePath)
        {
            SixLabors.ImageSharp.Image img = SixLabors.ImageSharp.Image.Load(basePath);
            BaseImg = new ImageContainer(img.GetBitmapImage(), img);
        }
    }
    public partial class MainWindow : Window
    {
        private MainWindowDataContext _dataContext = new MainWindowDataContext();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = _dataContext;
        }

        private void AddLogoButtonClick(object sender, RoutedEventArgs e)
        {
            new AddLogoForm(_dataContext.BaseImg).ShowDialog();
        }

        private void OpenImageButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                _dataContext.SetBaseImage(Path.GetFullPath(openFileDialog.FileName));
            }
        }
    }
}

using System.IO;
using System.Windows;
using Microsoft.Win32;


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
            BaseImg = new ImageContainer(img);
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

        private void EditMetaButtonClick(object sender, RoutedEventArgs e)
        {
            new EditMetadataForm(_dataContext.BaseImg).ShowDialog();
        }
    }
}

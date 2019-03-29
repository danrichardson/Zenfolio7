using Zenfolio7.Zenfolio;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Zenfolio7.ZenfolioBrowser
{
    public partial class ZenfolioBrowserControl : UserControl
    {
        readonly ZenfolioBrowserViewModel _viewModel;
        //private DispatcherTimer timer;
        //private Uri imageUri;

        public ZenfolioBrowserControl()
        {
            InitializeComponent();

            // Create UI-friendly wrappers around the 
            // raw data objects (i.e. the view-model).
            _viewModel = new ZenfolioBrowserViewModel();

            // Let the UI bind to the view-model.
            base.DataContext = _viewModel;
            
        }

        private void ImageLoaded(object sender, EventArgs e)
        {
            ((BitmapSource)(Thumbnail.Source)).DownloadCompleted -= ImageLoaded;
            LoadingImageStatusBar.Visibility = Visibility.Hidden;
            Thumbnail.Visibility = Visibility.Visible;
        }

        void searchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                _viewModel.SearchCommand.Execute(null);
        }

        private void MenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if ((sender as MenuItem).DataContext as Photo != null)
            {
                MyPopup.IsOpen = false;
                var photo = (sender as MenuItem).DataContext as Photo;
                var url = photo.OriginalUrl;
                var uri = new Uri(url);
                Thumbnail.Source = new BitmapImage(uri);
                ((BitmapSource)(Thumbnail.Source)).DownloadCompleted += ImageLoaded;
                Thumbnail.Visibility = Visibility.Hidden;
                LoadingImageStatusBar.Visibility = Visibility.Visible;
                MyPopup.IsOpen = true;
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MyPopup.IsOpen = false;
        }

        private void SelectedPhotos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //See this: https://www.zenfolio.com/zf/help/api/guide/download
            if (sender as ListView != null && (sender as ListView).SelectedItem != null)
            {
                MyPopup.IsOpen = false;
                var photo = (sender as ListView).SelectedItem as Photo;
                int Size = 2; //400x400
                var url = $"http://{photo.UrlHost}/{photo.UrlCore}-{Size}.jpg?sn={photo.Sequence}&tk={photo.UrlToken}";
                var imageUri = new Uri(url);
                Thumbnail.Source = new BitmapImage(imageUri);
                ((BitmapSource)(Thumbnail.Source)).DownloadCompleted += ImageLoaded;
                Thumbnail.Visibility = Visibility.Hidden;
                LoadingImageStatusBar.Visibility = Visibility.Visible;
                MyPopup.IsOpen = true;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender as PasswordBox != null)
            {
                var passwordBox = sender as PasswordBox;
                _viewModel.Password = passwordBox.Password;
            }
        }
    }
}
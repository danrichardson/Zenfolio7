using ImageMagick;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Zenfolio7.Model;
using Zenfolio7.Utilities;
using Zenfolio7.Zenfolio;
using Zenfolio7.ZenfolioBrowser.ViewModel;

namespace Zenfolio7.ZenfolioBrowser
{
    /// <summary>
    /// Interaction logic for ContactSheet.xaml
    /// </summary>
    public partial class ContactSheet : Window
    {
        private ContactSheetViewModel _viewModel;

        private const int MaxSize = 500;
        private const int MinSize = 50;

        //public Popup ContactSheepPopup;

        public ContactSheet(Photo[] photos)
        {
            InitializeComponent();

            _viewModel = new ContactSheetViewModel();
            _viewModel.LoadPhotos(photos);
            //ContactSheepPopup = Popup;

            base.DataContext = _viewModel;
        }

        private void SelectedPhotos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //See this: https://www.zenfolio.com/zf/help/api/guide/download
            if (sender as ListView != null && (sender as ListView).SelectedItem != null)
            {
                ContactSheetPopup.IsOpen = false;
                var photo = (sender as ListView).SelectedItem as Photo;
                int Size = 2; //400x400
                var url = $"http://{photo.UrlHost}/{photo.UrlCore}-{Size}.jpg?sn={photo.Sequence}&tk={photo.UrlToken}";
                var imageUri = new Uri(url);
                Thumbnail.Source = new BitmapImage(imageUri);
                ((BitmapSource)(Thumbnail.Source)).DownloadCompleted += ImageLoaded;
                Thumbnail.Visibility = Visibility.Hidden;
                LoadingImageStatusBar.Visibility = Visibility.Visible;
                ContactSheetPopup.IsOpen = true;
            }
        }

        private void ImageLoaded(object sender, EventArgs e)
        {
            ((BitmapSource)(Thumbnail.Source)).DownloadCompleted -= ImageLoaded;
            LoadingImageStatusBar.Visibility = Visibility.Hidden;
            Thumbnail.Visibility = Visibility.Visible;
        }

        private async void ShowPopup(object sender, RoutedEventArgs e)
        {
            //See this: https://www.zenfolio.com/zf/help/api/guide/download
            if (sender as Button != null && (sender as Button).DataContext != null)
            {
                ContactSheetPopup.IsOpen = true;
                Thumbnail.Visibility = Visibility.Hidden;
                LoadingImageStatusBar.Visibility = Visibility.Visible;
                //ContactSheetPopup.IsOpen = false;
                var photo = ((sender as Button).DataContext as DisplayPhoto).PhotoObject;
                int Size = 4; //800x630
                var url = $"http://{photo.UrlHost}/{photo.UrlCore}-{Size}.jpg?sn={photo.Sequence}&tk={photo.UrlToken}";
                var uri = new Uri(url);
                byte[] data;
                ExifProfile profile;
                using (var client = new AwesomeWebClient())
                {
                    client.Proxy = null;
                    data = await client.DownloadDataTaskAsync(uri);
                    var magickImage = new MagickImage(data);
                    profile = magickImage.GetExifProfile();
                }

                var imageUri = new Uri(url);
                //MagickImage magickImage = new MagickImage(new BitmapImage(imageUri).);
                Thumbnail.Source = LoadImage(data);
                LoadingImageStatusBar.Visibility = Visibility.Hidden;
                Thumbnail.Visibility = Visibility.Visible;

                Thumbnail.ToolTip = MakeTooltip(profile);
                //((BitmapSource)(Thumbnail.Source)).DownloadCompleted += ImageLoaded;
                //Thumbnail.Visibility = Visibility.Hidden;
                //LoadingImageStatusBar.Visibility = Visibility.Visible;
                ContactSheetPopup.IsOpen = true;
            }
        }

        private string MakeTooltip(ExifProfile profile)
        {
            var tooltip = "StuffAndThings";
            if (profile != null)
            {
                tooltip = string.Empty;
                foreach(var item in profile.Values)
                {
                    tooltip += item.ToString() + "\n";
                }
            }
            return tooltip;
        }

        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ContactSheetPopup.IsOpen = false;
        }

        private void ThumbDragDelta(object sender,
          DragDeltaEventArgs e)
        {
            Thumb t = sender as Thumb;

            if (t.Cursor == Cursors.SizeWE
              || t.Cursor == Cursors.SizeNWSE)
            {
                ContactSheetPopup.Width = Math.Min(MaxSize,
                  Math.Max(ContactSheetPopup.Width + e.HorizontalChange,
                  MinSize));
            }

            if (t.Cursor == Cursors.SizeNS
              || t.Cursor == Cursors.SizeNWSE)
            {
                ContactSheetPopup.Height = Math.Min(MaxSize,
                  Math.Max(ContactSheetPopup.Height + e.VerticalChange,
                  MinSize));
            }
        }

        private void ThumbDragStarted(object sender,
          DragStartedEventArgs e)
        {
            //This is called when the user
            //starts dragging the thumb
        }

        private void ThumbDragCompleted(object sender,
          DragCompletedEventArgs e)
        {
            //This is called when the user
            //finishes dragging the thumb
        }
    }






}
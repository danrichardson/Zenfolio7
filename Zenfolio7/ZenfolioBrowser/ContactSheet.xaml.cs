using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Zenfolio7.Zenfolio;

namespace Zenfolio7.ZenfolioBrowser
{
    /// <summary>
    /// Interaction logic for ContactSheet.xaml
    /// </summary>
    public partial class ContactSheet : Window
    {
        private double[] _imageWidths = new double[] { 80, 60, 400, 580, 800, 1100, 1550, 0, 0, 0, 120, 120 };
        private double[] _imageHeights = new double[] { 80, 60, 400, 450, 630, 850, 960, 0, 0, 0, 120, 120 };
        private const int MaxSize = 500;
        private const int MinSize = 50;
        private List<Photo> _photos { get; set; }
        private int _photoSize = 0; //up to 80x80 thumbnail 0 -> 6 is the range
        private ConcurrentBag<DisplayPhoto> _photoCollection;
        public ObservableCollection<DisplayPhoto> PhotoCollection { get; set; }
        public double DefaultImageHeight { get; set; }
        public double DefaultImageWidth { get; set; }
        private Popup _MyPopup;

        public ContactSheet(Photo[] photos)
        {
            PhotoCollection = new ObservableCollection<DisplayPhoto>();
            _photos = new List<Photo>(photos);
            LoadPhotos(_photos);
            InitializeComponent();
            _photos = new List<Photo>(photos);
            _MyPopup = Resources["myPopup"] as Popup;
        }

        public void LoadPhotos(List<Photo> photos)
        {
            PhotoCollection.Clear();

            DefaultImageHeight = _imageWidths[_photoSize];
            DefaultImageWidth = _imageWidths[_photoSize];
            _photoCollection = new ConcurrentBag<DisplayPhoto>();
            var parallelOptions = new ParallelOptions();
            parallelOptions.MaxDegreeOfParallelism = 4;
            //Parallel.ForEach(photos, parallelOptions, photo => // (var photo in photos)
            int numImages = photos.Count;
            int currentImage = 1;
            //foreach (var photo in photos)
            Parallel.ForEach(photos, parallelOptions, photo => // (var photo in photos)
            {
                BitmapImage image;
                var url = $"http://{photo.UrlHost}/{photo.UrlCore}-{_photoSize}.jpg?sn={photo.Sequence}&tk={photo.UrlToken}";
                try
                {
                    image = DownloadImage(numImages, currentImage, photo, url);
                    currentImage++;
                    //Console.WriteLine($"Downloading photo {currentImage} of {numImages}");
                }
                catch (Exception ex)
                {
                    //try again
                    Console.WriteLine($"Trying to download photo {currentImage} again");
                    image = DownloadImage(numImages, currentImage, photo, url);
                    currentImage++;
                }
                _photoCollection.Add(new DisplayPhoto(photo, image, new Uri(url), DefaultImageHeight, DefaultImageWidth));

            });
            PhotoCollection.Clear();
            PhotoCollection.AddRange(_photoCollection);
        }

        private BitmapImage DownloadImage(int numImages, int currentImage, Photo photo, string url)
        {
            BitmapImage image = new BitmapImage();
            bool success = false;
            int numTries = 0;
            while (!success)
            {
                try
                {
                    using (var client = new AwesomeWebClient())
                    {
                        client.Proxy = null;
                        var data = client.DownloadData(url);
                        image = LoadImage(data);
                        //PhotoCollection.Add(new DisplayPhoto(photo, image, new Uri(url), DefaultImageHeight, DefaultImageWidth));
                        //Console.WriteLine($"Downloaded photo {currentImage++} of {numImages}");
                        success = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Download photo {currentImage} of {numImages} failed, trying again");
                    numTries++;
                    if (numTries > 3)
                    {
                        //bail
                        success = true;
                    }
                }
            }

            return image;
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
        private void ShowPopup(object sender, RoutedEventArgs e)
        {
            _MyPopup.IsOpen = true;
        }

        private void ThumbDragDelta(object sender,
          DragDeltaEventArgs e)
        {
            Thumb t = sender as Thumb;

            if (t.Cursor == Cursors.SizeWE
              || t.Cursor == Cursors.SizeNWSE)
            {
                _MyPopup.Width = Math.Min(MaxSize,
                  Math.Max(_MyPopup.Width + e.HorizontalChange,
                  MinSize));
            }

            if (t.Cursor == Cursors.SizeNS
              || t.Cursor == Cursors.SizeNWSE)
            {
                _MyPopup.Height = Math.Min(MaxSize,
                  Math.Max(_MyPopup.Height + e.VerticalChange,
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

        private void ShrinkIt(object sender, RoutedEventArgs e)
        {
            if (_photoSize > 0)
            {
                _photoSize--;
                LoadPhotos(_photos);
            }
        }

        private void Embiggen(object sender, RoutedEventArgs e)
        {
            if (_photoSize < 5)
            {
                _photoSize++;
                LoadPhotos(_photos);
            }
        }
        private void SmallThumbnail(object sender, RoutedEventArgs e)
        {
            _photoSize = 0;
            LoadPhotos(_photos);
        }
        private void MediumThumbnail(object sender, RoutedEventArgs e)
        {
            _photoSize = 10;    
            LoadPhotos(_photos);
        }
        private void LargeThumbnail(object sender, RoutedEventArgs e)
        {
            _photoSize = 11;
            LoadPhotos(_photos);
        }

    }

    public class DisplayPhoto
    {
        public Photo PhotoObject { get; set; }
        public Uri PhotoURI { get; set; }
        public BitmapImage Image { get; set; }
        public double ImageHeight { get; set; }
        public double ImageWidth { get; set; }
        public DisplayPhoto(Photo photoObject, BitmapImage image, Uri photoURI, double imageHeight, double imageWidth)
        {
            PhotoObject = photoObject;
            PhotoURI = photoURI;
            ImageHeight = imageHeight;
            ImageWidth = imageWidth;
            Image = image;
            //BitmapImage src = new BitmapImage();
            //src.BeginInit();
            //src.UriSource = PhotoURI;
            //src.CacheOption = BitmapCacheOption.OnLoad;
            //src.DownloadProgress += progress;
            //src.EndInit();
            //src.DownloadCompleted += completed;
            //src.DownloadFailed += failed;
            //src.
            
        }

        //private void progress(object sender, DownloadProgressEventArgs e)
        //{
        //    Console.WriteLine("Image download progress");
        //}

        //private void failed(object sender, ExceptionEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        //private void completed(object sender, EventArgs e)
        //{

        //    Image = sender as BitmapImage;
        //}
    }
    public class MyWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            w.Timeout = 3000; //in milliseconds
            return w;
        }
    }

    public class AwesomeWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest req = (HttpWebRequest)base.GetWebRequest(address);
            req.ServicePoint.ConnectionLimit = 20;
            req.Timeout = 10000; //in milliseconds
            return (WebRequest)req;
        }
    }

    public static class ExtensionMethods
    {
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            items.ToList().ForEach(collection.Add);
        }
    }
}
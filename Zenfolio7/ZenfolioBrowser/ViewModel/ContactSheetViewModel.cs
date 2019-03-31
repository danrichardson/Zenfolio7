using ImageMagick;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Zenfolio7.Model;
using Zenfolio7.Utilities;
using Zenfolio7.View.ViewModel;
using Zenfolio7.Zenfolio;

namespace Zenfolio7.ZenfolioBrowser.ViewModel
{
    public class ContactSheetViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private double[] _imageWidths = new double[] { 80, 60, 400, 580, 800, 1100, 1550, 0, 0, 0, 120, 120 };
        private double[] _imageHeights = new double[] { 80, 60, 400, 450, 630, 850, 960, 0, 0, 0, 120, 120 };
        private List<Photo> _photos { get; set; }
        private int _photoSize = 0; //up to 80x80 thumbnail 0 -> 6 is the range
        public ConcurrentBag<DisplayPhoto> PhotoBag { get; set; }
        public void OnPhotoBagChanged()
        {
            App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                PhotoCollection.Clear();
                PhotoCollection.AddRange(PhotoBag);
            });
        }
        public ObservableCollection<DisplayPhoto> PhotoCollection { get; set; }
        public double DefaultImageHeight { get; set; }
        public double DefaultImageWidth { get; set; }

        public RelayCommand ShrinkItCommand {get; set;}
        public RelayCommand EmbiggenCommand {get; set;}
        public RelayCommand SmallThumbnailCommand {get; set;}
        public RelayCommand MediumThumbnailCommand {get; set;}
        public RelayCommand LargeThumbnailCommand { get; set;}

        public ContactSheetViewModel()
        {
            ShrinkItCommand = new RelayCommand(ShrinkIt);
            EmbiggenCommand = new RelayCommand(Embiggen);
            SmallThumbnailCommand = new RelayCommand(SmallThumbnail);
            MediumThumbnailCommand = new RelayCommand(MediumThumbnail);
            LargeThumbnailCommand = new RelayCommand(LargeThumbnail);

            PhotoCollection = new ObservableCollection<DisplayPhoto>();
            _photos = new List<Photo>();

        }

        public void LoadPhotos(Photo[] photos)
        {
            _photos = new List<Photo>(photos);

            Task.Run(() =>
            {
               DefaultImageHeight = _imageWidths[_photoSize];
               DefaultImageWidth = _imageWidths[_photoSize];
               PhotoBag = new ConcurrentBag<DisplayPhoto>();
               var parallelOptions = new ParallelOptions();
               parallelOptions.MaxDegreeOfParallelism = 4;
               int numImages = photos.Count();
               int currentImage = 1;
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
                   PhotoBag.Add(new DisplayPhoto(photo, image, DefaultImageHeight, DefaultImageWidth));
               });
                PhotoBag = new ConcurrentBag<DisplayPhoto>(PhotoBag);
            });
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
                        //var info = new MagickImageInfo(data);

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

        private void ShrinkIt(object sender)
        {
            if (_photoSize > 0)
            {
                _photoSize--;
                LoadPhotos(_photos.ToArray());
            }
        }

        private void Embiggen(object sender)
        {
            if (_photoSize < 5)
            {
                _photoSize++;
                LoadPhotos(_photos.ToArray());
            }
        }
        private void SmallThumbnail(object sender)
        {
            _photoSize = 0;
            LoadPhotos(_photos.ToArray());
        }
        private void MediumThumbnail(object sender)
        {
            _photoSize = 10;
            LoadPhotos(_photos.ToArray());
        }
        private void LargeThumbnail(object sender)
        {
            _photoSize = 11;
            LoadPhotos(_photos.ToArray());
        }
    }
}

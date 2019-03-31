using System.Windows.Media.Imaging;
using Zenfolio7.Zenfolio;

namespace Zenfolio7.Model
{
    public class DisplayPhoto
    {
        public Photo PhotoObject { get; set; }
        public BitmapImage Image { get; set; }
        public double ImageHeight { get; set; }
        public double ImageWidth { get; set; }
        public DisplayPhoto(Photo photoObject, BitmapImage image, double imageHeight, double imageWidth)
        {
            PhotoObject = photoObject;
            ImageHeight = imageHeight;
            ImageWidth = imageWidth;
            Image = image;
        }
    }
}

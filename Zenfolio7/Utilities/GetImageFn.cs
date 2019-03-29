using System;
using System.IO;
using System.Windows.Media.Imaging;
using System.Collections.Generic;     // add to references

// Note this procedure can be used as part of code that: 
// - does some tests on filename so that in specific cases specific icons can be used
// - uses a dictionairy for extensions

namespace Zenfolio7.Utilities
{
    // ImageCache for some speed
    public static class ImageCache
    {
        public static Dictionary<String, BitmapSource> imageList = new Dictionary<String, BitmapSource>();

        public static BitmapSource GetImage(string fullpath)
        {
            string ext = Path.GetExtension(fullpath);
            ext.ToLower();

            // if in the list we are done
            if (imageList.ContainsKey(ext))
            {
                return imageList[ext];
            }

            // get the image
            BitmapSource myIcon;
            myIcon = GetIconFn.GetIconDll(fullpath);

            // put myIcon in the imageList, unless its extension says that it
            if ((ext != "") && (ext != ".exe") && (ext != ".lnk") && (ext != ".ico"))
            {
                imageList.Add(ext, myIcon);
            }
            return myIcon;
        }
    }

    public static class GetIconFn
    {

        public static System.Windows.Media.Imaging.BitmapSource GetIconDll(string fileName)
        {
            BitmapSource myIcon = null;

            Boolean validDrive = false;
            foreach (DriveInfo D in System.IO.DriveInfo.GetDrives())
            {   //D.DriveType.
                if (fileName == D.Name)
                {
                    validDrive = true;
                }
            }

            if ((File.Exists(fileName)) || (Directory.Exists(fileName)) || (validDrive))
            {
                using (System.Drawing.Icon sysIcon = ShellIcon.GetLargeIcon(fileName))
                {
                    try
                    {
                        myIcon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                                        sysIcon.Handle,
                                        System.Windows.Int32Rect.Empty,
                                        BitmapSizeOptions.FromWidthAndHeight(34, 34));
                    }
                    catch
                    {
                        myIcon = null;
                    }
                }
            }
            return myIcon;
        }
    }
}

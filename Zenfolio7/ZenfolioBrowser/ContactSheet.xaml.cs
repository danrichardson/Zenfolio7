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

        private Popup _MyPopup;

        public ContactSheet(Photo[] photos)
        {
            InitializeComponent();

            _viewModel = new ContactSheetViewModel();
            _viewModel.LoadPhotos(photos);
            _MyPopup = Resources["myPopup"] as Popup;

            base.DataContext = _viewModel;
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

 
    }






}
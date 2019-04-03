using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Zenfolio7.Zenfolio;

namespace Zenfolio7.ZenfolioBrowser
{
    /// <summary>
    /// A UI-friendly wrapper around a Person object.
    /// </summary>
    public class ZenfolioTreeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Data

        public ObservableCollection<ZenfolioTreeViewModel> Children { get; set; }
        public string Name { get { return (GroupElement != null) ? GroupElement.Title : Photo.FileName; } }
        public GroupElement GroupElement { get; set; }
        public Photo Photo { get; set; }
        public ZenfolioTreeViewModel Parent { get; }
 
        #region IsExpanded
        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded { get; set; }
        public void OnIsExpandedChanged()
        {
            if (IsExpanded && Parent != null)
                Parent.IsExpanded = true;
        }

        #endregion // IsExpanded

        #region IsSelected
        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected { get; set; }

        #endregion // IsSelected

        #endregion // Data

        #region Constructors

        public ZenfolioTreeViewModel(GroupElement groupElement)
            : this(groupElement, null)
        {

            //just playing around here
            //var uri = new Uri("http://www.theimagedepot.com/img/s/v-3/p1850655431.jpg");
            //using (var client = new AwesomeWebClient())
            //{
            //    client.Proxy = null;
            //    var data = client.DownloadData(uri);
            //    var magickImage = new MagickImage(data);
            //    ExifProfile profile = magickImage.GetExifProfile();
            //}
        }

        public ZenfolioTreeViewModel(Photo photo)
        {
            Photo = photo; 
        }

        private ZenfolioTreeViewModel(GroupElement groupElement, ZenfolioTreeViewModel parent)
        {
            GroupElement = groupElement;
            Parent = parent;

            if (groupElement is Group)
            {
                Children = (GroupElement as Group).Elements != null ? new ObservableCollection<ZenfolioTreeViewModel>(
                        (from child in (GroupElement as Group).Elements
                         select new ZenfolioTreeViewModel(child, this))
                         .ToList<ZenfolioTreeViewModel>())
                         : null;
            }
        }

        #endregion // Constructors

        #region NameContainsText

        public bool NameContainsText(string text)
        {
            if (String.IsNullOrEmpty(text) || String.IsNullOrEmpty(this.Name))
                return false;

            return this.Name.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) > -1;
        }

        #endregion // NameContainsText
    }
}
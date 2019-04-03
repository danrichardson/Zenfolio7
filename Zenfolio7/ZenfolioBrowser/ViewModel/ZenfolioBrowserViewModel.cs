using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Zenfolio7.Zenfolio;
using Zenfolio7.DataAccess;
using Zenfolio7.View.ViewModel;
using System.Threading.Tasks;
using MVVMLight.Messaging;
using Zenfolio7.Messages;
using System.ComponentModel;

namespace Zenfolio7.ZenfolioBrowser
{
    /// <summary>
    /// This is the view-model for browsing the user's Zenfolio collection,
    /// viewing image files, and selecting a root-direction to sync to
    /// </summary>

    public class ZenfolioBrowserViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        #region Data

        public bool IsBusy { get; set; }
        public bool CredentialsValid { get; private set; }
        public string ZenfolioResponse { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string LoadDataText { get { return IsBusy ? "Fetching Unicorns..." : "Round Up Unicorns"; }  }

        public GroupElement SelectedGroupElement { get; set; }

        IEnumerator<ZenfolioTreeViewModel> _matchingPeopleEnumerator;
        public RelayCommand TreeviewSelectedItemChangedCommand { get; set; }
        public RelayCommand LoginToZenfolioCommand { get; set; }
        #endregion // Data

        #region Constructor

        public ZenfolioBrowserViewModel()
        {
            CredentialsValid = false;
            CheckForCredentials();
            
            TreeviewSelectedItemChangedCommand = new RelayCommand(TreeviewSelectedItemChanged);
            SearchCommand = new SearchFamilyTreeCommand(this);
            LoadDataCommand = new RelayCommand(LoadData);
            LoginToZenfolioCommand = new RelayCommand(LoginToZenfolio);
        }

        private bool CheckForCredentials()
        {
            Username = Properties.Settings.Default.Zenfolio7_Username;
            Password = Properties.Settings.Default.Zenfolio7_PasswordHash;
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                CredentialsValid = false;
                return CredentialsValid;
            }
            LoginToZenfolio(null);
            return CredentialsValid;
        }

        private void LoginToZenfolio(object obj)
        {
            //Store the credentials
            Properties.Settings.Default.Zenfolio7_Username = Username;
            Properties.Settings.Default.Zenfolio7_PasswordHash = Password;
            Properties.Settings.Default.Save();

            var CredentialsValid = Database.Login(Username, Password);
            if (!CredentialsValid)
            {
                ZenfolioResponse = Database.ZenfolioResponse; 
            }
            else
            {
                ZenfolioResponse = "Login Successful";
                LoadData(null);
            }
        }

        public async void LoadData(object obj)
        {
            IsBusy = true;  
            await Task.Run(() =>
            {
                // Get raw family tree data from a database.
                RefreshRootGroup(new ZenfolioTreeViewModel(Database.GetRootGroup()));
                IsBusy = false;
            });
            FirstGeneration.First().IsExpanded = true;
        }

        private void RefreshRootGroup(ZenfolioTreeViewModel rootGroup)
        {
            FirstGeneration = new ObservableCollection<ZenfolioTreeViewModel>(new ZenfolioTreeViewModel[] { rootGroup });
        }

        private void TreeviewSelectedItemChanged(object obj)
        {
            Task.Run(() =>
            {
                if (obj as ZenfolioTreeViewModel == null)
                {
                    //error
                    return;
                }
                var viewModel = obj as ZenfolioTreeViewModel;
                var selectedGroupElement = viewModel.GroupElement;
                if (selectedGroupElement == null)
                {
                    return;
                }
                SelectedGroupElement = selectedGroupElement;
                var dataUpdateMessage = new DataUpdateMessage("Test", DataUpdateMessage.DataPacketType.Group, SelectedGroupElement);
                Messenger.Default.Send(dataUpdateMessage);

                if (selectedGroupElement is Group)
                {
                    //None of this really does anything
                    Group groupElement = selectedGroupElement as Group;
                    var groupElements = groupElement.Elements;
                    var groups = groupElements.Where(o => o as Group != null).ToList();
                    var photoSets = groupElements.Where(o => o as PhotoSet != null).ToList();
                }
                else if (selectedGroupElement is PhotoSet)
                {
                    IsBusy = true;
                    var photoSet = selectedGroupElement as PhotoSet;
                    //Load photo set on demand
                    if (photoSet.Photos == null || photoSet.Photos.Length == 0)
                    {
                        // clear dummy nodes

                        photoSet = Database.Client.LoadPhotoSet(photoSet.Id, InformationLevel.Full, true);
                        ((ZenfolioTreeViewModel)obj).GroupElement = photoSet;
                        ObservableCollection<ZenfolioTreeViewModel> children = new ObservableCollection<ZenfolioTreeViewModel>();
                        //TODO: This code works now that Fody is all scaffolded in and PropertyChanged BS is taken care of.  
                        //It would eliminate the need for the right-side dockpanel item
                        //Hmmm....
                        // add nodes for each of the loaded photos
                        //foreach (Photo p in photoSet.Photos)
                        //{
                        //    children.Add(new TreeViewModel(p));
                        //}
                        //((TreeViewModel)obj).Children = children;
                    }
                    SelectedPhotos = new ObservableCollection<Photo>(photoSet.Photos);
                    IsBusy = false;
                }
            });
        }

        #endregion // Constructor

        #region Properties

        #region FirstGeneration

        /// <summary>
        /// Returns a read-only collection containing the first person 
        /// in the family tree, to which the TreeView can bind.
        /// </summary>
        public ObservableCollection<ZenfolioTreeViewModel> FirstGeneration { get; set; }
        
        public void OnFirstGenerationChanged()
        {
            Messenger.Default.Send(new DataUpdateMessage("Test", DataUpdateMessage.DataPacketType.Group, FirstGeneration.First().GroupElement));
        }

        public ObservableCollection<Photo> SelectedPhotos { get; set; }
    
        #endregion // FirstGeneration

        #region LoadDataCommand

        public ICommand LoadDataCommand { get;  }

        #endregion

        #region SearchCommand

        /// <summary>
        /// Returns the command used to execute a search in the family tree.
        /// </summary>
        public ICommand SearchCommand { get; }

        private class SearchFamilyTreeCommand : ICommand
        {
            readonly ZenfolioBrowserViewModel _familyTree;

            public SearchFamilyTreeCommand(ZenfolioBrowserViewModel familyTree)
            {
                _familyTree = familyTree;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            event EventHandler ICommand.CanExecuteChanged
            {
                // I intentionally left these empty because
                // this command never raises the event, and
                // not using the WeakEvent pattern here can
                // cause memory leaks.  WeakEvent pattern is
                // not simple to implement, so why bother.
                add { }
                remove { }
            }

            public void Execute(object parameter)
            {
                _familyTree.PerformSearch();
            }
        }

        #endregion // SearchCommand

        #region SearchText

        /// <summary>
        /// Gets/sets a fragment of the name to search for.
        /// </summary>
        public string SearchText { get; set; }

        public void OnSearchTextChanged()
        {
            _matchingPeopleEnumerator = null;

        }

        #endregion // SearchText

        #endregion // Properties

        #region Search Logic

        void PerformSearch()
        {
            if (_matchingPeopleEnumerator == null || !_matchingPeopleEnumerator.MoveNext())
                this.VerifyMatchingPeopleEnumerator();

            var person = _matchingPeopleEnumerator.Current;

            if (person == null)
                return;

            // Ensure that this person is in view.
            if (person.Parent != null)
                person.Parent.IsExpanded = true;

            person.IsSelected = true;
        }

        void VerifyMatchingPeopleEnumerator()
        {
            //var matches = this.FindMatches(_searchText, _rootPerson);
            //_matchingPeopleEnumerator = matches.GetEnumerator();

            //if (!_matchingPeopleEnumerator.MoveNext())
            //{
            //    MessageBox.Show(
            //        "No matching names were found.",
            //        "Try Again",
            //        MessageBoxButton.OK,
            //        MessageBoxImage.Information
            //        );
            //}
        }

        IEnumerable<ZenfolioTreeViewModel> FindMatches(string searchText, ZenfolioTreeViewModel person)
        {
            if (person.NameContainsText(searchText))
                yield return person;

            foreach (ZenfolioTreeViewModel child in person.Children)
                foreach (ZenfolioTreeViewModel match in this.FindMatches(searchText, child))
                    yield return match;
        }

        #endregion // Search Logic
    }
}
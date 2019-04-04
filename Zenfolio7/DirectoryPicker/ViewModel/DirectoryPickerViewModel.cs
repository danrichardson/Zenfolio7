using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Zenfolio7.Utilities;
using Zenfolio7.DataModel;
using System.ComponentModel;
using Zenfolio7.View.ViewModel;
using Zenfolio7.Messages;
using MVVMLight.Messaging;

namespace Zenfolio7.DirectoryPicker.ViewModel
{
    public class DirectoryPickerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand SelectedPathFromTreeCommand { get; set; }

        // a Name to bind to the NavTreeTabs
        public string TreeName { get; set; }
        public string SelectedPath { get; set; }
        public void OnSelectedPathChanged()
        {
            var dataUpdateMessage = new DataUpdateMessage("Test", DataUpdateMessage.DataPacketType.Group, SelectedPath);
            Messenger.Default.Send(dataUpdateMessage);
        }
        public INavTreeItem SelectedItem { get; set; }
        // RootNr determines nr of RootItem that is used as RootNode 
        public int RootNr { get; set; }

        // RootChildren are used to bind to TreeView
        public ObservableCollection<INavTreeItem> RootChildren { get; set; }

        public void RebuildTree(int pRootNr = -1, bool pIncludeFileChildren = false)
        {
            SelectedPathFromTreeCommand = new RelayCommand(SelectedPathFromTree);
            // First take snapshot of current expanded items
            List<String> SnapShot = NavTreeUtils.TakeSnapshot(RootChildren);

            // As a matter of fact we delete and construct the tree//RoorChildren again.....
            // Delete all rootChildren
            foreach (INavTreeItem item in RootChildren) item.DeleteChildren();
            RootChildren.Clear();

            // Create treeRootItem 
            if (pRootNr != -1) RootNr = pRootNr;
            NavTreeItem treeRootItem = NavTreeRootItemUtils.ReturnRootItem(RootNr, pIncludeFileChildren);
            if (pRootNr != -1) TreeName = treeRootItem.FriendlyName;

            // Copy children treeRootItem to RootChildren, set up new tree 
            foreach (INavTreeItem item in treeRootItem.Children) { RootChildren.Add(item); }

            //Expand previous snapshot
            NavTreeUtils.ExpandSnapShotItems(SnapShot, treeRootItem);
        }

        // Constructors
        public DirectoryPickerViewModel(int pRootNumber = 0, bool pIncludeFileChildren = false)
        {
            SelectedPathFromTreeCommand = new RelayCommand(SelectedPathFromTree);
            RootChildren = new ObservableCollection<INavTreeItem> { };

            // create a new RootItem given rootNumber using convention
            RootNr = pRootNumber;
            NavTreeItem treeRootItem = NavTreeRootItemUtils.ReturnRootItem(pRootNumber, pIncludeFileChildren);
            TreeName = treeRootItem.FriendlyName;

            // Delete RootChildren and init RootChildren ussing treeRootItem.Children
            foreach (INavTreeItem item in RootChildren) { item.DeleteChildren(); }
            RootChildren.Clear();

            foreach (INavTreeItem item in treeRootItem.Children) { RootChildren.Add(item); }
        }

        private void SelectedPathFromTree(object obj)
        {
            if (obj as String != null)
            {
                SelectedPath = obj as String;
            }
        }

        // Well I suppose with the implicit values these are just for the record/illustration  
        public DirectoryPickerViewModel(int rootNumber) : this(rootNumber, false) { }
        public DirectoryPickerViewModel() : this(0) { }
    }
}




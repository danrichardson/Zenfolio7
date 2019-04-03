﻿using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Zenfolio7.Utilities;

namespace Zenfolio7.DataModel
{
    // Model of tree items: NavTreeItem

    // This file: (INavTabItem) - (NavTabItem abstract Class as basis) 

    // See File NavTreeItems.cs for more specific NavTreeItem classes and how we use them. 
    // These specific classes define own method for icon, children and constructor

    // Interface INavTreeItem, just summary of class 
    // Normally better to define smaller interfaces and then compose INavTreeItem for a SOLID basis 
    public interface INavTreeItem : INotifyPropertyChanged
    {
        // For text in treeItem
        string FriendlyName { get; set; }

        // Image used in TreeItem
        BitmapSource MyIcon { get; }

        // Drive/Folder/File naming scheme to retrieve children
        string FullPathName { get; set; }

        ObservableCollection<INavTreeItem> Children { get; }

        bool IsExpanded { get; set; }

        // Design decisions: 
        // - do we use INotifyPropertyChanged. Maybe not quite aproporiate in model, but without MVVM framework practical shortcut
        // - do we introduce IsSelected, in most cases I would advice: Yes. I use now button+command to set Path EACH time item pressed
        // bool IsSelected { get; set; }
        // void DeselectAll();

        // Specific for this application, could be introduced later in more specific interface/classes
        bool IncludeFileChildren { get; set; }

        // For resetting the tree
        void DeleteChildren();
    }

    // Abstact classs next step to implementation
    public abstract class NavTreeItem : INavTreeItem
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // for display in tree
        public string FriendlyName { get; set; }

        public BitmapSource MyIcon {  get { return GetMyIcon(); } }

        // .. to retrieve info
        public string FullPathName { get; set; }

        // Question/ to do. Note that to be sure we use ObservableCollection as property with a notification, remove notification??
        public ObservableCollection<INavTreeItem> Children { get { return GetMyChildren(); } }

        public bool IsExpanded { get; set; }
        public bool IsSelected { get; set; }

        public bool IncludeFileChildren { get; set; }

        // We will define these Methods in other derived classes ...
        public abstract BitmapSource GetMyIcon();
        public abstract ObservableCollection<INavTreeItem> GetMyChildren();


        // DeleteChildren, used to 
        // 1) remove old tree 2) set children=null, so a new tree is build

        // Question, not enough C#/Wpf knowledge:
        // - If we delete an NavTreeItem in the root are all its children and corresponding treeview elements garbage collected??
        // - If not, does DeleteChildren() does the work?? 
        // - For now we decide to use DeleteChildren() but no destructor ~NavTreeItem() that calls DeleteChildren.      
        public void DeleteChildren()
        {
            //if (children != null)
            {
                // Console.WriteLine(this.FullPathName);

                for (int i = Children.Count - 1; i >= 0; i--)
                {
                    Children[i].DeleteChildren();
                    Children[i] = null;
                    Children.RemoveAt(i);
                }

                //Children = null;
            }
        }
    }
}


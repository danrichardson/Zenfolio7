using System.Windows.Controls;
using Zenfolio7.DataModel;
using Zenfolio7.DirectoryPicker.ViewModel;

namespace Zenfolio7.DirectoryPicker
{
    /// <summary>
    /// Interaction logic for TabbedNavTrees.xaml
    /// </summary>
    public partial class DirectoryPickerControl : UserControl
    {
        public readonly DirectoryPickerViewModel SingleTree;
         
        public DirectoryPickerControl()
        {
            InitializeComponent();

            SingleTree = new DirectoryPickerViewModel();
            this.DataContext = SingleTree;
        }

        private void TreeView_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue as DriveItem != null)
            {
                SingleTree.SelectedPath = (e.NewValue as DriveItem).FullPathName;
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var button = (sender as Button);
            if (button != null)
            {
                INavTreeItem selectedItem = (INavTreeItem)((sender as Button).DataContext);
                SingleTree.SelectedPath = selectedItem.FullPathName;
            }
        }
    }
}

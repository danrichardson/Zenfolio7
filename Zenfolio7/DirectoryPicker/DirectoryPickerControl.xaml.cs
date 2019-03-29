using System.Windows.Controls;
using Zenfolio7.DirectoryPicker.ViewModel;

namespace Zenfolio7.DirectoryPicker
{
    /// <summary>
    /// Interaction logic for TabbedNavTrees.xaml
    /// </summary>
    public partial class DirectoryPickerControl : UserControl
    {
        public readonly ViewModel.DirectoryPickerViewModel SingleTree;
         
        public DirectoryPickerControl()
        {
            InitializeComponent();

            SingleTree = new ViewModel.DirectoryPickerViewModel();
            this.DataContext = SingleTree;
        }
    }
}

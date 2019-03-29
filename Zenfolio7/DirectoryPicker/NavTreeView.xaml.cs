using System.Windows.Controls;
using Zenfolio7.DirectoryPicker.ViewModel;

namespace Zenfolio7.DirectoryPicker
{
    /// <summary>
    /// Interaction logic for NavTreeView.xaml
    /// </summary>
    public partial class NavTreeView : UserControl
    {
        public readonly ViewModel.DirectoryPickerViewModel SingleTree;
        public NavTreeView()
        {
            InitializeComponent();

            SingleTree = new ViewModel.DirectoryPickerViewModel();
        }
    }
}

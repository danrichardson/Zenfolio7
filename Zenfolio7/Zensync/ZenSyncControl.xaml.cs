using System.Windows;
using System.Windows.Controls;

namespace Zenfolio7.ZenSync
{
    public partial class ZenSyncControl : UserControl
    {
        public ZenSyncControl()
        {
            InitializeComponent();

            //Group rootGroup = Database.GetRootGroup();
            ZenSyncViewModel viewModel = new ZenSyncViewModel();
            base.DataContext = viewModel;
            //Application.Current.Properties.
            this.GotFocus += ZenSyncControl_GotFocus;
        }

        private void ZenSyncControl_GotFocus(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
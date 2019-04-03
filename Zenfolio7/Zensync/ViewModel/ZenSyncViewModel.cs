using System;
using System.ComponentModel;
using System.IO;
using MVVMLight.Messaging;
using Zenfolio7.Messages;
using Zenfolio7.View.ViewModel;
using Zenfolio7.Zenfolio;

namespace Zenfolio7.ZenSync
{
    public class ZenSyncViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public GroupElement SelectedGroupElement { get; set; }
        public string SelectedDirectory { get; set; }
        public RelayCommand SyncCommand { get; set; }
        public ZenSyncViewModel() 
        {
            Messenger.Default.Register<DataUpdateMessage>(this, UpdateData);
            SyncCommand = new RelayCommand(Sync, CanSync);
        }

        private bool CanSync(object obj)
        {
            return Directory.Exists(SelectedDirectory) && SelectedGroupElement != null;
        }

        private void Sync(object obj)
        {
            
        }

        private void UpdateData(DataUpdateMessage obj)
        {
            if (obj.Packet.GetType() == typeof(string))
            {
                SelectedDirectory = (string)(obj.Packet);
            }
            if (obj.Packet.GetType() == typeof(Group))
            {
                SelectedGroupElement = (Group)(obj.Packet);
            }
            if (obj.Packet.GetType() == typeof(PhotoSet))
            {
                SelectedGroupElement = (PhotoSet)(obj.Packet);
            }
        }
    }
}
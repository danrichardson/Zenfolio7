using System.ComponentModel;
using MVVMLight.Messaging;
using Zenfolio7.Messages;
using Zenfolio7.Zenfolio;

namespace Zenfolio7.ZenSync
{
    public class ZenSyncViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public GroupElement SelectedGroupElement { get; set; }
        public string SelectedDirectory { get; set; }
        public ZenSyncViewModel() 
        {
            Messenger.Default.Register<DataUpdateMessage>(this, UpdateData);
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
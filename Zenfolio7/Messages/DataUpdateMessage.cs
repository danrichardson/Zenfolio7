namespace Zenfolio7.Messages
{
    public class DataUpdateMessage
    {
        public enum DataPacketType
        {
            Group,
            GroupElement,
            PhotoSet,
            Photo,
            String
        }
        public string Message { get; set; }
        public DataPacketType DataPacket { get; set; }
        public object Packet { get; set; }

        public DataUpdateMessage(string message, DataPacketType dataPacket, object packet = null)
        {
            Message = message;
            DataPacket= dataPacket;
            Packet = packet;
        }
    }
}
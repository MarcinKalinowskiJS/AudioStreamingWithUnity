using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Model.Additional
{
    public abstract class TransferProtocol
    {
        public enum ConnectionType { Receive, Send, ReceiveAndSend };
        public enum DataType { LeftChannel, RightChannel, String, LRChannelBuffer};
        public string system;
        public ConnectionType connectionType;
        public string receiveIP, destinationIP;
        public int receivePort, destinationPort;

        public TransferProtocol(string system, ConnectionType connectionType, string receiveIP, int receivePort, string destinationIP, int destinationPort) {
            this.system = system;
            this.connectionType = connectionType;
            this.receiveIP = receiveIP;
            this.receivePort = receivePort;
            this.destinationIP = destinationIP;
            this.destinationPort = destinationPort;
        }

        public abstract bool send(byte[] data, DataType dataType);
        public abstract byte[] receive();

        public bool isSendingActive() {
            switch (connectionType) {
                case ConnectionType.Send: return true;
                case ConnectionType.ReceiveAndSend: return true;
                default: return false;
            }
        }

        public bool isReceivingActive()
        {
            switch (connectionType)
            {
                case ConnectionType.Receive: return true;
                case ConnectionType.ReceiveAndSend: return true;
                default: return false;
            }
        }

        public bool isReceivingAndSendingActive() {
            if (connectionType == ConnectionType.ReceiveAndSend)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public DataType getDataType(byte[] data)
        {
            switch (data[data.Length])
            {
                case 0: return DataType.LeftChannel;
                case 1: return DataType.RightChannel;
                case 2: return DataType.String;
                default: return DataType.String;
            }
        }
    }
}

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
        public string system;
        public ConnectionType connectionType;
        public string destinationIP;
        public int destinationPort;

        public TransferProtocol(string system, ConnectionType connectionType, string destinationIP, int destinationPort) {
            this.system = system;
            this.connectionType = connectionType;
        }

        public abstract bool send(byte[] data);
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
    }
}

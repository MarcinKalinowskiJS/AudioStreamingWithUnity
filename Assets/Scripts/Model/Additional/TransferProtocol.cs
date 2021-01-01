using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Model.Additional
{
    public abstract class TransferProtocol
    {
        public enum ConnectionType { Receive, Send, ReceiveAndSend };
        public enum DataType { LeftChannel, RightChannel, String, LRChannelBuffer };
        public string system;
        public ConnectionType connectionType;
        public string receiveIP, destinationIP;
        public int receivePort, destinationPort;

        public TransferProtocol(string system, ConnectionType connectionType, string receiveIP, int receivePort, string destinationIP, int destinationPort)
        {
            this.system = system;
            this.connectionType = connectionType;
            this.receiveIP = receiveIP;
            this.receivePort = receivePort;
            this.destinationIP = destinationIP;
            this.destinationPort = destinationPort;
        }

        public abstract bool send(byte[] data, DataType dataType);
        //Tuple<buffer, bufferLength, DataType>
        public abstract Tuple<byte[], int, DataType> receive();

        public bool isSendingActive()
        {
            switch (connectionType)
            {
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

        public bool isReceivingAndSendingActive()
        {
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

        public byte[] getDataCombined(byte[] data, DataType dataType)
        {
            byte[] dataProcessed = new byte[1 + 4 + data.Length];

            //Add data type byte - first 1 byte
            dataProcessed[0] = (byte)dataType;

            //Copy data length - 4 bytes
            byte[] length = BitConverter.GetBytes(data.Length);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(length);
            }
            System.Buffer.BlockCopy(length, 0, dataProcessed, 1, length.Length);

            //Copy data
            System.Buffer.BlockCopy(data, 0, dataProcessed, 5, data.Length);

            

            return dataProcessed;
        }

        public static string decodeBytesToString(byte[] data)
        {
            string s = "";
            foreach (byte b in data)
            {
                s += ((char)b);
            }
            return s;
        }

        public static byte[] codeStringToBytes(string message)
        {
            List<byte> byteMessage = new List<byte>();
            foreach (char c in message)
            {
                byteMessage.Add((byte)c);
            }
            return byteMessage.ToArray();
        }
    }
}

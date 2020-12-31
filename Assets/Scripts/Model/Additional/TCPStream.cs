using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Model.Additional
{
    class TCPStream : TransferProtocol
    {
        TcpClient tcpWrite;
        TcpListener tcpServerRead;
        TcpClient tcpRead; //Maybe add a list for many incoming connections 
        BinaryWriter writer;
        BinaryReader reader;
        BackgroundWorker waitForConnectionsWorker;

        public TCPStream(string system, ConnectionType connectionType, string receiveIP, int receivePort, string destinationIP,
            int destinationPort) : base(system, connectionType, receiveIP, receivePort, destinationIP, destinationPort)
        {
            //First start listening
            if (base.isSendingActive())
            {
                tcpServerRead = new TcpListener(IPAddress.Parse(destinationIP), destinationPort);
                tcpServerRead.Start();
                //May add progress bar for how many connections were added
                waitForConnectionsWorker = new BackgroundWorker();
                waitForConnectionsWorker.DoWork += waitForConnection_DoWork(argument: false);
                waitForConnectionsWorker.RunWorkerAsync();
            }
            if (base.isReceivingActive()) {
                tcpWrite = new TcpClient();
                //AddressFamily might be wrong
                tcpWrite.Client() = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                tcpWrite.Client.Connect(IPAddress.Parse(destinationIP), destinationPort);
            }
            
        }

        private DoWorkEventHandler waitForConnection_DoWork(object sender, DoWorkEventArgs e) {
            Debug.Log("Started background worker");
            if (e == null && e.Argument == null) {
                Debug.Log("Error in background worker - not found neccessary argument(bool manyConnections)");
                return;
            }
            bool many = (bool)e.Argument;
            if (many)
            {
                while (many)
                {
                    while (!tcpServerRead.Pending())
                    {
                        tcpRead = tcpServerRead.AcceptTcpClient();
                    }
                }
            } else
            {
                while (!tcpServerRead.Pending())
                {
                    tcpRead = tcpServerRead.AcceptTcpClient();
                }
            }
            Debug.Log("Passed background worker");
        }

        public override bool send(byte[] data, DataType dataType)
        {
            byte[] dataCombined = base.getDataCombined(data, dataType);
            tcpWrite.GetStream().Write(dataCombined, 0, dataCombined.Length);
        }

        public override byte[] receive()
        {
            throw new NotImplementedException();
        }

        public void Clean()
        {
            if (writer != null)
            {
                //Maybe unnecessary flush
                writer.Flush();
                writer.Close();
            }
            if (reader != null)
            {
                reader.Close();
            }
            if (tcpRead != null)
            {
                tcpRead.Close();
            }
            if (tcpWrite != null)
            {
                tcpWrite.Close();
            }
            if (tcpServerRead != null)
            {
                tcpServerRead.Stop();
            }
        }

        ~TCPStream()
        {
            Clean();
        }
    }
}

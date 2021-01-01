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
        TcpClient tcpRead = null; //Maybe add a list for many incoming connections 
        BinaryWriter writer;
        BinaryReader reader;
        BackgroundWorker waitForConnectionsWorker;
        int waitForConnectionsWorkerConnectionsNumber = 0;
        byte[] buffer = new byte[2048];

        public TCPStream(string system, ConnectionType connectionType, string receiveIP, int receivePort, string destinationIP,
            int destinationPort) : base(system, connectionType, receiveIP, receivePort, destinationIP, destinationPort)
        {
            if (base.isReceivingActive()) {
                tcpServerRead = new TcpListener(IPAddress.Parse(destinationIP), destinationPort);
                tcpServerRead.Start();
                waitForConnectionsWorker = new BackgroundWorker();
                waitForConnectionsWorker.DoWork += waitForConnection_DoWork;
                waitForConnectionsWorker.ProgressChanged += waitForConnection_ProgressChanged;
                waitForConnectionsWorker.WorkerReportsProgress = true;
                waitForConnectionsWorker.RunWorkerAsync(argument: true);
            }
            //First start listening
            if (base.isSendingActive())
            {
                tcpWrite = new TcpClient();
                //AddressFamily might be wrong
                tcpWrite.Client = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                tcpWrite.Client.Connect(IPAddress.Parse(destinationIP), destinationPort);

            }
        }

        private void waitForConnection_DoWork(object sender, DoWorkEventArgs e) {
            Debug.Log("Started background worker");
            int connections = 0;
            //Check if there is and argument
            if (e == null && e.Argument == null) {
                Debug.Log("Error in background worker - not found neccessary argument(bool manyConnections)");
                return;
            }
            //Get the value of argument
            bool many = (bool)e.Argument;
            //If waiting for many connections
            if (many)
            {
                while (many)
                {
                    while (tcpServerRead.Pending())
                    {
                        tcpRead = tcpServerRead.AcceptTcpClient();
                        connections++;
                        waitForConnectionsWorker.ReportProgress(connections);
                        e.Result = connections;
                        if (waitForConnectionsWorker.CancellationPending) {
                            return;
                        }
                    }

                }
                //If waiting for only one connection
            } else
            {
                while (tcpServerRead.Pending())
                {
                    tcpRead = tcpServerRead.AcceptTcpClient();
                    connections++;
                    waitForConnectionsWorker.ReportProgress(connections);
                    e.Result = connections;
                }
            }
        }

        private void waitForConnection_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e) {
            waitForConnectionsWorkerConnectionsNumber = e.ProgressPercentage;
        }

        public int getConnectionsNumber() {
            return waitForConnectionsWorkerConnectionsNumber;
        }

        public override bool send(byte[] data, DataType dataType)
        {
            byte[] dataCombined = base.getDataCombined(data, dataType);
            tcpWrite.GetStream().Write(dataCombined, 0, dataCombined.Length);
            return true; //Need to rethink the return value
        }

        public override Tuple<byte[], int, DataType>  receive()
        {
            tcpRead.GetStream().Read(buffer, 0, 1);
            DataType dataType = (DataType)buffer[0];

            tcpRead.GetStream().Read(buffer, 1, 4);

            //May affect performance
            int length = int.Parse(String.Concat(buffer[0].ToString(), buffer[1].ToString(),
                buffer[2].ToString(), buffer[3].ToString()));
            tcpRead.GetStream().Read(buffer, 0, length);
            
            return new Tuple<byte[], int, DataType>(buffer, length, dataType);
        }

        public void Clean()
        {
            if (waitForConnectionsWorker != null) {
                waitForConnectionsWorker.CancelAsync();
                waitForConnectionsWorker.Dispose();
            }
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

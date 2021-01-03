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
        int bufferLength = 65535;
        byte[] buffer;

        public TCPStream(string system, ConnectionType connectionType, string receiveIP, int receivePort, string destinationIP,
            int destinationPort) : base(system, connectionType, receiveIP, receivePort, destinationIP, destinationPort)
        {
            buffer = new byte[bufferLength];

            if (base.isReceivingActive()) {
                tcpServerRead = new TcpListener(IPAddress.Parse(destinationIP), destinationPort);
                tcpServerRead.Start();
                waitForConnectionsWorker = new BackgroundWorker();
                waitForConnectionsWorker.DoWork += waitForConnection_DoWork;
                waitForConnectionsWorker.WorkerSupportsCancellation = true;
                waitForConnectionsWorker.ProgressChanged += waitForConnection_ProgressChanged;
                waitForConnectionsWorker.WorkerReportsProgress = true;
                //Argument = Wait for many connections
                waitForConnectionsWorker.RunWorkerAsync(argument: false);
            }
            //First start listening
            if (base.isSendingActive())
            {
                tcpWrite = new TcpClient();
                //AddressFamily might be wrong
                tcpWrite.Client = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                tcpWrite.Client.Connect(IPAddress.Parse(destinationIP), destinationPort);
                writer = new BinaryWriter(tcpWrite.GetStream());
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
                    if (waitForConnectionsWorker.CancellationPending)
                    {
                        return;
                    }
                }
                //If waiting for only one connection
            } else
            {
                while (tcpServerRead.Pending())
                {
                    tcpRead = tcpServerRead.AcceptTcpClient();
                    reader = new BinaryReader(tcpRead.GetStream());
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

        public bool send1(byte[] data, DataType dataType)
        {
            Debug.Log("here works 0");
            byte[] dataCombined = base.getDataCombined(data, dataType);
            Debug.Log("here works 1");
            tcpWrite.GetStream().Write(dataCombined, 0, dataCombined.Length);
            Debug.Log("here works 2");
            return true; //Need to rethink the return value
        }

        public override bool send(byte[] data, DataType dataType)
        {
            Debug.Log("Data to send len: " + data.Length);
            writer.Write(data);
            return true;
        }

        public override Tuple<byte[], int, DataType> receive() {
            //if (tcpRead != null)
            Debug.Log("TCPRead Connected?:" + tcpRead.Connected);
            if (tcpRead.GetStream().DataAvailable)
            {
                int rec = reader.Read(buffer, 0, bufferLength);
                Debug.Log("Readed: " + rec);
                return new Tuple<byte[], int, DataType>(buffer, rec, DataType.LRChannelBuffer);
            }
            else {
                Debug.Log("No data to read in TCPStream ");
                return new Tuple<byte[], int, DataType>(buffer, 0, DataType.LRChannelBuffer);
            }
        }

        public Tuple<byte[], int, DataType> receive1()
        {
            if (tcpRead.GetStream().DataAvailable)
            {
                tcpRead.GetStream().Read(buffer, 0, 1);
                DataType dataType = (DataType)buffer[0];

                tcpRead.GetStream().Read(buffer, 0, 4);


                //May affect performance
                string space = " ";
                Debug.Log("RecLenToRead: " + String.Concat(buffer[0].ToString(), space, buffer[1].ToString(), space,
                    buffer[2].ToString(), space, buffer[3].ToString()));
                int length = int.Parse(String.Concat(buffer[0].ToString(), buffer[1].ToString(),
                    buffer[2].ToString(), buffer[3].ToString()));

                
                tcpRead.GetStream().Read(buffer, 0, length);

                return new Tuple<byte[], int, DataType>(buffer, length, dataType);
            }
            else {
                return new Tuple<byte[], int, DataType>(new byte[] { 0 }, 0, DataType.String);
            }
        }

        public void Clean()
        {
            Debug.Log("Clean Beggining");
            if (waitForConnectionsWorker != null)
            {
                Debug.Log("WaitForConnectionsWorker != null");
                waitForConnectionsWorker.CancelAsync();
                waitForConnectionsWorker.Dispose();
            }
            else {
                Debug.Log("WaitForConnectionsWorker == null");
            }
            if (writer != null)
            {
                //Maybe unnecessary flush
                writer.Flush();
                writer.Close();
            }
            if (tcpServerRead != null)
            {
                tcpServerRead.Stop();
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
            
            Debug.Log("Clean Ending");
        }

        public byte[] getOnlyBuffer(Tuple<byte[], int, DataType> dataWithHeader) {
            byte[] buffer = new byte[dataWithHeader.Item2];
            Buffer.BlockCopy(dataWithHeader.Item1, 0, buffer, 0, dataWithHeader.Item2);
            return buffer;
        }

        ~TCPStream()
        {
            Clean();
        }
    }
}

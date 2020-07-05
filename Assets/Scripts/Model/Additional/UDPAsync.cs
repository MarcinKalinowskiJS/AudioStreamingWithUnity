using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Model.Additional
{
    //TODO: Get the ip of received packets
    //https://stackoverflow.com/questions/5964846/get-client-ip-from-udp-packages-received-with-udpclient

    //TODO: Send async
    //https://docs.microsoft.com/pl-pl/dotnet/api/system.net.sockets.udpclient.sendasync?view=netcore-3.1
    class UDPAsync : TransferProtocol
    {
        UdpClient udpClient, udpServer; //Server-Receive Client-Send
        System.Net.IPEndPoint epServer;
        private bool isReceiveUDPCancellationRequested = false;
        public bool IsReceiveUDPCancellationRequested
        {
            get { return isReceiveUDPCancellationRequested; }
            set { isReceiveUDPCancellationRequested = value; }
        }

        public UDPAsync(string system, ConnectionType connectionType, string receiveIP, int receivePort, string destinationIP, int destinationPort) 
            : base(system, connectionType, receiveIP, receivePort, destinationIP, destinationPort){
            udpServer = new UdpClient(receivePort);
            udpClient = new UdpClient();
            System.Net.IPEndPoint epClient = new IPEndPoint(System.Net.IPAddress.Parse(destinationIP), destinationPort);
            udpClient.Connect(epClient);
            epServer = new IPEndPoint(System.Net.IPAddress.Any, receivePort);
        }

        public override bool send(byte[] data) {
            return false;
        }

        public override byte[] receive() {
            if (base.isReceivingActive())
            {
                return receiveUDPAsync().Result;
            }
            return null;
        }

        private async System.Threading.Tasks.Task<byte[]> receiveUDPAsync()
        {
            Debug.Log("Started Receiving Async");
            System.Net.Sockets.UdpReceiveResult? data = null;
            while (!isReceiveUDPCancellationRequested)
            {
                data = await udpServer.ReceiveAsync();
                if (data != null)
                {
                    //Debug.Log(System.Text.Encoding.UTF8.GetString(data.Value.Buffer));
                }
            }

            Debug.Log("Ended Receiving Async");
            return data.Value.Buffer;
        }
    }
}

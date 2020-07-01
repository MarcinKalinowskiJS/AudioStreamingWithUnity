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
    class UDP : TransferProtocol
    {
        UdpClient udpClient, udpServer; //Server-Receive Client-Send
        System.Net.IPEndPoint epServer;

        public UDP(string system, ConnectionType connectionType, int receivePort, string destinationIP, int destinationPort) 
            : base(system, connectionType){
            Debug.Log("Created UDP Connection");
            udpServer = new UdpClient(receivePort);
            udpClient = new UdpClient();
            System.Net.IPEndPoint epClient = new IPEndPoint(System.Net.IPAddress.Parse(destinationIP), destinationPort);
            udpClient.Connect(epClient);
            epServer = new IPEndPoint(System.Net.IPAddress.Any, receivePort);
        }

        public override bool send(byte[] data) {
            if (base.isSendingActive())
            {
                udpClient.Send(data, data.Length);
                return true;
            }
            else {
                return false;
            }
        }

        public override byte[] receive()
        {
            Debug.Log("Active? " + base.isReceivingActive());
            if (base.isReceivingActive())
            {
                if (udpServer.Available > 0)
                {
                    return udpServer.Receive(ref epServer);
                }
            }
            return null;   
        }
    }
}

﻿using System;
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
        System.Net.IPEndPoint epClient, epServer;

        public UDP(string system, ConnectionType connectionType, string receiveIP, int receivePort, string destinationIP, int destinationPort)
            : base(system, connectionType, receiveIP, receivePort, destinationIP, destinationPort)
        {
            if (base.isReceivingActive())
            {
                udpServer = new UdpClient(receivePort);
                epServer = new IPEndPoint(System.Net.IPAddress.Any, receivePort);
            }

            if (base.isSendingActive())
            {
                udpClient = new UdpClient();
                epClient = new IPEndPoint(System.Net.IPAddress.Parse(destinationIP), destinationPort);
                udpClient.Connect(epClient);
            }
        }

        public override bool send(byte[] data, TransferProtocol.DataType dataType)
        {
            byte[] dataProcessed = new byte[data.Length + 1];
            System.Buffer.BlockCopy(data, 0, dataProcessed, 0, data.Length);
            Debug.Log("DL:" + data.Length+ " new:" + dataProcessed.Length);

            switch (dataType)
            {
                case DataType.LeftChannel:
                    {
                        dataProcessed[data.Length] = (byte)0;
                        break;
                    }
                case DataType.RightChannel:
                    {
                        dataProcessed[data.Length] = (byte)1;
                        break;
                    }
                case DataType.String:
                    {
                        dataProcessed[data.Length] = (byte)2;
                        break;
                    }
                default:
                    {
                        dataProcessed[data.Length] = (byte)2;
                        break;
                    }
            }

            if (base.isSendingActive())
            {
                udpClient.Send(dataProcessed, dataProcessed.Length);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override byte[] receive()
        {
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

//https://stackoverflow.com/questions/20038943/simple-udp-example-to-send-and-receive-data-from-same-socket

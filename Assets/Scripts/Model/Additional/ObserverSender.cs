using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class ObserverSender
{
    public enum Protocol { UDP, TCP }
    public string system;
    public string IPAddress;
    public string port;
    public Protocol protocol;
    public byte[] lastReceivedData;
    UdpClient udpClient, udpServer; //Server-Receive Client-Send
    System.Net.IPEndPoint epServer;

    public ObserverSender(string sys, string IP, string port, Protocol protocol)
    {
        var mth = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod();
        var cls = mth.ReflectedType.Name;
        Debug.Log(cls);
        udpServer = new UdpClient(65535);
        udpClient = new UdpClient();
        System.Net.IPEndPoint epClient = new IPEndPoint(System.Net.IPAddress.Parse("192.168.0.3"), 65535);
        udpClient.Connect(epClient);
        epServer = new IPEndPoint(System.Net.IPAddress.Any, 65535);

        lastReceivedData = null;
        system = sys;
        IPAddress = IP;
        this.port = port;
        this.protocol = protocol;
    }

    public void updateObserverErgoSendData(byte[] data)
    {
        Debug.Log("Sending started");
        switch (protocol)
        {
            case Protocol.UDP:
                {
                    sendUDP(data);
                    break;
                }
            case Protocol.TCP:
                {
                    sendTCP();
                    break;
                }
            default:
                {
                    Debug.Log("Wrong Number of Protocol in ObserverSender Class");
                    break;
                }
        }
        Debug.Log("Sending ended");
    }

    public void receiveUDP()
    {

        bool received = false;
        int i = 0;
        while (received == false)
        {
            i++;

            if (udpServer.Available > 0)
            {
                received = true;
                if (lastReceivedData == null)
                {
                    lastReceivedData = udpServer.Receive(ref epServer);
                }
            }
            if (i > 255)
            {
                Debug.Log("Nothing received");
                break;
            }
            System.Threading.Thread.Sleep(10);
        }

        string s = "";
        foreach (byte b in lastReceivedData) {
            s += "|" + b;
        }
        Debug.Log("LRDInRec: " + s);
        //May be needed to call method endSend() and endReceive()
        /*
        udpServer.Dispose();
        udpServer.Close();
        */
    }

    public void sendUDP(byte[] dataS)
    {

        udpClient.Send(dataS, dataS.Length);

        //May be needed to call method endSend() and endReceive()
        /*udpClient.Dispose();
        udpClient.Close();
        */
    }
    public void sendTCP() { }

}




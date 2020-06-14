using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class ObserverSender
{
    public enum Protocol { UDP, TCP }
    public string _system;
    public string _IPAddress;
    public string _port;
    public Protocol _protocol;
    public byte[] lastReceivedData;
    UdpClient udpClient, udpServer;
    System.Net.IPEndPoint epServer;

    public ObserverSender(string sys, string IP, string port, Protocol protocol)
    {
        udpServer = new UdpClient(65535);
        udpClient = new UdpClient();
        System.Net.IPEndPoint epClient = new IPEndPoint(IPAddress.Parse("192.168.0.107"), 65535);
        udpClient.Connect(epClient);
        epServer = new IPEndPoint(IPAddress.Any, 65535);

        lastReceivedData = null;
        _system = sys;
        _IPAddress = IP;
        _port = port;
        _protocol = protocol;

        string name = Dns.GetHostName();
        //string[] addresses = Dns.Resolve(name).AddressList;
        IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());

        Debug.Log("Host Addresses beginning:");
        foreach (var tmp in ips)
        {
            Debug.Log(tmp);
        }
        Debug.Log("Host Addresses ending:");
    }

    public void updateObserverErgoSendData(byte[] data)
    {
        Debug.Log("Sending started");
        switch (_protocol)
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




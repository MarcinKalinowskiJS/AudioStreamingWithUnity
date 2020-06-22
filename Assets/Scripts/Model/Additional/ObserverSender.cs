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
    private bool isReceiveUDPCancellationRequested = false;

    public bool IsReceiveUDPCancellationRequested
    {
        get { return isReceiveUDPCancellationRequested; }    
        set { isReceiveUDPCancellationRequested = value; }
    }

    public ObserverSender(string sys, string IP, string port, Protocol protocol)
    {
        /*Who invoked the constructor/method
        var mth = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod();
        var cls = mth.ReflectedType.Name;
        Debug.Log(cls);
        */
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

    //Strategy pattern may be useful
    public byte[] receive()
    {
        return receiveUDPAsync().Result;
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

    private async System.Threading.Tasks.Task<byte[]> receiveUDPAsync()
    {
        Debug.Log("Started Receiving Async");
        System.Net.Sockets.UdpReceiveResult? data = null;
        while (!isReceiveUDPCancellationRequested)
        {
            data = await udpServer.ReceiveAsync();
            if (data != null) {
                //Debug.Log(System.Text.Encoding.UTF8.GetString(data.Value.Buffer));
            } 
        }

        Debug.Log("Ended Receiving Async");
        return data.Value.Buffer;
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
        Debug.Log("Sending: " + dataS.ToString());
        udpClient.Send(dataS, dataS.Length);
        Debug.Log("Sent");

        //May be needed to call method endSend() and endReceive()
        /*udpClient.Dispose();
        udpClient.Close();
        */
    }
    public void sendTCP() { }

}




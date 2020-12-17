using Assets.Scripts.Model.Additional;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.Model.Additional.TransferProtocol;

public class NetworkModel
{
    public enum Protocol { UDP, UDPAsync, TCP }
    private static NetworkModel instance = null;
    List<TransferProtocol> observers;

    public static NetworkModel Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new NetworkModel();
                instance.observers = new List<TransferProtocol>();
            }
            return instance;
        }
    }


    public IPAddress[] getOriginIPs()
    {
        string name = Dns.GetHostName();
        //string[] addresses = Dns.Resolve(name).AddressList;
        IPAddress[] iPs = Dns.GetHostAddresses(Dns.GetHostName());
        return iPs;
    }

    public void sendChatText(string text) {
        byte[] bytesToSend = System.Text.Encoding.UTF8.GetBytes(text);
        //o.sendUDP(bytesToSend);      
        throw new NotImplementedException();
    }

    public List<byte[]> Receive()
    {
        List<byte[]> receivedData = new List<byte[]>();
        foreach (TransferProtocol tp in observers)
        {
            receivedData.Add(tp.receive());
        }
        if (receivedData.Count > 0)
        {
            return receivedData;
        }
        return null;
    }

    public bool Send(byte[] data) {
        bool sended = false;
        foreach (TransferProtocol tp in observers) {
            if (tp.isSendingActive()) {
                tp.send(data);
                sended = true;
            }
        }
        return sended;
    }

    public string addObserver(string system, Protocol protocol, ConnectionType connectionType, string receiveIP, int receivePort, string destinationIP, int destinationPort)
    {
        switch (protocol)
        {
            case Protocol.UDP:
                observers.Add(new UDP(system, connectionType, receiveIP, receivePort, destinationIP, destinationPort));
                return "Added UDP";
            case Protocol.UDPAsync:
                observers.Add(new UDPAsync(system, connectionType, receiveIP, receivePort, destinationIP, destinationPort));
                return "Added UDP Async";
            default:
                Debug.Log("Unsupported");
                return "Wrong type of connection";
        }
    }

    public List<TransferProtocol> getConnections() {
        return observers;
    }
}

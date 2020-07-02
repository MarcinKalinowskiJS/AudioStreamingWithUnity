using Assets.Scripts.Model.Additional;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Presenter
{
    private static Presenter instance = null;

    public static Presenter Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Presenter();
            }
            return instance;
        }
    }

    public void addConnection(int receivePort, string destinationIP, int destinationPort) {
        NetworkModel.Instance.addObserver("Win", NetworkModel.Protocol.UDP, TransferProtocol.ConnectionType.ReceiveAndSend, receivePort, destinationIP, destinationPort);
    }

    public void send(byte[] data) {
        NetworkModel.Instance.Send(data);
    }

    public List<byte[]> receive() {
        return NetworkModel.Instance.Receive();
    }

    public List<string> getConnectedIPs() {
        List<string> connectedIPs = new List<string>();
        foreach (TransferProtocol tp in NetworkModel.Instance.getConnections()) {
            connectedIPs.Add(tp.destinationIP + " : " + tp.destinationPort);
        }

        return connectedIPs;
    }

    public TransferProtocol getConnection(string IP, int port)
    {
        return NetworkModel.Instance.getConnections().Find(x => x.destinationIP.Equals(IP) & x.destinationPort == port);
    }
}

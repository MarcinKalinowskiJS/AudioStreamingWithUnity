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

    public void addConnection(string receiveIP, string receivePort, string destinationIP, string destinationPort) {
        //TODO: checking value, converting, exectuing addObserver()
        int receivePortInt, destinationPortInt;
        bool allChecksOk = true;

        //TODO: check with ternary operator because if first is false and second true allChecksOk will be true
        allChecksOk = int.TryParse(receivePort, out receivePortInt);
        allChecksOk = int.TryParse(destinationPort, out destinationPortInt);

        if (allChecksOk)
        {
            //TODO: add radio buttons with connection type
            NetworkModel.Instance.addObserver("Win", NetworkModel.Protocol.UDP, TransferProtocol.ConnectionType.ReceiveAndSend, receiveIP, receivePortInt, destinationIP, destinationPortInt);
        }
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
        Debug.Log("ConnectedIPS: " + connectedIPs.Count);
        return connectedIPs;
    }

    public List<TransferProtocol> getConnections() {
        return NetworkModel.Instance.getConnections();
    }

    public TransferProtocol getConnection(string IP, int port)
    {
        return NetworkModel.Instance.getConnections().Find(x => x.destinationIP.Equals(IP) & x.destinationPort == port);
    }

    public List<string> getConnectionsAdjusted() {
        List<string> connectionsAdjustedList = new List<string>();
        foreach (TransferProtocol tp in NetworkModel.Instance.getConnections()) {
            connectionsAdjustedList.Add(getConnectionAdjusted(tp));
        }
        return connectionsAdjustedList;
    }

    public List<string> getOriginIPsString() {
        List<string> receiveIPs = new List<string>();
        foreach (System.Net.IPAddress iPA in NetworkModel.Instance.getOriginIPs()) {
            receiveIPs.Add(iPA.ToString());
        }
        return receiveIPs;
    }

    public string getConnectionAdjusted(TransferProtocol tp) {
        string connectionAdjusted = "";
        switch (tp.connectionType)
        {
            case TransferProtocol.ConnectionType.Receive:
                connectionAdjusted = tp.receiveIP + ":" + tp.receivePort + "<-";
                break;
            case TransferProtocol.ConnectionType.ReceiveAndSend:
                connectionAdjusted = tp.receiveIP + ":" + tp.receivePort + "<->" + tp.destinationIP + ":" + tp.destinationPort;
                break;
            case TransferProtocol.ConnectionType.Send:
                connectionAdjusted = "->" + tp.destinationIP + ":" + tp.destinationPort;
                break;
            default:
                connectionAdjusted = "Error in connection";
                break;
        }
        return connectionAdjusted;
    }
}

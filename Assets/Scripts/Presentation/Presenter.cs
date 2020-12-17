using Assets.Scripts.Model.Additional;
using Assets.Scripts.Overall;
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

    public string addConnection(string oS, NetworkModel.Protocol protocol, TransferProtocol.ConnectionType conType,string receiveIP, string receivePort, string destinationIP, string destinationPort)
    {
        //TODO: checking value, converting, exectuing addObserver()
        int receivePortInt, destinationPortInt;
        bool receivePortCheckOK = true;
        bool sendPortCheckOK = true;

        receivePortCheckOK = int.TryParse(receivePort, out receivePortInt);
        sendPortCheckOK = int.TryParse(destinationPort, out destinationPortInt);

        //All ok?
        if (receivePortCheckOK == true && sendPortCheckOK == true)
        {
            //TODO: add radio buttons with connection type
            NetworkModel.Instance.addObserver(oS, protocol, conType, receiveIP, receivePortInt, destinationIP, destinationPortInt);
            return "Added";
            //Else - if error is somewhere
        }
        else
        {
            if (receivePortCheckOK == false && sendPortCheckOK == false) {
                return "Receive port and send port is wrong";
            }
            if (receivePortCheckOK == false)
            {
                return "Receive port is wrong";
            }
            if (sendPortCheckOK == false)
            {
                return "Send port is wrong";
            }
        }
        return "";
    }

    /*//TODO: How maybe error and info codes should be handled
     * public string addConnection(string receiveIP, string receivePort, string destinationIP, string destinationPort) {
        List<Tuple<string, byte>> errorCodes = null;

        //TODO: checking value, converting, exectuing addObserver()
        int receivePortInt, destinationPortInt;
        bool receivePortCheckOK = true;
        bool sendPortCheckOK = true;
        
        receivePortCheckOK = int.TryParse(receivePort, out receivePortInt);
        sendPortCheckOK = int.TryParse(destinationPort, out destinationPortInt);

        //All ok?
        if (receivePortCheck == true && sendPortCheckOK == true)
        {
            //TODO: add radio buttons with connection type
            errorCodes.Add(NetworkModel.Instance.addObserver("Win", NetworkModel.Protocol.UDP, TransferProtocol.ConnectionType.ReceiveAndSend, receiveIP, receivePortInt, destinationIP, destinationPortInt));
            return addConnectionGenerateResultFromErrorCodes(errorCodes);
            //Else if error is somewhere
        } else
        {

            if (receivePortCheckOK == false) {
                errorCodes.Add(new Tuple<"addConnection", 2);
            }
            if (sendPortCheckOK == false)
            {
                errorCodes.Add(new Tuple<"addConnection", 1);
            }
            return addConnectionGenerateResultFromErrorCodes(errorCodes);
        }
    }

        //TODO: Later need to think about handling logging to the user
    public string addConnectionGenerateResultFromErrorCodes(List<Tuple<string, byte>> errorCodes) {
        string result = "";
        foreach (Tuple<string, byte> errorData in errorCodes) {
            switch (errorData.Item1) {
                case "addConnection": { 
                        switch (errorData) {
                            case 0: {
                                        break;
                            }
                            default: {
                                    result += "error";
                                break;
                            }
                        }
                        break;
                    }
            }
        }
        return result;
    }
    */
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

    public List<string> getAllConnectionTypes() {
        List<string> types = new List<string>();
        foreach (TransferProtocol.ConnectionType ct in EnumUtil.GetValues<TransferProtocol.ConnectionType>()) {
            types.Add(ct.ToString());
        }
        return types;
    }
}

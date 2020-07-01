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
}

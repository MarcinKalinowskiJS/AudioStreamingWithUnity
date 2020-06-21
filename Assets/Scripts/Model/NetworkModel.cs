using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class NetworkModel
{
    private static NetworkModel instance = null;
    List<ObserverSender> observers;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static NetworkModel Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new NetworkModel();
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
        byte byteToSend = System.Convert.ToByte(text);
        
    }

    //public static string ByteArrayToString()

    public void addObserver(string data)
    {
        //o = new ObserverSender("1", "1", "1", ObserverSender.Protocol.UDP);
    }
}

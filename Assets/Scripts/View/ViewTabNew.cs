using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewTabNew : MonoBehaviour
{
    private static ViewTabNew instance = null;

    private UnityEngine.UI.Button buttonSend;
    private UnityEngine.UI.Button buttonReceive;
    private UnityEngine.UI.Toggle toggleSend;
    private UnityEngine.UI.Toggle toggleReceive;
    private UnityEngine.UI.Dropdown originIP;
    private UnityEngine.UI.InputField originPort;
    private UnityEngine.UI.InputField destinationIP;
    private UnityEngine.UI.InputField destinationPort;
    private UnityEngine.UI.Text scrollAreaText;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null) {
            instance = this;
        }
        linkUI();
    }

    private void linkUI() {
        //View
        buttonSend = GameObject.Find("ButtonSend").GetComponent<UnityEngine.UI.Button>();
        buttonReceive = GameObject.Find("ButtonReceive").GetComponent<UnityEngine.UI.Button>();
        toggleSend = GameObject.Find("ToggleSend").GetComponent<UnityEngine.UI.Toggle>();
        toggleReceive = GameObject.Find("ToggleReceive").GetComponent<UnityEngine.UI.Toggle>();
        buttonSend.onClick.AddListener(onClickSend);
        buttonReceive.onClick.AddListener(onClickReceive);
        

        //NetworkModel
        originIP = GameObject.Find("OriginIP").GetComponent<UnityEngine.UI.Dropdown>();
        originPort = GameObject.Find("OriginPort").GetComponent<UnityEngine.UI.InputField>();
        destinationIP = GameObject.Find("DestinationIP").GetComponent<UnityEngine.UI.InputField>();
        destinationPort = GameObject.Find("DestinationPort").GetComponent<UnityEngine.UI.InputField>();
        scrollAreaText = GameObject.Find("ScrollArea").GetComponentInChildren<UnityEngine.UI.Text>();
        originIP.options.Clear();
        foreach (System.Net.IPAddress iPA in NetworkModel.Instance.getOriginIPs())
        {
            originIP.options.Add(new UnityEngine.UI.Dropdown.OptionData() { text = iPA.ToString() });
        }
        originIP.RefreshShownValue();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static ViewTabNew Instance {
        get {
            if (instance == null) {
                GameObject go = new GameObject();
                instance = go.AddComponent<ViewTabNew>();
            }
            return instance;
        }
    }

    public void addConnection() {
        Presenter.Instance.addConnection(65535, "192.168.0.3", 65535);
    }

    public void onClickSend()
    {
        //NetworkModel.Instance.sendChatText("Test!");
        Presenter.Instance.send(new byte[] { 1, 2, 5 });
    }

    public void onClickReceive()
    {
        //UnityEngine.Events.UnityEvent receiveDataFromClient = new UnityEngine.Events.UnityEvent();
        string s = "";
        List<byte[]> receivedData = Presenter.Instance.receive();
        s += (receivedData == null ? "NULL" : receivedData[0].Length + " " + decodeByteToString(receivedData[0]) );

        scrollAreaText.text = s;
    }

    public string decodeByteToString(byte[] data) {
        string s = "";
        foreach (byte b in data) {
            s += b + " ";
        }
        return s;
    }

    public string getAll()
    {
        string all = "\nBeginning:";
        all += "\nOriginIP: " + originIP.options[originIP.value].text;
        all += "\noriginPort: " + originPort.text;
        all += "\ndestinationIP: " + destinationIP.text;
        all += "\ndestinationPort: " + destinationPort.text;
        all += "\nscrollArea: " + scrollAreaText.text;
        all += "\nEnding";

        return all;
    }

    public string getSelectedOriginIPString()
    {
        return originIP.options[originIP.value].text;
    }
}

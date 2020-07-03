using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewTabNew : MonoBehaviour
{
    private static ViewTabNew instance = null;

    private UnityEngine.UI.Button buttonSend;
    private UnityEngine.UI.Button buttonReceive;
    private UnityEngine.UI.Button buttonAdd;
    private UnityEngine.UI.Toggle toggleSend;
    private UnityEngine.UI.Toggle toggleReceive;
    private UnityEngine.UI.Dropdown receiveIP;
    private UnityEngine.UI.InputField receivePort;
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
        addTestConnection();
    }

    private void addTestConnection() {
        Presenter.Instance.addConnection("192.168.0.3", "65535", "192.168.0.3", "65535");
    }

    private void linkUI() {
        //View
        buttonSend = GameObject.Find("ButtonSend").GetComponent<UnityEngine.UI.Button>();
        buttonReceive = GameObject.Find("ButtonReceive").GetComponent<UnityEngine.UI.Button>();
        buttonAdd = GameObject.Find("ButtonAdd").GetComponent<UnityEngine.UI.Button>();
        toggleSend = GameObject.Find("ToggleSend").GetComponent<UnityEngine.UI.Toggle>();
        toggleReceive = GameObject.Find("ToggleReceive").GetComponent<UnityEngine.UI.Toggle>();
        buttonSend.onClick.AddListener(onClickSend);
        buttonReceive.onClick.AddListener(onClickReceive);
        buttonAdd.onClick.AddListener(onClickAddConnection);


        //NetworkModel
        receiveIP = GameObject.Find("ReceiveIP").GetComponent<UnityEngine.UI.Dropdown>();
        receivePort = GameObject.Find("ReceivePort").GetComponent<UnityEngine.UI.InputField>();
        destinationIP = GameObject.Find("DestinationIP").GetComponent<UnityEngine.UI.InputField>();
        destinationPort = GameObject.Find("DestinationPort").GetComponent<UnityEngine.UI.InputField>();
        scrollAreaText = GameObject.Find("ScrollArea").GetComponentInChildren<UnityEngine.UI.Text>();

        //TODO:move refreshReceiveIPDropdown to some more appropiate place
        refreshReceiveIPDropdown();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void refreshReceiveIPDropdown() {
        receiveIP.options.Clear();

        //TODO: Change to Presenter getOriginIPs()
        foreach (System.Net.IPAddress iPA in NetworkModel.Instance.getOriginIPs())
        {
            receiveIP.options.Add(new UnityEngine.UI.Dropdown.OptionData() { text = iPA.ToString() });
        }
        receiveIP.RefreshShownValue();
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

    public void onClickAddConnection() {
        Presenter.Instance.addConnection(getSelectedOriginIPString(), receivePort.text, destinationIP.text, destinationPort.text);
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
        all += "\nOriginIP: " + receiveIP.options[receiveIP.value].text;
        all += "\noriginPort: " + receivePort.text;
        all += "\ndestinationIP: " + destinationIP.text;
        all += "\ndestinationPort: " + destinationPort.text;
        all += "\nscrollArea: " + scrollAreaText.text;
        all += "\nEnding";

        return all;
    }

    public string getSelectedOriginIPString()
    {
        return receiveIP.options[receiveIP.value].text;
    }
}

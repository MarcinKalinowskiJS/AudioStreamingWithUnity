using Assets.Scripts.Model.Additional;
using Assets.Scripts.Overall;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewTabNew : MonoBehaviour
{
    private static ViewTabNew instance = null;

    private GameObject tabNewPanel = null;
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
    private UnityEngine.UI.InputField dataField;
    private List<Tuple<string, DateTime>> InfoScrollAreaTextQueue = new List<Tuple<string, DateTime>>();
    private float InfoScrollAreaMessageEvery = 0.5f; //Message every x seconds
    private int InfoScrollAreaTextLines = 5; //Max lines of messages

    //HERETODO: repair add connection button

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null) {
            instance = this;
        }
        linkUI();
        addTestConnections();
    }

    private void addTestConnections() {
        Presenter.Instance.addConnection("Win", NetworkModel.Protocol.UDP, TransferProtocol.ConnectionType.ReceiveAndSend, "192.168.0.3", "65535", "192.168.0.3", "65535");
    }

    private void linkUI() {
        //Main Panel for script
        tabNewPanel = AppModel.Instance.tabNewPanel;

        //View
        buttonSend = tabNewPanel.transform.FindDeepChild("SendButton").gameObject.GetComponent<UnityEngine.UI.Button>();
        buttonReceive = tabNewPanel.transform.FindDeepChild("ReceiveButton").gameObject.GetComponent<UnityEngine.UI.Button>();
        buttonAdd = tabNewPanel.transform.FindDeepChild("AddButton").gameObject.GetComponent<UnityEngine.UI.Button>();
        toggleSend = tabNewPanel.transform.FindDeepChild("SendToggle").gameObject.GetComponent<UnityEngine.UI.Toggle>();
        toggleReceive = tabNewPanel.transform.FindDeepChild("ReceiveToggle").gameObject.GetComponent<UnityEngine.UI.Toggle>();
        buttonSend.onClick.AddListener(onClickSend);
        buttonReceive.onClick.AddListener(onClickReceive);
        buttonAdd.onClick.AddListener(onClickAddConnection);


        //NetworkModel
        receiveIP = tabNewPanel.transform.FindDeepChild("ReceiveIP").gameObject.GetComponent<UnityEngine.UI.Dropdown>();
        receivePort = tabNewPanel.transform.FindDeepChild("ReceivePort").gameObject.GetComponent<UnityEngine.UI.InputField>();
        destinationIP = tabNewPanel.transform.FindDeepChild("DestinationIP").gameObject.GetComponent<UnityEngine.UI.InputField>();
        destinationPort = tabNewPanel.transform.FindDeepChild("DestinationPort").gameObject.GetComponent<UnityEngine.UI.InputField>();
        scrollAreaText = tabNewPanel.transform.FindDeepChild("InfoScrollArea").gameObject.GetComponentInChildren<UnityEngine.UI.Text>();
        dataField = tabNewPanel.transform.FindDeepChild("DataField").gameObject.GetComponentInChildren<UnityEngine.UI.InputField>();


        //TODO:move refreshReceiveIPDropdown to some more appropiate place
        refreshReceiveIPDropdown();

        //Start info scroll area text coroutine
        StartCoroutine("WaitAndPrintToInfoScrollArea");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addTextToInfoScrollArea(string text) {
        InfoScrollAreaTextQueue.Add(new Tuple<string, DateTime>(text, DateTime.Now));
    }

    private IEnumerator WaitForReceivingData(TransferProtocol tp) {
        bool receiveData = true;
        while (receiveData) {
            //https://gamedevbeginner.com/coroutines-in-unity-when-and-how-to-use-them/
            yeild return new WaitUntil(ReceivedData);
        }
    }

    // every 500 ms print
    private IEnumerator WaitAndPrintToInfoScrollArea()
    {
        while (true)
        {
            //If there are messages in the queue
            if (InfoScrollAreaTextQueue.Count > 0){
                //Append text
                scrollAreaText.text += "\n" + InfoScrollAreaTextQueue[InfoScrollAreaTextQueue.Count - 1].Item2 + " " + 
                    InfoScrollAreaTextQueue[InfoScrollAreaTextQueue.Count - 1].Item1;
                
                //Delete last text in the queue
                InfoScrollAreaTextQueue.RemoveAt(InfoScrollAreaTextQueue.Count-1);

                //If text for user is too long then delete last line
                if (scrollAreaText.text.Split('\n').Length > InfoScrollAreaTextLines) {
                    scrollAreaText.text = scrollAreaText.text.Substring(scrollAreaText.text.IndexOf('\n')+1);
                }
            }
            yield return new WaitForSeconds(InfoScrollAreaMessageEvery);
        }
    }

    private void refreshReceiveIPDropdown() {
        receiveIP.options.Clear();
        foreach (string s in Presenter.Instance.getOriginIPsString())
        {
            receiveIP.options.Add(new UnityEngine.UI.Dropdown.OptionData() { text = s });
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

    private void onClickAddConnection() {
        string result = Presenter.Instance.addConnection("Win", NetworkModel.Protocol.UDP, TransferProtocol.ConnectionType.ReceiveAndSend, getSelectedOriginIPString(), receivePort.text, destinationIP.text, destinationPort.text);
        //Adding message to the info panel
        addTextToInfoScrollArea("Adding connection: " + getSelectedOriginIPString() + " " + receivePort.text + " ..." + result);
    }

    public void onClickSend()
    {
        //NetworkModel.Instance.sendChatText("Test!");
        string message = dataField.text;
        Presenter.Instance.send(codeStringToByte(message));
        addTextToInfoScrollArea("Sent:" + message);
    }

    public byte[] codeStringToByte(string message)
    {
        List<byte> byteMessage = new List<byte>();
        foreach (char c in message)
        {
            byteMessage.Add((byte)c);
        }
        return byteMessage.ToArray();
    }

    public void onClickReceive()
    {
        //UnityEngine.Events.UnityEvent receiveDataFromClient = new UnityEngine.Events.UnityEvent();
        /*string s = "";
        List<byte[]> receivedData = Presenter.Instance.receive();
        s += (receivedData == null ? "NULL" : decodeByteToString(receivedData[0]) );

        addTextToInfoScrollArea("Received:" + s);
        */

    }

    public string decodeByteToString(byte[] data) {
        string s = "";
        foreach (byte b in data) {
            s += ((char)b);
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

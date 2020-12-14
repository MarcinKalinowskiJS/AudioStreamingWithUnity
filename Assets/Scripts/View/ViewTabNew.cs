using System;
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
        Presenter.Instance.addConnection("192.168.0.3", "65535", "192.168.0.3", "65535");
        Presenter.Instance.addConnection("192.168.0.3", "65534", "192.168.0.1", "65535");
        Presenter.Instance.addConnection("192.168.0.3", "65533", "192.168.0.100", "65535");
    }

    private void linkUI() {
        //View
        buttonSend = GameObject.Find("SendButton").GetComponent<UnityEngine.UI.Button>();
        buttonReceive = GameObject.Find("ReceiveButton").GetComponent<UnityEngine.UI.Button>();
        buttonAdd = GameObject.Find("AddButton").GetComponent<UnityEngine.UI.Button>();
        toggleSend = GameObject.Find("SendToggle").GetComponent<UnityEngine.UI.Toggle>();
        toggleReceive = GameObject.Find("ReceiveToggle").GetComponent<UnityEngine.UI.Toggle>();
        buttonSend.onClick.AddListener(onClickSend);
        buttonReceive.onClick.AddListener(onClickReceive);
        buttonAdd.onClick.AddListener(onClickAddConnection);


        //NetworkModel
        receiveIP = GameObject.Find("ReceiveIP").GetComponent<UnityEngine.UI.Dropdown>();
        receivePort = GameObject.Find("ReceivePort").GetComponent<UnityEngine.UI.InputField>();
        destinationIP = GameObject.Find("DestinationIP").GetComponent<UnityEngine.UI.InputField>();
        destinationPort = GameObject.Find("DestinationPort").GetComponent<UnityEngine.UI.InputField>();
        scrollAreaText = GameObject.Find("InfoScrollArea").GetComponentInChildren<UnityEngine.UI.Text>();

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

    // every 500 ms print
    private IEnumerator WaitAndPrintToInfoScrollArea()
    {
        while (true)
        {
            //If there are messages in the queue
            if (InfoScrollAreaTextQueue.Count > 0){
                //Append text
                scrollAreaText.text += InfoScrollAreaTextQueue[InfoScrollAreaTextQueue.Count - 1].Item2 + " " + 
                    InfoScrollAreaTextQueue[InfoScrollAreaTextQueue.Count - 1].Item1 + "\n";
                
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
        Presenter.Instance.addConnection(getSelectedOriginIPString(), receivePort.text, destinationIP.text, destinationPort.text);
        //Adding message to the info panel
        addTextToInfoScrollArea("Adding connection: " + getSelectedOriginIPString() + " " + receivePort.text + " ...");
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

using Assets.Scripts.Model.Additional;
using Assets.Scripts.Overall;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewTabNew : MonoBehaviour
{
    private static ViewTabNew instance = null;
    private bool receiveSth = false;

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
    private List<Tuple<string, DateTime?>> InfoScrollAreaTextQueue = new List<Tuple<string, DateTime?>>();
    private float InfoScrollAreaMessageEvery = 0.5f; //Message every x seconds
    private int InfoScrollAreaTextLines = 5; //Max lines of messages
    private List<float> checkMessagesEvery = new List<float>();
    private float defaultCheckMessageEvery = 0.0f;
    int iterator = 0;

    //HERETODO: Check how this script is added to the GameObject in editor. 
    //Maybe there are two scripts ViewTabNew added: one in code one in editor.

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        linkUI();
        //Start info scroll area text coroutine
        StartCoroutine("WaitAndPrintToInfoScrollArea");


        addTestConnections();
    }

    private void addTestConnections()
    {
        //string result = Presenter.Instance.addConnection("Win", NetworkModel.Protocol.UDP, TransferProtocol.ConnectionType.ReceiveAndSend, "192.168.0.3", "65535", "192.168.0.3", "65535");
        //Adding message to the info panel
        //addTextToInfoScrollArea("Adding connection: 192.168.0.3:65535 - " + result);

        //List<byte[]> dataTest = NetworkModel.Instance.Receive();


        /*
        //Receive data after some time
        if (result.Contains("Added"))
        {
            checkMessagesEvery.Add(defaultCheckMessageEvery);
            StartCoroutine(WaitForReceivingData(checkMessagesEvery.Count-1));
        }
        */
    }

    private void linkUI()
    {
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
        //StartCoroutine(WaitForSampleData());
        Debug.Log("AMI" + AudioModel.Instance);
    }

    // Update is called once per frame
    void Update()
    {
        //iterator++;
        Debug.Log("vtnSENDED?: " + NetworkModel.Instance.Send(new byte[] { 28, 14, 121, 190 }, TransferProtocol.DataType.LeftChannel));
    }

    IEnumerator WaitForSampleData() {
        string receivedBytes;
        while (true)
        {
            List<byte[]> receivedData = Presenter.Instance.receive();

            if (receivedData != null && receivedData.Count > 0 && receivedData[0] != null)
            {
                Debug.Log("R:" + receivedData[0].Length);
            }

            yield return new WaitForSeconds(0.0f);
        }
    }
    
    public void addTextToInfoScrollArea(string text)
    {
        bool withDate = false;
        if (withDate == true)
        {
            InfoScrollAreaTextQueue.Add(new Tuple<string, DateTime?>(text, DateTime.Now));
        }
        else {
            InfoScrollAreaTextQueue.Add(new Tuple<string, DateTime?>(text, null));
        }
    }

    private IEnumerator WaitForReceivingData(int checkIndex)
    {
        bool receiveData = true;
        string s = "";
        while (receiveData)
        {
            s = receiveDataAsString();

            if (!s.Equals(""))
            {
                addTextToInfoScrollArea("Received:" + s);
            }
            yield return new WaitForSeconds(checkMessagesEvery[checkIndex]);
        }
    }


    // every 500 ms print
    private IEnumerator WaitAndPrintToInfoScrollArea()
    {
        while (true)
        {
            //If there are messages in the queue
            if (InfoScrollAreaTextQueue.Count > 0)
            {
                //Append text
                scrollAreaText.text += "\n" + InfoScrollAreaTextQueue[InfoScrollAreaTextQueue.Count - 1].Item2 + " " +
                    InfoScrollAreaTextQueue[InfoScrollAreaTextQueue.Count - 1].Item1;

                //Delete last text in the queue
                InfoScrollAreaTextQueue.RemoveAt(InfoScrollAreaTextQueue.Count - 1);

                //If text for user is too long then delete last line
                if (scrollAreaText.text.Split('\n').Length > InfoScrollAreaTextLines)
                {
                    scrollAreaText.text = scrollAreaText.text.Substring(scrollAreaText.text.IndexOf('\n') + 1);
                }
            }
            yield return new WaitForSeconds(InfoScrollAreaMessageEvery);
        }
    }

    private void refreshReceiveIPDropdown()
    {
        receiveIP.options.Clear();
        foreach (string s in Presenter.Instance.getOriginIPsString())
        {
            receiveIP.options.Add(new UnityEngine.UI.Dropdown.OptionData() { text = s });
        }
        receiveIP.RefreshShownValue();
    }

    public static ViewTabNew Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject();
                instance = go.AddComponent<ViewTabNew>();
            }
            return instance;
        }
    }

    private void onClickAddConnection()
    {
        string result = Presenter.Instance.addConnection("Win", NetworkModel.Protocol.UDP, TransferProtocol.ConnectionType.ReceiveAndSend, getSelectedOriginIPString(), receivePort.text, destinationIP.text, destinationPort.text);
        if (result.Contains("Added")) {
            checkMessagesEvery.Add(defaultCheckMessageEvery);
        }
        //Adding message to the info panel
        addTextToInfoScrollArea("Adding connection: " + getSelectedOriginIPString() + " " + receivePort.text + "-" + result);
        //Receive data after some time
        StartCoroutine(WaitForReceivingData(checkMessagesEvery.Count));
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
        s += (receivedData[0] == null ? "" : decodeByteToString(receivedData[0]));

        if (!s.Equals(""))
        {
            addTextToInfoScrollArea("Received:" + s);
        }*/

        addTextToInfoScrollArea(AudioModel.Instance.getAverageSampleRateForSecond().ToString());
    }

    public string receiveDataAsString()
    {
        List<byte[]> receivedData = Presenter.Instance.receive();
        return (receivedData[0] == null ? "" : decodeByteToString(receivedData[0]));
    }

    public string decodeByteToString(byte[] data)
    {
        string s = "";
            foreach (byte b in data)
            {
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

    private void OnApplicationQuit()
    {
        //AudioModel.Instance.Cleanup();
    }

}


//https://gamedevbeginner.com/coroutines-in-unity-when-and-how-to-use-them/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View : MonoBehaviour
{
    private static View instance = null;

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
        

        //NetworkController
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

    public static View Instance {
        get {
            if (instance == null) {
                GameObject go = new GameObject();
                instance = go.AddComponent<View>();
            }
            return instance;
        }
    }

    public void onClickSend()
    {
        NetworkModel.Instance.sendChatText("Test!");
        //getAll();
    }

    public void onClickReceive()
    {
        UnityEngine.Events.UnityEvent receiveDataFromClient = new UnityEngine.Events.UnityEvent();
        //Presenter.Instance.receive(receiveDataFromClient);
        NetworkModel.Instance.receiveUDPAsync();

        //scrollAreaText.text = data;
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

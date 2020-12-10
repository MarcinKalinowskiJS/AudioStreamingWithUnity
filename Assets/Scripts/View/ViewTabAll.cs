using Assets.Scripts.Model.Additional;
using Assets.Scripts.Overall;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class ViewTabAll : MonoBehaviour
{
    private GameObject TabAllPanel;
    private DropdownStoreTransferProtocolExtension connectionsDropdown;
    private UnityEngine.UI.Dropdown connectionTypeDropdown;
    private UnityEngine.UI.Dropdown receiveIP;
    private UnityEngine.UI.InputField receivePort;
    private UnityEngine.UI.InputField destinationIP;
    private UnityEngine.UI.InputField destinationPort;
    //TODO: Load TransferProtocol object to be viewed and edited


    // Start is called before the first frame update
    void Start()
    {

        TabAllPanel = AppModel.Instance.tabAllPanel;
        connectionTypeDropdown = AppModel.Instance.tabAllPanel.transform.FindDeepChild("ConnectionTypeDropdown").gameObject.GetComponent<UnityEngine.UI.Dropdown>();
        receiveIP = TabAllPanel.transform.FindDeepChild("ReceiveIP").GetComponent<UnityEngine.UI.Dropdown>();
        receivePort = TabAllPanel.transform.FindDeepChild("ReceivePort").GetComponent<UnityEngine.UI.InputField>();
        destinationIP = TabAllPanel.transform.FindDeepChild("DestinationIP").GetComponent<UnityEngine.UI.InputField>();
        destinationPort = TabAllPanel.transform.FindDeepChild("DestinationPort").GetComponent<UnityEngine.UI.InputField>();
        connectionsDropdown = TabAllPanel.transform.FindDeepChild("ConnectionsDropdown").GetComponent<DropdownStoreTransferProtocolExtension>();
        connectionsDropdown.onValueChanged.AddListener(onValueChangedInConnectionsDropdown);


        refreshConnectionsDropdown();
        loadConnectionDetails();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void onValueChangedInConnectionsDropdown(int arg0)
    {
        loadConnectionDetails();
    }

    public void refreshConnectionsDropdown() {
        //connectionsDropdown.transform.Translate(new Vector3(100, 100, 0));
        //Commented because is invoked on click in the app on tab(All)
        
        connectionsDropdown.ClearOptionsWithObjects();
        foreach (TransferProtocol tp in Presenter.Instance.getConnections()) {
            connectionsDropdown.AddOptionWithObject(tp, new UnityEngine.UI.Dropdown.OptionData() { text = Presenter.Instance.getConnectionAdjusted(tp) });
        }
        connectionsDropdown.RefreshShownValue();
    }

    public void loadConnectionDetails() {
        if (connectionsDropdown.options.Count != 0)
        {
            receiveIP.addBulkSelectRefresh(Presenter.Instance.getOriginIPsString, getSelectedConnectionTP().receiveIP, Presenter.Instance, "getOriginIPsString");
            /*
            receiveIP.options.Clear();
            foreach (string ip in Presenter.Instance.getOriginIPsString())
            {
                receiveIP.options.Add(new UnityEngine.UI.Dropdown.OptionData() { text = ip });
            }
            for (int i = 0; i < receiveIP.options.Count; i++) {
                if (String.Equals(receiveIP.options[i].text, getSelectedConnectionTP().receiveIP)) {
                    receiveIP.value = i;
                    receiveIP.RefreshShownValue();
                    break;
                }
            }

            connectionTypeDropdown.options.Clear();
            foreach (string s in Presenter.Instance.getAllConnectionTypes()) {
                connectionTypeDropdown.options.Add(new UnityEngine.UI.Dropdown.OptionData() { text = ip });
            }
            */
        }
    }

    public TransferProtocol getSelectedConnectionTP() {
        return connectionsDropdown.GetTransferProtocol(connectionsDropdown.value);
    }
    //TODO: Check package Unity UI Extensions
}
/*
System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
string list = "";
        for (int i = 0; i<st.FrameCount; i++) {
            list += st.GetFrame(i).GetMethod().Name + "<=";
        }

        Debug.Log("L: " + list);
        */
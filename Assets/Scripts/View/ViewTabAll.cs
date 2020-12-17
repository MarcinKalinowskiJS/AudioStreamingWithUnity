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
    private GameObject tabAllPanel;
    private DropdownStoreTransferProtocolExtension connectionsDropdown;
    private UnityEngine.UI.Dropdown connectionTypeDropdown;
    private UnityEngine.UI.Dropdown receiveIP;
    private UnityEngine.UI.InputField receivePort;
    private UnityEngine.UI.InputField destinationIP;
    private UnityEngine.UI.InputField destinationPort;


    // Start is called before the first frame update
    void Start()
    {

        tabAllPanel = AppModel.Instance.tabAllPanel;
        connectionTypeDropdown = tabAllPanel.transform.FindDeepChild("ConnectionTypeDropdown").gameObject.GetComponent<UnityEngine.UI.Dropdown>();
        receiveIP = tabAllPanel.transform.FindDeepChild("ReceiveIP").GetComponent<UnityEngine.UI.Dropdown>();
        receivePort = tabAllPanel.transform.FindDeepChild("ReceivePort").GetComponent<UnityEngine.UI.InputField>();
        destinationIP = tabAllPanel.transform.FindDeepChild("DestinationIP").GetComponent<UnityEngine.UI.InputField>();
        destinationPort = tabAllPanel.transform.FindDeepChild("DestinationPort").GetComponent<UnityEngine.UI.InputField>();
        connectionsDropdown = tabAllPanel.transform.FindDeepChild("ConnectionsDropdown").GetComponent<DropdownStoreTransferProtocolExtension>();
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
            receiveIP.addBulkSelectRefresh(getSelectedConnectionTP().receiveIP, 
                Presenter.Instance, "getOriginIPsString");
            connectionTypeDropdown.addBulkSelectRefresh(getSelectedConnectionTP().connectionType.ToString(),
                Presenter.Instance, "getAllConnectionTypes");
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
          
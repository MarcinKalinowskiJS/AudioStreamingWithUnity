using Assets.Scripts.Model.Additional;
using Assets.Scripts.Overall;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewTabAll : MonoBehaviour
{
    private DropdownStoreTransferProtocolExtension connectionsDropdown;
    private UnityEngine.UI.Dropdown connectionTypeDropdown;
    private UnityEngine.UI.Dropdown receiveIP;
    private UnityEngine.UI.InputField receivePort;
    private UnityEngine.UI.InputField destinationIP;
    private UnityEngine.UI.InputField destinationPort;
    public GameObject prefab;
    //TODO: Load TransferProtocol object to be viewed and edited


    // Start is called before the first frame update
    void Start()
    {
        //GameObject.Find("Canvas").AddComponent<DropdownStoreTransferProtocolExtension>();
        //connectionsDropdown = new DropdownStoreTransferProtocolExtension<TransferProtocol>(AppModel.Instance.tabAllPanel
        //            .transform.FindDeepChild("ConnectionsDropdown").gameObject.GetComponent<UnityEngine.UI.Dropdown>());
        

        //TODO: Transfer linked fields from dropdown to DropdownStoreTransferProtocolExcension component
        GameObject goTmp0 = AppModel.Instance.tabAllPanel.transform.FindDeepChild("ConnectionsDropdownToReplaceByScript").gameObject;
        GameObject goTmp0Clone = Instantiate(goTmp0);
        DestroyImmediate(goTmp0.GetComponent<UnityEngine.UI.Dropdown>());
        connectionsDropdown = goTmp0.AddComponent<DropdownStoreTransferProtocolExtension>();
        DropdownStoreTransferProtocolExtension.Copy(goTmp0Clone.GetComponent<UnityEngine.UI.Dropdown>(), ref connectionsDropdown);
        //CopyValues(goTmp0Clone.GetComponent<UnityEngine.UI.Dropdown>(), goTmp0.AddComponent<DropdownStoreTransferProtocolExtension>());
        Destroy(goTmp0Clone);

        //Destroy(goTmp1);
        //goTmp2.AddComponent<>

        //Destroy(goTmp.GetComponent<UnityEngine.UI.Dropdown>());
        //foreach (Transform tTmp in goTmp.transform.Find("")) {
        //    Debug.Log("Destroy:");
        //    Destroy(tTmp.gameObject);
        //}
        //connectionsDropdown = goTmp.AddComponent<DropdownStoreTransferProtocolExtension>();


        //connectionsDropdown.transform.position = goTmp.GetComponent<UnityEngine.UI.Dropdown>().transform.position;
        //connectionsDropdown.transform.rotation = goTmp.GetComponent<UnityEngine.UI.Dropdown>().transform.rotation;

        //TODO: Replacing component dropdown for extended dropdown
        //TODO: Create hierarchy linker singleton class
        connectionTypeDropdown = AppModel.Instance.tabAllPanel.transform.FindDeepChild("ConnectionTypeDropdown").gameObject.GetComponent<UnityEngine.UI.Dropdown>();
        receiveIP = GameObject.Find("ReceiveIP").GetComponent<UnityEngine.UI.Dropdown>();
        receivePort = GameObject.Find("ReceivePort").GetComponent<UnityEngine.UI.InputField>();
        destinationIP = GameObject.Find("DestinationIP").GetComponent<UnityEngine.UI.InputField>();
        destinationPort = GameObject.Find("DestinationPort").GetComponent<UnityEngine.UI.InputField>();

        //Replacing actual connections dropdown with extended dropdown for storing objects
        //GameObject goTmp = Instantiate(prefab, transform.position, Quaternion.identity, this.transform);//AppModel.Instance.tabAllPanel.transform.Find("Content").gameObject.transform);
        
        //connectionsDropdown = goTmp.GetComponent < prefab.GetType().FullName > ();

        refreshConnectionsDropdown();
        //loadConnectionDetails();
    }

    public void CopyValues<T>(T from, T to)
    {
        JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(from), to);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void refreshConnectionsDropdown() {
        //connectionsDropdown.transform.Translate(new Vector3(100, 100, 0));
        //Commented because is invoked on click in the app on tab(All)
        /*connectionsDropdown.ClearOptionsWithObjects();
        foreach (TransferProtocol tp in Presenter.Instance.getConnections()) {
            connectionsDropdown.AddOptionWithObject(tp, new UnityEngine.UI.Dropdown.OptionData() { text = Presenter.Instance.getConnectionAdjusted(tp) });
        }
        connectionsDropdown.RefreshShownValue();
        */
    }

    public void loadConnectionDetails() {
        /*receiveIP.options.Clear
        foreach (string ip in Presenter.Instance.getOriginIPsString()) {
            receiveIP.options.Add(new UnityEngine.UI.Dropdown.OptionData() { text = ip });
        }
        receiveIP.value = Presenter.Instance.getConnection()
        */
    }

    //TODO: Check package Unity UI Extensions
}

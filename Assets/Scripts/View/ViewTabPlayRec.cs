using Assets.Scripts.Model.Additional;
using Assets.Scripts.Overall;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

public class ViewTabPlayRec : MonoBehaviour
{
    private GameObject tabPlayRecPanel;
    private DropdownStoreTransferProtocolExtension connectionsDropdown;
    private UnityEngine.UI.Button fileChooseButton;
    private UnityEngine.UI.Dropdown connectionTypeDropdown;
    private UnityEngine.UI.Dropdown receiveIP;
    private UnityEngine.UI.InputField receivePort;
    private UnityEngine.UI.InputField destinationIP;
    private UnityEngine.UI.InputField destinationPort;


    // Start is called before the first frame update
    void Start()
    {

        tabPlayRecPanel = AppModel.Instance.tabPlayRecPanel;
        fileChooseButton = tabPlayRecPanel.transform.FindDeepChild("FileChooseButton").gameObject.GetComponent<UnityEngine.UI.Button>();
        /*receiveIP = tabAllPanel.transform.FindDeepChild("ReceiveIP").GetComponent<UnityEngine.UI.Dropdown>();
        receivePort = tabAllPanel.transform.FindDeepChild("ReceivePort").GetComponent<UnityEngine.UI.InputField>();
        destinationIP = tabAllPanel.transform.FindDeepChild("DestinationIP").GetComponent<UnityEngine.UI.InputField>();
        destinationPort = tabAllPanel.transform.FindDeepChild("DestinationPort").GetComponent<UnityEngine.UI.InputField>();
        connectionsDropdown = tabAllPanel.transform.FindDeepChild("ConnectionsDropdown").GetComponent<DropdownStoreTransferProtocolExtension>();
        connectionsDropdown.onValueChanged.AddListener(onValueChangedInConnectionsDropdown);
        */

        fileChooseButton.onClick.AddListener(OnClickOpenFileChooseDialog);

    }

   
    private void OnClickOpenFileChooseDialog()
    {
        Debug.Log("Button test");
        //Application.isMobilePlatform
        string filePath = EditorUtility.OpenFilePanel("Select waveformat info file", Directory.localFolder, "wfi");

        //update button name
        //fileChooseButton.(filePath);
        if (!filePath.Equals("")){
            ((UnityEngine.UI.Text)fileChooseButton.GetComponentInChildren<UnityEngine.UI.Text>()).text = filePath;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
    

}
          
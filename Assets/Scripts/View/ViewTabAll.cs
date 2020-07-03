using Assets.Scripts.Overall;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewTabAll : MonoBehaviour
{
    private UnityEngine.UI.Dropdown connectionsDropdown;


    //TODO: Load TransferProtocol object to be viewed and edited


    // Start is called before the first frame update
    void Start()
    {
        connectionsDropdown = AppModel.Instance.tabAllPanel.transform.FindDeepChild("ConnectionsDropdown").gameObject.GetComponent<UnityEngine.UI.Dropdown>();
        refreshConnectionsDropdown();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void refreshConnectionsDropdown() {
        connectionsDropdown.options.Clear();
        foreach (string connection in Presenter.Instance.getConnectionsAdjusted())
        {
            connectionsDropdown.options.Add(new UnityEngine.UI.Dropdown.OptionData(connection));
        }
        connectionsDropdown.RefreshShownValue();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewTabAll : MonoBehaviour
{
    private UnityEngine.UI.Dropdown connectionsDropdown;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Tu: " + AppModel.Instance.tabAllPanel.transform.Find("ConnectionsDropdown").
        //    gameObject.GetComponentInChildren<UnityEngine.UI.Dropdown>());
        Debug.Log("Namos: " + AppModel.Instance.tabAllPanel.transform.Find("Content/ConnectionsDropdown"));

        connectionsDropdown = AppModel.Instance.tabAllPanel.transform.Find("ConnectionsDropdown").gameObject.GetComponent<UnityEngine.UI.Dropdown>();



        getAllChilds(AppModel.Instance.tabAllPanel.transform);
        



        connectionsDropdown.options.Clear();
        connectionsDropdown.options.Add(new UnityEngine.UI.Dropdown.OptionData("abde"));
        connectionsDropdown.RefreshShownValue();
    }

    public void getAllChilds(Transform t) {
        List<string> childList = new List<string>();
        foreach (string s in getAllChildsHelper(t, childList))
        {
            Debug.Log(s);
        }
    }

    public static List<string> getAllChildsHelper(Transform t, List<string> childList) {
        //Middle
        childList.Add(t.gameObject.name);

        //Recursive call
        for (int i = 0; i < t.childCount; i++) {
            getAllChildsHelper(t.GetChild(i), childList);
        }

        //End
        return childList;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}

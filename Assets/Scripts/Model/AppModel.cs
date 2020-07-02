using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppModel : MonoBehaviour
{
    private static AppModel instance;

    public GameObject tabAllPanel = null;
    public GameObject tabNewPanel = null;
    // Start is called before the first frame update
    void Start()
    {
        instance = GameObject.Find("AppGameObject").GetComponent<AppModel>();
    }

    public static AppModel Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.Find("AppGameObject").AddComponent<AppModel>();
                instance.tabNewPanel = GameObject.Find("TabNew");
                instance.tabAllPanel = GameObject.Find("TabAll");
            }
            return instance;
        }
    }
}

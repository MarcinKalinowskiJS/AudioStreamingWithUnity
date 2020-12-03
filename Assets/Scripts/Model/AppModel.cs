using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppModel : MonoBehaviour
{
    
    private static AppModel instance;

    public GameObject tabAllPanel = null;
    public GameObject tabNewPanel = null;
    //DO STREAMOWANIA AUDIO W UNITY https://forum.unity.com/threads/singleton-monobehaviour-script.99971/ 
    //Then do not ever add it to something, just call MyClass.Instance whenever you need it. Note: If you want it to persist accross 
    //scenes and not be destroyed and recreated add DontDestroyOnLoad(this); in the Awake Function. Another Option if you really want 
    //to add it to a particular object is to set mInstance in the MyClass's Awake function and if mInstance is not null Destroy any 
    //subsequent attempts to create an Instance of MyClass. But in that case do not Set mInstance in the Instance Property, 
    //as it's being set by Awake.

    // Start is called before the first frame update
    void Start()
    {
        //Entity Component System
        //https://www.raywenderlich.com/7630142-entity-component-system-for-unity-getting-startedhttps://www.raywenderlich.com/7630142-entity-component-system-for-unity-getting-started

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

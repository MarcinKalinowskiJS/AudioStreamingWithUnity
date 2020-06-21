using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Presenter
{
    private static Presenter instance = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static Presenter Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Presenter();
            }
            return instance;
        }
    }

    
    
}

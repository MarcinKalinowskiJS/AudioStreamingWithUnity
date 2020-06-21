using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.UI.Text t = this.GetComponent<UnityEngine.UI.Text>();
        string testText = "Test1\n";
        for (int i = 0; i < 5; i++) {
            testText += i + " text";
        }

        t.text = testText;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

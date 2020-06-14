using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class NetworkController : MonoBehaviour
{
    private UnityEngine.UI.Dropdown originIpDropdown;
    // Start is called before the first frame update
    void Start()
    {
        originIpDropdown = GameObject.Find("OriginIPDropdown").GetComponent<UnityEngine.UI.Dropdown>();
        originIpDropdown.options.Clear();
        string name = Dns.GetHostName();
        //string[] addresses = Dns.Resolve(name).AddressList;
        IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
        foreach (IPAddress iPA in ips) {
            originIpDropdown.options.Add(new Dropdown.OptionData() { text = iPA.ToString() });
        }
        originIpDropdown.RefreshShownValue();
    }

    public string getSelectedOption() {
        return originIpDropdown.options[originIpDropdown.value].text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

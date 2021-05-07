using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class TestWebView : MonoBehaviour {

    private void OnGUI()
    {
        if (GUILayout.Button("Start", GUILayout.Width(100), GUILayout.Height(50)))
        {
            Application.OpenURL("www.baidu.com");
        }

        if (GUILayout.Button("IP", GUILayout.Width(100), GUILayout.Height(50)))
        {
            Debug.Log(string.Format("ipv4:{0}", GetIPv4Address()));
        }
    }

    public string GetIPv4Address()
    {
        IPHostEntry ipHostList = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var adr in ipHostList.AddressList)
        {
            if (adr.AddressFamily == AddressFamily.InterNetwork)
            {
                return adr.ToString();
            }
        }
        return null;
    }


}

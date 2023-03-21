using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerInfo
{
    public string ip;
}

public class ServerItemUI : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text info;

    public void Setup(ServerInfo serverInfo)
    {
        info.SetText(serverInfo.ip);
    }

    public void Connect()
    {
        FindObjectOfType<ServerDiscoveryUI>().Connect(transform.GetSiblingIndex());
    }
}

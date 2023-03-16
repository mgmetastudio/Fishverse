using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class OpenWorldManager : MonoBehaviour
{
    [Header("Network Manager Settings")]
    public Mirror.NetworkManagerHUD NetworkManagerUI;
    public Mirror.Discovery.NetworkDiscoveryHUD NetworkDiscoveryUI;
    [Header("Network")]
    public Mirror.NetworkManager NetworkManager;
    [Header("Change Name")]
    public string PlayerName;

    private void Start()
    {
        PlayerName = Fishverse_Core.instance.account_username;

        NetworkManagerUI.showGUI = false;
        NetworkDiscoveryUI.showGUI = false;

        FindServer();
    }

    public async void FindServer()
    {
        NetworkDiscoveryUI.FindServers();
        await UniTask.WaitForSeconds(.5f);
        if (!EnterServer())
            HostServer();
    }

    public void HostServer()
    {
        NetworkDiscoveryUI.StartHost();
    }

    public bool EnterServer()
    {
        return NetworkDiscoveryUI.ConnectRandom();
    }
}

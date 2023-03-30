using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Mirror.Discovery;
using UnityEngine;

public class ServerDiscoveryUI : MonoBehaviour
{
    [SerializeField] NetworkDiscoveryHUD discovery;

    [SerializeField] ServerItemUI serverItem;
    [SerializeField] Transform context;

    bool refresh;

    void OnEnable()
    {
        // discovery.FindServers();
        // RefreshList();
    }

    void OnDisable()
    {
        refresh = false;
    }

    async void RefreshList()
    {
        refresh = true;

        while (refresh)
        {
            context.DestroyAllChild();

            // discovery.FindServers();
            foreach (ServerResponse info in discovery.discoveredServers.Values)
            {
                var server = Instantiate(serverItem, context);
                server.Setup(new ServerInfo { ip = info.EndPoint.Address.ToString() });
            }

            await UniTask.WaitForSeconds(.5f);
        }
    }

    public void Connect(int index)
    {
        discovery.Connect(discovery.discoveredServers.Values.ToList()[index]);
    }
}

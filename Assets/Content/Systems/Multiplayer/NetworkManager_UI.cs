using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkManager_UI : MonoBehaviour
{
    public Button btn_host;
    public Button btn_server;
    public Button btn_client;

    private void Awake()
    {
        btn_host.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });

        btn_server.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
        });

        btn_client.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
    }
}

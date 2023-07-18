using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using static LibEngineInstaller;
using Zenject;
using LibEngine.Auth;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public string menu_scene;
    public GameObject player_prefab;
    public Transform[] spawn_points;

    private void Start()
    {
        GetComponent<UserListManager>().RefreshUserList();

        if (!PhotonNetwork.IsConnected)
        {
            return;
        }

        var photonView = PhotonView.Get(this);
        int index = 0;
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (Player pl in PhotonNetwork.PlayerList)
            {
                photonView.RPC("InstantiationPlayer", pl, index);
                index++;
            }
        }
    }

    private InventoryRemote _inventoryRemote;
    private DiContainer _diContainer;


    [Inject]
    public void SomeInject(IAuthManager authTest)
    {
        _inventoryRemote = null;
    }

    [Inject]
    public void SomeInject2(InventoryRemote inventoryRemote, DiContainer diContainer)
    {
        _inventoryRemote = inventoryRemote;
        _diContainer = diContainer;
    }

    [PunRPC]
    void InstantiationPlayer(int index)
    {
        var spawnedPlayerObj = PhotonNetwork.Instantiate(player_prefab.name, spawn_points[index].position, spawn_points[index].rotation);
        _diContainer.InjectGameObject(spawnedPlayerObj);
    }

    public void Leave()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            SceneManager.LoadScene(menu_scene);
        }
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(menu_scene);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogFormat("Player {0} entered room", newPlayer.NickName);

        GetComponent<UserListManager>().RefreshUserList();
return;
        if (PhotonNetwork.IsMasterClient)
        {
                photonView.RPC("InstantiationPlayer", newPlayer, Random.Range(0, spawn_points.Length - 1));
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("Player {0} left room", otherPlayer.NickName);

        GetComponent<UserListManager>().RefreshUserList();
    }
}

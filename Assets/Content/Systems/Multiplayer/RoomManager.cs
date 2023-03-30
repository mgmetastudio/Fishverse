using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

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

        // int i = 0;
        // foreach (var item in PhotonNetwork.CurrentRoom.Players)
        // {
        //     if (item.Value.IsLocal)
        //         PhotonNetwork.Instantiate(player_prefab.name, spawn_points[i].position, spawn_points[i].rotation);
        //     i++;
        // }

        // PhotonNetwork.Instantiate(player_prefab.name,
        //     spawn_points[Random.Range(0, spawn_points.Length - 1)].position,
        //     spawn_points[Random.Range(0, spawn_points.Length - 1)].rotation);

        var photonView = PhotonView.Get(this);
        int index = 0;
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (Player pl in PhotonNetwork.PlayerList)
            {
                //Debug.Log(pl);
                photonView.RPC("InstantiationPlayer", pl, index);

                index++;
            }
        }
    }


    [PunRPC]
    void InstantiationPlayer(int index)
    {
        PhotonNetwork.Instantiate(player_prefab.name, spawn_points[index].position, spawn_points[index].rotation);
        // PhotonNetwork.Instantiate(Playerprefabs[value].name, SpawnPoints[index].transform.position, Quaternion.identity, 0);
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
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("Player {0} left room", otherPlayer.NickName);

        GetComponent<UserListManager>().RefreshUserList();
    }
}

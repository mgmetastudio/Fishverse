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
        if (!PhotonNetwork.IsConnected)
        {
            return;
        }

        PhotonNetwork.Instantiate(player_prefab.name,
            spawn_points[Random.Range(0, spawn_points.Length - 1)].position,
            spawn_points[Random.Range(0, spawn_points.Length - 1)].rotation);
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
        SceneManager.LoadScene(menu_scene);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogFormat("Player {0} entered room", newPlayer.NickName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("Player {0} left room", otherPlayer.NickName);
    }
}

using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaitRoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] string menu_scene;

    [SerializeField] GameObject panelLoading;
    [SerializeField] TMPro.TMP_Text roomCode;

    int playersToStart = 2;

    async void Start()
    {
        // roomCode.SetText(LobbyManager.lastRoomCode);
        roomCode.SetText(PhotonNetwork.CurrentRoom.CustomProperties["RoomCode"].ToString());

        GetComponent<UserListManager>().RefreshUserList();

        await UniTask.WaitForSeconds(5f);

        LoadScene();
    }

    [ContextMenu("Leave Room")]
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
        
        if (PhotonNetwork.CurrentRoom.Players.Count == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            LoadScene();
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("Player {0} left room", otherPlayer.NickName);

        GetComponent<UserListManager>().RefreshUserList();

    }

    [ContextMenu("Load Game")]
    public void LoadScene()
    {
        //"MiniGame_" + 
        PhotonNetwork.LoadLevel(PhotonNetwork.CurrentRoom.CustomProperties["GameMap"].ToString());
        panelLoading.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaitRoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] string menu_scene;

    [SerializeField] GameObject panelLoading;
    [SerializeField] TMPro.TMP_Text roomCode;
    [SerializeField] TMPro.TMP_Text GameMode;
    [SerializeField] TMPro.TMP_Text GameMap;

    [Header("Game Modes Sprites")]
    public Sprite[] sprites;
    public Image imageComponent;

    int playersToStart = 2;

    async void Start()
    {
        // roomCode.SetText(LobbyManager.lastRoomCode);
        roomCode.SetText(PhotonNetwork.CurrentRoom.CustomProperties["RoomCode"].ToString());
        GameMode.SetText(PhotonNetwork.CurrentRoom.CustomProperties["GameMode"].ToString());
        GameMap.SetText(PhotonNetwork.CurrentRoom.CustomProperties["GameMap"].ToString());
        GetComponent<UserListManager>().RefreshUserList();
        if(PhotonNetwork.CurrentRoom.CustomProperties["GameMode"].ToString()== "Fishing")
        {
            imageComponent.sprite = sprites[0];
        }
        else if(PhotonNetwork.CurrentRoom.CustomProperties["GameMode"].ToString() == "Racing")
        {
            imageComponent.sprite = sprites[1];
        }
        else if (PhotonNetwork.CurrentRoom.CustomProperties["GameMode"].ToString() == "Open World")
        {
            imageComponent.sprite = sprites[2];
        }

        // await UniTask.WaitForSeconds(5f);

        // LoadScene();
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

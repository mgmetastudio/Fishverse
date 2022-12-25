using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public bool auto_connect = false;
    public string game_scene_name = "MP_Playground";

    [Space(10)]
    [Header("UI Refs:")]
    public GameObject panel_lobby;
    public GameObject panel_loading;
    public UI_InfoMessage panel_info_message;

    public void Start()
    {
        if (auto_connect)
        {
            Connect();
        }
    }

    public void Connect()
    {
        panel_lobby.SetActive(false);
        panel_loading.SetActive(true);
        
        //Set Username

        if (Fishverse_Core.instance)
        {
            if (Fishverse_Core.instance.account_username != "")
            {
                PhotonNetwork.NickName = Fishverse_Core.instance.account_username;
            }
            else
            {
                PhotonNetwork.NickName = "Guest_" + Random.Range(1000, 9999);
            }
        }
        else
        {
            PhotonNetwork.NickName = "Guest_" + Random.Range(1000, 9999);
        }

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected to server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("Connected to lobby");

        panel_loading.SetActive(false);
        panel_lobby.SetActive(true);
    }
    public void JoinRoom(string room_name)
    {
        panel_loading.SetActive(true);
        PhotonNetwork.JoinOrCreateRoom(room_name, new Photon.Realtime.RoomOptions { MaxPlayers = 2 }, null);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(game_scene_name);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        panel_loading.SetActive(false);
        panel_info_message.ShowMessage("Server is full");
        Debug.Log(message);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        panel_info_message.ShowMessage("Connection error");
        Debug.Log(cause);
    }
}

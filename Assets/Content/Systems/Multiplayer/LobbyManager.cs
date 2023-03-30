using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public bool auto_connect = false;
    public string roomScene;
    public static string gameplayScene = "MP_Playground";

    [Space(10)]
    [Header("UI Refs:")]
    public GameObject panel_lobby;
    public GameObject panel_loading;
    public UI_InfoMessage panel_info_message;

    System.Action onJoinedLobbyAction;

    public static string lastRoomCode;

    void Awake()
    {
        LobbyManager[] objs = FindObjectsOfType<LobbyManager>();

        if (objs.Length > 1) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        if (auto_connect)
        {
            Connect();
        }
    }

    public void Connect()
    {
        // panel_lobby.SetActive(false);
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

    public void ConnectHost()
    {
        Connect();
        onJoinedLobbyAction = JoinRoomRandom;
    }

    public void ConnectJoin(string roomCode)
    {
        Connect();
        onJoinedLobbyAction = () => JoinRoom(roomCode);
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

        onJoinedLobbyAction();

        // panel_loading.SetActive(false);
        // panel_lobby.SetActive(true);
    }
    public void JoinRoom(string room_name)
    {
        lastRoomCode = room_name;

        panel_loading.SetActive(true);
        // PhotonNetwork.JoinOrCreateRoom(room_name, new Photon.Realtime.RoomOptions { MaxPlayers = 2 }, null);

        PhotonNetwork.JoinRoom(room_name, null);
    }

    public void JoinRoomRandom()
    {
        string roomCode = RandomString(roomCodeLen);
        lastRoomCode = roomCode;

        panel_loading.SetActive(true);

        // PhotonNetwork.JoinOrCreateRoom(roomCode, new Photon.Realtime.RoomOptions { MaxPlayers = 2 }, null);
        PhotonNetwork.CreateRoom(roomCode, new Photon.Realtime.RoomOptions { MaxPlayers = 0 }, null);
        print("roomCode " + roomCode);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(roomScene);
        // PhotonNetwork.LoadLevel(gameplayScene);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        panel_loading.SetActive(false);
        // panel_info_message.ShowMessage("Server is full");
        panel_info_message.ShowMessage(message);
        Debug.Log(message);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        panel_info_message.ShowMessage("Connection error");
        Debug.Log(cause);
    }

    readonly int roomCodeLen = 5;
    static System.Random random = new System.Random();
    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    // void OnGUI()
    // {
    //     GUI.Label(new Rect(100, 100, 200, 200), _lastRoomCode);
    // }
}

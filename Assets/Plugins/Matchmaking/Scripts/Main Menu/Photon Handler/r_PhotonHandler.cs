using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

[System.Serializable]
public enum SeverRegionCode
{
    /// <summary>European servers in Amsterdam.</summary>
    eu = 0,
    /// <summary>US servers (East Coast).</summary>
    us = 1,
    /// <summary>Asian servers in Singapore.</summary>
    asia = 2,
    /// <summary>Japanese servers in Tokyo.</summary>
    jp = 3,
    /// <summary>Australian servers in Melbourne.</summary>
    au = 5,
    ///<summary>USA West, San José, usw</summary>
    usw = 6,
    ///<summary>South America, Sao Paulo, sa</summary>
    sa = 7,
    ///<summary>Canada East, Montreal, cae</summary>
    cae = 8,
    ///<summary>South Korea, Seoul, kr</summary>
    kr = 9,
    ///<summary>India, Chennai, in</summary>
    @in = 10,
    /// <summary>Russia, ru</summary>
    ru = 11,
    /// <summary>Russia East, rue</summary>
    rue = 12,
    /// <summary>South Africa, za</summary>
    za = 13,
    /// <summary>No region selected.</summary>
    none = 4
};

public class r_PhotonHandler : MonoBehaviourPunCallbacks
{
    public static r_PhotonHandler instance;

    [SerializeField] string roomScene;
    [SerializeField] string roomScene_QuickMatch;
    public r_RoomBrowserController m_RoomUI;
    public r_CreateRoomControllerUI r_create;

    [Header("PhotonSettings")]
    public SeverRegionCode DefaultServer = SeverRegionCode.eu;

    /// <summary>
    /// Here we are instancing our script to call easily from other scripts.
    /// We don't destroy it because this is our main helper. This script is still available when we are in game in another scene
    /// </summary>

    #region Unity Calls
    private void Awake()
    {
        if (instance)
        {
            Destroy(instance);
            Destroy(instance.gameObject);
        }

        instance = this;
        PhotonNetwork.IsMessageQueueRunning = false;

        Init();
    }

    private void Init()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = DefaultServer.ToString();
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Update()
    {
        Debug.Log("Room Count: " + PhotonNetwork.CountOfRooms);
        Debug.Log("Player in Room Count: " + PhotonNetwork.CountOfPlayersInRooms);
        Debug.Log("Server Region Im using: " + PhotonNetwork.CloudRegion);

        if (PhotonNetwork.CurrentRoom != null)
        {
            try
            {
                Debug.Log("Current room is visible: " + PhotonNetwork.CurrentRoom.IsVisible);
                Debug.Log("Room browser list: " + r_RoomBrowserController.instance.m_RoomBrowserList.Count);
            }
            catch (System.Exception)
            {
            }
            
        }
    }
    #endregion

    /// <summary>
    /// Here we are connecting to Photon and make our connection ready to play.
    /// </summary>

    #region Connecting
    public void ConnectToPhoton() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnected() => Debug.Log("Connected to Photon");

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby(TypedLobby.Default);
    #endregion

    /// <summary>
    /// We are creating a room easily from here. For this we need the room name and room options.
    /// </summary>

    #region Room Creation
    public void CreateRoom(string _RoomName, RoomOptions _RoomOptions) => PhotonNetwork.CreateRoom(_RoomName, _RoomOptions, null, null);

    public override void OnCreatedRoom() => Debug.Log("Created Room");
    #endregion

    /// <summary>
    /// In the section below the are catching the room list. We are displaying the rooms in the room browser.
    /// </summary>
    
    #region Room List
    public override void OnJoinedLobby() => Debug.Log("Connected to Lobby");

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (r_RoomBrowserController.instance != null)
        {
            r_RoomBrowserController.instance.m_RoomBrowserList = roomList;
            r_RoomBrowserController.instance.RefreshRoomBrowser();
        }

        r_LobbyController.instance.ReceiveRoomList(roomList);
    }
    #endregion

    /// <summary>
    /// When we joined a room, we are checking if the room players are in the lobby or in the game. 
    /// If they are waiting in the lobby, we have to call the enter lobby function. See the Lobby Controller for the functionalities.
    /// If the room players already started with playing the game, we join the game scene.
    /// </summary>
    
    #region On Join
    public override void OnJoinedRoom()
    {
        PhotonNetwork.IsMessageQueueRunning = true;

        string _RoomState = (string)PhotonNetwork.CurrentRoom.CustomProperties["RoomState"];

        switch (_RoomState)
        {
            case "InLobby": r_LobbyController.instance.EnterLobby(); Debug.Log("Joined Game"); break;
            case "InGame": LoadGame(); Debug.Log("Joined Game Lobby"); break;
        }
    }
    #endregion

    /// <summary>
    /// When we left the room we have to load the main menu scene. This is the build index 0.
    /// We also have to update the player list for players who still in the lobby.
    /// </summary>

    #region On Left
    public override void OnLeftRoom()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
            PhotonNetwork.LoadLevel(0);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        string _RoomState = (string)PhotonNetwork.CurrentRoom.CustomProperties["RoomState"];

        switch (_RoomState)
        {
            case "InLobby": r_LobbyController.instance.ListLobbyPlayers(); break;
            case "InGame": r_InGameController.instance.CheckMasterClient(); break;
        }
    }
    #endregion

    /// <summary>
    /// We are loading the game scene. Loading the game scene is used by the game map + "_" + the game mode.
    /// Like this we can have the same map with different game modes.
    /// </summary>
    
    #region Matchmaking
    public void LoadGame()
    {
        if (PhotonNetwork.IsMasterClient && m_RoomUI.IsButtonClicked == false)
        { PhotonNetwork.LoadLevel(roomScene); }
      
        if(PhotonNetwork.IsMasterClient && m_RoomUI.IsButtonClicked == true)
        { PhotonNetwork.LoadLevel(roomScene_QuickMatch); }
        // PhotonNetwork.LoadLevel(PhotonNetwork.CurrentRoom.CustomProperties["GameMap"].ToString() + "_" + PhotonNetwork.CurrentRoom.CustomProperties["GameMode"].ToString());
    }
    #endregion
}
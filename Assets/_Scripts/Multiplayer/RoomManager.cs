using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using static LibEngineInstaller;
using Zenject;
using LibEngine.Auth;
using LibEngine;
using System.Collections.Generic;


public class RoomManager : MonoBehaviourPunCallbacks
{
    public string menu_scene;
    public GameObject player_prefab;
    public Transform[] spawn_points;
    private List<Transform> availableSpawnPoints = new List<Transform>(); // List to track available spawn points
    private List<int> usedSpawnPointIndexes = new List<int>(); // List to track used spawn point indexes
    [Inject]
    private DiContainer _diContainer;
    public ArcadeVehicleController_Network boatController;

    private void Start()
    {
        GetComponent<UserListManager>().RefreshUserList();

        if (!PhotonNetwork.IsConnected)
        {
            return;
        }

        // Create a copy of the spawn points list for randomization
        availableSpawnPoints.AddRange(spawn_points);

        var photonView = PhotonView.Get(this);
        //int index = 0;
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (Player pl in PhotonNetwork.PlayerList)
            {
                int randomIndex = GetRandomUnusedSpawnIndex();
                photonView.RPC("InstantiationPlayer", pl, randomIndex);
                //index++;
            }
        }
    }

    [PunRPC]
    void InstantiationPlayer(int index)
    {
        var spawnedPlayerObj = PhotonNetwork.Instantiate(player_prefab.name, spawn_points[index].position, spawn_points[index].rotation);
        if(_diContainer != null)
            _diContainer.InjectGameObject(spawnedPlayerObj);

        boatController = spawnedPlayerObj.GetComponent<ArcadeVehicleController_Network>();

    }
    private int GetRandomUnusedSpawnIndex()
    {
        int randomIndex = Random.Range(0, availableSpawnPoints.Count);
        while (usedSpawnPointIndexes.Contains(randomIndex))
        {
            randomIndex = Random.Range(0, availableSpawnPoints.Count);
        }
        usedSpawnPointIndexes.Add(randomIndex);
        return randomIndex;
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

        foreach (PhotonView photonView in PhotonNetwork.PhotonViews)
        {
            // Check if the PhotonView is owned by the leaving player
            if (photonView.Owner.ActorNumber == otherPlayer.ActorNumber)
            {
                // Disable the player GameObject associated with the leaving player
                photonView.gameObject.SetActive(false);
            }
            if(!PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(photonView);
            }
        }

            GetComponent<UserListManager>().RefreshUserList();
 
    }
}

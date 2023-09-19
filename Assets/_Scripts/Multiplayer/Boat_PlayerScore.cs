using UnityEngine;
using Photon.Pun;

public class Boat_PlayerScore : MonoBehaviour
{
    public int score;
    private PhotonView photon_view;
    private UserListManager userlist_manager;
    private MiniGame_Manager MiniGame_Manager;
    private void Start()
    {
        photon_view = GetComponent<PhotonView>();
        userlist_manager = FindObjectOfType<UserListManager>();
        MiniGame_Manager = FindObjectOfType<MiniGame_Manager>();
        if (userlist_manager)
        {
            userlist_manager.RefreshUserList();
        }
    }
    private void Update()
    {
        if (MiniGame_Manager != null)
        {
            photon_view.RPC("UpdateHighScoreOnServer", RpcTarget.All, score);
            MiniGame_Manager.UpdatePlayerScore(photon_view.ViewID, score);

        }
    }
    [ContextMenu("Increse Score")]
    public void AddScore()
    {
        if (photon_view.IsMine)
        {
            photon_view.RPC("RPC_ScoreIncrese", RpcTarget.All, score);
        }
    }

    [ContextMenu("Increse Score")]
    public void AddScore(int scoreAmount)
    {
        if (photon_view.IsMine)
        {
            photon_view.RPC("RPC_ScoreIncrese", RpcTarget.All, score + scoreAmount);
        }
    }

    [PunRPC]
    public void RPC_ScoreIncrese(int new_score)
    {
        score = new_score;
        
        if (userlist_manager)
        {
            userlist_manager.RefreshUserList();
        }
    }
    [PunRPC]
    private void UpdateHighScoreOnServer(int newScore)
    {
        MiniGame_Manager MiniGame_Manager = FindObjectOfType<MiniGame_Manager>();
        MiniGame_Manager.UpdateHighScore(newScore,MiniGame_Manager.timePlayed);
    }
}

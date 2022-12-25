using UnityEngine;
using Photon.Pun;

public class Boat_PlayerScore : MonoBehaviour
{
    public int score;
    private PhotonView photon_view;
    private UserListManager userlist_manager;

    private void Start()
    {
        photon_view = GetComponent<PhotonView>();
        userlist_manager = FindObjectOfType<UserListManager>();

        if (userlist_manager)
        {
            userlist_manager.RefreshUserList();
        }
    }

    [ContextMenu("Increse Score")]
    public void AddScore()
    {
        if (photon_view.IsMine)
        {
            photon_view.RPC("RPC_ScoreIncrese", RpcTarget.All);
        }
    }

    [PunRPC]
    public void RPC_ScoreIncrese()
    {
        score = score + 10;
        
        if (userlist_manager)
        {
            userlist_manager.RefreshUserList();
        }
    }
}

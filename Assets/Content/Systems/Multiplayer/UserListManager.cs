using UnityEngine;
using Photon.Pun;
using TMPro;

public class UserListManager : MonoBehaviour
{
    public TMP_Text[] texts_all_users;

    [SerializeField] bool inGame = true;

    public void RefreshUserList()
    {
        foreach (TMP_Text text in texts_all_users)
        {
            text.text = "";
        }

        GameObject[] all_boats = GameObject.FindGameObjectsWithTag("Boat");

        if (inGame)
        {
            for (int i = 0; i < all_boats.Length; i++)
            {
                string player_name = all_boats[i].GetComponent<PhotonView>().Owner.NickName;
                string player_score = all_boats[i].GetComponent<Boat_PlayerScore>().score.ToString();
                texts_all_users[i].text = player_name + ": " + player_score;
            }
        }
        else
        {
            // for (int i = 0; i < PhotonNetwork.CurrentRoom.Players.Count; i++)
            // {
            //     string player_name = PhotonNetwork.CurrentRoom.Players[i].NickName;
            //     texts_all_users[i].text = player_name;
            // }
        }
    }
}

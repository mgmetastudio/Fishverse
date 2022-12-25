using UnityEngine;
using Photon.Pun;
using TMPro;

public class UserListManager : MonoBehaviour
{
    public TMP_Text[] texts_all_users;

    public void RefreshUserList()
    {
        foreach (TMP_Text text in texts_all_users)
        {
            text.text = "";
        }

        GameObject[] all_boats = GameObject.FindGameObjectsWithTag("Boat");

        for (int i = 0; i < all_boats.Length; i++)
        {
            string player_name = all_boats[i].GetComponent<PhotonView>().Owner.NickName;
            string player_score = all_boats[i].GetComponent<Boat_PlayerScore>().score.ToString();
            texts_all_users[i].text = player_name + ": " + player_score;
        }
    }
}

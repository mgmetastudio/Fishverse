using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class MiniGameLeaderboard_UI : MonoBehaviour
{
    public Leaderboard_Entry[] leaderboard_entry;

    public void GenerateLeaderboard(List<MiniGameServer_API.MiniGameLeaderboardUser> users)
    {
        for(int i = 0; i < users.Count; i++)
        {
            leaderboard_entry[i].SetData(users[i].username, users[i].score.ToString());
        }
    }
}

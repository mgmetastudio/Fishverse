using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class PlayerScore
{
    public Player Player { get; private set; }
    public int Score { get; set; }

    public PlayerScore(Player player, int score)
    {
        Player = player;
        Score = score;
    }
}
public class ScoreManager : MonoBehaviourPunCallbacks
{
    public int wins = 0;
    public int draws = 0;
    public int losses = 0;

    void Start()
    {
        // Subscribe to events
        PhotonNetwork.AddCallbackTarget(this);
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    private void Update()
    {
      //  EndGame();
    }
    public void EndGame()
    {
        List<PlayerScore> playerScores = CalculateScores();

        // Determine the winner
        int winnerIndex = DetermineWinner(playerScores);

        int[] scoresArray = new int[playerScores.Count];
        for (int i = 0; i < playerScores.Count; i++)
        {
            scoresArray[i] = playerScores[i].Score;
           // Debug.Log("Player name is" + playerScores[i].Player.NickName + "Player Score is" + playerScores[i].Score);
        }
        

        ShowResultPanel( scoresArray, winnerIndex);
        Debug.Log("the winner is" + winnerIndex+" //   you win"+wins+" your draws"+draws+ " your loses"+losses);
    }

    List<PlayerScore> CalculateScores()
    {
        List<PlayerScore> playerScores = new List<PlayerScore>();

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            int score = GetPlayerScore(player);
            playerScores.Add(new PlayerScore(player, score));
        }

        return playerScores;
    }


    int DetermineWinner(List<PlayerScore> playerScores)
    {
        if (playerScores.Count > 1)
        {
            int maxScore = playerScores.Max(ps => ps.Score);
            int winnerIndex = playerScores.FindIndex(ps => ps.Score == maxScore);
            return winnerIndex;
        }
        else
        {
            return -1;
        }

    }
    int GetPlayerScore(Player player)
    {
        int score = 0;

        // Retrieve the player's custom properties
        if (player.CustomProperties.TryGetValue("Score", out object scoreValue))
        {
            score = (int)scoreValue;
        }

        return score;
    }
    [PunRPC]
    void ShowResultPanelAll(int[] scores, int winnerIndex)
    {
        if (PhotonNetwork.PlayerList.Length > 1)
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (i == winnerIndex)
                {
                    // Display win panel for the winner
                   // resultText.text = "Player " + (i + 1) + " wins!";
                }
                else if (scores[i] == scores[winnerIndex])
                {
                    // Display draw panel for players with the same score as the winner
                   // resultText.text = "It's a draw!";
                }
                else
                {
                    // Display lose panel for other players
                  // resultText.text = "Player " + (i + 1) + " loses!";
                }

            }
        }
    }

    [PunRPC]
    void ShowResultPanel(int[] scores, int winnerIndex)
    {
        if (PhotonNetwork.PlayerList.Length > 1)
        {
            int localPlayerIndex = Array.IndexOf(PhotonNetwork.PlayerList, PhotonNetwork.LocalPlayer);

           

            bool isDraw = true; 

            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (i == localPlayerIndex)
                    continue; // Skip comparing with the local player

                if (scores[i] != scores[localPlayerIndex])
                {
                    isDraw = false;
                    break; // If any player has a different score, it's not a draw
                }
            }

            // Display the overall result for the local player
            if (isDraw)
            {
                draws = 1; wins = 0;  losses = 0;
            }
            else if (scores[localPlayerIndex] == scores[winnerIndex])
            {
                draws = 0; wins = 1; losses = 0;
            }
            else
            {
                draws = 0; wins = 0; losses = 1;
            }
        }
        else
        {
            draws = 0; wins = 1; losses = 0;
        }
    }

}



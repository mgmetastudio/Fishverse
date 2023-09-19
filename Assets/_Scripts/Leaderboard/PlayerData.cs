using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor;

public class PlayerData
{
    public List<LeaderboardEntryDTO> Players = new List<LeaderboardEntryDTO>();

  //  [MenuItem("Custom/Generate Player Data")]
   /* static void GeneratePlayerData()
    {
        PlayerData playerData = new PlayerData();

        for (int i = 1; i <= 5; i++)
        {
            LeaderboardEntryDTO player = new LeaderboardEntryDTO
            {
                Place = 0, // Initialize place to 0
                Login = $"player{i}@example.com",
                NickName = $"Player {i}",
                LeaderBoardData = new Dictionary<string, Dictionary<string, float>>
                {
                    {
                        "BoatRace",
                        new Dictionary<string, float>
                        {
                            { "Score", Random.Range(500, 2000) },
                            { "Win", Random.Range(0, 20) }, // Example: 0 or 20
                            { "Lose", Random.Range(0, 20) }, // Example: 0 or 20
                            { "Draw", Random.Range(0, 20) }, // Example: 0 or 20
                            { "BestScore", Random.Range(1, 200) } // Initialize BestScore to 0
                        }
                    },
                    {
                        "DuelFishing",
                        new Dictionary<string, float>
                        {
                            { "Score", Random.Range(500, 2000) },
                            { "Win", Random.Range(0, 20) },
                            { "Lose", Random.Range(0, 20) },
                            { "Draw", Random.Range(0, 20) },
                            { "BestScore", Random.Range(1, 200) }
                        }
                    },
                    {
                        "SurvivalFishing",
                        new Dictionary<string, float>
                        {
                            { "Score", Random.Range(500, 2000) },
                            { "Win", Random.Range(0, 20) },
                            { "Lose", Random.Range(0, 20) },
                            { "Draw", Random.Range(0, 20) },
                            { "BestScore", Random.Range(1, 200) }
                        }
                    }
                }
            };


            playerData.Players.Add(player);
        }

        // Sort players by BoatRace score in descending order
        playerData.Players.Sort((a, b) => b.LeaderBoardData["BoatRace"]["Score"].CompareTo(a.LeaderBoardData["BoatRace"]["Score"]));

        // Assign places based on score, with the highest scorer having a place of 0
        for (int i = 0; i < playerData.Players.Count; i++)
        {
            playerData.Players[i].Place = i;
        }

        string json = JsonConvert.SerializeObject(playerData.Players, Formatting.Indented);
        string filePath = "Assets/leaderboard.json";

        System.IO.File.WriteAllText(filePath, json);
        AssetDatabase.Refresh();
    }*/
}

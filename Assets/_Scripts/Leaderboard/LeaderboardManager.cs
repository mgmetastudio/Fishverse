using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using Zenject;
using LibEngine.Leaderboard;

public class LeaderboardManager : MonoBehaviour
{
    [Inject]
    private LeaderboardController leaderboardController;

    public LeaderboardPlayerRecordDTO testCurrentPlayer;

    public List<LeaderboardEntryDTO> leaderboardData = new List<LeaderboardEntryDTO>();
    string json;
    public GameObject itemPrefab;
    public TextAsset MyItemJSONDatabase;
    [Header("Leaderboard Layouts ")]
    public Transform BoatraceLayout;
    public Transform DuelFishingLayout;
    public Transform SurvivalFishingLayout;
    void Start()
    {
        LoadLeaderboardData();
        CreateLeaderboardUI();

        testCurrentPlayer = leaderboardController.GetScheme();
    }

    private void LoadLeaderboardData()
    {
        if (MyItemJSONDatabase != null)
        {
            json = MyItemJSONDatabase.text;

            if (!string.IsNullOrEmpty(json))
            {
                leaderboardData = JsonConvert.DeserializeObject<List<LeaderboardEntryDTO>>(json);
            }
            else
            {
                Debug.LogError("JSON data is null or empty.");
            }
        }
        else
        {
            Debug.LogError("MyItemJSONDatabase is not assigned.");
        }

    }

    private void CreateLeaderboardUI()
    {
        CreateLeaderBoard_Modes("BoatRace", BoatraceLayout);
        CreateLeaderBoard_Modes("SurvivalFishing", SurvivalFishingLayout);
        CreateLeaderBoard_Modes("DuelFishing", DuelFishingLayout);
    }

    private void CreateLeaderBoard_Modes(string Mode,Transform Layout)
    {
        // Sort leaderboardData for Modes
        leaderboardData.Sort((a, b) => b.LeaderBoardData[Mode]["Score"].CompareTo(a.LeaderBoardData[Mode]["Score"]));

        // Set initial place value
        int place = 0;

        //Duel Fishing Data
        float previousScore = int.MaxValue; // Initialize previousScore with a high value
        foreach (var entry in leaderboardData)
        {
            // Check if the current player has a different score from the previous player
            if (entry.LeaderBoardData[Mode]["Score"] != previousScore)
            {
                // If the score is different, increment the place
                place++;
            }

            // Set the "place" value in the entry
            entry.Place = place;

            GameObject item = Instantiate(itemPrefab, Layout);
            LeaderboardItemUI itemUI = item.GetComponent<LeaderboardItemUI>();
            if(Mode== "BoatRace")
            {
                itemUI.SetBoatraceData(entry);
            }
            else if(Mode == "SurvivalFishing")
            {
                itemUI.SetSurvivalFishingData(entry);
            }
            else if (Mode == "DuelFishing")
            {
                itemUI.SetDuelFishingData(entry);
            }
           

            // Update the previousScore for the next player
            previousScore = entry.LeaderBoardData[Mode]["Score"];
        }

    }
  
}

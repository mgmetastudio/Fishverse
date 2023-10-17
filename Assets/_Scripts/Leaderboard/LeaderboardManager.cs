using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using Zenject;
using LibEngine.Leaderboard;
using NaughtyAttributes;

public class LeaderboardManager : MonoBehaviour
{
    [Inject]
    private ILeaderboardController leaderboardController;

    [SerializeField]
    private ResultsDataDTO testResult;

    [Button]
    public void AddResultsDataDTO()
    {
        leaderboardController.AddMatchResult(testResult);
    }

    public LeaderboardPlayerRecordDTO testCurrentPlayer;

    public List<LeaderboardPlayerRecordDTO> leaderboardData = new List<LeaderboardPlayerRecordDTO>();
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
    }

    private void LoadLeaderboardData()
    {
        leaderboardData = leaderboardController.GetCollection();
        return;

        if (MyItemJSONDatabase != null)
        {
            json = MyItemJSONDatabase.text;

            if (!string.IsNullOrEmpty(json))
            {
                leaderboardData = JsonConvert.DeserializeObject<List<LeaderboardPlayerRecordDTO>>(json);
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
        leaderboardData.Sort((a, b) => b.LeaderBoardData[Mode].Score.CompareTo(a.LeaderBoardData[Mode].Score));

        // Set initial place value
        int place = 0;

        //Duel Fishing Data
        float previousScore = int.MaxValue; // Initialize previousScore with a high value
        foreach (var entry in leaderboardData)
        {
            // Check if the current player has a different score from the previous player
            if (entry.LeaderBoardData[Mode].Score != previousScore)
            {
                // If the score is different, increment the place
                place++;
            }

            // Set the "place" value in the entry
            //entry.Place = place;

            GameObject item = Instantiate(itemPrefab, Layout);
            LeaderboardItemUI itemUI = item.GetComponent<LeaderboardItemUI>();
            itemUI.SetData(entry, Mode, place);

            // Update the previousScore for the next player
            previousScore = entry.LeaderBoardData[Mode].Score;
        }

    }
  
}

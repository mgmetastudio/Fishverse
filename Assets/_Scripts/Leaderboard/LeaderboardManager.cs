using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Zenject;
using LibEngine.Leaderboard;
using NaughtyAttributes;
using System.Threading;
using System;

public class LeaderboardManager : MonoBehaviour
{
    [Inject]
    private ILeaderboardController leaderboardController;

    [SerializeField]
    private ResultsDataDTO testResult;
    [SerializeField]
    private GameEventsIncrement gameEventsIncrement;

    private SynchronizationContext unitySyncContext;

    [Button]
    public void AddResultsDataDTO()
    {
        leaderboardController.AddMatchResult(testResult);
    }

    [Button]
    public void AddGameEventsIncrement()
    {
        leaderboardController.AddGameEventsValues(gameEventsIncrement);
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
    public Transform OpenWorldSoloLayout;

    private void Start()
    {
        unitySyncContext = SynchronizationContext.Current;
    }

    private void OnEnable()
    {
        ReloadView();
        CallReloadData();
    }

    public void CallReloadData()
    {
        leaderboardController.LoadUpdate(() => SyncPost(() => ReloadView()));
    }

    private void SyncPost(Action act)
    {
        unitySyncContext.Post(_ => act(), null);
    }

    private void ReloadView()
    {
        Debug.Log("Leaderboard View ReloadView");

        LoadLeaderboardData();
        CreateLeaderboardUI();
    }

    private void LoadLeaderboardData()
    {
        leaderboardData = leaderboardController.GetCollection();
    }

    private void CreateLeaderboardUI()
    {
        CreateLeaderBoard_Modes("BoatRace", BoatraceLayout);
        CreateLeaderBoard_Modes("SurvivalFishing", SurvivalFishingLayout);
        CreateLeaderBoard_Modes("DuelFishing", DuelFishingLayout);
        CreateLeaderBoard_Modes("OpenWorldSolo", OpenWorldSoloLayout);
    }

    private void CreateLeaderBoard_Modes(string Mode,Transform Layout)
    {
        Layout.DestroyAllChild();

        // Sort leaderboardData for Modes
        leaderboardData.Sort((a, b) => {
            if (a.LeaderBoardData.ContainsKey(Mode) && b.LeaderBoardData.ContainsKey(Mode))
            {
                return b.LeaderBoardData[Mode].Score.CompareTo(a.LeaderBoardData[Mode].Score);
            }
            else if (a.LeaderBoardData.ContainsKey(Mode))
            {
                return -1;  // b doesn't have Mode, so a should come before b
            }
            else if (b.LeaderBoardData.ContainsKey(Mode))
            {
                return 1;   // a doesn't have Mode, so b should come before a
            }
            else
            {
                return 0;   // both a and b don't have Mode, so their order doesn't matter
            }
        });

        // Set initial place value
        int place = 0;

        //Duel Fishing Data
        float previousScore = int.MaxValue; // Initialize previousScore with a high value
        foreach (var entry in leaderboardData)
        {
            if (!entry.LeaderBoardData.ContainsKey(Mode))
                continue;
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

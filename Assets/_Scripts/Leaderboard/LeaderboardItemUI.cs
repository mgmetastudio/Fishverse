using UnityEngine;
using TMPro; // Import the necessary namespace for TextMeshPro

public class LeaderboardItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text placeText;
    [SerializeField] private TMP_Text nickNameText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text winText;
    [SerializeField] private TMP_Text loseText;
    [SerializeField] private TMP_Text drawText;
    [SerializeField] private TMP_Text bestScoreText;

    public void SetBoatraceData(LeaderboardEntryDTO entry)
    {
        // Populate the UI text elements with data from the leaderboard entry
        placeText.text = entry.Place.ToString();
        nickNameText.text = entry.NickName.ToString();
        scoreText.text = entry.LeaderBoardData["BoatRace"]["Score"].ToString();
        winText.text = entry.LeaderBoardData["BoatRace"]["Win"].ToString();
        loseText.text = entry.LeaderBoardData["BoatRace"]["Lose"].ToString();
        drawText.text = entry.LeaderBoardData["BoatRace"]["Draw"].ToString();
        bestScoreText.text = entry.LeaderBoardData["BoatRace"]["BestScore"].ToString();
    }
    public void SetDuelFishingData(LeaderboardEntryDTO entry)
    {
        // Populate the UI text elements with data from the leaderboard entry
        placeText.text = entry.Place.ToString();
        nickNameText.text = entry.NickName.ToString();
        scoreText.text = entry.LeaderBoardData["DuelFishing"]["Score"].ToString();
        winText.text = entry.LeaderBoardData["DuelFishing"]["Win"].ToString();
        loseText.text = entry.LeaderBoardData["DuelFishing"]["Lose"].ToString();
        drawText.text = entry.LeaderBoardData["DuelFishing"]["Draw"].ToString();
        bestScoreText.text = entry.LeaderBoardData["DuelFishing"]["BestScore"].ToString();
    }
    public void SetSurvivalFishingData(LeaderboardEntryDTO entry)
    {
        // Populate the UI text elements with data from the leaderboard entry
        placeText.text = entry.Place.ToString();
        nickNameText.text = entry.NickName.ToString();
        scoreText.text = entry.LeaderBoardData["SurvivalFishing"]["Score"].ToString();
        winText.text = entry.LeaderBoardData["SurvivalFishing"]["Win"].ToString();
        loseText.text = entry.LeaderBoardData["SurvivalFishing"]["Lose"].ToString();
        drawText.text = entry.LeaderBoardData["SurvivalFishing"]["Draw"].ToString();
        bestScoreText.text = entry.LeaderBoardData["SurvivalFishing"]["BestScore"].ToString();
    }
}


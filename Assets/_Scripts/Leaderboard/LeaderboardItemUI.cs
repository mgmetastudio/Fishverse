using UnityEngine;
using TMPro; // Import the necessary namespace for TextMeshPro
using LibEngine.Leaderboard;

public class LeaderboardItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text placeText;
    [SerializeField] private TMP_Text nickNameText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text winText;
    [SerializeField] private TMP_Text loseText;
    [SerializeField] private TMP_Text drawText;
    [SerializeField] private TMP_Text bestScoreText;
    [SerializeField] public RectTransform FishCaughtsRectTransform;

    public void SetData(LeaderboardPlayerRecordDTO entry, string mode, int place)
    {
        placeText.text = place.ToString();
        nickNameText.text = entry.NickName.ToString();
        var gameModeData = entry.LeaderBoardData[mode];

        if(mode== "OpenWorldSolo")
        {
            scoreText.text = gameModeData.MoneyEarned.ToString();
            bestScoreText.text = gameModeData.Level.ToString();
            winText.text = gameModeData.XpCount.ToString();
            FishCaughtsRectTransform.anchoredPosition = new Vector2(-129f, FishCaughtsRectTransform.anchoredPosition.y);
            loseText.text = gameModeData.FishCatched.ToString();
            drawText.text = "";
        }
        else
        {
            scoreText.text = gameModeData.Score.ToString();
            winText.text = gameModeData.Win.ToString();
            loseText.text = gameModeData.Lose.ToString();
            drawText.text = gameModeData.Draw.ToString();
            bestScoreText.text = gameModeData.BestScore.ToString();
        }

    }
}


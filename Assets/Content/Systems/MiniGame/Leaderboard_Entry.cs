using UnityEngine;
using TMPro;

public class Leaderboard_Entry : MonoBehaviour
{
    public string rank = "1";

    public TMP_Text text_rank;
    public TMP_Text text_username;
    public TMP_Text text_score;

    private void Start()
    {
        text_rank.text = rank;
        text_username.text = "Empty";
        text_score.text = "0";
    }

    public void SetData(string username, string score)
    {
        text_rank.text = rank;
        text_username.text = username;
        text_score.text = score;
    }
}

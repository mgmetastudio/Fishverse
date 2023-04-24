using UnityEngine;
using TMPro;

public class MiniGame_UI : MonoBehaviour
{
    public TMP_Text text_score;
    public TMP_Text text_energy;
    public TMP_Text text_top_message;

    [Header("Ending Screen")] 
    public GameObject panel_end_screen;
    public TMP_Text text_ending_score;
    public TMP_Text text_best_score;
    
    private MiniGame_API minigame_api;

    private void Start()
    {
        minigame_api = GetComponent<MiniGame_API>();
    }

    public void Update()
    {
        if (minigame_api.game_started)
        {
            text_score.text = "Score: " + Mathf.Round(minigame_api.score).ToString();
            text_energy.text = "Energy: " + Mathf.Round(minigame_api.energy).ToString() + "%";
        }
    }

    public void ShowTopMessage(string message_text)
    {
        text_top_message.gameObject.SetActive(false);
        text_top_message.text = message_text;
        text_top_message.gameObject.SetActive(true);
    }

    public void ShowEndingScreen()
    {
        text_ending_score.text = "Score: " + Mathf.Round(minigame_api.score);
        text_best_score.text = "Best: " + Mathf.Round(minigame_api.best_score);
        panel_end_screen.SetActive(true);
    }
}

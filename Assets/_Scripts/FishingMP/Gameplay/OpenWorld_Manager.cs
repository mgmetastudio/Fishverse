using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
public class OpenWorld_Manager : MonoBehaviour
{
    public GameObject GameEndPanel;
    [Header("GameEndPanel Text")]
    [SerializeField] TMP_Text ScoreText;
    [SerializeField] TMP_Text BonusWinningText;
    [SerializeField] TMP_Text HighScoreText;
    [SerializeField] Text Username;
    [Header("GameEndPanel Enable/Disable")]
    public GameObject WinningPanel;
    public GameObject LosePanel;
    public Image ImgWinLose;
    public List<Sprite> Sprites;
    private float startTime;
    public string timer;
    public int Score = 0;
    public int Bonus = 10;
    [SerializeField] float HighestScore;
    private bool game_started = false;
    private bool isLocalPlayerWinner = false;
    // Start is called before the first frame update
    public int highScore = 0;
    private bool GameEnded=false;
    void Start()
    {
        startTime = Time.time;
        GameEndPanel.SetActive(false);
        WinningPanel.SetActive(false);
        LosePanel.SetActive(false);

        // Only the master client starts the game
        StartGame();
 
 


    }

    private void StartGame()
    {
        StartCoroutine(SynchronizeStartTime());
    }

    // Update is called once per frame
    void Update()
    {
        if (game_started)
        {
            UpdateTimerText();
            if (timer == "00:00" && !GameEnded)
            {
                EndGame();
            }

        }
    }
    private void UpdateTimerText()
    {
        float elapsedTime = Time.time - startTime;
        float countdownDuration = 240f; // 4 minutes in seconds

        float remainingTime = Mathf.Max(countdownDuration - elapsedTime, 0f);

        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);

        // Format the timer string with leading zeros
        string timerString = string.Format("{0:00}:{1:00}", minutes, seconds);
        timer = timerString;
    }
    public void Addscore(int coin)
    {
        Score = coin;
    }
  
    public void EndGame()
    {

        ScoreText.text = (Score + Bonus).ToString();
        BonusWinningText.text = Bonus.ToString();
        GameEndPanel.SetActive(true);
        Username.text = Fishverse_Core.instance.account_username;
        HighScoreText.text = (highScore + Bonus).ToString();

        // Determine if the local player has won or lost
        isLocalPlayerWinner = Score >= highScore || PhotonNetwork.CurrentRoom.PlayerCount == 1;

        if (isLocalPlayerWinner)
        {
            WinningPanel.SetActive(true);
            LosePanel.SetActive(false);
            ImgWinLose.sprite = Sprites[0];
        }
        else
        {
            WinningPanel.SetActive(false);
            LosePanel.SetActive(true);
            ImgWinLose.sprite = Sprites[1];
        }
        GameEnded = true;
    }
   
    IEnumerator SynchronizeStartTime()
    {
        yield return new WaitForSeconds(1.5f);
        // Notify other players that the game has started
        GameStarted();
    }
    private void GameStarted()
    {
        game_started = true; // Start the game for all players
    }



    public void UpdateHighScore(int newScore)
    {
        if (newScore > highScore)
        {
            highScore = newScore;
           // PlayerPrefs.SetInt("HighScore", highScore);
            SyncHighScore(highScore);
        }
    }


    private void SyncHighScore(int newHighScore)
    {
        Debug.Log( "High Score: " + newHighScore) ;
    }

}

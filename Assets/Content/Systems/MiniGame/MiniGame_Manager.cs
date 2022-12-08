using System.Collections;
using UnityEngine;
using TMPro;

public class MiniGame_Manager : MonoBehaviour
{
    [Header("Scene Refs:")]
    public GameObject panel_game_started;
    public GameObject camera_spectator;
    public GameObject panel_score;
    public GameObject panel_time;
    public GameObject panel_add_score;

    [Space(10)]
    [Header("Texts Refs:")]
    public TMP_Text text_score;
    public TMP_Text text_time;

    [Space(10)]
    [Header("Game Data:")]
    public int score;
    public int start_time = 45;
    public int time = 25;
    public bool start_instant = false;

    [Space(10)]
    [Header("Pickups:")]
    public GameObject[] all_pickups;
    public MiniGame_Marker ui_marker;

    private bool game_started = false;
    private float timer = 0;
    private int pickup_index = 0;

    private void Start()
    {
        ui_marker.gameObject.SetActive(false);

        score = 0;
        time = start_time;

        all_pickups.Shuffle(40);
        foreach (GameObject pickup in all_pickups)
        {
            pickup.SetActive(false);
        }

        all_pickups[pickup_index].SetActive(true);
        ui_marker.world_target = all_pickups[pickup_index].transform;

        RefreshTexts_UI();

        if (start_instant)
        {
            camera_spectator.SetActive(false);
            panel_game_started.SetActive(true);
            panel_score.SetActive(true);
            panel_time.SetActive(true);
            ui_marker.gameObject.SetActive(true);
            game_started = true;
        }
        else
        {
            StartCoroutine(StartGame());
        }
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(3f);
        camera_spectator.SetActive(false);
        
        yield return new WaitForSeconds(2f);
        panel_game_started.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        panel_score.SetActive(true);
        panel_time.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        ui_marker.gameObject.SetActive(true);
        game_started = true;
    }

    private void Update()
    {
        if (game_started)
        {
            timer += Time.deltaTime;
            if (timer > 1f)
            {
                timer = 0;
                time = Mathf.Clamp(time - 1, 0, 99);
                RefreshTexts_UI();

                if (time == 0)
                {
                    EndGame();
                }
            }
        }
    }

    public void EndGame()
    {
        game_started = false;
    }

    public void ShowNextPickup()
    {
        pickup_index++;

        if (pickup_index >= all_pickups.Length)
        {
            all_pickups.Shuffle(15);

            foreach (GameObject pickup in all_pickups)
            {
                pickup.SetActive(false);
            }
        }

        all_pickups[pickup_index].SetActive(true);
        ui_marker.world_target = all_pickups[pickup_index].transform;
    }

    public void AddScore()
    {
        panel_add_score.SetActive(false);
        panel_add_score.SetActive(true);

        score += 10;
        time += 10;

        RefreshTexts_UI();

        ShowNextPickup();
    }

    public void RefreshTexts_UI()
    {
        text_score.text = score.ToString();
        text_time.text = time.ToString();
    }
}

using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;

public class MiniGame_Manager : MonoBehaviour
{
    [Header("Scene Refs:")]
    public ArcadeVehicleController vehicle_controller;
    public ArcadeVehicleNitro vehicle_nitro;
    public GameObject panel_game_started;
    public GameObject camera_spectator;
    public GameObject panel_score;
    public GameObject panel_fishes;
    public GameObject panel_time;
    public GameObject panel_add_score;
    public GameObject joystick;
    public GameObject btn_boost;
    public GameObject btn_pause;

    [Space(10)]
    [Header("Texts Refs:")]
    public TMP_Text text_score;
    public TMP_Text text_time;
    public TMP_Text text_fishes;
    public TMP_Text text_bonus;

    [Header("Ending Screen")]
    public GameObject panel_end_screen;
    public TMP_Text text_ending_score;
    public TMP_Text text_best_score;

    [Space(10)]
    [Header("Game Data:")]
    public int score;
    public int best_score;
    public int fishes;
    public int start_time = 90;
    public int time = 90;
    public bool start_instant = false;
    public bool unlimitedTime;
    public GameObject[] all_fishes;

    [Space(10)]
    [Header("Bonuses:")]
    public GameObject[] all_bonuses;

    [Space(10)]
    [Header("Events:")]
    public UnityEvent on_catch_fish;
    public UnityEvent on_remove_fish;

    private bool game_started = false;
    private float timer = 0;
    private int fishes_score_combo = 0;

    //Text Animations
    DOTweenAnimation text_score_anim;
    DOTweenAnimation text_fish_anim;

    private void Start()
    {
#if !UNITY_EDITOR
        unlimitedTime = false;
#endif

        //Init Components
        text_score_anim = text_score.GetComponent<DOTweenAnimation>();
        text_fish_anim = text_fishes.GetComponent<DOTweenAnimation>();

        if (Fishverse_Core.instance)
            GetComponent<MiniGameServer_API>().GetBestScore();

        //Set FPS
        Application.targetFrameRate = 60;

        //Set Default Parameters
        fishes = 0;
        score = 0;
        time = start_time;

        //Shuffle Bonuses
        all_bonuses.Shuffle(60);

        foreach (GameObject bonus in all_bonuses)
        {
            bonus.SetActive(false);
        }

        for (int i = 0; i < 22; i++)
        {
            all_bonuses[i].SetActive(true);
        }

        RefreshTexts_UI();
        StartCoroutine(StartGame(start_instant));
    }

    IEnumerator StartGame(bool instant = false)
    {
        if (instant)
        {
            yield return new WaitForSeconds(0.01f);
            camera_spectator.SetActive(false);
            panel_game_started.SetActive(true);
            panel_score.SetActive(true);
            panel_time.SetActive(true);
            panel_fishes.SetActive(true);
            joystick.SetActive(true);
            btn_boost.SetActive(true);
            btn_pause.SetActive(true);
            game_started = true;
        }
        else
        {
            yield return new WaitForSeconds(3f);
            camera_spectator.SetActive(false);

            yield return new WaitForSeconds(2f);
            panel_game_started.SetActive(true);

            yield return new WaitForSeconds(0.5f);
            panel_score.SetActive(true);
            panel_time.SetActive(true);
            panel_fishes.SetActive(true);

            yield return new WaitForSeconds(0.5f);
            joystick.SetActive(true);
            btn_boost.SetActive(true);
            btn_pause.SetActive(true);

            game_started = true;
        }
    }

    private void Update()
    {
        if (game_started)
        {
            timer += Time.deltaTime;
            if (timer > 1f)
            {
                timer = 0;
                time = Mathf.Clamp(time - 1, 0, 999);
                RefreshTexts_UI();

                if (time == 0 && !unlimitedTime)
                {
                    EndGame();
                }
            }
        }
    }

    public void EndGame()
    {
        game_started = false;

        vehicle_controller.gameObject.SetActive(false);

        if (score > best_score)
        {
            best_score = score;
            GetComponent<MiniGameServer_API>().SubmitScore(best_score);
        }

        text_ending_score.text = "Score: " + score;
        text_best_score.text = "Your Best: " + best_score;
        panel_end_screen.SetActive(true);
    }

    public void AddFish()
    {
        if (fishes < 20)
        {
            fishes += 1;

            if (all_fishes[fishes - 1])
            {
                all_fishes[fishes - 1].SetActive(true);
            }

            on_catch_fish.Invoke();

            RefreshTexts_UI();

            //Play texts animations

            if (text_fish_anim)
            {
                text_fish_anim.DORestart();
            }
        }
    }

    public void AddScore(bool remove_fish = false)
    {
        //Convert fishes to score

        if (remove_fish)
        {
            if (all_fishes[Mathf.Max(fishes - 1, 0)])
            {
                all_fishes[Mathf.Max(fishes - 1, 0)].SetActive(false);
            }

            fishes--;
            fishes = Mathf.Clamp(fishes, 0, 9999);

            on_remove_fish.Invoke();
        }

        score += (10 + fishes_score_combo);

        if (fishes > 0)
        {
            fishes_score_combo++;
        }
        else
        {
            fishes_score_combo = 0;
        }

        RefreshTexts_UI();


        //Play texts animations

        if (text_score_anim)
        {
            text_score_anim.DORestart();
        }

        if (text_fish_anim && remove_fish)
        {
            text_fish_anim.DORestart();
        }
    }

    public void AddBonus(MiniGame_Bonus.BonusType bonus_type)
    {
        if (bonus_type == MiniGame_Bonus.BonusType.Time)
        {
            time += 20;
            text_bonus.text = "+20 Seconds";
        }

        else if (bonus_type == MiniGame_Bonus.BonusType.Coins)
        {
            score += 15;
            text_bonus.text = "+15 Points";
        }

        else if (bonus_type == MiniGame_Bonus.BonusType.CoinsBig)
        {
            score += 35;
            text_bonus.text = "+35 Points";
        }

        else if (bonus_type == MiniGame_Bonus.BonusType.Nitro)
        {
            vehicle_nitro.nitro = 1;
            vehicle_nitro.RefreshNitroUI();

            text_bonus.text = "Nitro Refilled";
        }

        panel_add_score.SetActive(false);
        panel_add_score.SetActive(true);

        RefreshTexts_UI();
    }

    public void RefreshTexts_UI()
    {
        text_score.text = score.ToString();
        text_time.text = time.ToString();

        if (fishes < 20)
        {
            text_fishes.color = Color.white;
            text_fishes.text = fishes.ToString() + "/20";
        }
        else
        {
            text_fishes.color = Color.red;
            text_fishes.text = fishes.ToString() + "/20 (FULL)";
        }
    }
}

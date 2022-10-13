using System.Collections;
using UnityEngine;

public class MiniGame_API : MonoBehaviour
{
    public bool game_started;
    public float score = 0;
    public float energy = 100f;
    public float energy_reduce_rate = 1f;
    public float score_add_rate = 1f;

    private MiniGame_UI minigame_ui;
    public CharacterController boat_controller;
    public float best_score;

    IEnumerator Start()
    {
        if (PlayerPrefs.HasKey(("minigame_best")))
        {
            best_score = Mathf.Round(PlayerPrefs.GetFloat("minigame_best"));
        }
        
        minigame_ui = GetComponent<MiniGame_UI>();
        yield return new WaitForSeconds(0.75f);
        StartGame();
    }

    public void StartGame()
    {
        game_started = true;
        minigame_ui.ShowTopMessage("Game Started");
    }

    public void EndGame()
    {
        boat_controller.enabled = false;
        game_started = false;

        if (score > best_score)
        {
            best_score = score;
            PlayerPrefs.SetFloat("minigame_best", best_score);
        }
        
        minigame_ui.ShowEndingScreen();
    }

    private void Update()
    {
        if (game_started)
        {
            score += score_add_rate * Time.deltaTime;
            energy -= energy_reduce_rate * Time.deltaTime;
            energy = Mathf.Clamp(energy, 0, 100);

            if (energy <= 0)
            {
                EndGame();
            }
        }
    }

    public void AddEnergy()
    {
        energy += 10;
        energy = Mathf.Clamp(energy, 0, 100);
        minigame_ui.ShowTopMessage("+10% Energy");
    }
    public void AddScore()
    {
        score += 20;
        minigame_ui.ShowTopMessage("+20 Points");
    }
}

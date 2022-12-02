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
    public Transform boat;
    public Transform[] start_points;

    IEnumerator Start()
    {
        int index = Random.Range(0, start_points.Length);
        if(start_points[index])
        {
            boat.transform.position = start_points[index].transform.position;
        }

        if(Fishverse_Core.instance)
            GetComponent<MiniGameServer_API>().GetBestScore();
        
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
            GetComponent<MiniGameServer_API>().SubmitScore(Mathf.Round(best_score));
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

    public void RandomBonus()
    {
        int bonus = Random.Range(0, 5);

        switch (bonus)
        {
            case 4:
                energy += 10;
                energy = Mathf.Clamp(energy, 0, 100);
                minigame_ui.ShowTopMessage("+5% Energy");
                break;
            case 3:
                energy += 20;
                energy = Mathf.Clamp(energy, 0, 100);
                minigame_ui.ShowTopMessage("+15% Energy");
                break;
            case 2:
                score += 20;
                minigame_ui.ShowTopMessage("+5 Points");
                break;
            case 1:
                score += 20;
                minigame_ui.ShowTopMessage("+30 Points");
                break;
            case 0:
                energy -= 10;
                energy = Mathf.Clamp(energy, 0, 100);
                minigame_ui.ShowTopMessage("-10% Energy");
                break;
        }

    }

    public void SetEnergyReduceRate(float new_rate)
    {
        energy_reduce_rate = new_rate;
    }
}

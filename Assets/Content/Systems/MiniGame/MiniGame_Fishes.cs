using UnityEngine;
using DG.Tweening;

public class MiniGame_Fishes : MonoBehaviour
{
    public MiniGame_Manager game_manager;
    public GameObject[] fish_child;
    public Transform[] fish_spawn_points;
    public GameObject btn_catch;

    private int fishes_catched = 0;
    private int max_fish_count = 5;

    private void Start()
    {
        Reset();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boat") && gameObject.activeSelf)
        {
            btn_catch.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Boat"))
        {
            btn_catch.SetActive(false);
        }
    }

    public void Reset()
    {
        int random_index = Random.Range(0, fish_spawn_points.Length);
        transform.position = fish_spawn_points[random_index].position;

        foreach (GameObject fish in fish_child)
        {
            fish.SetActive(true);
        }

        float random = Random.value;

        if (random < 0.35f)
        {
            max_fish_count = 3;
            fish_child[3].SetActive(false);
            fish_child[4].SetActive(false);
        }

        else if(random < 0.65f)
        {
            max_fish_count = 4;
            fish_child[4].SetActive(false);
        }

        else
        {
            max_fish_count = 5;
        }

        fishes_catched = 0;
    }

    public void CatchFish()
    {
        if(game_manager.fishes < 20)
        {
            fishes_catched++;

            game_manager.AddFish();

            fish_child[fishes_catched - 1].SetActive(false);

            DOTween.Restart("fish_" + fishes_catched);

            if (fishes_catched >= max_fish_count)
            {
                Reset();
            }
        }
    }
}

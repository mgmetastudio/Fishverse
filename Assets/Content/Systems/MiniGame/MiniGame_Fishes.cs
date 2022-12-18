using UnityEngine;
using DG.Tweening;

public class MiniGame_Fishes : MonoBehaviour
{
    public MiniGame_Manager game_manager;
    public GameObject[] fish_child;
    public Transform[] fish_spawn_points;
    private int fishes_catched = 0;
    public GameObject btn_catch;

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

            if (fishes_catched >= 5)
            {
                Reset();
            }
        }
    }
}

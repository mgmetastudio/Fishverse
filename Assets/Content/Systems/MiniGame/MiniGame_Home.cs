using UnityEngine;
using System.Collections;

public class MiniGame_Home : MonoBehaviour
{
    public MiniGame_Manager game_manager;
    private bool fishes_is_transfering;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boat") && gameObject.activeSelf)
        {
            if (game_manager.fishes > 0 && !fishes_is_transfering)
            {
                StartCoroutine(TransferFish_Coroutine());
            }
        }
    }

    IEnumerator TransferFish_Coroutine()
    {
        fishes_is_transfering = true;

        while (game_manager.fishes > 0)
        {
            game_manager.AddScore(true);
            yield return new WaitForSeconds(0.08f);
        }

        fishes_is_transfering = false;
    }
}

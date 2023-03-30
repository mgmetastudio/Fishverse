using UnityEngine;
using DG.Tweening;
using Photon.Pun;
// using Mirror;

public class MiniGame_Fishes : MonoBehaviourPun
{
    public MiniGame_Manager game_manager;
    public GameObject[] fish_child;
    public Transform[] fish_spawn_points;
    public GameObject btn_catch;

    [SerializeField] FishCatchVFXController fishVFX;

    // [SyncVar]
    public int fishes_catched = 0;
    private int max_fish_count = 5;

    GameObject playerBoat;

    private void Start()
    {
        // Reset();
        ResetRPC();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boat") && gameObject.activeSelf)// && other.GetComponent<NetworkIdentity>().isLocalPlayer
        {
            playerBoat = other.gameObject;
            btn_catch.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Boat"))//&& other.GetComponent<NetworkIdentity>().isLocalPlayer
        {
            btn_catch.SetActive(false);
            playerBoat = null;
        }
    }

    // [Command(requiresAuthority = false)]
    public void ResetRPC()
    {
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("Reset", RpcTarget.All);
        // Reset();
    }

    [PunRPC]
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

        else if (random < 0.65f)
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

    // [Command(requiresAuthority = false)]
    public void CatchFish()
    {
        print("CATCH");
        if (game_manager.fishes < 20)
        {
            fishes_catched++;

            game_manager.AddFish();

            fish_child[fishes_catched - 1].SetActive(false);

            // DOTween.Restart("fish_" + fishes_catched);
            fishVFX.Play(fish_child[fishes_catched - 1].transform.position, playerBoat.transform.position);

            if (fishes_catched >= max_fish_count)
            {
                // CmdReset();
                ResetRPC();
            }
        }
    }
}

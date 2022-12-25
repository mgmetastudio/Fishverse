using UnityEngine;
using Photon.Pun;

public class MP_SpawnPickups : MonoBehaviour
{
    public bool is_master;
    public MP_Pickup[] current_pickups;

    public string pickup_prefab;
    public Transform[] spawn_points;
    public int spawn_index = 0;

    private void Start()
    {
        InvokeRepeating("SpawnLoop", 0f, 2f);
    }

    public void SpawnLoop()
    {
        is_master = PhotonNetwork.IsMasterClient;

        if (is_master)
        {
            current_pickups = FindObjectsOfType<MP_Pickup>();
            int remaining_pickups = Mathf.Max(3 - current_pickups.Length, 0);
            
            if (remaining_pickups > 0)
            {
                for (int i = 0; i < remaining_pickups; i++)
                {
                    SpawnPickup();
                }
            }

        }
    }

    public void SpawnPickup()
    {
        PhotonNetwork.Instantiate(pickup_prefab, spawn_points[spawn_index].position, spawn_points[spawn_index].rotation);
        
        spawn_index++;

        if (spawn_index >= spawn_points.Length)
        {
            spawn_index = 0;
        }
    }
}

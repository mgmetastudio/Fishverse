using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class FishSpawnerTest : MonoBehaviour
{
    [SerializeField] FishSpawner spawner;

    [SerializeField] private Vector3 _position;
    [SerializeField] Vector3 bounds = Vector3.one;
    [SerializeField] public List<int> fishUniqueIds = new List<int>(0);

    [Space]
    [SerializeField] int startSpawnCount = 10;

    async void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        var fishes = GetComponentsInChildren<FishEntity>();
        foreach (var item in fishes)
        {
            item.SetBounds(bounds);
        }

        if(fishes.Length > 0) return;

        for (int i = 0; i < startSpawnCount; i++)
        {

            await UniTask.NextFrame();
            Spawn();
        }
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.P))
        // {
        //     Spawn();
        // }
    }

    void Spawn()
    {
        // if (true)//Mirror.NetworkServer.active
        
            spawner.Spawn(transform.position + _position, fishUniqueIds.GetRandom(), bounds);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + _position, 1f);
        Gizmos.DrawWireCube(transform.position + _position, bounds);
    }
#endif

}

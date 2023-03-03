using Cysharp.Threading.Tasks;
using UnityEngine;

public class FishSpawnerTest : MonoBehaviour
{
    [SerializeField] FishSpawner spawner;

    [SerializeField] private Vector3 _position;
    [SerializeField] Vector3 bounds = Vector3.one;
    [SerializeField] public int _fishUniqueId;

    [Space]
    [SerializeField] int startSpawnCount = 10;

    async void Start()
    {

        for (int i = 0; i < startSpawnCount; i++)
        {

            await UniTask.NextFrame();
            Spawn();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Spawn();
        }
    }

    void Spawn()
    {
        if (Mirror.NetworkServer.active)
        {
            spawner.Spawn(transform.position + _position, _fishUniqueId, bounds);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + _position, 1f);
        Gizmos.DrawWireCube(transform.position + _position, bounds);
    }
#endif

}

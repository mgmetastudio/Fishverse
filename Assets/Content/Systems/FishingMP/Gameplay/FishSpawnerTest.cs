using UnityEngine;

public class FishSpawnerTest : MonoBehaviour
{

    [SerializeField] private Vector3 _position;
    [SerializeField] public int _fishUniqueId;

    [Space]
    [SerializeField] int startSpawnCount = 10;

    void Start()
    {
        for (int i = 0; i < startSpawnCount; i++)
            Spawn();
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
            FishSpawner.instance.Spawn(_position, _fishUniqueId);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_position, 1f);
    }
#endif
}

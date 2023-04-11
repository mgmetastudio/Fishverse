using UnityEngine;
// using Mirror;
using Cysharp.Threading.Tasks;

public class FishSpawner : MonoBehaviour
{

    [SerializeField] private GameObject _fishEntityBasePrefab;

    public static FishSpawner instance;
    // public FishSpawner()
    // {
    //     if (instance == null)
    //     {
    //         instance = this;
    //     }
    //     else
    //     {
    //         throw new UnityException("instance != null");
    //     }
    // }

    public FishEntity Spawn(Vector3 position, int fishUniqueId)
    { // (Server)
        GameObject fishEntityObj = Instantiate(_fishEntityBasePrefab);
        fishEntityObj.transform.position = position;
        FishEntity fishEntity = fishEntityObj.GetComponent<FishEntity>();
        fishEntity.fishUniqueId = fishUniqueId;
        // NetworkServer.Spawn(fishEntityObj);

        return fishEntity;
    }

    public async void Spawn(Vector3 position, int fishUniqueId, Vector3 bounds)
    {
        var fish = Spawn(position, fishUniqueId);

		await UniTask.NextFrame();

        fish.SetBounds(bounds);
    }
}

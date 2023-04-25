using UnityEngine;
// using Mirror;
using Cysharp.Threading.Tasks;
using Photon.Pun;

public class FishSpawner : MonoBehaviour
{

    [SerializeField] GameObject _fishEntityBasePrefab;
    [SerializeField] GameObject FishCaughtMessage;


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
        // GameObject fishEntityObj = Instantiate(_fishEntityBasePrefab);
        GameObject fishEntityObj = PhotonNetwork.Instantiate(_fishEntityBasePrefab.name, position, Quaternion.identity);
        FishEntity fishEntity = fishEntityObj.GetComponent<FishEntity>();
        // fishEntity.FishCaughtMessage = FishCaughtMessage;
        fishEntity.FishUniqueId = fishUniqueId;
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

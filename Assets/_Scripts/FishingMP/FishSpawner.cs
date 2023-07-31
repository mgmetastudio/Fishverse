using UnityEngine;
// using Mirror;
using Cysharp.Threading.Tasks;
using Photon.Pun;

public class FishSpawner : MonoBehaviour
{
    [SerializeField] GameObject _fishEntityBasePrefab;
    [SerializeField] GameObject FishCaughtMessage;
    
    public static FishSpawner instance;

    public FishEntity Spawn(Vector3 position, int fishUniqueId)
    {
        GameObject fishEntityObj = PhotonNetwork.Instantiate(_fishEntityBasePrefab.name, position, Quaternion.identity);
        FishEntity fishEntity = fishEntityObj.GetComponentInChildren<FishEntity>();
        fishEntity.FishUniqueId = fishUniqueId;
        return fishEntity;
    }

    public async void Spawn(Vector3 position, int fishUniqueId, Vector3 bounds)
    {
        var fish = Spawn(position, fishUniqueId);

        await UniTask.NextFrame();

        fish.SetBounds(bounds);
    }
}

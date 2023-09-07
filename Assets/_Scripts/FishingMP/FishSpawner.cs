using UnityEngine;
// using Mirror;
using Cysharp.Threading.Tasks;
using Photon.Pun;

public class FishSpawner : MonoBehaviour
{
    [SerializeField] GameObject _fishEntityBasePrefab;
    [SerializeField] GameObject FishCaughtMessage;
    
    public static FishSpawner instance;
    public enum Location
    {
        Orange,
        Blue,
        Yellow,
        Purple
    }
    public Location currentLocation;
    public FishEntity Spawn(Vector3 position, int fishUniqueId, Location spawnLocation)
    {
        string locationName = spawnLocation.ToString();
        GameObject fishEntityObj = PhotonNetwork.Instantiate(_fishEntityBasePrefab.name , position, Quaternion.identity);
        FishEntity fishEntity = fishEntityObj.GetComponentInChildren<FishEntity>();
        fishEntity.FishUniqueId = fishUniqueId;
        fishEntity.Location = spawnLocation.ToString();
        return fishEntity;
    }

    public async void Spawn(Vector3 position, int fishUniqueId, Vector3 bounds, Location spawnLocation)
    {
        var fish = Spawn(position, fishUniqueId, spawnLocation);

        await UniTask.NextFrame();

        fish.SetBounds(bounds);
    }
}

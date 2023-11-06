using UnityEngine;
// using Mirror;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using LibEngine.Leaderboard;
using Zenject;
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
    public enum Level
    {
        None = 0,
        Level1 = 1,
        Level2 = 2,
        Level3 = 3,
        Level4 = 4,
        Level5 = 5,
        Level6 = 6,
        Level7 = 7,
        Level8 = 8,
        Level9 = 9,
        Level10 = 10,
        Level11 = 11,
        Level12 = 12,
        Level13 = 13,
        Level14 = 14,
        Level15 = 15,
        Level16 = 16,
        Level17 = 17,
        Level18 = 18,
        Level19 = 19,
        Level20 = 20,
        Level21 = 21,
        Level22 = 22,
        Level23 = 23,
        Level24 = 24,
        Level25 = 25,
        Level26 = 26,
        Level27 = 27,
        Level28 = 28,
        Level29 = 29,
        Level30 = 30
    }
    [SerializeField]
    public MatchModes matchMode;

    public Location currentLocation;
    public Level LevelRequired;
    public FishEntity Spawn(Vector3 position, int fishUniqueId, MatchModes matchMode, Location spawnLocation, Level LevelRequired)
    {
        string locationName = spawnLocation.ToString();
        GameObject fishEntityObj = PhotonNetwork.Instantiate(_fishEntityBasePrefab.name , position, Quaternion.identity);
        FishEntity fishEntity = fishEntityObj.GetComponentInChildren<FishEntity>();
        fishEntity.FishUniqueId = fishUniqueId;
        fishEntity.Location = spawnLocation.ToString();
        if((int)LevelRequired != 0)
        {
            fishEntity.Level = ((int)LevelRequired).ToString();
        }
        else
        {
            fishEntity.Level = LevelRequired.ToString();
        }
        fishEntity.MatchMode = matchMode.ToString();
        return fishEntity;
    }
    public async void Spawn(Vector3 position, int fishUniqueId, Vector3 bounds, MatchModes matchMode, Location spawnLocation, Level LevelRequired)
    {
        var fish = Spawn(position, fishUniqueId, matchMode, spawnLocation, LevelRequired);

        await UniTask.NextFrame();

        fish.SetBounds(bounds);
    }
}

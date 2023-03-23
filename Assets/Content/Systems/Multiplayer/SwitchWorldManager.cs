using System.Collections;
using System.Collections.Generic;
using Mirror;
using Trisibo;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class WorldInfo
{
    public string scanePath;
    public int minPlayers;
    public GameObject playerPrefab;
}
public class SwitchWorldManager : MonoBehaviour
{
    [SerializeField] NetworkRoomManager manager;

    [SerializeField] List<WorldInfo> worlds;

    public void SwitchScene(int index)
    {
        var world = worlds[index];

        manager.GameplayScene = world.scanePath;
        manager.playerPrefab = world.playerPrefab;
        manager.minPlayers = world.minPlayers;
    }
}

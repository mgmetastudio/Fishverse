using System.Collections;
using System.Collections.Generic;
using Mirror;
using Trisibo;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchWorldManager : MonoBehaviour
{
    [SerializeField] NetworkRoomManager manager;

    [SerializeField] List<string> scenes;

    public void SwitchScene(int index) => SwitchScene(scenes[index]);

    public void SwitchScene(string scenePath)
    {
        manager.GameplayScene = scenePath;
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using NullSave.GDTK.Stats;
using System.IO;

public class StatsDatabaseController : MonoBehaviour
{
    [Header("PlayerCharacterStats")]
    public PlayerCharacterStats PlayerCharacterStats;

    [Header("File DataBase")]
    [SerializeField]
    public string fileName;

    private void Start()
    {
        PlayerCharacterStats.DataLoad(fileName);
    }
  
  
}

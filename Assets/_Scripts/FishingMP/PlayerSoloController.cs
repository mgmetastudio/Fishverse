using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NullSave.TOCK.Inventory;
using NullSave.GDTK.Stats;
using System.IO;

public class PlayerSoloController : MonoBehaviour
{
    private string playerName;
    [Header("Player Name Text")]
    public TMPro.TMP_Text PlayerName;

    [Header("PlayerCharacterStats")]
    public PlayerFishing PlayerFishing;

    [Header("PlayerCharacterStats")]
     public PlayerCharacterStats PlayerCharacterStats;

    [Header("File DataBase")]
    [SerializeField]
    public string fileName;
    void Start()
    { 
        playerName = Fishverse_Core.instance.account_username;
        PlayerName.text = playerName;
        PlayerCharacterStats.DataLoad(fileName);
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerFishing.FishingFloat != null)
        {
            if (PlayerFishing.FishingFloat.fish != null)
            {
                if (PlayerFishing.FishingFloat.fish.controller.iscatched)
                {
                   PlayerCharacterStats.DataSave(fileName);
                }
            }
        }
    }
}

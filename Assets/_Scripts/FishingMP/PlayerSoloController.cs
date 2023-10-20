using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NullSave.TOCK.Inventory;
using NullSave.GDTK.Stats;
using System.IO;
using NullSave.TOCK.Stats;

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

    [Header("InventoryCog")]
    public InventoryCog InventoryCog;

    public int MoneyEarned;
    public int FishCatched;
    public int Xp;

    private int previousFishCurrency;
    private int previousTotalFishCatched;
    private int previousXp;
    void Start()
    { 
        playerName = Fishverse_Core.instance.account_username;
        PlayerName.text = playerName;

        if (PlayerCharacterStats != null)
        {
            //Load Stats
            PlayerCharacterStats.DataLoad(fileName);
            previousXp = (int)PlayerCharacterStats.stats["experience"].value;
        }

        if (InventoryCog != null)
        {
            //Load Items
            InventoryCog.InventoryStateLoad("InventoryDB.sav");
            previousFishCurrency = (int)InventoryCog.Fishcurrency;
            previousTotalFishCatched = InventoryCog.GetItems("Fishes").Count;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerCharacterStats != null)
        {
            if (PlayerCharacterStats.stats.ContainsKey("experience"))
            {
                int Xp_ = (int)PlayerCharacterStats.stats["experience"].value;

                if (Xp_ != previousXp)
                {
                    // Xp count
                    AddXpCount(Xp_ - previousXp);
                    previousXp = Xp_;
                }
            }
        }
        if (InventoryCog != null)
        {
            // Get the current amount of in-game currency (FishCurrency)
            int FishCurrency = (int)InventoryCog.Fishcurrency;

            // Check if the money earned has changed
            if (FishCurrency != previousFishCurrency)
            {
                // Save the game state
                InventoryCog.InventoryStateSave("InventoryDB.sav");

                // MoneyEarned count
                if (FishCurrency > previousFishCurrency)
                {
                    AddMoneyEarnedCount(FishCurrency - previousFishCurrency);
                }

                previousFishCurrency = FishCurrency;
            }

            // Get the current amount of Fish Catched (TotalFishCatched)
            int TotalFishCatched = InventoryCog.GetItems("Fishes").Count;

            // Check if the total fish catched has changed
            if (TotalFishCatched != previousTotalFishCatched)
            {
                // Save the game state
                InventoryCog.InventoryStateSave("InventoryDB.sav");

                // FishCatched count
                if (TotalFishCatched > previousTotalFishCatched)
                {
                    AddFishCatchCount(TotalFishCatched - previousTotalFishCatched);
                }

                previousTotalFishCatched = TotalFishCatched;
            }
        }
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
    void AddFishCatchCount(int incrementValue)
    {
        FishCatched += incrementValue;
        
    }
    void AddMoneyEarnedCount(int incrementValue)
    {
        MoneyEarned += incrementValue;
    }

    void AddXpCount(int incrementValue)
    {
        Xp += incrementValue;
    }
}

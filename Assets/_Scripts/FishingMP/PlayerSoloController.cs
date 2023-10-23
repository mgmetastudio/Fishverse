using UnityEngine;
using NullSave.TOCK.Inventory;
using NullSave.GDTK.Stats;
using LibEngine.Leaderboard;
using System.Collections.Generic;
using Zenject;

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
    [Header("Canvas Control")]
    public List<GameObject> CanvasControl;

    [Inject]
    private ILeaderboardController leaderboardService;
    public ControlSwitch ControlSwitch;
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
        ControlSwitch = FindObjectOfType<ControlSwitch>();
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

        SetLevel();
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
                    var incrementValue = FishCurrency - previousFishCurrency;
                    AddMoneyEarnedCount(incrementValue);
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
                    var incrementValue = TotalFishCatched - previousTotalFishCatched;
                    AddFishCatchCount(incrementValue);
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
        if (ControlSwitch != null)
        {
            if (ControlSwitch.controllerToToggle.enabled)
            {
                foreach (GameObject obj in CanvasControl)
                {
                    obj.SetActive(false);
                }
            }
            else
            {
                foreach (GameObject obj in CanvasControl)
                {
                    obj.SetActive(true);
                }
            }
        }
    }
    void AddFishCatchCount(int incrementValue)
    {
        IncrementGameValues(new GameEventsIncrement(fishCatched: incrementValue));
        FishCatched += incrementValue;
        
    }
    void AddMoneyEarnedCount(int incrementValue)
    {
        IncrementGameValues(new GameEventsIncrement(moneyEarned: incrementValue));
        MoneyEarned += incrementValue;
    }

    void AddXpCount(int incrementValue)
    {
        IncrementGameValues(new GameEventsIncrement(xpCount: incrementValue));
        Xp += incrementValue;
        SetLevel();
    }

    private void IncrementGameValues(GameEventsIncrement gameEventsValuesIncrement)
    {
        leaderboardService.AddGameEventsValues(gameEventsValuesIncrement);
    }

    private void SetLevel()
    {
        var level = PlayerCharacterStats.GetCharacterLevel();
        leaderboardService.AddGameEventsValues(new GameEventsIncrement(setLevel: level));
    }
}

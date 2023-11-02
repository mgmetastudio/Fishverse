using UnityEngine;
using NullSave.TOCK.Inventory;
using NullSave.GDTK.Stats;
using LibEngine.Leaderboard;
using System.Collections.Generic;
using Zenject;
using TMPro;
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

    [Header("InventoryCog")]
    public InventoryCog InventoryCog;
    [Header("Canvas Control")]
    public List<GameObject> CanvasControl;
    [Header("Panel For Required Level")]
    public GameObject PanelrequiredLevel;
    public TMP_Text LevelRequirementText;

    string relativePath = "InventoryDB.sav";
    string fullPath;
    public int PlayerLevel;
    public int FishLevel ;
    private int LastFishLevel;
    private bool IsFishingSpot;
    private float lastUpdateTime;
    private float timeThreshold = 3f; // Set the time threshold in seconds
    public bool IsLevelValid { get; set; }
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

#if UNITY_ANDROID && !UNITY_EDITOR
        fullPath = Path.Combine(Application.persistentDataPath, relativePath);
#elif UNITY_IOS && !UNITY_EDITOR
        fullPath = Path.Combine(Application.persistentDataPath, relativePath);
#else
        fullPath = Path.Combine(Application.dataPath, relativePath);
#endif
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
            InventoryCog.InventoryStateLoad(fullPath);
            previousFishCurrency = (int)InventoryCog.Fishcurrency;
            previousTotalFishCatched = InventoryCog.GetItems("Fishes").Count;
        }

        SetLevel();
        LastFishLevel = 0;
        lastUpdateTime = Time.time;
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
                InventoryCog.InventoryStateSave(fullPath);

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
                InventoryCog.InventoryStateSave(fullPath);

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
            PlayerLevel = PlayerCharacterStats.GetCharacterLevel();

            // Check if Fishing float outside of Fishing Spot
            int currentFishLevel = FishLevel;

            if (currentFishLevel != LastFishLevel)
            {
                LastFishLevel = currentFishLevel;
                lastUpdateTime = Time.time; // Update the last update time
                //Debug.Log("Fish level changed. New level: " + LastFishLevel);
                IsFishingSpot = false;
            }
            else
            {
                IsFishingSpot = true;
            }

            float elapsedTime = Time.time - lastUpdateTime;

            if (elapsedTime >= timeThreshold)
            {
                PanelrequiredLevel.SetInactive();
                //Debug.Log("Panel set inactive. Fish level: " + LastFishLevel);
            }

            // Check if Fishing float Inside Fishing Spot
            if (IsLevelValid || FishLevel == 0)
            {
                //Debug.Log("The level is valid");
                PanelrequiredLevel.SetInactive();
            }
            else if(FishLevel != 0 && !IsLevelValid && !IsFishingSpot)
            {
                LevelRequirementText.text = "YOU CAN'T FISH IN THIS FISHING SPOT. <br> LEVEL " + FishLevel + " REQUIRED.";
                PanelrequiredLevel.SetActive();
                //Debug.Log("The level is not valid");
            }

            if (PlayerFishing.FishingFloat.fish != null)
            {

                if (PlayerFishing.FishingFloat.fish.controller.iscatched)
                {
                    PlayerCharacterStats.DataSave(fileName);
                }
            }
        }
        else
        {
            FishLevel = 0;
            LastFishLevel = 0;
            PanelrequiredLevel.SetInactive();
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
    public void SetLevelValid(bool _IsLevelValid, int _FishLevel)
    {
        IsLevelValid = _IsLevelValid;
        FishLevel = _FishLevel;
    }
}

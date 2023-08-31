using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using Mirror;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using System.Linq;
using UnityEngine.Events;
using XpModule;
using NullSave.TOCK.Inventory;
using NullSave.TOCK.Stats;

public class PlayerFishingInventory : MonoBehaviourPun
{
    // public Opsive.UltimateInventorySystem.Core.InventoryCollections.Inventory playerInventory;
    // public Opsive.UltimateInventorySystem.UI.Panels.DisplayPanelManager inventoryUI;
    // public Currency currency;

    [Space]
    [SerializeField] InventoryCog inventoryCog;
    [SerializeField] StatsCog statsCog;


    [Space]
    [SerializeField] InventoryItem fishBaseItem;
    [SerializeField] Category fishCategory;
    [SerializeField] StatEffect fishWeightEffect;
    // public  XpTracker xpLevel;
    [Header("GamePlay UI")]
    public GameObject GamePlayUI;
    [Header("Inventory")]
    public GameObject InventoryCanvas;
    public Transform Content;
    [Header("Items")]
    public GameObject InventoryFishPrefab;
    [Header("Messages")]
    public GameObject NoItemsInInventoryMessage;
    [Header("Player GameObjects")]
    public List<GameObject> Fishes;
    public GameObject FishHolder;
    [Header("Animation")]
    public string FishHolderAnimationName = "Fish_Caught";
    [Header("Fishing Line")]
    [SerializeField] public Transform _rodEndPoint;
    [SerializeField] private LineRenderer _rodLineRenderer;
    public GameObject LineStart;
    public GameObject LineEnd;
    public GameObject LineEndPrefab;
    GameObject SpawnedLineEndPrefab;
    // [SyncVar]
    private bool floatHasChanged = false;
    [Header("Camera")]
    public Camera Camera;
    [Header("Equipment")]

    [SerializeField] private EquipPoint _equipPoint;
    [SerializeField] private Animator _animatorFishingRodAnim;
    [SerializeField] private Animator _animatorCharacter;

    // [SyncVar]
    private int currentSelectedFloat = 0;
    // [SyncVar]
    private int lastSelectedFloat = 0;
    public Float[] Floats;
    public Transform FloatContent;
    public GameObject FloatSelectionMenu;
    public Image CurrentSelectedFloatImage;
    //Fishing Rod
    // [SyncVar]
    private int currentSelectedFishingRod = 0;
    public FishingRod[] FishingRods;
    public Transform FishingRodContent;
    public GameObject FishingRodSelectionMenu;
    public Image CurrentSelectedFishingRodImage;
    //Bait
    // [SyncVar]
    private int currentSelectedBait ;
    public Bait[] Baits;
    public Transform BaitContent;
    public GameObject BaitSelectionMenu;
    public Image CurrentSelectedBaitImage;
    [Header("Player Name")]
    public GameObject Manager;
    public string playerName;
    public Text PlayerNameText;

    public bool inInventory;

    public TMPro.TMP_Text moneyText;

    public GameObject soldUI;
    public TMPro.TMP_Text soldText;

    public List<InventoryFish> fishInv;

    public UnityEvent<bool> onInventory;
    public InventoryItem currentBait;
    public InventoryItem currentRod;
    public InventoryItem currentFloat;
    public bool IsopenMenu;
    public int Score =0;
    public int highestScore = 0;
    private OpenWorld_Manager openWorldManager;


    int _money;
    public int Money
    {
        get => _money;
        set
        {
            _money = value;
            moneyText.SetText(_money + "$");
        }
    }


    [Space]
    [SerializeField] float fishHoldTime = 2f;

    int _lastUniqueId = -1;

    public bool FloatHasChanged { get => floatHasChanged; set { if (photonView.IsMine) photonView.RPC("SetFloatHasChanged", RpcTarget.All, value); } }
    public int CurrentSelectedFloat { get => currentSelectedFloat; set { if (photonView.IsMine) photonView.RPC("SetCurrentSelectedFloat", RpcTarget.All, value); } }
    public int LastSelectedFloat { get => lastSelectedFloat; set { if (photonView.IsMine) photonView.RPC("SetLastSelectedFloat", RpcTarget.All, value); } }
    public int CurrentSelectedFishingRod { get => currentSelectedFishingRod; set { if (photonView.IsMine) photonView.RPC("SetCurrentSelectedFishingRod", RpcTarget.All, value); } }
    public int CurrentSelectedBait { get => currentSelectedBait; set { if (photonView.IsMine) photonView.RPC("SetCurrentSelectedBait", RpcTarget.All, value); } }

    [PunRPC]
    void SetFloatHasChanged(bool value) => floatHasChanged = value;
    [PunRPC]
    void SetCurrentSelectedFloat(int value) => currentSelectedFloat = value;
    [PunRPC]
    void SetLastSelectedFloat(int value) => lastSelectedFloat = value;
    [PunRPC]
    void SetCurrentSelectedFishingRod(int value) => currentSelectedFishingRod = value;
    [PunRPC]
    void SetCurrentSelectedBait(int value) => currentSelectedFishingRod = value;

    private void Start()
    {
        if (!photonView.IsMine)
        {
            InventoryCanvas.SetInactive();
            GamePlayUI.SetInactive();
            return;
        }
        if (photonView.IsMine)
        {
            playerName = Fishverse_Core.instance.account_username;
            // Delay the RPC to give time for synchronization
            Invoke("SendPlayerNameRPC", 0.5f); 
        }
        Manager = GameObject.FindGameObjectWithTag("Manager");
        openWorldManager = FindObjectOfType<OpenWorld_Manager>();
        Camera = Camera.main;

        // SetUpFloat();

        CheckForItems();

        SpawnEquipment();
        _equipPoint = GetComponentInChildren<EquipPoint>();

    }

    public void OnItemEquip(InventoryItem equipedItem)
    {
        if (equipedItem.displayName == "Fishing Rod")
        {
            currentRod = equipedItem;

            SetUpFloat();
        }
        if (equipedItem.subtext == "Fishing Bait")
        {
            currentBait = equipedItem;
            SetUpFloatBait();
        }

        if (equipedItem.displayName == "Float")
        {
            currentFloat = equipedItem;
            if (currentRod != null)
            {
                SetUpFloat();
            }
        }

    }

    public void OnItemUnequip(InventoryItem unequipedItem)
    {

        if (unequipedItem.displayName == "Fishing Rod")
        {
            currentRod = null;

        }
        if (unequipedItem.subtext == "Fishing Bait")
        {
            currentBait = null;
        }
        if (unequipedItem.displayName == "Float")
        {
            currentFloat = null;
        }
    }

    public void SetPlayerName(string PlayerN)
    {
        PlayerN = playerName;
        PlayerNameText.text = PlayerN;

        photonView.RPC("CmdSetPlayerName", RpcTarget.All, PlayerN);
        // CmdSetPlayerName(PlayerN);
    }

    // [Command]
    [PunRPC]
    public void CmdSetPlayerName(string PlayerN)
    {
        PlayerN = playerName;
        PlayerNameText.text = PlayerN;

        photonView.RPC("RpcSetPlayerName", RpcTarget.All, PlayerN);
        // RpcSetPlayerName(PlayerN);
    }

    // [ClientRpc]


    private void SendPlayerNameRPC()
    {
        photonView.RPC("RpcSetPlayerName", RpcTarget.AllBuffered, playerName);
    }

    [PunRPC]
    private void RpcSetPlayerName(string name)
    {
        playerName = name;
        UpdatePlayerNameDisplay();
    }

    private void UpdatePlayerNameDisplay()
    {
        PlayerNameText.text = playerName;
    }

    public void SetUpFloat()
    {
        for (int i = 0; i < Floats.Length; i++)
        {
            if (Floats[i].ID == currentFloat.previewScale)
            {
                SpawnedLineEndPrefab = Instantiate(Floats[i].LineEndPrefab, transform.position + (transform.forward * 2), Floats[i].LineEndPrefab.transform.rotation);
            }
        }

        if (LineEnd != null)
            Destroy(LineEnd);

        LineEnd = SpawnedLineEndPrefab;

        LineStart.GetComponent<SpringJoint>().connectedBody = SpawnedLineEndPrefab.GetComponent<Rigidbody>();

        for (int i = 0; i < SpawnedLineEndPrefab.GetComponent<BaitActivator>().Baits.Length; i++)
        {
            SpawnedLineEndPrefab.GetComponent<BaitActivator>().Baits[i].Bait.SetActive(false);

            if (SpawnedLineEndPrefab.GetComponent<BaitActivator>().Baits[i].ID == CurrentSelectedBait)
            {
                SpawnedLineEndPrefab.GetComponent<BaitActivator>().Baits[i].Bait.SetActive(true);
            }
        }
    }
    public void SetUpFloatBait()
    {

        if (LineEnd != null)
        { 

            for (int i = 0; i < SpawnedLineEndPrefab.GetComponent<BaitActivator>().Baits.Length; i++)
            {
                LineEnd.GetComponent<BaitActivator>().Baits[i].Bait.SetActive(false);
                if (LineEnd.GetComponent<BaitActivator>().Baits[i].ID == currentBait.previewScale)
                {
                    LineEnd.GetComponent<BaitActivator>().Baits[i].Bait.SetActive(true);
                }
             
            }
        }
 
    }

    public void SetFloat(int ID)
    {
        CurrentSelectedFloat = ID;

        photonView.RPC("CmdSetFloat", RpcTarget.All, ID);
        // CmdSetFloat(ID);
    }

    // [Command]
    [PunRPC]
    public void CmdSetFloat(int ID)
    {
        CurrentSelectedFloat = ID;

        SetUpFloat();

        photonView.RPC("RpcSetFloat", RpcTarget.All, ID);
        // RpcSetFloat(ID);
    }

    // [ClientRpc]
    [PunRPC]
    public void RpcSetFloat(int ID)
    {
        CurrentSelectedFloat = ID;

        SetUpFloat();
    }

    public void SetFishingRod(int ID)
    {
        CurrentSelectedFishingRod = ID;
        photonView.RPC("CmdSetFishingRod", RpcTarget.All, ID);
        // CmdSetFishingRod(ID);

        SetUpFloat();
    }

    // [Command]
    [PunRPC]
    public void CmdSetFishingRod(int ID)
    {
        CurrentSelectedFishingRod = ID;

        for (int i = 0; i < FishingRods.Length; i++)
        {
            FishingRods[i].FishingRodGameObject.SetActive(false);

            if (FishingRods[i].ID == CurrentSelectedFishingRod)
            {
                FishingRods[i].FishingRodGameObject.SetActive(true);

                photonView.RPC("RpcSetFishingRod", RpcTarget.All, ID);
                // RpcSetFishingRod(ID);
            }
        }
    }

    // [ClientRpc]
    [PunRPC]
    public void RpcSetFishingRod(int ID)
    {
        CurrentSelectedFishingRod = ID;

        for (int i = 0; i < FishingRods.Length; i++)
        {
            FishingRods[i].FishingRodGameObject.SetActive(false);

            if (FishingRods[i].ID == CurrentSelectedFishingRod)
            {
                FishingRods[i].FishingRodGameObject.SetActive(true);
            }
        }
    }

    public void SetBait(int ID)
    {
        CurrentSelectedBait = ID;

        photonView.RPC("CmdSetBait", RpcTarget.All, ID);
        // CmdSetBait(ID);
    }

    // [Command]
    [PunRPC]
    public void CmdSetBait(int ID)
    {
        CurrentSelectedBait = ID;

        SetUpFloat();

        photonView.RPC("RpcSetBait", RpcTarget.All, ID);
        // RpcSetBait(ID);
    }

    // [ClientRpc]
    [PunRPC]
    public void RpcSetBait(int ID)
    {
        CurrentSelectedBait = ID;

        SetUpFloat();
    }

    public void SpawnEquipment()
    {
        for (int i = 0; i < Floats.Length; i++)
        {
            Floats[i].Spawn();
        }

        for (int i = 0; i < FishingRods.Length; i++)
        {
            FishingRods[i].Spawn();
        }

        for (int i = 0; i < Baits.Length; i++)
        {
            Baits[i].Spawn();
        }
    }

    public void SpawnFloatUI(GameObject prefab, Transform parent, int ID, string Name, Sprite Image)
    {
        GameObject SpawnedPrefab;

        SpawnedPrefab = Instantiate(prefab);
        SpawnedPrefab.transform.SetParent(parent);
        var equipmentItem = SpawnedPrefab.GetComponent<EquipmentItem>();
        equipmentItem.InventorySystem = this;
        equipmentItem.EquipmentType = "Float";
        equipmentItem.EquipmentID = ID;
        equipmentItem.EquipmentName.text = Name;
        equipmentItem.EquipmentImage = Image;
    }

    public void SpawnFishingRodUI(GameObject prefab, Transform parent, int ID, string Name, Sprite Image)
    {
        GameObject SpawnedPrefab;

        SpawnedPrefab = Instantiate(prefab);
        SpawnedPrefab.transform.SetParent(parent);

        var eqItem = SpawnedPrefab.GetComponent<EquipmentItem>();
        eqItem.InventorySystem = this;
        eqItem.EquipmentType = "Fishing Rod";
        eqItem.EquipmentID = ID;
        eqItem.EquipmentName.text = Name;
        eqItem.EquipmentImage = Image;
    }

    public void SpawnBaitUI(GameObject prefab, Transform parent, int ID, string Name, Sprite Image)
    {
        GameObject SpawnedPrefab;

        SpawnedPrefab = Instantiate(prefab);
        SpawnedPrefab.transform.SetParent(parent);

        var eqItem = SpawnedPrefab.GetComponent<EquipmentItem>();
        eqItem.InventorySystem = this;
        eqItem.EquipmentType = "Bait";
        eqItem.EquipmentID = ID;
        eqItem.EquipmentName.text = Name;
        eqItem.EquipmentImage = Image;
    }

    private void Update()
    {
        IsopenMenu = inventoryCog.IsMenuOpen;

        if (FloatHasChanged == true & LastSelectedFloat != CurrentSelectedFloat)
        {
            SetUpFloat();
            FloatHasChanged = false;
        }

        if (LineEnd)
        {

            if (this.GetComponent<PlayerFishing>().FishingFloat == null)
            {
                _rodLineRenderer.SetPosition(0, _rodEndPoint.position);
                _rodLineRenderer.SetPosition(1, LineEnd.transform.position);
                LineEnd.SetActive(true);
                LineStart.SetActive(true);
            }
            else
            {
                LineStart.SetActive(false);
                LineEnd.SetActive(false);
            }
        }

        if (!photonView.IsMine)
            return;

    }

    public async void SellAllFish(int amount)
    {
        if (amount == 0) return;

        Money += amount;

        ClearInventory();
        soldText.SetText(amount + "$");

        soldUI.SetActive();
        await UniTask.WaitForSeconds(2f);
        soldUI.SetInactive();
    }
    private void FindFishingRodAnimator()
    {
        // Find the child Animator component dynamically
        _animatorFishingRodAnim = _equipPoint.GetComponentInChildren<Animator>();
    }

    public void ShowInventory()
    {
        inInventory = true;
        // this.GetComponent<TestPlayerController>().canRotateCamera = false;
        // InventoryCanvas.GetComponent<Animator>().ResetTrigger("FadeOut");
        // InventoryCanvas.GetComponent<Animator>().SetTrigger("FadeIn");
        //  Cursor.visible = true;
        // Cursor.lockState = CursorLockMode.None;
        onInventory.Invoke(inInventory);

        if (_equipPoint != null)
        {
            FindFishingRodAnimator();
            if (_animatorFishingRodAnim != null)
            {
                _animatorFishingRodAnim.SetFloat("FishingRod_Up_Speed", 0);
                _animatorFishingRodAnim.Play("IdleState");
                _animatorCharacter.SetFloat("Fishing_Up_Speed", 0);
                _animatorCharacter.Play("Fishing_RightArm_Idle");
            }
        }
    }

    public void HideInventory()
    {
        inInventory = false;
        // this.GetComponent<TestPlayerController>().canRotateCamera = true;
        // InventoryCanvas.GetComponent<Animator>().ResetTrigger("FadeIn");
        // InventoryCanvas.GetComponent<Animator>().SetTrigger("FadeOut");
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;
        onInventory.Invoke(inInventory);

    }

    public void ClearInventory()
    {
        fishInv.Clear();
        Content.DestroyAllChild();
    }

    public void AddFishItem(FishScriptable fishInfo)
    {
        /*if (!isLocalPlayer)
            return;*/
        // InventorySystemManager inventoryManager = InventorySystemManager.Instance;
        // ItemDefinition itemDefinition = inventoryManager.Database.ItemDefinitions.First(x => x.name == FishName);

        // itemDefinition.GetAttribute<Attribute<float>>("Length").SetOverrideValue(FishLength);
        // itemDefinition.GetAttribute<Attribute<float>>("Weight").SetOverrideValue(FishWeight);
        // int waterTypeID = itemDefinition.GetAttribute<Attribute<int>>("WaterType").GetValue();
        // int rarity = waterTypeID.GetRandom();
        // itemDefinition.GetAttribute<Attribute<int>>("Rarity").SetOverrideValue(rarity);

        // var curr = new CurrencyAmounts(new CurrencyAmount[1] { new CurrencyAmount(currency, FishRetailValue) });
        // itemDefinition.GetAttribute<Attribute<CurrencyAmounts>>("PriceValue").SetOverrideValue(curr);

        // playerInventory.AddItem(itemDefinition, 1);

        // ItemDefinition baitDefinition = inventoryManager.Database.ItemDefinitions.First(x => x.Category.name == "Bait");
        // playerInventory.RemoveItem(baitDefinition)

        // xpLevel.Grant("Fishing", rarity + 1);
        // int maxRarity = int.Parse(currentRod.GetCustomTag("WaterType"));

        float fishSizeMulti = 1f.GetRandom();

        float fishLength = (float)System.Math.Round(fishInfo.FishLength.Lerp(fishSizeMulti), 1, System.MidpointRounding.AwayFromZero);
        float fishWeightValue = (float)System.Math.Round(fishInfo.FishWeight.Lerp(fishSizeMulti), 1, System.MidpointRounding.AwayFromZero);
        int fishValue = (int)(fishInfo.FishRetailValue * (1 + fishSizeMulti));


        int maxRarity = currentRod.rarity + 1;

        var fishItem = Instantiate(fishBaseItem);
        fishItem.category = fishCategory;

        fishItem.icon = fishInfo.FishSprite;
        fishItem.value = fishValue;
        fishItem.weight = fishLength;
        fishItem.displayName = fishInfo.FishName;
        fishItem.rarity = Random.Range(0, maxRarity);

        var fishWeight = Instantiate(fishWeightEffect);
        fishWeight.displayName = fishWeightValue + "cm";
        fishWeight.description += fishWeight.displayName;
        fishItem.statEffects.Add(fishWeight);

        inventoryCog.AddToInventory(fishItem);
        inventoryCog.RemoveItem(currentBait, 1);

        var xpStat = statsCog.Stats.First(x => x.displayName == "XP");
        xpStat.SetValue(xpStat.CurrentValue + fishValue);

        GameObject SpawnedInventoryFish;

        _animatorCharacter.SetTrigger(FishHolderAnimationName);
        if (_equipPoint != null)
        {
            FindFishingRodAnimator();
            if (_animatorFishingRodAnim != null)
            {
                _animatorFishingRodAnim.SetFloat("FishingRod_Up_Speed", 0);
                _animatorFishingRodAnim.Play("IdleState");
            }
        }
            HoldCaughtFish(fishInfo.uniqueId);
        SpawnedInventoryFish = Instantiate(InventoryFishPrefab);
        var invFish = SpawnedInventoryFish.GetComponent<InventoryFish>();

        SpawnedInventoryFish.transform.SetParent(Content);
        // invFish.FishName.text = FishName;
        // invFish.FishLength.text = $"Length: {FishLength}cm";
        // invFish.FishWeight.text = $"Weight: {FishWeight} kg";
        // invFish.FishRetailValue.text = $"Value: {FishRetailValue}$";
        // invFish.FishImage.sprite = FishSprite;

        fishInv.Add(invFish);
        if (openWorldManager != null)
        {
            openWorldManager.Addscore(35); // Update score in OpenWorld_Manager
            int totalScore = openWorldManager.Score;
            photonView.RPC("UpdateHighScoreOnServer", RpcTarget.All, totalScore);

        }

        photonView.RPC("CmdHoldCaughtFish", RpcTarget.All, fishInfo.uniqueId);
        // CmdHoldCaughtFish(uniqueId);


        SpawnedInventoryFish = null;

        CheckForItems();
    }
    [PunRPC]
    private void UpdateHighScoreOnServer(int newScore)
    {
        OpenWorld_Manager openWorldManager = FindObjectOfType<OpenWorld_Manager>();
        openWorldManager.UpdateHighScore(newScore);
    }
    public async void HoldCaughtFish(int uniqueId)
    {
        _lastUniqueId = uniqueId;

        FishHolder.SetActive(true);
        Fishes[uniqueId].gameObject.SetActive(true);

        await UniTask.WaitForSeconds(fishHoldTime);

        HideFish();
    }

    public void HideFish()
    {
        FishHolder.SetActive(false);
        if (_lastUniqueId != -1)
            Fishes[_lastUniqueId].gameObject.SetActive(false);
    }

    // [Command]
    [PunRPC]
    public void CmdHoldCaughtFish(int uniqueId)
    {
        HoldCaughtFish(uniqueId);

        photonView.RPC("RpcHoldCaughtFish", RpcTarget.All, uniqueId);
        // RpcHoldCaughtFish(uniqueId);

    }

    // [ClientRpc]
    [PunRPC]
    public void RpcHoldCaughtFish(int uniqueId)
    {
        HoldCaughtFish(uniqueId);
    }

    public void CheckForItems()
    {
        if (!photonView.IsMine)
            return;

        if (Content.childCount < 1)
        {
            NoItemsInInventoryMessage.SetActive(true);
        }
        if (Content.childCount > 0)
        {
            NoItemsInInventoryMessage.SetActive(false);
        }
    }

    public void ToggleCameraClippingPlanes(float Value)
    {
        if (!photonView.IsMine) return;

        Camera.nearClipPlane = Value;
    }


}

[System.Serializable]
public class Float
{
    public PlayerFishingInventory InventorySystem;
    public GameObject FloatPrefab;
    public GameObject LineEndPrefab;
    // [SyncVar]
    public int ID;
    public string Name;
    public Sprite Image;

    public void Spawn()
    {
        InventorySystem.SpawnFloatUI(FloatPrefab, InventorySystem.FloatContent, ID, Name, Image);
    }
}

[System.Serializable]
public class FishingRod
{
    public PlayerFishingInventory InventorySystem;
    public GameObject FishingRodPrefab;
    public GameObject FishingRodGameObject;
    // [SyncVar]
    public int ID;
    public string Name;
    public Sprite Image;

    public void Spawn()
    {
        InventorySystem.SpawnFishingRodUI(FishingRodPrefab, InventorySystem.FishingRodContent, ID, Name, Image);
    }
}

[System.Serializable]
public class Bait
{
    public PlayerFishingInventory InventorySystem;
    public GameObject BaitPrefab;
    // [SyncVar]
    public int ID;
    public string Name;
    public Sprite Image;

    public void Spawn()
    {
        InventorySystem.SpawnBaitUI(BaitPrefab, InventorySystem.BaitContent, ID, Name, Image);
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using Mirror;
using Cysharp.Threading.Tasks;
using Photon.Pun;
public class Inventory : MonoBehaviourPun
{
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
    private int currentSelectedBait = 0;
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
            return;
        }
        
        playerName = Fishverse_Core.instance.account_username;
        SetPlayerName(playerName);

        Manager = GameObject.FindGameObjectWithTag("Manager");

        Camera = Camera.main;

        SetUpFloat();

        CheckForItems();

        SpawnEquipment();
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
    [PunRPC]
    public void RpcSetPlayerName(string PlayerN)
    {
        PlayerN = playerName;
        PlayerNameText.text = PlayerN;
    }

    public void SetUpFloat()
    {
        for (int i = 0; i < Floats.Length; i++)
        {
            if (Floats[i].ID == CurrentSelectedFloat)
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
        SpawnedPrefab.GetComponent<EquipmentItem>().InventorySystem = this;
        SpawnedPrefab.GetComponent<EquipmentItem>().EquipmentType = "Float";
        SpawnedPrefab.GetComponent<EquipmentItem>().EquipmentID = ID;
        SpawnedPrefab.GetComponent<EquipmentItem>().EquipmentName.text = Name;
        SpawnedPrefab.GetComponent<EquipmentItem>().EquipmentImage = Image;
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

        if (Input.GetKeyDown(KeyCode.I))
            ToggleInventory();
    }

    public void ToggleInventory()
    {
        if (inInventory) HideInventory();
        else ShowInventory();
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

    void ShowInventory()
    {
        inInventory = true;
        // this.GetComponent<TestPlayerController>().canRotateCamera = false;
        InventoryCanvas.GetComponent<Animator>().ResetTrigger("FadeOut");
        InventoryCanvas.GetComponent<Animator>().SetTrigger("FadeIn");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void HideInventory()
    {
        inInventory = false;
        // this.GetComponent<TestPlayerController>().canRotateCamera = true;
        InventoryCanvas.GetComponent<Animator>().ResetTrigger("FadeIn");
        InventoryCanvas.GetComponent<Animator>().SetTrigger("FadeOut");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ClearInventory()
    {
        fishInv.Clear();
        Content.DestroyAllChild();
    }

    public void AddFishItem(int uniqueId, string FishName, string FishLength, string FishWeight, string FishRetailValue, Sprite FishSprite)
    {
        /*if (!isLocalPlayer)
            return;*/

        GameObject SpawnedInventoryFish;

        this.GetComponent<Animator>().SetTrigger(FishHolderAnimationName);

        HoldCaughtFish(uniqueId);


        SpawnedInventoryFish = Instantiate(InventoryFishPrefab);
        var invFish = SpawnedInventoryFish.GetComponent<InventoryFish>();

        SpawnedInventoryFish.transform.SetParent(Content);
        invFish.FishName.text = FishName;
        invFish.FishLength.text = FishLength;
        invFish.FishWeight.text = "Weight: " + FishWeight;
        invFish.FishRetailValue.text = FishRetailValue;
        invFish.FishImage.sprite = FishSprite;

        fishInv.Add(invFish);


        photonView.RPC("CmdHoldCaughtFish", RpcTarget.All, uniqueId);
        // CmdHoldCaughtFish(uniqueId);


        SpawnedInventoryFish = null;

        CheckForItems();
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
    public Inventory InventorySystem;
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
    public Inventory InventorySystem;
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
    public Inventory InventorySystem;
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


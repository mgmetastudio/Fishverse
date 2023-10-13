using UnityEngine;
// using Mirror;
using System.Collections;
using Photon.Pun;
using NullSave.GDTK.Stats;
public class FishEntity : MonoBehaviourPun
{

    // [SyncVar] 
    private int fishUniqueId;
    public int FishUniqueId
    {
        get => fishUniqueId;
        set
        {
            if (photonView.IsMine)
                photonView.RPC("SetFishUniqueId", RpcTarget.All, value);
        }
    }
    public string Location;
    private static FishScriptable[] _fishScriptables;
    public FishScriptable _scriptable;
    public FishAIController controller;
    public GameObject FishCaughtMessage;
    public float frameTime = 0.019f;
    public GameObject FishModel;
    public FishAIController fishAIController;
    [Space]
    [SerializeField] float minDist = 0.2f;
    public Rigidbody rb;
    // [SyncVar(hook = "HookedChanged")]
    private FishingFloat _hookedTo;
    private float targetStamina = 0.2f;
    private float staminaTransitionDuration = 1.0f; // Adjust the duration as needed
    private bool isStaminaTransitioning = false;
    public Canvas Canvas;
    public FishHealth fishHealth;
    public bool isFishCatched = false;
    public bool isDestroyFloat;

    [SerializeField] float FailedCatchValue;
    bool isreelrotate;
    float currentfloat;
    string BaitLocation = "";
    string FloatLocation = "";
    string RodLocation = "";
    public StatArea StatArea;
    public FishingFloat HookedTo
    {
        get => _hookedTo;
        set
        {
            if (photonView.IsMine)
            {
                int id = value ? value.photonView.ViewID : -1;
                photonView.RPC("SetHookedTo", RpcTarget.All, id);
            }
        }
    }

    [PunRPC]
    void SetFishUniqueId(int value) => fishUniqueId = value;
    [PunRPC]
    void SetHookedTo(int value)
    {
        FishingFloat _hookedToNew = value == -1 ? null : PhotonView.Find(value).GetComponent<FishingFloat>();

        HookedChanged(_hookedTo, _hookedToNew);
        _hookedTo = _hookedToNew;

    }

    // [PunRPC]
    // void SetHookedTo(FishingFloat value)
    // {
    //     FishingFloat _hookedToNew = PhotonView.Find(x);

    //     HookedChanged(_hookedTo, value);
    //     _hookedTo = value;

    // }

    private void Awake()
    {
        if (_fishScriptables == null)
        {
            _fishScriptables = Resources.LoadAll<FishScriptable>("Fish");
        }
    }

    private void Start()
    {
        for (int i = 0; i < _fishScriptables.Length; i++)
        {
            if (_fishScriptables[i].uniqueId == FishUniqueId)
            {
                _scriptable = _fishScriptables[i];
                break;
            }
        }
        if (_scriptable == null)
        {
            throw new UnityException("_scriptable == null");
        }

        FishModel = Instantiate(_scriptable.modelPrefab, transform);
        controller = gameObject.AddComponent<FishAIController>();
        controller.Setup(_scriptable);
        StartCoroutine(BiteLoop());
        Canvas.enabled = false;
        if (StatArea != null)
        {
            StatArea.enabled = false;
        }

    }

    public void SetBounds(Vector3 bounds)
    {
        if (PhotonNetwork.IsMasterClient)//isServer
        {
            controller.SetBounds(bounds);
        }
    }

#pragma warning disable IDE0051
    private void HookedChanged(FishingFloat oldValue, FishingFloat newValue)
    {
#pragma warning restore IDE0051
        if (oldValue != null)
        {
            // oldValue.GetComponent<PhotonView>().OwnershipTransfer = OwnershipOption.Fixed;
            oldValue.GetComponent<PhotonView>().RequestOwnership();
            // newValue.GetComponent<MonoBehaviourPun>().clientAuthority = false;

            oldValue.transform.SetParent(null);
            oldValue._collider.enabled = true;
            oldValue._interactor.enabled = false;
            oldValue._rb.isKinematic = true;
            oldValue._rb.useGravity = false;
        }
        if (newValue != null)
        {
            // newValue.GetComponent<MonoBehaviourPun>().clientAuthority = false;
            // newValue.GetComponent<PhotonView>().OwnershipTransfer = OwnershipOption.Fixed;

            newValue.GetComponent<PhotonView>().RequestOwnership();

            newValue.transform.SetParent(transform);
            newValue._collider.enabled = false;
            newValue._interactor.enabled = false;
            newValue._rb.isKinematic = true;
            newValue._rb.useGravity = false;
        }
    }

    private void Bite(FishingFloat _targetFloat)
    {
        if (_targetFloat.fish) return;
        isreelrotate = _targetFloat.Owner.isreelrotate;
        currentfloat = _targetFloat.Owner.GetComponent<PlayerFishingInventory>().currentFloat.previewScale;
        if (HookedTo == null && ((currentfloat == 2 && isreelrotate) || (currentfloat <= 1)))
        {
            // _targetFloat.GetComponent<NetworkTransform>().clientAuthority = false;
            // _targetFloat.GetComponent<PhotonView>().OwnershipTransfer = OwnershipOption.Fixed;
            Canvas.enabled = false;
            photonView.RPC("SetHookedToValue", RpcTarget.All, _targetFloat.photonView.ViewID);
            _targetFloat.fish = this;

            this.rb = gameObject.AddComponent<Rigidbody>();
            this.rb.isKinematic = true;
            this.rb.useGravity = false;

            controller.stamina = 1f;
            controller.doNotUpdateTarget = true;
            controller.fearfulness = 1f;
            controller.currentOffHookTime = 0f;

            if (_targetFloat.Owner.SpawnedFloatSimulation)
                _targetFloat.Owner.SpawnedFloatSimulation.SimulateBite();


            // HookedTo = _targetFloat;

            // var owner = HookedTo.Owner.SpawnedFloatSimulation.;
            // var player = owner.GetComponent<PlayerFishing>();
            // var floatSim = player.SpawnedFloatSimulation;
            // // var floatSimComp = floatSim.GetComponent<FloatSimulation>();
            // Debug.Log($"floatSimComp {floatSim.gameObject.name}", _targetFloat);
            // floatSim.SimulateBite();
        }
    }
    [PunRPC]
    private void SetHookedToValue(int viewID)
    {
        FishingFloat _hookedToNew = viewID == -1 ? null : PhotonView.Find(viewID).GetComponent<FishingFloat>();

        _hookedToNew.GetComponent<PhotonView>().RequestOwnership();
        _hookedToNew._collider.enabled = false;
        _hookedToNew._interactor.enabled = false;
        _hookedToNew._rb.isKinematic = true;
        _hookedToNew._rb.useGravity = false;
        _hookedToNew.transform.SetParent(transform);
        Vector3 NewFloatPosition = new Vector3(transform.position.x, transform.position.y + 0.0f, transform.position.z);
        _hookedToNew.transform.position = NewFloatPosition;
        _hookedTo = _hookedToNew;
    }

    private IEnumerator BiteLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (HookedTo == null && controller != null)
            {
                if (controller.target != null && Random.Range(.0f, 1f) > .4f)
                {
                    if (controller.target.gameObject.TryGetComponent<FishingFloat>(out FishingFloat fishingFloat))
                    {
                        if (fishingFloat.CheckCompability(controller._scriptable))
                        {
                            FailedCatchValue = Random.value;
                            Bite(fishingFloat);
                        }
                    }
                    //bruh too many ifs
                    // Bite(controller.target.gameObject.GetComponent<FishingFloat>());
                }
            }
        }
    }

    private void Update()
    {
        if (controller.target != null)
        {
            //Debug.Log("target is +" + controller.target.name);
        }
        if (HookedTo == null && rb != null)
        {
            Canvas.enabled = false;
            Destroy(rb);
            rb = null;
            controller.doNotUpdateTarget = false;
            controller.fearfulness = .0f;
            isDestroyFloat = true;
            if (StatArea != null)
            {
                StatArea.enabled = false;
            }

        }
        if (HookedTo == null)
        {
            if (controller.stamina < 0.2)
            {
                FishModel.transform.rotation = new Quaternion(0, 0, 0, 0);
                controller.stamina_move = 0.5f;
            }
        }

        if (HookedTo != null)
        {
            if (!HookedTo.Owner.GetComponent<PhotonView>().IsMine) return;
            isDestroyFloat = false;
            itemsCompatibility();
            Debug.Log("Bait location" + BaitLocation + "/ Float location" + FloatLocation + "/ Rod location" + RodLocation);
            if (((BaitLocation != Location && HookedTo.Owner.GetComponent<PlayerFishingInventory>().currentFloat.previewScale <= 1) || FloatLocation != Location || RodLocation != Location) && FailedCatchValue > 0 && FailedCatchValue < 0.5)
            {
                controller.isUpgradeFishingRod = true;
                HookedTo.Owner.UpgradeFishingRodText.SetActive(true);
            }
            else
            {
                controller.isUpgradeFishingRod = false;
                FailedCatchValue = 0;
            }
            // Vector3 NewFishModelPosition = new Vector3(HookedTo.Hook.transform.position.x, HookedTo.Hook.transform.position.y - 0.15f, HookedTo.Hook.transform.position.z);
            // FishModel.transform.position = NewFishModelPosition;
            //transform.position = Vector3.Lerp(FishModel.transform.position, NewFishModelPosition, 10 * Time.deltaTime);
            if (fishHealth.healthBar.value != 0)
            {
                Canvas.enabled = true;
            }
             /*if (HookedTo.Owner.isDestroyFloat)
             {
                 controller.HealthBar += 1;
                 Canvas.enabled = false;
                 HookedTo = null;
                 FishModel.transform.rotation = new Quaternion(0, 0, 0, 0);
             }*/

            if (fishHealth.healthBar.value == 0)
            {
                if (!isStaminaTransitioning)
                {
                    isStaminaTransitioning = true;
                    StartCoroutine(SmoothStaminaTransition());
                }

                FishModel.transform.LookAt(HookedTo.Owner._rodEndPoint.position);
                Canvas.enabled = false;

            }
            if (controller.stamina < 0.2 && HookedTo != null)
            {
                FishModel.transform.LookAt(HookedTo.Owner._rodEndPoint.position);
            }
            if ((controller.StaminaBar == 0f || controller.StaminaBar == 1f) && fishHealth.healthBar.value != 0)
            {
                HookedTo.Owner._Linebroke.SetBool("Linebroke_Start", false);
                HookedTo.Owner.GetComponent<PlayerFishing>().photonView.RPC("CmdDestroyFloat", RpcTarget.All);
                controller.doNotUpdateTarget = false;
                controller.fearfulness = .0f;
                HookedTo = null;
                FishModel.transform.rotation = new Quaternion(0, 0, 0, 0);

            }
            if (((controller.StaminaBar > 0 && controller.StaminaBar < 0.25) || (controller.StaminaBar > 0.75 && controller.StaminaBar < 1)) && fishHealth.healthBar.value != 0 && controller.doNotUpdateTarget)
            {
                if (HookedTo != null)
                {
                    HookedTo.Owner._Linebroke.SetBool("Linebroke_Start", true);
                }
            }
            else
            {
                if (HookedTo != null)
                {
                    HookedTo.Owner._Linebroke.SetBool("Linebroke_Start", false);
                }
            }
            // Vector3.Distance(transform.position.WithY(0), HookedTo.Owner._rodEndPoint.position.WithY(0)) < minDist
            if (StatArea != null)
            {
                if (fishHealth.healthBar.value == 0)
                {
                    StatArea.enabled = true;
                }
            }
            if (controller.iscatched && fishHealth.healthBar.value == 0)
            {
                HookedTo.Owner._Linebroke.SetBool("Linebroke_Start", false);

                var anim = HookedTo.Owner.GetComponent<PlayerAnimator>();
                PlayerFishingInventory inv = HookedTo.Owner.GetComponent<PlayerFishingInventory>();
                Debug.Log("Fish with ID " + controller._scriptable.uniqueId + " caught!");
                anim.FishCatch();
                if (inv != null && controller != null)
                {
                    inv.HoldCaughtFish(controller._scriptable.uniqueId);
                    inv.AddFishItem(controller._scriptable);
                }
                var fishInfo = controller._scriptable;
                Canvas.enabled = false;
                photonView.RPC("RpcHoldCaughtFish", RpcTarget.All, controller._scriptable.uniqueId);
                // RpcHoldCaughtFish(ai._scriptable.uniqueId);
               
                Instantiate(FishCaughtMessage).GetComponent<FishCaughtMessage>().Message.text = "<color=orange>" + inv.playerName + "</color>" + " caught a " + "<color=green>" + controller._scriptable.FishWeight + "</color>" + " " + "<color=green>" + controller._scriptable.FishName + "</color>";
                if (photonView.IsMine)
                {
                    // Destroy the GameObject across the network.
                    PhotonNetwork.Destroy(gameObject);
                }
                HookedTo.Owner.GetComponent<PlayerFishing>().photonView.RPC("CmdDestroyFloat", RpcTarget.All);
                // HookedTo.Owner.GetComponent<PlayerFishing>().DestroyFloatSimulation();


                // NetworkServer.Destroy(gameObject);
                // Debug.Break();
                
            }
        }



    }
    void itemsCompatibility()
    {
        if (HookedTo.Owner.GetComponent<PlayerFishingInventory>().currentBait != null)
        {
            foreach (var tag in HookedTo.Owner.GetComponent<PlayerFishingInventory>().currentBait.customTags)
            {
                string tagValue1 = tag.Value;
                if (tagValue1 == "Yellow")
                {
                    BaitLocation = tagValue1;
                    break;
                }
                else if (tagValue1 == "Blue")
                {
                    BaitLocation = tagValue1;
                    break;
                }
                else if (tagValue1 == "Orange")
                {
                    BaitLocation = tagValue1;
                    break;
                }
                else if (tagValue1 == "Purple")
                {
                    BaitLocation = tagValue1;
                    break;
                }

            }
        }
        if (HookedTo.Owner.GetComponent<PlayerFishingInventory>().currentFloat != null)
        {
            foreach (var tag in HookedTo.Owner.GetComponent<PlayerFishingInventory>().currentFloat.customTags)
            {
                string tagValue2 = tag.Value;
                if (tagValue2 == "Yellow")
                {
                    FloatLocation = tagValue2;
                    break;
                }
                else if (tagValue2 == "Blue")
                {
                    FloatLocation = tagValue2;
                    break;
                }
                else if (tagValue2 == "Orange")
                {
                    FloatLocation = tagValue2;
                    break;
                }
                else if (tagValue2 == "Purple")
                {
                    FloatLocation = tagValue2;
                    break;
                }
            }
        }
        if (HookedTo.Owner.GetComponent<PlayerFishingInventory>().currentRod != null)
        {
            foreach (var tag in HookedTo.Owner.GetComponent<PlayerFishingInventory>().currentRod.customTags)
            {
                string tagValue3 = tag.Value;
                if (tagValue3 == "Yellow")
                {
                    RodLocation = tagValue3;
                    break;
                }
                else if (tagValue3 == "Blue")
                {
                    RodLocation = tagValue3;
                    break;
                }
                else if (tagValue3 == "Orange")
                {
                    RodLocation = tagValue3;
                    break;
                }
                else if (tagValue3 == "Purple")
                {
                    RodLocation = tagValue3;
                    break;
                }

            }
        }
    }

    // [ClientRpc]
    [PunRPC]
    public void RpcHoldCaughtFish(int uniqueId)
    {
        print("FISH");
        var inv = HookedTo.Owner.GetComponent<PlayerFishingInventory>();

        inv.HoldCaughtFish(uniqueId);
        HookedTo.Owner.GetComponent<Animator>().Play(inv.FishHolderAnimationName);

        GameObject SpawnedInventoryFish;

        SpawnedInventoryFish = Instantiate(inv.InventoryFishPrefab);
        // SpawnedInventoryFish.transform.SetParent(inv.Content);

        var fish = SpawnedInventoryFish.GetComponent<InventoryFish>();
        fish.FishName.text = _scriptable.FishName;
        fish.FishLength.text = "";
        fish.FishWeight.text = "";
        fish.FishRetailValue.text = "";
        fish.FishImage.sprite = _scriptable.FishSprite;

        SpawnedInventoryFish = null;

        inv.CheckForItems();
    }


    private void OnGUI()
    {
        if (PhotonNetwork.IsMasterClient)//isServer
        {
            if (HookedTo != null)
            {
                GUI.Label(new Rect(1820, 0, 100, 100), "Server Fish Stamina: " + controller.stamina + "\n hooked!");
            }
        }
    }
    private IEnumerator SmoothStaminaTransition()
    {
        float elapsedTime = 0f;
        float startStamina = controller.stamina;

        while (elapsedTime < staminaTransitionDuration)
        {
            elapsedTime += frameTime;
            float t = Mathf.Clamp01(elapsedTime / staminaTransitionDuration);
            controller.stamina = Mathf.Lerp(startStamina, targetStamina, t);
            yield return null;
        }
        controller.stamina = targetStamina;
        isStaminaTransitioning = false;
    }
}

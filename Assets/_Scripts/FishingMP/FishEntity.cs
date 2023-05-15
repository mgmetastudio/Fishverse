using UnityEngine;
// using Mirror;
using System.Collections;
using Photon.Pun;

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


    private static FishScriptable[] _fishScriptables;
    private FishScriptable _scriptable;
    public FishAIController controller;
    public GameObject FishCaughtMessage;
    public GameObject FishModel;

    [Space]
    [SerializeField] float minDist = 2f;

    public Rigidbody rb;
    // [SyncVar(hook = "HookedChanged")]
    private FishingFloat _hookedTo;
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

        if (PhotonNetwork.IsMasterClient)//isServer
        {
            controller = gameObject.AddComponent<FishAIController>();
            controller.Setup(_scriptable);
        }

        StartCoroutine(BiteLoop());
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

        if (HookedTo == null)
        {
            // _targetFloat.GetComponent<NetworkTransform>().clientAuthority = false;
            // _targetFloat.GetComponent<PhotonView>().OwnershipTransfer = OwnershipOption.Fixed;
            _targetFloat.GetComponent<PhotonView>().RequestOwnership();

            _targetFloat._collider.enabled = false;
            _targetFloat._interactor.enabled = false;
            _targetFloat._rb.isKinematic = true;
            _targetFloat._rb.useGravity = false;
            _targetFloat.transform.SetParent(transform);
            Vector3 NewFloatPosition = new Vector3(transform.position.x, transform.position.y + 0.0f, transform.position.z);
            _targetFloat.transform.position = NewFloatPosition;
            HookedTo = _targetFloat;
            HookedTo.fish = this;
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            controller.stamina = 1f;
            controller.doNotUpdateTarget = true;
            controller.fearfulness = 1f;
            controller.currentOffHookTime = 0f;

            if (HookedTo.Owner.SpawnedFloatSimulation)
                HookedTo.Owner.SpawnedFloatSimulation.SimulateBite();
            // var owner = HookedTo.Owner.SpawnedFloatSimulation.;
            // var player = owner.GetComponent<PlayerFishing>();
            // var floatSim = player.SpawnedFloatSimulation;
            // // var floatSimComp = floatSim.GetComponent<FloatSimulation>();
            // Debug.Log($"floatSimComp {floatSim.gameObject.name}", _targetFloat);
            // floatSim.SimulateBite();
        }
    }

    private IEnumerator BiteLoop()
    {
        while (PhotonNetwork.IsMasterClient)
        {
            yield return new WaitForSeconds(1f);
            if (HookedTo == null && controller != null)
            {
                if (controller.target != null && Random.Range(.0f, 1f) > .4f)
                {
                    if (controller.target.gameObject.TryGetComponent<FishingFloat>(out FishingFloat fishingFloat))
                        Bite(fishingFloat);

                    // Bite(controller.target.gameObject.GetComponent<FishingFloat>());
                }
            }
        }
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)//isServer
        {
            if (HookedTo == null && rb != null)
            {
                Destroy(rb);
                rb = null;
                controller.doNotUpdateTarget = false;
                controller.fearfulness = .0f;
            }

            if (HookedTo != null)
            {



                Vector3 NewFishModelPosition = new Vector3(HookedTo.Hook.transform.position.x, HookedTo.Hook.transform.position.y - 0.15f, HookedTo.Hook.transform.position.z);
                FishModel.transform.position = NewFishModelPosition;
                if (controller.stamina < 0.7f)
                {
                    FishModel.transform.LookAt(HookedTo.Owner._rodEndPoint.position);
                    /*Quaternion NewFishModelRotation = new Quaternion(FishModel.transform.rotation.x, 180, 180, 0);
                    FishModel.transform.rotation = Quaternion.Lerp(FishModel.transform.rotation, NewFishModelRotation, 5f);*/
                }
                if (controller.stamina > 0.7f)
                {
                    if (controller.currentOffHookTime >= controller.getOffHookTime)
                    {
                        HookedTo.Owner.GetComponent<PlayerFishing>().photonView.RPC("CmdDestroyFloat", RpcTarget.All);

                        controller.doNotUpdateTarget = false;
                        controller.fearfulness = .0f;
                        transform.parent = null;
                        HookedTo = null;
                        return;
                    }

                    // FishModel.transform.LookAt(HookedTo.Owner._rodEndPoint.position);
                    FishModel.transform.rotation = new Quaternion(0, 0, 0, 0);
                }
                if (Vector3.Distance(transform.position.WithY(0), HookedTo.Owner._rodEndPoint.position.WithY(0)) < minDist)
                {
                    var anim = HookedTo.Owner.GetComponent<PlayerAnimator>();
                    Inventory inv = HookedTo.Owner.GetComponent<Inventory>();

                    FishAIController ai = this.GetComponent<FishAIController>();

                    Debug.Log("Fish with ID " + ai._scriptable.uniqueId + " caught!");

                    anim.FishCatch();
                    inv.HoldCaughtFish(ai._scriptable.uniqueId);

                    var fishInfo = ai._scriptable;

                    float fishSizeMulti = 1f.GetRandom();

                    float fishLength = fishInfo.FishLength.Lerp(fishSizeMulti);
                    float fishWeight = fishInfo.FishWeight.Lerp(fishSizeMulti);
                    int fishvalue = (int)(fishInfo.FishRetailValue * (1 + fishSizeMulti));

                    inv.AddFishItem(ai._scriptable.uniqueId, ai._scriptable.FishName, fishLength, fishWeight, fishvalue, ai._scriptable.FishSprite);
                    photonView.RPC("RpcHoldCaughtFish", RpcTarget.All, ai._scriptable.uniqueId);
                    // RpcHoldCaughtFish(ai._scriptable.uniqueId);
                    Instantiate(FishCaughtMessage).GetComponent<FishCaughtMessage>().Message.text = "<color=orange>" + inv.playerName + "</color>" + " caught a " + "<color=green>" + ai._scriptable.FishWeight + "</color>" + " " + "<color=green>" + ai._scriptable.FishName + "</color>";
                    HookedTo.Owner.GetComponent<PlayerFishing>().photonView.RPC("CmdDestroyFloat", RpcTarget.All);
                    // HookedTo.Owner.GetComponent<PlayerFishing>().DestroyFloatSimulation();


                    // NetworkServer.Destroy(gameObject);
                    // Debug.Break();
                    PhotonNetwork.Destroy(gameObject);
                }
            }
        }
    }

    // [ClientRpc]
    [PunRPC]
    public void RpcHoldCaughtFish(int uniqueId)
    {
        print("FISH");
        var inv = HookedTo.Owner.GetComponent<Inventory>();

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
}

using UnityEngine;
// using Mirror;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class FishingFloat : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks
{

    // [SyncVar] 
    private int floatUniqueId;
    public int FloatUniqueId
    {
        get => floatUniqueId;
        set
        {
            if (photonView.IsMine)
                photonView.RPC("SetFloatUniqueId", RpcTarget.All, value);
        }
    }

    public FluidInteractorBase _interactor;

    private static FishingFloatScriptable[] _floatScriptables;
    private FishingFloatScriptable _scriptable;

    public Rigidbody _rb;
    public Collider _collider;

    // [SyncVar] 
    private PlayerFishing owner;
    public PlayerFishing Owner
    {
        get => owner;
        set
        {
            if (photonView.IsMine)
            {
                int id = value.photonView.ViewID;
                photonView.RPC("SetOwner", RpcTarget.All, id);
            }
        }
    }

    public FishEntity fish;

    public Transform Hook;

    [PunRPC]
    void SetFloatUniqueId(int value) => floatUniqueId = value;
    [PunRPC]
    void SetOwner(int value)
    {
        PlayerFishing newPlayerFishing = PhotonView.Find(value).GetComponent<PlayerFishing>();
        print("SETING OWNER: " + newPlayerFishing);
        owner = newPlayerFishing;
    }

    // [PunRPC]
    // void SetOwner(PlayerFishing value) => owner = value;


    private void Awake()
    {
        _interactor = GetComponent<FluidInteractorBase>();
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        if (_floatScriptables == null)
        {
            _floatScriptables = Resources.LoadAll<FishingFloatScriptable>("FishingFloats");
        }
    }

    // public void Destroy(NetworkConnection sender)
    // {
    //     if (sender != null && sender.identity != null && sender.identity.GetComponent<PlayerFishing>() == _owner)
    //     {
    //         NetworkServer.Destroy(gameObject);
    //     }
    // }

    // [Command(requiresAuthority = true)]
    
    [PunRPC]
    public void Pull()
    {
        if (fish != null)
        {
            fish.controller.target = Owner._rodEndPoint;
            fish.controller.pullForce = 1f;
        }
        else
        {
            photonView.RPC("TargetPull", RpcTarget.All);
            // TargetPull();
        }
    }

    [PunRPC]
    public void TargetPull()
    {
        _rb.AddForce((Owner._rodEndPoint.position - transform.position) * .2f);
    }

    private void Start()
    {
        if (!photonView.IsMine)//hasAuthority
        {
            _interactor.enabled = false;
            _rb.isKinematic = true;
            _rb.useGravity = false;
        }

        for (int i = 0; i < _floatScriptables.Length; i++)
        {
            if (_floatScriptables[i].uniqueId == FloatUniqueId)
            {
                _scriptable = _floatScriptables[i];
                break;
            }
        }

        // Assign your customization variables here ~
        _ = Instantiate(_scriptable.modelPrefab, transform); // Model
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        _interactor.enabled = true;
        _rb.isKinematic = false;
        _rb.useGravity = true;
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        _interactor.enabled = false;
        _rb.isKinematic = true;
        _rb.useGravity = false;
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
        _interactor.enabled = false;
        _rb.isKinematic = true;
        _rb.useGravity = false;
    }

    public bool CheckCompability(FishScriptable fish)
    {
        var inv = owner.GetComponent<PlayerFishingInventory>();

        var feedType = inv.currentBait.statEffects.FirstOrDefault(x => x == fish.feedType);
        if(feedType == null)
            return false;

        var waterType = inv.currentRod.statEffects.FirstOrDefault(x => x == fish.waterType);
        if(waterType == null)
            return false;

        return true;
    }

    // public override void OnStartAuthority()
    // {
    //     base.OnStartAuthority();
    //     _interactor.enabled = true;
    //     _rb.isKinematic = false;
    //     _rb.useGravity = true;   
    // }

    // public override void OnStopAuthority()
    // {
    //     base.OnStopAuthority();
    //     _interactor.enabled = false;
    //     _rb.isKinematic = true;
    //     _rb.useGravity = false;
    // }
}

using UnityEngine;
using System.Collections.Generic;
// using Mirror;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;
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
    [SerializeField] PlayerFishingInventory inv;
    public Rigidbody _rb;
    public Collider _collider;

    // [SyncVar] 
    private PlayerFishing owner;
    public static UnityEvent onLineBroke;

    [SerializeField] public bool Destroyfloat= false;
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

    [Header("Audio Source")]
    public AudioSource r_AudioSource;

    [Header("Float Land Clips")]
    public List<AudioClip> FloatLandClips;
    [PunRPC]
    void SetFloatUniqueId(int value) => floatUniqueId = value;
    [PunRPC]
    void SetOwner(int value)
    {
        PlayerFishing newPlayerFishing = PhotonView.Find(value).GetComponent<PlayerFishing>();
        print("SETING OWNER: " + newPlayerFishing+ "SETING OWNER"+value);
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
            Destroyfloat = false;
        }
        else
        {
          
            photonView.RPC("TargetPull", RpcTarget.All);
            //TargetPull();
        }
    }

    [PunRPC]
    public void TargetPull()
    {
         
        _rb.AddForce((Owner._rodEndPoint.position - transform.position) * .2f);
        Vector3 targetPosition = Owner._rodEndPoint.position;
        targetPosition.y = transform.position.y; // Preserve the initial height
        if (fish == null)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 1.5f);
        }
        // owner.DestroyFloatmaxdistance();
        bool isTouchingGround=false ;

        // Check if the float is touching the ground collider
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.5f);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Default"))
            {
                isTouchingGround = true;
                break;
            }
        }

        if (Vector3.Distance(owner._rodEndPoint.position, owner.FishingFloat.transform.position) < 2f || isTouchingGround && fish == null)
        {
            Destroyfloat = true;
        }


    }

    private void Start()
    {
        if (!photonView.IsMine)//hasAuthority
        {
            _interactor.enabled = false;
            _rb.isKinematic = true;
            _rb.useGravity = false;
        }

        inv = owner.GetComponent<PlayerFishingInventory>();

        for (int i = 0; i < _floatScriptables.Length; i++)
        {
            if (inv.currentFloat != null)
            {
                if (_floatScriptables[i].uniqueId == inv.currentFloat.previewScale)
                {
                    _scriptable = _floatScriptables[i];
                    break;
                }
            }
        }


        // Assign your customization variables here ~
        if (photonView.IsMine)
        {
            _ = Instantiate(_scriptable.modelPrefab, transform); // Model
        }

        if (FloatLandClips.Count > 0)
        {
            // Generate a random index to select a random clip from the list
            int randomIndex = Random.Range(0, FloatLandClips.Count);

            // Get the randomly selected AudioClip
            AudioClip randomClip = FloatLandClips[randomIndex];

            // Set the AudioSource's clip to the randomly selected clip
            r_AudioSource.clip = randomClip;

            // Play the audio
            r_AudioSource.Play();
        }
        else
        {
            Debug.LogError("FloatLandClips list is empty. Add some audio clips to the list.");
        }
    }
    public void Update()
    {
        if (fish == null)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 0.5f);
            bool isTouchingGround = false;
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.layer == LayerMask.NameToLayer("Default"))
                {
                    isTouchingGround = true;
                    break;
                }
            }
            if (isTouchingGround && fish == null)
            {
                Destroyfloat = true;
            }
        }
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
        if (!photonView.IsMine)
            return false;

        inv = Owner.GetComponent<PlayerFishingInventory>();

        if (inv.currentBait != null)
        {
            var feedType = inv.currentBait.statEffects.FirstOrDefault(x => x == fish.feedType);

            if (feedType == null)
                return false;
        }
        if (inv.currentRod != null)
        {
            var waterType = inv.currentRod.statEffects.FirstOrDefault(x => x == fish.waterType);
            if (waterType == null)
                return false;
        }



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

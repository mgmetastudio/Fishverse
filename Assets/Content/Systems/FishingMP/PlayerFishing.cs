using UnityEngine;
// using Mirror;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using Photon.Pun;

public class PlayerFishing : MonoBehaviourPun
{

    [SerializeField] private GameObject _fishingFloatBasePrefab;
    // [SyncVar]
    [HideInInspector] public FishingFloat _fishingFloat;

    [Space]
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private LayerMask _fluidMask;

    [Space]
    [SerializeField] private GameObject _floatDemoPrefab;
    private GameObject _floatDemo;
    [SerializeField] public Transform _rodEndPoint;
    [SerializeField] private LineRenderer _rodLineRenderer;
    [SerializeField] private int _maxLineDistance;
    [SerializeField] private int _maxLineThrowDistance;
    [SerializeField] public string crankUpAnimationName = "Fishing_Up";
    [SerializeField] public string CastAnimationName = "Fishing_In";
    [SerializeField] public GameObject FloatSimulation;
    public GameObject SpawnedFloatSimulation;

    [SerializeField] float onCastWait;

    [SerializeField] GameObject fishingRod;
    [SerializeField] GameObject fishingRope;

    // public UnityEvent onCast;

    public static UnityEvent onLineBroke;

    Inventory _inv;
    Animator _anim;

    Transform _localCamera;

    void Start()
    {
        if (photonView.IsMine)
        {
            _floatDemo = Instantiate(_floatDemoPrefab);
            _localCamera = Camera.main.transform;//GetComponentInChildren<Camera>().transform;
        }

        _inv = GetComponent<Inventory>();
        _anim = GetComponent<Animator>();

    }

    private async void Update()
    {
        if (photonView.IsMine)
        {
            if (_fishingFloat == null)
            {
                if (fishingRod.activeSelf && Physics.Raycast(_localCamera.position, _localCamera.forward, out RaycastHit hitInfo, _maxLineThrowDistance, _fluidMask))
                {
                    if (!Physics.Raycast(_localCamera.position, _localCamera.forward, Vector3.Distance(_localCamera.position, hitInfo.point) + .01f, _obstacleMask))
                    {
                        _floatDemo.SetActive(true);
                        _floatDemo.transform.position = hitInfo.point;
                    }
                    else
                    {
                        _floatDemo.SetActive(false);
                    }
                }
                else
                {
                    _floatDemo.SetActive(false);
                }

                if (fishingRod.activeSelf && Input.GetMouseButtonDown(0))
                {
                    if (_floatDemo.activeSelf)
                    {
                        // onCast.Invoke();
                        _inv.HideFish();

                        _anim.SetTrigger("FishingCast");


                        await UniTask.WaitForSeconds(onCastWait);

                        //TODO: this
                        CmdSpawnFloat(_floatDemo.transform.position, _inv.CurrentSelectedFloat);
                        foreach (GameObject AllFish in _inv.Fishes)
                        {
                            AllFish.SetActive(false);
                        }
                        _inv.FishHolder.SetActive(false);
                    }
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    DrawFishingRod(!fishingRod.activeSelf);

                }
            }
            else
            {
                _floatDemo.SetActive(false);

                if (Input.GetButton("CrankUp"))
                {
                    _fishingFloat.Pull();
                    _anim.SetFloat("Fishing_Up_Speed", 1);
                    _anim.Play(crankUpAnimationName);
                }

                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    _anim.SetFloat("Fishing_Up_Speed", 0);
                }

                if (Input.GetMouseButtonDown(1))
                {
                    CmdDestroyFloat();
                }

                if (Vector3.Distance(_rodEndPoint.position, _fishingFloat.transform.position) > _maxLineDistance)
                {
                    onLineBroke?.Invoke();
                    CmdDestroyFloat();
                }
            }
        }

        if (_fishingFloat == null)
        {
            _rodLineRenderer.SetPosition(0, Vector3.zero);
            _rodLineRenderer.SetPosition(1, Vector3.zero);
        }
        else
        {
            _rodLineRenderer.SetPosition(0, _rodEndPoint.position);
            _rodLineRenderer.SetPosition(1, _fishingFloat.transform.position);
        }
    }

    public void DrawFishingRod(bool draw)
    {
        print("FISHING ROD");

        fishingRod.SetActive(draw);
        fishingRope.SetActive(draw);

        var inv = GetComponent<Inventory>();

        if (draw)
            inv.SetFloat(0);
        else
            Destroy(inv.LineEnd);

        _anim.SetLayerWeight(2, draw.Int());

    }

    // [Command(requiresAuthority = true)]
    private void CmdSpawnFloat(Vector3 position, int uniqueId)
    {
        if (_fishingFloat == null)
        {
            foreach (GameObject AllFish in _inv.Fishes)
            {
                AllFish.SetActive(false);
            }
            _inv.FishHolder.SetActive(false);
            RpcDisableHoldingFish();
            GameObject fishingFloatObj = Instantiate(_fishingFloatBasePrefab);
            FishingFloat temp = fishingFloatObj.GetComponent<FishingFloat>();
            temp.transform.position = position;
            temp.floatUniqueId = uniqueId;
            temp._owner = this;
            //TODO: SPAWN
            // NetworkServer.Spawn(fishingFloatObj, connectionToClient);
            _fishingFloat = temp;
            SpawnFloatSimulation();
        }
    }



    // [Command]
    private void CmdDestroyFloat()
    {
        if (_fishingFloat != null)
        {
            // _fishingFloat.Destroy(connectionToClient);
            //TODO: DESTROY
            DestroyFloatSimulation();
        }
    }

    // [ClientRpc]
    public void RpcDisableHoldingFish()
    {
        foreach (GameObject AllFish in _inv.Fishes)
        {
            AllFish.SetActive(false);
        }
        _inv.FishHolder.SetActive(false);
    }

    public void SpawnFloatSimulation()
    {
        if (!photonView.IsMine)
            return;

        SpawnedFloatSimulation = Instantiate(FloatSimulation);
    }

    public void DestroyFloatSimulation()
    {
        if (!photonView.IsMine)
            return;

        Destroy(SpawnedFloatSimulation);
    }
}

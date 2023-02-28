using UnityEngine;
using Mirror;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;

public class PlayerFishing : NetworkBehaviour
{

    [SerializeField] private GameObject _fishingFloatBasePrefab;
    [SyncVar, HideInInspector] public FishingFloat _fishingFloat;

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

    // public UnityEvent onCast;

    public static UnityEvent onLineBroke;

    Inventory _inv;
    Animator _anim;

    Transform _localCamera;

    void Start()
    {
        if (isLocalPlayer)
        {
            _floatDemo = Instantiate(_floatDemoPrefab);
            _localCamera = Camera.main.transform;//GetComponentInChildren<Camera>().transform;
        }

        _inv = GetComponent<Inventory>();
        _anim = GetComponent<Animator>();

    }

    private async void Update()
    {
        if (isLocalPlayer)
        {
            if (_fishingFloat == null)
            {
                if (Physics.Raycast(_localCamera.position, _localCamera.forward, out RaycastHit hitInfo, _maxLineThrowDistance, _fluidMask))
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

                if (Input.GetMouseButtonDown(0))
                {
                    if (_floatDemo.activeSelf)
                    {
                        // onCast.Invoke();
                        _inv.HideFish();

                        _anim.SetTrigger("FishingCast");


                        await UniTask.WaitForSeconds(onCastWait);

                        CmdSpawnFloat(_floatDemo.transform.position, _inv.CurrentSelectedFloat);
                        foreach (GameObject AllFish in _inv.Fishes)
                        {
                            AllFish.SetActive(false);
                        }
                        _inv.FishHolder.SetActive(false);
                    }
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

    [Command(requiresAuthority = true)]
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
            NetworkServer.Spawn(fishingFloatObj, connectionToClient);
            _fishingFloat = temp;
            SpawnFloatSimulation();
        }
    }



    [Command]
    private void CmdDestroyFloat()
    {
        if (_fishingFloat != null)
        {
            _fishingFloat.Destroy(connectionToClient);
            DestroyFloatSimulation();
        }
    }

    [ClientRpc]
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
        if (!isLocalPlayer)
            return;

        SpawnedFloatSimulation = Instantiate(FloatSimulation);
    }

    public void DestroyFloatSimulation()
    {
        if (!isLocalPlayer)
            return;

        Destroy(SpawnedFloatSimulation);
    }
}

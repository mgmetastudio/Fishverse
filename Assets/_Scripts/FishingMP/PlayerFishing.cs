using UnityEngine;
// using Mirror;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerFishing : MonoBehaviourPun
{

    [SerializeField] private GameObject _fishingFloatBasePrefab;
    // [SyncVar]
    [HideInInspector] private FishingFloat fishingFloat;
    public FishingFloat FishingFloat
    {
        get => fishingFloat;
        set
        {
            if (photonView.IsMine)
            {
                int id = value.photonView.ViewID;
                photonView.RPC("SetFishingFloat", RpcTarget.All, id);
            }
        }
    }

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
    public FloatSimulation SpawnedFloatSimulation;

    [SerializeField] float onCastWait;

    [SerializeField] GameObject fishingRod;
    [SerializeField] GameObject fishingRope;

    [Space]
    [SerializeField] Button holsterBtn;

    // public UnityEvent onCast;

    public static UnityEvent onLineBroke;

    Inventory _inv;
    Animator _anim;

    Transform _localCamera;

    CastRodUI rodUI;
    bool onRodDown;
    bool onRodUp;
    bool onRod;

    [PunRPC]
    void SetFishingFloat(int value)
    {
        FishingFloat newFishingFloat = PhotonView.Find(value).GetComponent<FishingFloat>();
        fishingFloat = newFishingFloat;
    }
    // [PunRPC]
    // void SetFishingFloat(FishingFloat value) => fishingFloat = value;


    void Start()
    {
        if (photonView.IsMine)
        {
            _floatDemo = Instantiate(_floatDemoPrefab);
            _localCamera = Camera.main.transform;
            rodUI = FindObjectOfType<CastRodUI>();

            holsterBtn.onClick.AddListener(Holster);

            rodUI.btn.onDown.AddListener(OnRodDown);
            rodUI.btn.onUp.AddListener(OnRodUp);
        }

        _inv = GetComponent<Inventory>();
        _anim = GetComponent<Animator>();
    }

    void OnRodDown()
    {
        onRodDown = true;
        onRod = true;
    }

    void OnRodUp()
    {
        onRodUp = true;
        onRod = false;
    }

    void Holster()
    {
        if (photonView.IsMine)
        {
            if (FishingFloat == null)
                photonView.RPC("DrawFishingRod", RpcTarget.All, !fishingRod.activeSelf);
            else
                photonView.RPC("CmdDestroyFloat", RpcTarget.All);
        }
    }

    private async void Update()
    {
        if (photonView.IsMine)
        {
            if (FishingFloat == null)
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

                if (fishingRod.activeSelf && CastInput())
                {
                    if (_floatDemo.activeSelf)
                    {
                        // onCast.Invoke();
                        _inv.HideFish();

                        _anim.SetTrigger("FishingCast");


                        await UniTask.WaitForSeconds(onCastWait);

                        //TODO: this
                        photonView.RPC("CmdSpawnFloat", RpcTarget.All, _floatDemo.transform.position, _inv.CurrentSelectedFloat);
                        // CmdSpawnFloat(_floatDemo.transform.position, _inv.CurrentSelectedFloat);
                        foreach (GameObject AllFish in _inv.Fishes)
                        {
                            AllFish.SetActive(false);
                        }
                        _inv.FishHolder.SetActive(false);
                    }
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    photonView.RPC("DrawFishingRod", RpcTarget.All, !fishingRod.activeSelf);

                    // DrawFishingRod(!fishingRod.activeSelf);
                }
            }
            else
            {
                _floatDemo.SetActive(false);

                if (CrankUpInput())
                {
                    // FishingFloat.Pull();
                    FishingFloat.photonView.RPC("Pull", RpcTarget.All);
                    _anim.SetFloat("Fishing_Up_Speed", 1);
                    _anim.Play(crankUpAnimationName);
                }

                if (CrankDownInput())
                {
                    _anim.SetFloat("Fishing_Up_Speed", 0);
                }

                if (Input.GetMouseButtonDown(1))
                {
                    // CmdDestroyFloat();
                    photonView.RPC("CmdDestroyFloat", RpcTarget.All);
                }

                if (Vector3.Distance(_rodEndPoint.position, FishingFloat.transform.position) > _maxLineDistance)
                {
                    onLineBroke?.Invoke();
                    photonView.RPC("CmdDestroyFloat", RpcTarget.All);
                    // CmdDestroyFloat();
                }
            }
        }

        if (FishingFloat == null)
        {
            _rodLineRenderer.SetPosition(0, Vector3.zero);
            _rodLineRenderer.SetPosition(1, Vector3.zero);
        }
        else
        {
            _rodLineRenderer.SetPosition(0, _rodEndPoint.position);
            _rodLineRenderer.SetPosition(1, FishingFloat.transform.position);
        }

        onRodDown = false;
        onRodUp = false;
    }

    bool CastInput()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            return onRodDown;
        return Input.GetMouseButtonDown(0);
    }

    bool CrankUpInput()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            return onRod;
        return Input.GetButton("CrankUp");
    }

    bool CrankDownInput()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            return onRodUp;
        return Input.GetKeyUp(KeyCode.Mouse0);
    }

    [PunRPC]
    public void DrawFishingRod(bool draw)
    {
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
    [PunRPC]
    private void CmdSpawnFloat(Vector3 position, int uniqueId)
    {
        if (FishingFloat == null)
        {
            foreach (GameObject AllFish in _inv.Fishes)
            {
                AllFish.SetActive(false);
            }
            _inv.FishHolder.SetActive(false);
            // RpcDisableHoldingFish();
            photonView.RPC("RpcDisableHoldingFish", RpcTarget.All);
            GameObject fishingFloatObj = PhotonNetwork.Instantiate(_fishingFloatBasePrefab.name, position, Quaternion.identity);
            // GameObject fishingFloatObj = Instantiate(_fishingFloatBasePrefab);
            FishingFloat temp = fishingFloatObj.GetComponent<FishingFloat>();
            // temp.photonView.RequestOwnership();
            temp.Owner = this;
            temp.FloatUniqueId = uniqueId;
            //TODO: SPAWN
            // NetworkServer.Spawn(fishingFloatObj, connectionToClient);
            FishingFloat = temp;
            // SpawnedFloatSimulation = FishingFloat.gameObject
            SpawnFloatSimulation();
        }
    }



    [PunRPC]
    private void CmdDestroyFloat()
    {
        if (FishingFloat != null)
        {
            // _fishingFloat.Destroy(connectionToClient);
            PhotonNetwork.Destroy(FishingFloat.gameObject);
            //TODO: DESTROY
            DestroyFloatSimulation();
        }
    }

    [PunRPC]
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

        SpawnedFloatSimulation = Instantiate(FloatSimulation).GetComponent<FloatSimulation>();
        // SpawnedFloatSimulation = PhotonNetwork.Instantiate(FloatSimulation.name, Vector3.zero, Quaternion.identity);

    }

    public void DestroyFloatSimulation()
    {
        if (!photonView.IsMine)
            return;

        Destroy(SpawnedFloatSimulation);
    }
}

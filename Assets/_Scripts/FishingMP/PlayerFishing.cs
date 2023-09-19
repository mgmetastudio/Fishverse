using UnityEngine;
// using Mirror;
using System.Collections.Generic;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using NullSave.TOCK.Inventory;
using System.Collections;

public class PlayerFishing : MonoBehaviourPun
{
    [SerializeField] Transform rodHolder;
    [SerializeField] bool canFish;
    [SerializeField] bool IsmissingBait;
    [SerializeField] bool IsmissingFloat;
    [SerializeField] bool Isfishingfloat;
    public GameObject FishingNotif;

    [SerializeField] private GameObject _fishingFloatBasePrefab;
    // [SyncVar]
    [HideInInspector] private FishingFloat fishingFloat;

    [Header("Upgrade Fishing Rod Texts ")]
    public TMP_Text UpgradeFishingRodText;

    [Header("Hidden Buttons In Fishing")]
    [SerializeField] public List <GameObject> HiddenButtons ;
    [SerializeField] public RectTransform btnholster;
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
    [SerializeField] public string crankUpAnimationNameRod = "FishingRod_Up";
    [SerializeField] public string CastAnimationName = "Fishing_In";
    [SerializeField] public GameObject FloatSimulation;
    public FloatSimulation SpawnedFloatSimulation;
    [SerializeField] float onCastWait;

    [SerializeField] GameObject fishingRod;
    [SerializeField] GameObject fishingRope;

    [Space]
    [SerializeField] Button holsterBtn;
    [SerializeField] Slider forceSlider;
    [SerializeField] Button RotateReel;
    [SerializeField] Button Btn_FishCast;
    [SerializeField] private EquipPoint _equipPoint;
    [SerializeField] private Animator _animatorFishingRodAnim;
    [SerializeField] PlayerUI PlayerUI;
    private bool hasInvoked = false;
    public bool isreelrotate= false;
    public bool isfishing=false;
    public bool isDestroyFloat=false;
    public GameObject Linebroke;
    private bool isLinebroke=false;
    // public UnityEvent onCast;
    PlayerFishingInventory _inv;
    Animator _anim;
    private Animator _animReel;
    private Animator _Btn_FishCast;
    public Animator _Linebroke ;
    public CameraController CameraController_;
    Transform _localCamera;

    CastRodUI rodUI;
    bool onRodDown;
    bool onRodUp;
    bool onRod;
    InputProxy _inputProxy;
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
             FishingNotif.SetActive(false);
            _floatDemo = Instantiate(_floatDemoPrefab);
            _localCamera = Camera.main.transform;
            rodUI = FindObjectOfType<CastRodUI>();
            Btn_FishCast.onClick.AddListener(FishingCast);
            rodUI.btn.onDown.AddListener(OnRodDown);
            rodUI.btn.onUp.AddListener(OnRodUp);
            forceSlider.SetInactive();
            RotateReel.SetInactive();
            _inputProxy = GetComponent<InputProxy>();
            _inv = GetComponent<PlayerFishingInventory>();
            _anim = GetComponent<Animator>();
            _equipPoint = GetComponentInChildren<EquipPoint>();
            _animReel = RotateReel.GetComponent<Animator>();
            _Btn_FishCast= Btn_FishCast.GetComponent<Animator>();
            _Linebroke= Linebroke.GetComponent<Animator>();
        }
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
    public void Holster()
    {
        if (!canFish) return;

        if (photonView.IsMine)
        {
            if (FishingFloat == null)
            {
                if (rodHolder.childCount > 1)
                {
                    // if (rodHolder.childCount > 1)
                    photonView.RPC("DrawFishingRod", RpcTarget.All, !fishingRod.activeSelf);
                    isLinebroke = false;
                }
            }
            else
            {
                photonView.RPC("CmdDestroyFloat", RpcTarget.All);
            }
        }
    }

    public void OnItemEquip(InventoryItem equipedItem)
    {
        if (equipedItem.subtext == "Fishing Rod" || equipedItem.subtext == "Fishing Rod Nft")
        {
            photonView.RPC("DrawFishingRod", RpcTarget.All, true);
            // fishingRod.SetActive();
        }
    }

    public void OnItemUnequip(InventoryItem unequipedItem)
    {
        if (unequipedItem.subtext == "Fishing Rod" || unequipedItem.subtext == "Fishing Rod Nft")
            photonView.RPC("DrawFishingRod", RpcTarget.All, false);

    }

    private void Update()
    {
        if (!canFish) return;

        if (photonView.IsMine)
        {
            //if(_inv.)
            if (FishingFloat == null)
            {
                NoFishingFloatLogic();
                RotateReel.SetActive(false);
            }
            else
            {
                FishingFloatLogic();
            }
            if (isLinebroke && canFish && fishingRod.activeSelf && FishingFloat.fish!=null)
            {
                if (!FishingFloat.fish.HookedTo)
                {
                    _Linebroke.SetBool("Linebroke_End", true);
                }
                else
                {
                    _Linebroke.SetBool("Linebroke_End", false);
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
    private void FindFishingRodAnimator()
    {
        // Find the child Animator component dynamically
        _animatorFishingRodAnim = _equipPoint.GetComponentInChildren<Animator>();
    }

    void FishingFloatLogic()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        _CrankDownInput();
        _CrankUpInput();
#endif

        ToggleZoom_Buttons_In();
        RotateReel.SetActive(fishingRod.activeSelf);
        Btn_FishCast.SetActive(false);
       _Btn_FishCast.Play("Default_FishCast");
        _Linebroke.SetBool("Linebroke_End", false);
        if (FishingFloat != null)
        {
            if (FishingFloat.fish)
            {
               // FishCamerafollow= FishingFloat.fish.transform.position;
                //  Debug.Log("FishCamerafollow is "+FishCamerafollow);
                if (FishingFloat.fish.controller.HealthBar == 0)
                {
                    _Linebroke.Play("Default_FishCast");
                    forceSlider.SetActive(false);
                    isfishing = true;
                    _animReel.SetBool("ReelIn_Reel", false);
                    RotateReel.SetActive(false);
                    Btn_FishCast.SetActive(true);
                    _Btn_FishCast.Play("Default_FishCast");
                }
                else
                {
                    forceSlider.SetActive(true);
                }

            }
        }
        if (forceSlider.gameObject.activeSelf)
        {
            isDestroyFloat = false;
            _animReel.SetBool("Hook_Reel", true);
            float speedFactor = 0.1f;
            forceSlider.value = 1f - (FishingFloat.fish.controller.StaminaBar);
        }
        else
        {
          _animReel.SetBool("Hook_Reel", false);
        }
        _floatDemo.SetActive(false);

        if (isfishing)
        {
            isreelrotate = true;
            FishingFloat.photonView.RPC("Pull", RpcTarget.All);
            _anim.SetFloat("Fishing_Up_Speed", 1);
            _anim.Play(crankUpAnimationName);
            if (_equipPoint != null)
            {
                FindFishingRodAnimator();
                if (_animatorFishingRodAnim != null && _anim.GetBool("Fish_In_Hand") == false)
                {
                    _animatorFishingRodAnim.speed = 1f;
                    _animatorFishingRodAnim.SetFloat("FishingRod_Up_Speed", 1);
                    _animatorFishingRodAnim.Play(crankUpAnimationNameRod);
                }
            }
        }

        if (!isfishing )
        {
            _anim.SetFloat("Fishing_Up_Speed", 0);

            if (_equipPoint != null)
            {
                FindFishingRodAnimator();
                if (_animatorFishingRodAnim != null && _anim.GetFloat("Fishing_Up_Speed") == 0)
                {
                    _animatorFishingRodAnim.speed = -1f;
                }
            }
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(1))
        {
            // CmdDestroyFloat();
            photonView.RPC("CmdDestroyFloat", RpcTarget.All);
            _anim.SetFloat("Fishing_Up_Speed", 0);
            _anim.Play("Fishing_RightArm_Idle");
            if (_equipPoint != null)
            {
                FindFishingRodAnimator();
                if (_animatorFishingRodAnim != null && _anim.GetFloat("Fishing_Up_Speed") == 0)
                {
                    _animatorFishingRodAnim.SetFloat("FishingRod_Up_Speed", 0);
                    _animatorFishingRodAnim.Play("IdleState");
                }
            }
        }
#endif
        DestroyFloatmaxdistance();
    }

    void NoFishingFloatLogic()
    {
        if (fishingRod.activeSelf && Physics.Raycast(_localCamera.position, _localCamera.forward, out RaycastHit hitInfo, _maxLineThrowDistance, _fluidMask) )
        {
            if (!Physics.Raycast(_localCamera.position, _localCamera.forward, Vector3.Distance(_localCamera.position, hitInfo.point) + .01f, _obstacleMask))
            {
                _floatDemo.SetActive(true);
                _floatDemo.transform.position = hitInfo.point;
                _Btn_FishCast.GetComponent<Animator>().Play("FishCast_button");

            }
            else
            {
                _floatDemo.SetActive(false);
                _Btn_FishCast.GetComponent<Animator>().Play("Default_FishCast");
            }
        }
        else
        {
            _floatDemo.SetActive(false);
            _Btn_FishCast.GetComponent<Animator>().Play("Default_FishCast");
        }

        if (fishingRod.activeSelf && CastInput())
        {
            Cast();
            isLinebroke = false;
        }
        Btn_FishCast.SetActive(true);
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (rodHolder.childCount > 1)
            {
                photonView.RPC("DrawFishingRod", RpcTarget.All, !fishingRod.activeSelf);
                isLinebroke = false;
            }

            // DrawFishingRod(!fishingRod.activeSelf);
        }
        isreelrotate = false;
        forceSlider.SetActive(false);
       // Invoke("ToggleZoom_Buttons_Out", 2f);
        ToggleZoom_Buttons_Out();
        UpgradeFishingRodText.SetActive(false);

    }

    public void DestroyFloatmaxdistance()
    {
        if (Vector3.Distance(_rodEndPoint.position, FishingFloat.transform.position) > _maxLineDistance || fishingFloat.Destroyfloat)
        {
            photonView.RPC("CmdDestroyFloat", RpcTarget.All);
            // CmdDestroyFloat();
            _anim.SetFloat("Fishing_Up_Speed", 0);
            _anim.Play("Fishing_RightArm_Idle");
            if (_equipPoint != null)
            {
                FindFishingRodAnimator();
                if (_animatorFishingRodAnim != null && _anim.GetFloat("Fishing_Up_Speed") == 0)
                {
                    _animatorFishingRodAnim.SetFloat("FishingRod_Up_Speed", 0);
                    _animatorFishingRodAnim.Play("IdleState");
                }
            }
        }
    }

    void FishingCast()
    {
        if(fishingRod.activeSelf)
        {
            Cast();
            isLinebroke = false;
        }
    }

    async Task Cast()
    {
        if (_floatDemo.activeSelf)
        {
            CheckEquipment();
            if (!PlayerUI.Isfishingfull && Isfishingfloat)
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
                isfishing = false;

            }
            else
            {
                _Linebroke.SetBool("PackBack_Full", true);
                if (!hasInvoked)
                {
                    StartCoroutine(InvokeStopPackBackAnim());
                }
            }
        }

    }
    private IEnumerator InvokeStopPackBackAnim()
    {
        hasInvoked = true;

        _Linebroke.SetBool("PackBack_Full", false);

        yield return new WaitForSeconds(1f);

        hasInvoked = false; // Reset the flag
    }
    bool CastInput()
    {
        if (_inputProxy.mobileInput) return onRodDown;
        return Input.GetMouseButtonDown(0);
    }

    bool CrankUpInput()
    {
        if (_inputProxy.mobileInput) return onRod;
        return Input.GetButton("CrankUp") ;
    }

    bool CrankDownInput()
    {
        if (_inputProxy.mobileInput) return onRodUp;
        return Input.GetKeyUp(KeyCode.Mouse0) ;
    }
    void _CrankDownInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            isfishing = true;
            if (FishingFloat.fish == null)
            { _animReel.SetBool("Rotate_Reel", true); }
            else
            {
                _animReel.SetBool("ReelIn_Reel", true);
            }
        }
    }
    void _CrankUpInput()
    {
        if (Input.GetButtonUp("Fire1"))
        {
            isfishing = false;
            _animReel.SetBool("Rotate_Reel", false);
            _animReel.SetBool("ReelIn_Reel", false);
        }
    }
    public void CrankUpInputMobile()
    {
        isfishing = false;
        _animReel.SetBool("Rotate_Reel", false);
        _animReel.SetBool("ReelIn_Reel", false);

    }

    public void CrankDownInputMobile()
    {
        isfishing = true;
        if (FishingFloat.fish == null)
        { _animReel.SetBool("Rotate_Reel", true); }
        else
        {
            _animReel.SetBool("ReelIn_Reel", true);
        }
    }

    [PunRPC]
    public void DrawFishingRod(bool draw)
    {
        //change to holder
        rodHolder.SetActive(draw);
        fishingRod.SetActive(draw);
        fishingRope.SetActive(draw);
        if (photonView.IsMine)
        {
         var inv = GetComponent<PlayerFishingInventory>();
        if (draw)
            inv.SetFloat(0);
        else
            Destroy(inv.LineEnd);
        
        _anim.SetLayerWeight(2, draw.Int());
        }

    }


    public void OnSwimStart()
    {
        if (!photonView.IsMine) return;
        if (fishingRod.activeSelf)
        {
            photonView.RPC("DrawFishingRod", RpcTarget.All, false);
        }
        _floatDemo.SetActive(false);
        canFish = false;
        _animReel.SetBool("Hook_Reel", false);
        _Btn_FishCast.Play("Default_FishCast");
        forceSlider.SetActive(false);
        ToggleZoom_Buttons_Out();
    }

    public void OnSwimEnd()
    {
        if (!photonView.IsMine) return;
        canFish = true;
        if (rodHolder.childCount > 1)
        {
            photonView.RPC("DrawFishingRod", RpcTarget.All, true);
        }
    }

    public void ToggleCantFish(bool value)
    {
        canFish = !value;
    }

    // [Command(requiresAuthority = true)]
    [PunRPC]
    private void CmdSpawnFloat(Vector3 position, int uniqueId)
    {
        if (!photonView.IsMine)
            return;

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
            if (_inv.LineEnd != null)
            {
                for (int i = 0; i < fishingFloatObj.GetComponent<BaitActivator>().Baits.Length; i++)
                {
                    fishingFloatObj.GetComponent<BaitActivator>().Baits[i].Bait.SetActive(false);
                    if (_inv.LineEnd.GetComponent<BaitActivator>().Baits[i].Bait.activeSelf)
                    {
                        fishingFloatObj.GetComponent<BaitActivator>().Baits[i].Bait.SetActive(true);
                    }

                }
            }
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
        if (!photonView.IsMine)
            return;

        if (FishingFloat != null)
        {
            ToggleZoom_Buttons_Out();
            UpgradeFishingRodText.SetActive(false);
            isDestroyFloat = true;
            isLinebroke = true;
            // _fishingFloat.Destroy(connectionToClient);
            forceSlider.SetActive(false);
            PhotonNetwork.Destroy(FishingFloat.gameObject);
            //TODO: DESTROY
            DestroyFloatSimulation();

        }
    }

    [PunRPC]
    public void RpcDisableHoldingFish()
    {
        if (photonView.IsMine)
        {

            foreach (GameObject AllFish in _inv.Fishes)
            {
                AllFish.SetActive(false);
            }
            _inv.FishHolder.SetActive(false);
        }

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

        if (SpawnedFloatSimulation.gameObject)
            Destroy(SpawnedFloatSimulation.gameObject);
    }

    // Zoom Camera and Hide Buttons in Fishing
    private void ToggleZoom_Buttons_In()
    {
        if (photonView.IsMine)
        {
            CameraController_.ThridPersonfishingToggleView();
            _inputProxy.CameraRotateUI.SetInactive();
            if (fishingFloat != null && FishingFloat.fish != null && FishingFloat.fish.controller.HealthBar != 0)
            {
                transform.LookAtY(fishingFloat.transform);
            }
            if (fishingFloat != null && FishingFloat.fish != null && FishingFloat.fish.controller.HealthBar == 0)
            {
                _inputProxy.Buttonholster.interactable = false;
            }
            HideControllerButtons(false, 86.875f);
        }
    }
    private void ToggleZoom_Buttons_Out()
    {
        if (photonView.IsMine)
        {
            CameraController_.ThridPersonToggleView();
            HideControllerButtons(true, 148.8f);
            _inputProxy.CameraRotateUI.SetActive();
            _inputProxy.Buttonholster.interactable = true;

        }
    }

    private void CheckEquipment()
    {
        if(_inv.currentBait != null)
        {
            IsmissingBait = false;
        }
        else
        {
            IsmissingBait = true;
        }

        if (_inv.currentFloat != null)
        {
            IsmissingFloat = false;
        }
        else
        {
            IsmissingFloat = true;
        }

        //Fishing Logic

        if ((!IsmissingBait && !IsmissingFloat && _inv.currentFloat.previewScale <= 1) || (!IsmissingFloat && _inv.currentFloat.previewScale == 2))
        {
            Isfishingfloat = true;
        }
        else
        {
            if(FishingNotif.activeSelf)
            {
                FishingNotif.SetActive(false);
            }
            Isfishingfloat = false;
            FishingNotif.SetActive(true);
            Invoke("HideFishingNotif", 1f);
        }

    }
    private void HideFishingNotif()
    {
        FishingNotif.SetActive(false);
    }
    public void HideControllerButtons(bool isactive, float t)
    {
        foreach (GameObject btn in HiddenButtons)
        {
            btn.SetActive(isactive);
        }
        Vector2 newPosition = new(btnholster.anchoredPosition.x, t);
        btnholster.anchoredPosition = newPosition;
    }

}

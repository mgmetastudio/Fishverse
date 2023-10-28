using System.Collections;
using System.Collections.Generic;
using EasyCharacterMovement.Examples.Cinemachine.FirstPersonExample;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using EasyCharacterMovement;
using DG.Tweening;
using NullSave.GDTK.Stats;
using TMPro;

public class ControlSwitch : MonoBehaviour
{
    [Header("Required Level")]
    public int requiredLevel;
    [Header("Panel For Required Level")]
    public GameObject PanelrequiredLevel;
    public TMP_Text LevelRequirementText;
    public bool CanDrive=false;
    [SerializeField] LayerMask playerMask;
    [SerializeField] LayerMask GroundMask;
    [SerializeField] public MonoBehaviour controllerToToggle;
    [SerializeField] ArcadeVehicleController_Network ArcadeVehicleController_;
    [SerializeField] PhotonView boatView;
    [SerializeField] Cinemachine.CinemachineVirtualCamera cam;
    [SerializeField] Transform playerSitPos;

    [SerializeField] Button promptBtn;
    [SerializeField] TMPro.TMP_Text promptText;

    [SerializeField] string enterText = "Enter Boat";
    [SerializeField] string exitText = "Exit Boat";

    [SerializeField] KeyCode inputKey = KeyCode.F;
    [SerializeField] public BoatInputProxy BoatInputProxy;
    [SerializeField] public GameObject Joystick;
    CMFirstPersonCharacter _player;
    private bool isTouchingSand = false;
    private bool StartDocksCollider = false;
    [SerializeField] public Collider DocksCollider;
    [SerializeField] public Collider Boat;
    [SerializeField] public GameObject Fisher;
    public Animator FadeAnim;
    private PlayerCharacterStats PlayerCharacterStats;




    void Start()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        enterText = "Press 'F' to drive the boat";
        exitText = "Press 'F' to exit the boat";
#else
        enterText = "Enter Boat";
        exitText = "Exit Boat";
#endif
        PlayerCharacterStats = FindObjectOfType<PlayerCharacterStats>();
        promptText.SetText(enterText);
        promptBtn.SetInactive();
        promptBtn.onClick.AddListener(OnButton);
        if (BoatInputProxy.mobileInput)
        {
            Joystick.SetActive(false);
        }
        if (!boatView.IsMine) enabled = false;
        Fisher.gameObject.SetActive(false);
        FadeAnim.gameObject.SetActive(false);
    }

    void OnButton()
    {
        if (!_player) return;

        ToggleController();
    }

    void Update()
    {
        if (!_player) return;
        if (CanDrive)
        {
            if (Input.GetKeyDown(inputKey))
                ToggleController();
            if (ArcadeVehicleController_ != null && controllerToToggle != null)
            {
                if (isTouchingSand && controllerToToggle.enabled)
                {
                    promptBtn.SetActive();
                }
                else if (!isTouchingSand && controllerToToggle.enabled)
                {
                    promptBtn.SetInactive();

                }
            }
            if (isTouchingSand && _player == null)
            {
                promptBtn.SetInactive();
            }
        }
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (!boatView.IsMine) return;
        if (!playerMask.Includes(other.gameObject.layer)) return;
        CheckPlayerLevel(PlayerCharacterStats.GetCharacterLevel());
        if (CanDrive)
        {
            if (!controllerToToggle.enabled)
            {
                promptBtn.SetActive();
            }

            _player = other.GetComponent<CMFirstPersonCharacter>();
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (!boatView.IsMine) return;
        if (controllerToToggle.enabled)
        {
            if (other.CompareTag("Ground"))
            {
                Debug.Log("Is Touching Ground");
                isTouchingSand = true;
            }
        }
        if (!playerMask.Includes(other.gameObject.layer)) return;
        CheckPlayerLevel(PlayerCharacterStats.GetCharacterLevel());
        if (CanDrive)
        {

            if (StartDocksCollider)
            {
                DocksCollider.enabled = false;
            }
          
            if (!controllerToToggle.enabled)
            {
                promptBtn.SetActive();
            }
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (!boatView.IsMine) return;
        if (controllerToToggle.enabled)
        {
            if (other.CompareTag("Ground"))
            {
                Debug.Log("Is Not Touching Ground");
                isTouchingSand = false;
            }
        }
        if (_player != null)
        {
            if (other.gameObject != _player.gameObject) return;
        }

        promptBtn.SetInactive();
        _player = null;
        if(!CanDrive)
        {
            PanelrequiredLevel.SetInactive();
        }

    }

    void ToggleController()
    {
        if (controllerToToggle.enabled && isTouchingSand)
        {
            TogglePlayerController();
            FadeAnim.SetTrigger("FadeIn");
        }
        else if(!controllerToToggle.enabled)
        {
            ToggleBoatController();
            FadeAnim.SetTrigger("FadeIn");
        }

    }

    void ToggleBoatController()
    {
        if (_player == null)
        {
            return;
        }
        FadeAnim.gameObject.SetActive(true);
        Fisher.gameObject.SetActive(true);
        StartDocksCollider = true;
        _player.StopSprinting();
        _player.SetMovementDirection(Vector3.zero);
        _player.handleInput = false;
        controllerToToggle.enabled = true;
        _player.transform.Find("GeometryThirdPerson").SetActive(false);
        _player.transform.SetParent(playerSitPos, true);
        _player.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        _player.transform.Find("Generated Foots Origin").SetActive(false);

        cam.Priority = 100;
        if (BoatInputProxy.mobileInput)
        {
            Joystick.SetActive(true);
        }
        SetPromptText(exitText);

    }

    void TogglePlayerController()
    {
        if (_player == null)
        {
            return;
        }
        FadeAnim.gameObject.SetActive(true);
        Fisher.gameObject.SetActive(false);
        _player.transform.Find("GeometryThirdPerson").SetActive(true);
        _player.transform.Find("Generated Foots Origin").SetActive(true);
        _player.handleInput = true;
        controllerToToggle.enabled = false;

        _player.transform.parent = null;

        cam.Priority = -1;
        if (BoatInputProxy.mobileInput)
        {
            Joystick.SetActive(false);
        }
        SetPromptText(enterText);
    }

    void SetPromptText(string text)
    {
        if (promptText)
        {
            promptText.text = text;
        }
    }
    public void CheckPlayerLevel(int playerLevel)
    {
        if (playerLevel >= requiredLevel)
        {
            CanDrive = true;
            PanelrequiredLevel.SetInactive();
        }
        else
        {
            CanDrive = false;
            LevelRequirementText.text = "YOU CAN'T DRIVE THE BOAT. <br> LEVEL " + requiredLevel + " REQUIRED.";
            PanelrequiredLevel.SetActive();
        }
    }
}


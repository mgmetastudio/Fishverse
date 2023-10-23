using System.Collections;
using System.Collections.Generic;
using EasyCharacterMovement.Examples.Cinemachine.FirstPersonExample;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using EasyCharacterMovement;

public class ControlSwitch : MonoBehaviour
{
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
    [SerializeField] public GameObject Joystick;
    CMFirstPersonCharacter _player;
    private bool isTouchingSand = false;

    void Start()
    {
        promptText.SetText(enterText);
        promptBtn.SetInactive();
        promptBtn.onClick.AddListener(OnButton);
        Joystick.SetActive(false);
        if (!boatView.IsMine) enabled = false;
    }

    void OnButton()
    {
        if (!_player) return;

        ToggleController();
    }

    void Update()
    {
        if (!_player) return;

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

    void OnTriggerEnter(Collider other)
    {
        if(!boatView.IsMine) return;
        if (!playerMask.Includes(other.gameObject.layer)) return;

        if (!controllerToToggle.enabled)
        {
            promptBtn.SetActive();
        }

        _player = other.GetComponent<CMFirstPersonCharacter>();
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

        if (!controllerToToggle.enabled)
        {
            promptBtn.SetActive();
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        if(!boatView.IsMine) return;
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
    }

    void ToggleController()
    {
        if (controllerToToggle.enabled && isTouchingSand)
        { 
            TogglePlayerController(); 
        }
        else if(!controllerToToggle.enabled)
        { 
            ToggleBoatController(); 
        }

    }

    void ToggleBoatController()
    {
        if (_player == null)
        {
            return;
        }
        _player.StopSprinting();
        _player.SetMovementDirection(Vector3.zero);
        _player.handleInput = false;
        controllerToToggle.enabled = true;

        _player.transform.SetParent(playerSitPos, true);
        _player.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        cam.Priority = 100;
        Joystick.SetActive(true);
        SetPromptText(exitText);
    }

    void TogglePlayerController()
    {
        if (_player == null)
        {
            return;
        }
       // _player.SetMovementMode(MovementMode.Walking);
        _player.handleInput = true;
        controllerToToggle.enabled = false;

        _player.transform.parent = null;

        cam.Priority = -1;
        Joystick.SetActive(false);
        SetPromptText(enterText);
    }

    void SetPromptText(string text)
    {
        if (promptText)
        {
            promptText.text = text;
        }
    }
}

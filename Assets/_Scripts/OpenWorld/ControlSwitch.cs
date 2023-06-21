using System.Collections;
using System.Collections.Generic;
using EasyCharacterMovement.Examples.Cinemachine.FirstPersonExample;
using UnityEngine;
using UnityEngine.UI;

public class ControlSwitch : MonoBehaviour
{
    [SerializeField] LayerMask playerMask;
    [SerializeField] MonoBehaviour controllerToToggle;

    [SerializeField] Cinemachine.CinemachineVirtualCamera cam;
    [SerializeField] Transform playerSitPos;

    [SerializeField] Button promptBtn;
    [SerializeField] TMPro.TMP_Text promptText;

    [SerializeField] string enterText = "Enter Boat";
    [SerializeField] string exitText = "Exit Boat";

    CMFirstPersonCharacter _player;

    void Start()
    {
        promptText.SetText(enterText);
        promptBtn.SetInactive();
        promptBtn.onClick.AddListener(OnButton);
    }

    void OnButton()
    {
        if (!_player) return;

        ToggleController();
    }

    void Update()
    {
        if (!_player) return;

        if (Input.GetKeyDown(KeyCode.E))
            ToggleController();

    }

    void OnTriggerEnter(Collider other)
    {
        if (!playerMask.Includes(other.gameObject.layer)) return;

        promptBtn.SetActive();

        _player = other.GetComponent<CMFirstPersonCharacter>();
        print("On Enter");
    }

    void OnTriggerExit(Collider other)
    {
        // if (!playerMask.Includes(other.gameObject.layer)) return;
        if(other.gameObject != _player.gameObject) return;

        promptBtn.SetInactive();


        print("On Exit");
        _player = null;
    }

    void ToggleController()
    {
        if (controllerToToggle.enabled)
            TogglePlayerController();
        else
            ToggleThisController();
    }

    void ToggleThisController()
    {
        _player.handleInput = false;
        controllerToToggle.enabled = true;

        _player.transform.SetParent(playerSitPos, true);
        _player.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        cam.Priority = 100;

        promptText.SetText(exitText);
    }

    void TogglePlayerController()
    {
        _player.handleInput = true;
        controllerToToggle.enabled = false;

        _player.transform.parent = null;

        cam.Priority = -1;

        promptText.SetText(enterText);
    }
}

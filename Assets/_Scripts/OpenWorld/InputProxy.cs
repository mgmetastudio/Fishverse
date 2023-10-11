using System.Collections;
using System.Collections.Generic;
using EasyCharacterMovement.Examples.Cinemachine.FirstPersonExample;
using UnityEngine;
using NullSave.TOCK.Inventory;
using UnityEngine.UI;

public class InputProxy : MonoBehaviour
{
    [SerializeField] Joystick moveJoystick;
    [SerializeField] Joystick lookJoystick;

    [Space]
    [SerializeField] ButtonXL ButtonRun;
    [SerializeField] ButtonXL ButtonJump;
    [SerializeField] public ButtonXL Buttonholster;
    [SerializeField] Button ButtonInventory;


    [Space]
    [SerializeField] Cinemachine.CinemachineVirtualCamera fpCam;
    [SerializeField] Cinemachine.CinemachineFreeLook tpCam;
    [SerializeField] PlayerFishing Playerfishing;
    [SerializeField] InventoryCog PlayerInventory;

    [Space]
    [SerializeField] CMFirstPersonCharacter character;

    [Space]
    public bool mobileInput;

    [Space]
    [SerializeField] public GameObject CameraRotateUI;
    [SerializeField] float smoothing = 5f;

    void Start()
    {
#if UNITY_EDITOR
#else
        mobileInput = Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
#endif

        if (mobileInput)
        {
            character.customInput = true;

            var pov = fpCam.GetPOV();
            pov.m_HorizontalAxis.m_InputAxisName = "";
            pov.m_VerticalAxis.m_InputAxisName = "";
            tpCam.m_XAxis.m_InputAxisName = "";
            tpCam.m_YAxis.m_InputAxisName = "";
            ButtonRun.onDown.AddListener(character.Sprint);
            ButtonRun.onUp.AddListener(character.StopSprinting);
            ButtonJump.onDown.AddListener(character.Jump);
            ButtonJump.onUp.AddListener(character.StopJumping);
            Buttonholster.onClick.AddListener(Playerfishing.Holster);
            ButtonInventory.onClick.AddListener(PlayerInventory.MenuOpen);

            // .m_HorizontalAxis.m_InputAxisName = "";
            // tpCam.GetRig(0).GetCinemachineComponent<Cinemachine.CinemachinePOV>().m_VerticalAxis.m_InputAxisName = "";

        }
        else
        {
            CameraRotateUI.SetInactive();
            enabled = false;
            ButtonRun.onDown.AddListener(character.Sprint);
            ButtonRun.onUp.AddListener(character.StopSprinting);
            ButtonJump.onDown.AddListener(character.Jump);
            ButtonJump.onUp.AddListener(character.StopJumping);
            Buttonholster.onClick.AddListener(Playerfishing.Holster);
            ButtonInventory.onClick.AddListener(PlayerInventory.MenuOpen);
        }
    }

    Vector2 lookMulti = new Vector2(2f, 1f);
    void Update()
    {
        character.MovementInput = moveJoystick.Direction;

        character.LookInput = lookJoystick.Direction * lookMulti;

    }
}

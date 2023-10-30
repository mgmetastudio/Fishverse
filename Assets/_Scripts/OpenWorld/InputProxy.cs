using System.Collections;
using System.Collections.Generic;
using EasyCharacterMovement.Examples.Cinemachine.FirstPersonExample;
using UnityEngine;
using NullSave.TOCK.Inventory;
using UnityEngine.UI;
using Photon.Pun;
using Cinemachine;

public class InputProxy : MonoBehaviour
{

    [Space]
    [SerializeField] Cinemachine.CinemachineVirtualCamera fpCam;
    [SerializeField] Cinemachine.CinemachineFreeLook tpCam;
    [SerializeField] PlayerFishing Playerfishing;
    [SerializeField] InventoryCog PlayerInventory;
    [SerializeField] public PhotonView photon_view;
    [Space]
    [SerializeField] CMFirstPersonCharacter character;

    [Space]
    [SerializeField] public GameObject CameraRotateUI;
    [SerializeField] float smoothing = 5f;

    [Space]
    public bool mobileInput;

    [Header("Buttons [Android/IOS]")]

    [Header("Joystick Move")]
    [SerializeField] Joystick moveJoystick;
    [Header("Joystick Rotate")]
    [SerializeField] Joystick lookJoystick;

    [Space]
    [Header("Button Run")]
    [SerializeField] ButtonXL ButtonRun;
    [Header("Button Jump")]
    [SerializeField] ButtonXL ButtonJump;
    [Header("Button Holster")]
    [SerializeField] public ButtonXL Buttonholster;
    [Header("Button Inventory")]
    [SerializeField] Button ButtonInventory;

    [Header("Other [Android/IOS] Buttons To Hide For [Standalone]")]
    [SerializeField] List<GameObject> Buttons;

    [Space]
    [Header("Input Keys [Standalone]")]
    [Header("Input Key / Pause Menu")]
    #region Variables

    public KeyCode Pausekey = KeyCode.KeypadEnter;

    #endregion
    public GameObject PauseMenuPanel;

    [Header("Input Key / Full Screen Map")]
    #region Variables

    public KeyCode Mapkey = KeyCode.KeypadEnter;

    #endregion
    public PlayerUI FullScreenMap;

    [Header("Input Key / Toggle Fishing Rod")]
    #region Variables

    public string ToggleFishingRod ;

    #endregion
    private bool IsOpenPauseMenu=false;
    void Start()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        mobileInput = false;
#else
        mobileInput = true;
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
            HideButtons();
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
        if (photon_view.IsMine)
        {
            if (Input.GetKey(Pausekey))
            {
                {
                    OpenPauseMenu();
                }
            }
            if (Input.GetKey(Mapkey) || Input.GetKey(KeyCode.Semicolon))
            {
                if (FullScreenMap != null)
                {
                    FullScreenMap.OpenFullscreenMap();
                }
            }
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        if (tpCam != null)
        {
            if (IsOpenPauseMenu || PlayerInventory.IsMenuOpen )
            {
                // Disable mouse input during pause
                tpCam.m_XAxis.m_InputAxisName = "";
                tpCam.m_YAxis.m_InputAxisName = "";
                character.enabled = false;
            }
            else
            {
                // Enable mouse input when not paused
                tpCam.m_XAxis.m_InputAxisName = "Mouse X";
                tpCam.m_YAxis.m_InputAxisName = "Mouse Y";
                character.enabled = true;
            }

        }
#endif

    }

    void HideButtons()
    {
        moveJoystick.SetInactive();
        lookJoystick.SetInactive();
        foreach (GameObject button in Buttons)
        {
            button.SetInactive();
        }
    }
    public void OpenPauseMenu()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
#endif
        PauseMenuPanel.SetActive();
        IsOpenPauseMenu = true;
    }
    public void ClosePauseMenu()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
#endif
        PauseMenuPanel.SetInactive();
        IsOpenPauseMenu = false;
    }
}

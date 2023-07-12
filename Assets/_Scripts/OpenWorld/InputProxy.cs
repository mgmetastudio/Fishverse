using System.Collections;
using System.Collections.Generic;
using EasyCharacterMovement.Examples.Cinemachine.FirstPersonExample;
using UnityEngine;

public class InputProxy : MonoBehaviour
{
    [SerializeField] Joystick moveJoystick;
    [SerializeField] Joystick lookJoystick;

    [Space]
    [SerializeField] Cinemachine.CinemachineVirtualCamera fpCam;
    [SerializeField] Cinemachine.CinemachineFreeLook tpCam;

    [Space]
    [SerializeField] CMFirstPersonCharacter character;

    [Space]
    public bool mobileInput;

    [Space]
    [SerializeField] List<GameObject> mobileUI;

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
            // .m_HorizontalAxis.m_InputAxisName = "";
            // tpCam.GetRig(0).GetCinemachineComponent<Cinemachine.CinemachinePOV>().m_VerticalAxis.m_InputAxisName = "";

        }
        else
        {
            mobileUI[0].SetInactive();
            enabled = false;
        }
    }

    Vector2 lookMulti = new Vector2(2f, 1f);
    void Update()
    {
        character.MovementInput = moveJoystick.Direction;
        character.LookInput = lookJoystick.Direction * lookMulti;
    }
}

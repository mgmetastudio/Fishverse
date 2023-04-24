using System.Collections;
using System.Collections.Generic;
using EasyCharacterMovement.Examples.Cinemachine.FirstPersonExample;
using UnityEngine;

public class InputProxy : MonoBehaviour
{
    [SerializeField] Joystick moveJoystick;
    [SerializeField] Joystick lookJoystick;

    [Space]
    [SerializeField] CMFirstPersonCharacter character;

    void Start()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            character.customInput = true;
    }

    void Update()
    {
        character.MovementInput = moveJoystick.Direction;
        character.LookInput = lookJoystick.Direction;
    }
}

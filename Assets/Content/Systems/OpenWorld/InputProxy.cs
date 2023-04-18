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
#if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
        character.customInput = true;
#endif
    }

    void Update()
    {
        character.MovementInput = moveJoystick.Direction;
        character.LookInput = lookJoystick.Direction;
    }
}

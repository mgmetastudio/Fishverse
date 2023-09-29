using System.Collections;
using EasyCharacterMovement.Examples.Cinemachine.FirstPersonExample;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventBridge : MonoBehaviour
{
    public FootstepSound footstepSound; // Reference to the FootstepSound script on the target GameObject.
    [SerializeField] CMFirstPersonCharacter character;
    private bool isrunning;
    public void Walk()
    {
        if(!character.IsSprinting() && !isrunning)
        {
         CallPlayFootstepSound();
        }
    }

    public void Run()
    {
        if (character.IsSprinting() && isrunning)
        {
            CallPlayFootstepSound();
        }
    }
    public void Update()
    {
        if(character.IsSprinting())
        {
            isrunning = true;
        }
        else
        {
            isrunning = false;
        }
    }
    public void CallPlayFootstepSound()
    {
        if (footstepSound != null)
        {
            footstepSound.PlayFootstepSound();
        }
    }
}

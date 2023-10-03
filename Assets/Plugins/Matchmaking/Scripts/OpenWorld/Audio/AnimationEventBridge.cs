using System.Collections;
using EasyCharacterMovement.Examples.Cinemachine.FirstPersonExample;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AnimationEventBridge : MonoBehaviourPun
{
    public FootstepSound footstepSound; // Reference to the FootstepSound script on the target GameObject.
    [SerializeField] CMFirstPersonCharacter character;
    private bool isrunning;
    
    public void Walk()
    {
        if(!character.IsSprinting() && !isrunning && photonView.IsMine)
        {
         CallPlayFootstepSound();
        }
    }

    public void Run()
    {
        if (character.IsSprinting() && isrunning && photonView.IsMine)
        {
            CallPlayFootstepSound();
        }
    }
    public void Update()
    {
        if(character.IsSprinting() && photonView.IsMine)
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
        if (footstepSound != null && photonView.IsMine)
        {
            footstepSound.PlayFootstepSound();
        }
    }
}

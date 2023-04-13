using System.Collections;
using System.Collections.Generic;
using EasyCharacterMovement.Examples.Cinemachine.FirstPersonExample;
using Photon.Pun;
using UnityEngine;

public class ScriptDisabler : MonoBehaviourPun
{
    // [SerializeField] List<MonoBehaviour> toDisable;
    [SerializeField] CMFirstPersonCharacter character;

    void Start()
    {
        if (photonView.IsMine)
        {
            character.handleInput = true;
            // foreach (var item in toDisable)
            // {
            //     item.enabled = false;
            // }
        }
    }
}

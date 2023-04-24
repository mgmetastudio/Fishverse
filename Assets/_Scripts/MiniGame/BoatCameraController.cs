using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
// using Mirror;
using UnityEngine;

public class BoatCameraController : MonoBehaviourPun
{
    [SerializeField]  List<Cinemachine.CinemachineVirtualCameraBase> cams;

    void Start()
    {
        if(!photonView.IsMine)
            foreach (var item in cams)
                item.SetInactive();
    }

}

using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class BoatCameraController : NetworkBehaviour
{
    [SerializeField]  List<Cinemachine.CinemachineVirtualCameraBase> cams;

    void Start()
    {
        if(!isLocalPlayer)
            foreach (var item in cams)
                item.SetInactive();
    }

}

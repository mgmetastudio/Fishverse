using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [SerializeField] Cinemachine.CinemachineVirtualCamera firstPerson;
    [SerializeField] Cinemachine.CinemachineFreeLook thirdPerson;
    [SerializeField] List<SkinnedMeshRenderer> firstPersonRend;
    [SerializeField] List<SkinnedMeshRenderer> thirdPersonRend;

    [Space]
    [SerializeField] Button cameraBtn;

    [Space]
    [SerializeField] PhotonView photonView;

    void Start()
    {
        if (photonView.IsMine)
        {
            // SetShadowMode(thirdPersonRend, UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly);
            thirdPerson.Priority = 10;

            // cameraBtn.onClick.AddListener(ToggleView);
        }

    }
    void Update()
    {
        if (!photonView.IsMine) return;

        // if (Input.GetKeyDown(KeyCode.F))
        // {
        //     ToggleView();
        // }

    }

    void ToggleView()
    {
        if (firstPerson.Priority == 0)
        {
            firstPerson.Priority = 10;
            thirdPerson.Priority = 0;
            SetShadowMode(thirdPersonRend, UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly);

        }
        else
        {
            firstPerson.Priority = 0;
            thirdPerson.Priority = 10;
            SetShadowMode(thirdPersonRend, UnityEngine.Rendering.ShadowCastingMode.On);
        }
    }

    void SetShadowMode(List<SkinnedMeshRenderer> rend, UnityEngine.Rendering.ShadowCastingMode mode)
    {
        foreach (var item in rend)
            item.shadowCastingMode = mode;
    }


}

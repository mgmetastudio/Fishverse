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
    [SerializeField] Cinemachine.CinemachineVirtualCamera thridPerson_fishing;
    [SerializeField] List<SkinnedMeshRenderer> firstPersonRend;
    [SerializeField] List<SkinnedMeshRenderer> thirdPersonRend;
    [SerializeField] List<SkinnedMeshRenderer> thridPersonfishingRend;

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

    }

    public void CameraToggleView()
    {
        if (thridPerson_fishing.Priority == 0)
        {
            thridPerson_fishing.Priority = 10;
            thirdPerson.Priority = 0;
            SetShadowMode(thirdPersonRend, UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly);
            SetShadowMode(thridPersonfishingRend, UnityEngine.Rendering.ShadowCastingMode.On);

        }
        else
        {
            thridPerson_fishing.Priority = 0;
            thirdPerson.Priority = 10;
            SetShadowMode(thirdPersonRend, UnityEngine.Rendering.ShadowCastingMode.On);
        }
    }
    public void ThridPersonToggleView()
    {
        if (thirdPerson.Priority == 0)
        {
            thridPerson_fishing.Priority = 0;
            thirdPerson.Priority = 10;
            SetShadowMode(thirdPersonRend, UnityEngine.Rendering.ShadowCastingMode.On);
        }
    }
    public void ThridPersonfishingToggleView()
    {
        if (thridPerson_fishing.Priority == 0)
        {
            thridPerson_fishing.Priority = 10;
            thirdPerson.Priority = 0;
            SetShadowMode(thirdPersonRend, UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly);
            SetShadowMode(thridPersonfishingRend, UnityEngine.Rendering.ShadowCastingMode.On);

        }

    }
    void SetShadowMode(List<SkinnedMeshRenderer> rend, UnityEngine.Rendering.ShadowCastingMode mode)
    {
        foreach (var item in rend)
            item.shadowCastingMode = mode;
    }


}

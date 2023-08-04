using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ArcadeVehicleNitro : MonoBehaviourPun
{
    [Header("Settings:")]
    public float nitro;
    public float nitro_usage = 1;
    public bool nitro_activated = false;
    public float nitro_boost_amount = 1.35f;

    [Space(10)]
    [Header("UI:")]
    public List<Image> progress_bars = new List<Image>();
    private ArcadeVehicleController vehicle_controller;

    PhotonView photon_view;
    private void OnEnable()
    {
        photon_view = GetComponent<PhotonView>();
        if (photon_view.IsMine)
        {
            BoatNitroUI.OnNitroClickedDown += NitroClickDownEventHandler;
            BoatNitroUI.OnNitroClickedUp += NitroClickUpEventHandler;
            BoatNitroUI.OnProgressBar += ProgressEventHandler;
        }
    }

    private void NitroClickDownEventHandler()
    {
        StartNitro();
    }
    private void ProgressEventHandler(Image img)
    {
        progress_bars.Add(img);
    }
    private void NitroClickUpEventHandler()
    {
        StopNitro();
    }
    private void OnDisable()
    {
        if (photon_view.IsMine)
        {
            BoatNitroUI.OnNitroClickedDown -= NitroClickDownEventHandler;
            BoatNitroUI.OnNitroClickedUp -= NitroClickUpEventHandler;
            BoatNitroUI.OnProgressBar -= ProgressEventHandler;
        }
    }

    private void Start()
    {
        vehicle_controller = GetComponent<ArcadeVehicleController>();
        //Debug.Log("ArcadeVehicleNitro Start");
    }
    public void RefreshNitroUI()
    {
        if (progress_bars.Count > 0)
        {
            foreach (Image pb in progress_bars)
            {
                pb.fillAmount = nitro;
            }
        }
    }

    public void Update()
    {
        if (photon_view.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift)) StartNitro();
            if (Input.GetKeyUp(KeyCode.LeftShift)) StopNitro();
        }

        if (nitro_activated)
        {
            nitro = Mathf.Max(0, nitro - nitro_usage * Time.deltaTime);

            RefreshNitroUI();

            if (nitro <= 0)
            {
                StopNitro();
            }
        }
    }

    public void StartNitro()
    {
        nitro_activated = true;
        vehicle_controller.speed_boost = nitro_boost_amount;
    }

    public void StopNitro()
    {
        nitro_activated = false;
        vehicle_controller.speed_boost = 1f;
    }

}

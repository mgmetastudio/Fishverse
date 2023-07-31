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
    public static ArcadeVehicleNitro InstanceArcadeVehicleNitro { get; private set; }
    private ArcadeVehicleController vehicle_controller;

    PhotonView photon_view;

    private void Start()
    {
        photon_view = GetComponent<PhotonView>();
        vehicle_controller = GetComponent<ArcadeVehicleController>();

      //  FindObjectOfType<BoatNitroUI>(true).Setup(this);
    }
    private void Awake()
    {
        // Ensure only one instance of the script exists
        if (InstanceArcadeVehicleNitro == null)
        {
            InstanceArcadeVehicleNitro = this;
        }
        else
        {
            // If an instance already exists, destroy this duplicate
            Destroy(gameObject);
        }
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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArcadeVehicleNitro : MonoBehaviour
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

    private void Start()
    {
        vehicle_controller = GetComponent<ArcadeVehicleController>();

        FindObjectOfType<BoatNitroUI>(true).Setup(this);
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

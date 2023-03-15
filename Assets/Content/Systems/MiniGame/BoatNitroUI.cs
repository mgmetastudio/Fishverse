using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoatNitroUI : MonoBehaviour
{
    [SerializeField] Image nitroImg;
    ArcadeVehicleNitro nitro;

    public void Setup(ArcadeVehicleNitro vNitro)
    {
        nitro = vNitro;
        vNitro.progress_bars.Add(nitroImg);
    }

    public void StartNitro()
    {
        nitro.StartNitro();
    }

    public void StopNitro()
    {
        nitro.StopNitro();
    }

}

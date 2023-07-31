using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoatNitroUI : MonoBehaviour
{
    [SerializeField] Image nitroImg;
   public ArcadeVehicleNitro nitro;

    public void Setup(ArcadeVehicleNitro vNitro)
    {
        nitro = vNitro;
        vNitro.progress_bars.Add(nitroImg);
    }
    public void Update()
    {
        ArcadeVehicleNitro.InstanceArcadeVehicleNitro.progress_bars.Add(nitroImg);
    }
    public void StartNitro()
    {
        ArcadeVehicleNitro.InstanceArcadeVehicleNitro.StartNitro();
    }

    public void StopNitro()
    {
        ArcadeVehicleNitro.InstanceArcadeVehicleNitro.StopNitro();
    }

}

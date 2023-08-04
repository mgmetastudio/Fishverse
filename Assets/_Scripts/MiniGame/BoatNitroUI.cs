using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoatNitroUI : MonoBehaviour
{
    public ArcadeVehicleNitro nitro;

    [SerializeField] Image nitroImg;
    RoomManager Rm;
    public delegate void NitroAction();
    public static event NitroAction OnNitroClickedDown;
    public static event NitroAction OnNitroClickedUp;
    public delegate void ProgressBarAction(Image img);
    public static event ProgressBarAction OnProgressBar;
    public void Setup(ArcadeVehicleNitro vNitro)
    {
        nitro = vNitro;
        vNitro.progress_bars.Add(nitroImg);
    }
    public void Update()
    {
        OnProgressBar?.Invoke(nitroImg);
    }
    public void StartNitro()
    {
        OnNitroClickedDown?.Invoke();
    }

    public void StopNitro()
    {
        OnNitroClickedUp?.Invoke();
    }
}

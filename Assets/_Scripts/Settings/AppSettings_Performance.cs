using UnityEngine;

public class AppSettings_Performance : MonoBehaviour
{
    public int target_fps = 60;

    void Start()
    {
        Application.targetFrameRate = target_fps;
    }
}

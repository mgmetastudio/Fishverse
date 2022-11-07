using UnityEngine;

[CreateAssetMenu(fileName = "AppSettings", menuName = "AppSettings Data")]
public class AppSettings_Data : ScriptableObject
{
    public bool sound_enabled = true;
    public bool high_quality_graphics = true;
    public bool show_fps_counter = false;
}

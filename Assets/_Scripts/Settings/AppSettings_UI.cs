using UnityEngine;
using UnityEngine.UI;

public class AppSettings_UI : MonoBehaviour
{
    public AppSettings_Data app_settings;
    private AppSettings_API app_settings_api;

    public Toggle toggle_sound_enabled;
    public Toggle toggle_high_quality_graphics;
    public Toggle toggle_show_fps_counter;

    private void Start()
    {
        app_settings_api = GetComponent<AppSettings_API>();
    }

    public void RefreshUI()
    {
        toggle_sound_enabled.isOn = app_settings.sound_enabled;
        toggle_high_quality_graphics.isOn = app_settings.high_quality_graphics;
        toggle_show_fps_counter.isOn = app_settings.show_fps_counter;
    }

    public void SetDataFromUI()
    {
        app_settings.sound_enabled = toggle_sound_enabled.isOn;
        app_settings.high_quality_graphics = toggle_high_quality_graphics.isOn;
        app_settings.show_fps_counter = toggle_show_fps_counter.isOn;

        app_settings_api.ApplySettings();
    }
}

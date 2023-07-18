using UnityEngine;

public class AppSettings_API : MonoBehaviour
{
    private AppSettings_UI app_settings_ui;
    public AppSettings_Data app_settings;
    void Start()
    {
        app_settings_ui = GetComponent<AppSettings_UI>();
        LoadSettings();
    }

    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey("settings_sound_enabled"))
        {
            app_settings.sound_enabled = PlayerPrefs.GetInt("settings_sound_enabled") == 1 ? true : false;
        }
        if (PlayerPrefs.HasKey("settings_high_quality_graphics"))
        {
            app_settings.high_quality_graphics = PlayerPrefs.GetInt("settings_high_quality_graphics") == 1 ? true : false;
        }
        if (PlayerPrefs.HasKey("settings_show_fps_counter"))
        {
            app_settings.show_fps_counter = PlayerPrefs.GetInt("settings_show_fps_counter") == 1 ? true : false;
        }
        app_settings_ui.RefreshUI();

    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("settings_sound_enabled", app_settings.sound_enabled ? 1 : 0);
        PlayerPrefs.SetInt("settings_high_quality_graphics", app_settings.high_quality_graphics ? 1 : 0);
        PlayerPrefs.SetInt("settings_show_fps_counter", app_settings.show_fps_counter ? 1 : 0);
    }

    public void ApplySettings()
    {
        AudioListener.volume = app_settings.sound_enabled ? 1 : 0;
        QualitySettings.SetQualityLevel(app_settings.high_quality_graphics ? 0 : 1, false);
    }
}
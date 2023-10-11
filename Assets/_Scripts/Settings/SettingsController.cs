using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsController : MonoBehaviour
{
    [Header("Graphics")]
    public string[] m_Graphic;[HideInInspector] public int m_CurrentGraphic;
    [Header("UI")]
    public Settings_UI Settings_UI;
    [Header("API/DATA")]
    public AppSettings_Data app_settings;
    private AppSettings_API app_settings_api;
    // Start is called before the first frame update
    private void Awake()
    {
        app_settings_api = FindObjectOfType<AppSettings_API>();
        HandleButtons();
        if (app_settings.high_quality_graphics)
        {
            m_CurrentGraphic = 0;
        }
        else
        {
            m_CurrentGraphic = 1;
        }
        UpdateUI();
    }

    // Update is called once per frame
    private void HandleButtons()
    {
        //Change Graphics 
        Settings_UI.m_NextGraphicsButton.onClick.AddListener(delegate { NextGraphics(true); r_AudioController.instance.PlayClickSound(); });
        Settings_UI.m_PreviousGraphicsButton.onClick.AddListener(delegate { NextGraphics(false); r_AudioController.instance.PlayClickSound(); });
    }
    private void NextGraphics(bool _Next)
    {
        if (_Next)
        {
            m_CurrentGraphic++;
            if (m_CurrentGraphic >= m_Graphic.Length) m_CurrentGraphic = 0;
        }
        else
        {
            m_CurrentGraphic--;
            if (m_CurrentGraphic < 0) m_CurrentGraphic = m_Graphic.Length - 1;
        }

        UpdateUI();
    }
    private void UpdateUI()
    {
        if (Settings_UI == null) return;
        Settings_UI.m_GraphicsText.text = m_Graphic[m_CurrentGraphic].ToString();
    }

    public void SetDataFromUI()
    {
        if(Settings_UI.m_GraphicsText.text == "SMOOTH")
        {
            app_settings.high_quality_graphics = true;
        }
        else
        {
            app_settings.high_quality_graphics = false;
        }
        app_settings_api.ApplySettings();
        UpdateUI();
    }

    public void RevertFromUI()
    {
        m_CurrentGraphic = 0;
        app_settings.high_quality_graphics = true;
        app_settings_api.ApplySettings();
        UpdateUI();
    }
}

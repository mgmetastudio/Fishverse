using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGamesInputProxy : MonoBehaviour
{
    [Header("Input Keys [Standalone]")]
    [Header("Input Key / Pause Menu")]
    #region Variables

    public KeyCode Pausekey = KeyCode.KeypadEnter;

    #endregion
    public GameObject PauseMenuPanel;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(Pausekey))
        {
            {
                OpenPauseMenu();
            }
        }
    }
    public void OpenPauseMenu()
    {
        PauseMenuPanel.SetActive();
    }
    public void ClosePauseMenu()
    { 
        PauseMenuPanel.SetInactive();
    }
}

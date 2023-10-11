
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
public class EntryControlManagerUI : MonoBehaviour
{
    #region Variables

    [Header("First Content UI")]
    public GameObject E_FirstContentPanel;
    public GameObject E_Panel;

    [Header("login Panel UI")]
    public GameObject E_LoginPanel;
    public Button E_Login;

    [Header("Register Panel UI")]
    public Button E_Register;

    [Header("WelcomeVideo UI")]
    public GameObject E_PanelWelcomeVideo;
    public VideoPlayer E_Video;
    public Button E_SkipButton;
   
    #endregion
}


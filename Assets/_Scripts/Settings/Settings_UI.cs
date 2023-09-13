using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Settings_UI : MonoBehaviour
{
    #region Variables

    [Header("Graphics Settings")]
    public Button m_NextGraphicsButton;
    public Button m_PreviousGraphicsButton;

    [Header("Graphics Field")]
    public TextMeshProUGUI m_GraphicsText;

    [Header("Bottom Buttons")]
    public Button m_applyButton;
    public Button m_revertButton;

    #endregion
}



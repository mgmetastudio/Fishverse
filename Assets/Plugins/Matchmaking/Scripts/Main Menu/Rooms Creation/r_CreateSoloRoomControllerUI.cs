using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class r_CreateSoloRoomControllerUI : MonoBehaviour
{
    #region Variables

    [Header("Create Room Button")]
    public Button OpenWorld_soloBtn;


    [Header("Match making panel")]
    public GameObject m_Matchmakingpanel;
    public Button m_RetourButton;
    public List <GameObject> m_HideGroupSinglePlayer;
    public TMPro.TMP_Text m_LoadingText;

    #endregion
}

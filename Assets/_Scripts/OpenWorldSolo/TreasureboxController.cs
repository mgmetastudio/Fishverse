using System.Collections;
using System.Collections.Generic;
using EasyCharacterMovement.Examples.Cinemachine.FirstPersonExample;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LibEngine.Reward;
using Zenject;

public class TreasureboxController : MonoBehaviour
{
    [Header("Treasure Reward Panel")]
    public int rewardId = 1;

    [Header("Treasure Reward Panel")]
    
    [Header("Panel of TreasureBOX")]
    public GameObject Panel;
    [Header("Text field of Digid code")]
    public TMP_Text codeInputField;

    [Header("Buttons [Android/IOS]")]
    public Button promptBtn;
    public Button CloseBtn;
    [Header("Input Keys [Standalone]")]
    [Header("Input Key / Open TreasureBOX")]

    [Inject]
    private IRewardController rewardController;

    #region Variables

    public KeyCode OpenBoxKey = KeyCode.KeypadEnter;

    #endregion

    [SerializeField] TMP_Text promptText;
    [SerializeField] string OpenText = "Open Treasure Box";
    public Animator Boxanim;
    public bool IsBoxOpened=false;
    public bool IsPanelOpened = false;
    [Space]
    public CMFirstPersonCharacter character;

    void Start()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        OpenText = "Press 'F' to Open the Treasure Box";
#else
        OpenText = "Open Treasure Box";
#endif
        promptBtn.onClick.AddListener(OpenTreasurPanel);
        CloseBtn.onClick.AddListener(CloseTreasurPanel);
        promptText.SetText(OpenText);
        Panel.SetActive(false);
        promptBtn.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (IsPanelOpened)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
#endif
        if (Input.GetKeyDown(OpenBoxKey) && promptBtn.gameObject.activeSelf)
        {
            OpenTreasurPanel();
        }

    }
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!IsBoxOpened)
            {
                promptBtn.SetActive(true);
            }
            Debug.Log("Is touching Treasurebox");
            character = other.GetComponent<CMFirstPersonCharacter>();
        }
    }

    void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            promptBtn.SetActive(false);
            CloseTreasurPanel();
            Debug.Log("Is Not touching Treasurebox");
        }

    }
    void OpenTreasurPanel()
    {
        Boxanim.SetBool("TreasureOpened", true);
        IsPanelOpened = true;
        IsBoxOpened = true;
        Panel.SetActive(true);
        promptBtn.SetActive(false);

        rewardController.SendReachRewardAndGenerate(rewardId);
        var playerReward = rewardController.GetRewardPlayerRecordDTO(rewardId);
        codeInputField.text = playerReward.Code;
    }
    void CloseTreasurPanel()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
#endif
        Boxanim.SetBool("TreasureOpened", false);
        IsPanelOpened = false;
        IsBoxOpened = false;
        Panel.SetActive(false);
    }


}

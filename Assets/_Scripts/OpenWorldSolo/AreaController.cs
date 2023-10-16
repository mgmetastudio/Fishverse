using UnityEngine;
using UnityEngine.UI;
using NullSave.GDTK.Stats;

public class AreaController : MonoBehaviour
{
    [Header("Required Level")]
    public int requiredLevel; // Set the required level for entering this area

    [Header("Area Collider")]
    public  Collider areaCollidertriggered;
    public Collider areaCollidernotTriggered;


    [Header("Level Requirement Panel")]
    public GameObject levelRequirementPanel;

    [Header("Level Requirement Text")]
    public TMPro.TMP_Text levelRequirementText;

    [Header("PlayerCharacterStats")]
    private PlayerCharacterStats PlayerCharacterStats;

    private void Start()
    {
        PlayerCharacterStats = FindObjectOfType<PlayerCharacterStats>();
        if (areaCollidernotTriggered != null)
        {
            areaCollidernotTriggered.enabled = true;
        }

    }

    public void CheckPlayerLevel(int playerLevel)
    {
        if (playerLevel >= requiredLevel)
        {
            areaCollidernotTriggered.enabled = false;
            areaCollidertriggered.enabled = false;
            HideLevelRequirementPanel();
            
        }
        else
        {
            areaCollidernotTriggered.enabled = true;
            areaCollidertriggered.enabled = true;
            ShowLevelRequirementPanel();
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger zone");
            CheckPlayerLevel(FindObjectOfType<PlayerCharacterStats>().GetCharacterLevel());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited the trigger zone");
            HideLevelRequirementPanel();
        }
    }

    private void ShowLevelRequirementPanel()
    {
        levelRequirementText.text = "YOU CAN'T ENTER THIS AREA. <br> LEVEL " + requiredLevel + " REQUIRED.";
        levelRequirementPanel.SetActive(true);
    }

    private void HideLevelRequirementPanel()
    {
        levelRequirementPanel.SetActive(false);
    }
}

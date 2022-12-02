using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Interactions_UI : MonoBehaviour
{
    public Button btn_interaction;
    public TMP_Text text_action_name;
    private Interactions_API interactions_api;

    public void Awake()
    {
        interactions_api = GetComponent<Interactions_API>();
    }

    public void ShowInteractionButton(string action_name)
    {
        if (btn_interaction.interactable == false)
        {
            btn_interaction.interactable = true;
            text_action_name.text = action_name;
        }
    }

    public void HideInteractionButton()
    {
        if (btn_interaction.interactable == true)
        {
            btn_interaction.interactable = false;
            text_action_name.text = "Use";
        }
    }

    public void ActivateInteraction()
    {
        if (interactions_api)
        {
            interactions_api.ActivateInteraction();
        }
    }
}

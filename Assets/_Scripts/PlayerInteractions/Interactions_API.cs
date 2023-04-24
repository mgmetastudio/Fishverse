using UnityEngine;

public class Interactions_API : MonoBehaviour
{
    private Interactions_UI interaction_ui;
    public GameObject object_to_interact;
    public CameraRaycaster raycaster;

    private void Awake()
    {
        interaction_ui = GetComponent<Interactions_UI>();
    }

    private void Update()
    {
        raycaster.Cast_Interactions();

        if (raycaster.data_interactions.is_hitting == true)
        {
            object_to_interact = raycaster.hit.collider.gameObject;
            interaction_ui.ShowInteractionButton(object_to_interact.GetComponent<Interface_Interactive>().GetInteractionText());
        }

        else
        {
            interaction_ui.HideInteractionButton();
        }
    }

    public void ActivateInteraction()
    {
        if (object_to_interact != null)
        {
            object_to_interact.GetComponent<Interface_Interactive>().Interact();
            object_to_interact = null;
            interaction_ui.HideInteractionButton();
        }
    }
}

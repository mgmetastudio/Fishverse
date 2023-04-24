using UnityEngine;
using UnityEngine.Events;

public class Interactive_Event : MonoBehaviour, Interface_Interactive
{
    public string interaction_text;
    public UnityEvent on_interact;

    public void Interact()
    {
        on_interact.Invoke();
    }

    public string GetInteractionText()
    {
        return interaction_text;
    }
}

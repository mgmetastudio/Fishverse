using UnityEngine;
using UnityEngine.EventSystems;

public class TouchFieldActivator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public FixedTouchField touch_field;

    public void OnPointerDown(PointerEventData eventData)
    {
        touch_field.OnPointerDown(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        touch_field.OnPointerUp(eventData);
    }
    
}
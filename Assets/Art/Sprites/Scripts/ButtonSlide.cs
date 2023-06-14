using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonSlide : MonoBehaviour, IPointerClickHandler
{
    public RectTransform slideContainer;
    public float slideDistance = 100f;
    public float slideSpeed = 5f;

    private bool isSliding = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isSliding)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            StartCoroutine(SlideContainer(-slideDistance));
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            StartCoroutine(SlideContainer(slideDistance));
        }
    }

    private IEnumerator SlideContainer(float targetDistance)
    {
        isSliding = true;

        Vector3 startPosition = slideContainer.localPosition;
        Vector3 targetPosition = startPosition + new Vector3(targetDistance, 0f, 0f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * slideSpeed;
            slideContainer.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        isSliding = false;
    }
}

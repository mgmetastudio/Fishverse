using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject parentObject;
    public Vector3 targetScale;
    public float scaleDuration = 0.3f;

    private Vector3 initialScale;
    private bool isScaling = false;
    private Coroutine scaleCoroutine;

    private void Awake()
    {
        initialScale = parentObject.transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);

        scaleCoroutine = StartCoroutine(ScaleAnimation(targetScale));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);

        scaleCoroutine = StartCoroutine(ScaleAnimation(initialScale));
    }

    private IEnumerator ScaleAnimation(Vector3 target)
    {
        isScaling = true;

        float elapsedTime = 0f;
        Vector3 startScale = parentObject.transform.localScale;
        while (elapsedTime < scaleDuration)
        {
            float t = elapsedTime / scaleDuration;
            parentObject.transform.localScale = Vector3.Lerp(startScale, target, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        parentObject.transform.localScale = target;
        isScaling = false;
    }
}

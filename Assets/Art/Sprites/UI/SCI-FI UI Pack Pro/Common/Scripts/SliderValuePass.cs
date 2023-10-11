using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class SliderValuePass : MonoBehaviour
{
    Text progress;

    void Start()
    {
        progress = GetComponent<Text>();
        StartCoroutine(AnimateProgress(1f, 6f));
    }

    IEnumerator AnimateProgress(float targetProgress, float duration)
    {
        float initialProgress = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float currentProgress = Mathf.Lerp(initialProgress, targetProgress, elapsedTime / duration);
            progress.text = Mathf.Round(currentProgress * 100) + "%";

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final progress is set accurately
        progress.text = Mathf.Round(targetProgress * 100) + "%";
    }
}
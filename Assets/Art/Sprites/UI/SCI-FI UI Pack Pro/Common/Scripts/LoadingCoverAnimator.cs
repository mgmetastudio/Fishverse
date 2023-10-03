using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class LoadingCoverAnimator : MonoBehaviour
{
    public List<Sprite> images;  // List of images to cycle through
    Image imageComponent;
    int currentIndex = 0;
    float fadeDuration = 1f; // Adjust as needed
    float displayDuration = 1f; // Time to display each image

    void Start()
    {
        imageComponent = GetComponent<Image>();

        // Start the animation
        StartCoroutine(CycleImages());
    }

    IEnumerator CycleImages()
    {
        while (true)
        {
            // Set the new image
            imageComponent.sprite = images[currentIndex];

            // Fade in
            yield return StartCoroutine(FadeImage(true));

            // Display the current image for 3 seconds
            yield return new WaitForSeconds(displayDuration);

            // Fade out
            yield return StartCoroutine(FadeImage(false));

            // Switch to the next image
            currentIndex = (currentIndex + 1) % images.Count;
        }
    }

    IEnumerator FadeImage(bool fadeIn)
    {
        float targetAlpha = fadeIn ? 1f : 0f;
        float elapsedTime = 0f;
        Color startColor = imageComponent.color;

        while (elapsedTime < fadeDuration)
        {
            imageComponent.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(startColor.a, targetAlpha, elapsedTime / fadeDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final alpha value is set accurately
        imageComponent.color = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
    }
}
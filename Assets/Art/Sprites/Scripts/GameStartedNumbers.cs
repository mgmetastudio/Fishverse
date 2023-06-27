using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameStartedNumbers : MonoBehaviour
{
    public Sprite[] numberSprites; // Array of number sprites (3 to 0)
    public float delay = 0.4f; // Delay between each number change
    public float fadeDuration = 0.5f; // Duration of the fade-in effect

    private Image imageComponent; // Reference to the Image component
    private float currentAlpha = 0f; // Current alpha value
    private int currentSpriteIndex = 0; // Current index of the number sprite

    private void Start()
    {
        imageComponent = GetComponent<Image>(); // Get the Image component on the same GameObject
        imageComponent.color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, currentAlpha); // Set initial alpha

        StartCoroutine(ChangeNumbers());
    }

    private IEnumerator ChangeNumbers()
    {
        while (true)
        {
            // Get the current sprite for this iteration
            Sprite currentSprite = numberSprites[currentSpriteIndex];

            // Update the sprite of the Image component
            imageComponent.sprite = currentSprite;

            // Start fading in
            yield return StartCoroutine(FadeIn());

            // Increment the sprite index
            currentSpriteIndex = (currentSpriteIndex + 1) % numberSprites.Length;

            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            currentAlpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            imageComponent.color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, currentAlpha);
            yield return null;
        }

        // Ensure the alpha is set to 1 at the end of the fade
        currentAlpha = 1f;
        imageComponent.color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, currentAlpha);
    }
}

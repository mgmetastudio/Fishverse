using UnityEngine;
using UnityEngine.UI;

public class TextLoader : MonoBehaviour
{
    [Header("Loading Text")]
    public Text textComponent;
    float timer = 0f;
    float animationSpeed = 1f; // Adjust as needed

    void Update()
    {
        AnimateText();
    }

    void AnimateText()
    {
        timer += Time.deltaTime * animationSpeed;

        // Ensure textComponent is not null before attempting to modify it
        if (textComponent != null)
        {
            // Example: Display "Loading" with dots based on the timer
            int dots = Mathf.FloorToInt(timer) % 4 + 1;
            string loadingText = "Loading";
            for (int i = 0; i < dots; i++)
            {
                loadingText += ".";
            }

            textComponent.text = loadingText;
        }
        else
        {
            Debug.LogError("Text component is null. Make sure the script is attached to a UI Text object.");
        }
    }
}

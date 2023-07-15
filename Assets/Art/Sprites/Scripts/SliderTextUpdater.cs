using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderTextUpdater : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI textMeshProText;

    private float previousValue;

    private void Start()
    {
        // Initialize the previous value with the initial value of the slider
        previousValue = slider.value;

        // Manually trigger the text update
        UpdateText(slider.value);
    }

    private void Update()
    {
        // Check if the value of the slider has changed
        if (slider.value != previousValue)
        {
            // Update the TextMeshPro-Text(UI) text with the new value
            UpdateText(slider.value);

            // Update the previous value with the new value of the slider
            previousValue = slider.value;
        }
    }

    private void UpdateText(float value)
    {
        // Update the TextMeshPro-Text(UI) text with the new value
        if (textMeshProText != null)
        {
            textMeshProText.text = Mathf.Round(value * 100f).ToString();
        }
    }
}

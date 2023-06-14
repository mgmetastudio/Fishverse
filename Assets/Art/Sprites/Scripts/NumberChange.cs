using UnityEngine;
using TMPro;
using System.Collections;

public class NumberChange : MonoBehaviour
{
    public TextMeshProUGUI text;
    public GameObject panel;
    private float startNumber = 0f;
    private float increment = 0.01f; // Amount to increment the number
    private float updateInterval = 0.5f; // Interval between number updates
    private bool isPanelActive = true;
    private bool shouldResetNumber = true;
    private Coroutine changeNumberCoroutine;

    private void OnEnable()
    {
        panel.SetActive(true); // Ensure the panel is initially active
        changeNumberCoroutine = StartCoroutine(ChangeNumberOverTime());
    }

    private void OnDisable()
    {
        StopCoroutine(changeNumberCoroutine);
        panel.SetActive(false); // Ensure the panel is deactivated when the script is disabled
    }

    private IEnumerator ChangeNumberOverTime()
    {
        float currentNumber = startNumber;

        while (true)
        {
            if (panel.activeSelf != isPanelActive)
            {
                isPanelActive = panel.activeSelf;
                if (isPanelActive)
                {
                    if (shouldResetNumber)
                    {
                        currentNumber = startNumber;
                        text.text = "0:00";
                        shouldResetNumber = false;
                    }
                }
                else
                {
                    shouldResetNumber = true;
                }
            }

            if (isPanelActive)
            {
                currentNumber += increment;

                // Convert the number to the desired text format
                string minutes = Mathf.Floor(currentNumber).ToString("00");
                string seconds = ((int)(currentNumber * 100) % 100).ToString("00");
                string newText = minutes + ":" + seconds;

                // Update the TextMeshProUGUI component
                text.text = newText;
            }

            yield return new WaitForSeconds(updateInterval);
        }
    }
}

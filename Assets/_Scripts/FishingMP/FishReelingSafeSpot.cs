using System.Collections;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FishReelingSafeSpot : MonoBehaviour
{
    [Header("XCounter")]
    public TMPro.TMP_Text XCounterText;

    public GameObject sprite;
    public GameObject bar;
    [SerializeField] PlayerFishing PlayerFishing;
    private float barScale;
    private float barScaleSpeed = 0.1f;
    private bool isInsideCondition = false;
    private int counterValue = 1;
    private float timer = 0f;
    private float updateInterval = 0.2f; // Adjust this value to control the update interval in seconds.
    private Coroutine scaleAnimationCoroutine; // Store the coroutine reference.
    private bool barScalestart = true;
    private bool isWaiting = true;
    public float minScale = 0.32f;
    public float maxScale = 1.1f;
    public float minMultiplier = 1.2f;
    public float maxMultiplier = 3.7f;
    private void Start()
    {
        // Initialize the XCounterText as disabled.
        XCounterText.enabled = false;
    }
    private void Update()
    {
        if (PlayerFishing.FishingFloat != null)
        {
            if (PlayerFishing.FishingFloat.fish != null)
            {

                float targetScale_ = Mathf.Lerp(barScale, 1.1f, barScaleSpeed * Time.deltaTime);
                float currentScale = targetScale_;
                float clampedScale = Mathf.Clamp(currentScale, minScale, maxScale);
                float lerpFactor = Mathf.InverseLerp(minScale, maxScale, clampedScale);
                float newMultiplier = Mathf.Lerp(minMultiplier, maxMultiplier, lerpFactor);
                bar.GetComponent<Image>().pixelsPerUnitMultiplier = newMultiplier;

                if (PlayerFishing.FishingFloat.fish.HookedTo && !PlayerFishing.FishingFloat.fish.controller.isUpgradeFishingRod)
                {
                    if (barScalestart)
                    {
                        barScale = PlayerFishing.FishingFloat.fish.controller._scriptable.SafeReelingSpot;
                        bar.GetComponent<RectTransform>().localScale = new Vector3(1f, barScale, 1f);
                        barScalestart = false;
                    }
                    if (isWaiting)
                    {
                        StartCoroutine(WaitFor3Seconds());
                    }
                    if (!isWaiting)
                    {
                        // Get the world coordinates of the sprite and the bar.
                        Vector3 spriteWorldPosition = sprite.transform.position;
                        Vector3 barWorldPosition = bar.transform.position;

                        // Calculate the distance between the sprite's world coordinates and the bar's world coordinates.
                        float distance = Vector3.Distance(spriteWorldPosition, barWorldPosition);

                        Vector3[] spriteCorners = new Vector3[4];
                        sprite.GetComponent<RectTransform>().GetWorldCorners(spriteCorners);

                        Vector3[] barCorners = new Vector3[4];
                        bar.GetComponent<RectTransform>().GetWorldCorners(barCorners);

                        Bounds spriteBounds = new Bounds(spriteCorners[0], Vector3.zero);
                        Bounds barBounds = new Bounds(barCorners[0], Vector3.zero);

                        for (int i = 1; i < 4; i++)
                        {
                            spriteBounds.Encapsulate(spriteCorners[i]);
                            barBounds.Encapsulate(barCorners[i]);
                        }

                        if (spriteBounds.Intersects(barBounds))
                        {
                            isInsideCondition = true;

                            // Calculate the target scale based on how far the sprite is from the bar.
                            float targetScale = Mathf.Lerp(barScale, 1.1f, barScaleSpeed * Time.deltaTime);

                            // Apply the target scale smoothly.
                            barScale = targetScale;

                            // If the bar scale is close to 1, set it to 1.
                            if (Mathf.Abs(barScale - 1.1f) < 0.01f)
                            {
                                barScale = 1.1f;
                            }

                            // Enable the XCounterText.
                            XCounterText.enabled = true;

                            XCounterText.text = "x" + counterValue.ToString();
                            bar.GetComponent<RectTransform>().localScale = new Vector3(1f, barScale, 1f);

                            // Start the scale animation coroutine.
                            if (scaleAnimationCoroutine == null)
                            {
                                scaleAnimationCoroutine = StartCoroutine(ScaleAnimation());
                            }
                        }
                        else
                        {
                            isInsideCondition = false;
                            // Calculate the target scale for the bar.
                            float targetScale = PlayerFishing.FishingFloat.fish.controller._scriptable.SafeReelingSpot;

                            // Lerp the barScale towards the target scale.
                            barScale = Mathf.Lerp(barScale, targetScale, barScaleSpeed * Time.deltaTime * 5);

                            // Apply the new barScale to the bar GameObject.
                            bar.GetComponent<RectTransform>().localScale = new Vector3(1f, barScale, 1f);

                        }
                    }

                }
            }
            else
            {
                // Reset the bar scale when not fishing.
                barScale = 0.3f;
                bar.GetComponent<RectTransform>().localScale = new Vector3(1f, barScale, 1f);
                counterValue = 1;
                // Disable the XCounterText when not in the condition.
                XCounterText.enabled = false;
                barScalestart = true;
                isWaiting = true;
                return;
            }

        }
        else
        {
            counterValue = 1;
        }
        // Update the counter value.
        if (counterValue == 1)
        {
            XCounterText.enabled = false;
        }
        timer += Time.deltaTime;

        if (timer >= updateInterval)
        {
            timer = 0f;
            // Call the counter update method here.
            UpdateCounter();
        }
    }

    private void UpdateCounter()
    {
        if (isInsideCondition)
        {
            // Count up when inside the condition.
            counterValue = Mathf.Min(counterValue + 1, 1000); // Max value set to 1000.

            // Update the XCounterText with the current counter value.
            XCounterText.text = "x" + counterValue.ToString();
        }
        else
        {
            // Count down when outside the condition.
            counterValue = Mathf.Max(counterValue - 1, 1); // Min value set to 1.
            XCounterText.text = "x" + counterValue.ToString();
        }
    }
    // Coroutine for the scale animation.
    private IEnumerator ScaleAnimation()
    {
        float duration = 0.2f; // Duration of each phase of the animation.
        Vector3 startScale = XCounterText.transform.localScale;
        Vector3 targetScale = new Vector3(1.6f, 1.6f, 1.6f);

        while (isInsideCondition)
        {
            float startTime = Time.time;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                XCounterText.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
                yield return null;
                elapsedTime = Time.time - startTime;
            }

            XCounterText.transform.localScale = targetScale;
            yield return new WaitForSeconds(0.06f);
            // Ensure that the scale reaches the exact start scale.
            XCounterText.transform.localScale = startScale;

        }

        // Ensure the scale is reset when outside the condition.
        XCounterText.transform.localScale = new Vector3(1f, 1f, 1f);

        // Coroutine has finished.
        scaleAnimationCoroutine = null;
    }
    private IEnumerator WaitFor3Seconds()
    {
        yield return new WaitForSeconds(2f);
        isWaiting = false;

    }
}

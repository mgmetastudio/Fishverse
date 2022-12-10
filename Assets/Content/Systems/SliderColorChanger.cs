using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class SliderColorChanger : MonoBehaviour
{
    public Slider slider;
    public Color color_green;
    public Color color_red;
    public Color color_current;
    private float lerp_value;

    public Image img_handle;
    public Image img_fill;
    public bool use_0_1;

    void Update()
    {
        if (use_0_1)
        {
            color_current = Color.Lerp(color_red, color_green, slider.value);
        }
        else
        {
            if (slider.value > 0.45f && slider.value < 0.55f)
            {
                color_current = color_green;
            }
            else if (slider.value <= 0.45f)
            {
                lerp_value = Mathf.InverseLerp(0f, 0.45f, slider.value);
                color_current = Color.Lerp(color_red, color_green, lerp_value);
            }
            else if (slider.value >= 0.55f)
            {
                lerp_value = Mathf.InverseLerp(0.55f, 1f, slider.value);
                color_current = Color.Lerp(color_green, color_red, lerp_value);
            }
        }

        img_handle.color = color_current;
        img_fill.color = color_current;
    }
}

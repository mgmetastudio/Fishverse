using UnityEngine;
using TMPro;

public class TabButton : MonoBehaviour
{
    public TMP_Text target_text;
    public Color color_normal;
    public Color color_active;

    public void SetColor_Normal()
    {
        target_text.color = color_normal;
    }

    public void SetColor_Active()
    {
        target_text.color = color_active;
    }
}

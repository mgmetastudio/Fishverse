using UnityEngine;
using TMPro;

public class UI_CrashMessage : MonoBehaviour
{
    public TMP_Text text_message;

    public void ShowMessage(string new_text)
    {
        text_message.text = new_text;
        gameObject.SetActive(true);
    }
}

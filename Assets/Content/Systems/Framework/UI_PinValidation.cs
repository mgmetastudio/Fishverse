using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class UI_PinValidation : MonoBehaviour
{
    public string pin = "1231";
    public TMP_InputField input_pin;
    public UnityEvent on_success;
    public UnityEvent on_fail;

    public void Validate()
    {
        if (input_pin.text == pin)
        {
            on_success.Invoke();
        }
        else
        {
            on_fail.Invoke();
        }
    }
}

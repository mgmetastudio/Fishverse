using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class AccountManagement_UI : MonoBehaviour
{
    [Header("EVENTS:")]
    public TMP_InputField input_email;
    public TMP_InputField input_password;
    public GameObject panel_loading;
    
    [Space(8)]
    [Header("EVENTS:")]
    public UnityEvent on_login_success;

    private AccountManagement_API account_api;

    private void Start()
    {
        account_api = GetComponent<AccountManagement_API>();
    }

    public void Event_Login()
    {
        bool canLogin = ValidateLoginInfo();
        if (canLogin)
        {
            panel_loading.SetActive(true);
            account_api.Login(input_email.text, input_password.text);
        }
    }

    public void Event_Register()
    {
        Application.OpenURL("https://fisher.thefishverse.com/");
    }
    
    public void OnLoginSuccess()
    {
        on_login_success.Invoke();
    }
    
    private bool ValidateLoginInfo()
    {
        if (input_email.text != "" && input_password.text != "")
        {
            return true;
        }

        else
        {
            return false;
        }
    }
}

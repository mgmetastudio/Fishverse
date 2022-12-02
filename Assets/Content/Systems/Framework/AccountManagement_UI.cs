using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class AccountManagement_UI : MonoBehaviour
{
    [Header("Refs:")]
    public TMP_InputField input_email;
    public TMP_InputField input_password;
    public GameObject panel_loading;
    public TMP_Text text_profile_username;
    public UI_InfoMessage info_message;
    public UI_CrashMessage crash_message;

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
        text_profile_username.text = Fishverse_Core.instance.account_username;
        on_login_success.Invoke();
    }

    public void OnLoginFailed(int error_code = 0)
    {
        panel_loading.SetActive(false);
        info_message.ShowMessage("Login failed.\nError Code: " + error_code.ToString());
    }

    public void ShowCrashPopup(string text)
    {
        crash_message.ShowMessage(text);
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

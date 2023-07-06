using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Leguar.TotalJSON;
using UnityEngine.SceneManagement;
using Zenject;
using LibEngine.Auth;
using Cysharp.Threading.Tasks;

public class AccountManagement_API : MonoBehaviour
{
    [Inject] protected IAuthManager _authManager;
    [Inject] protected IUserStatsController _userStats;

    private AccountManagement_UI account_ui;

    private void Start()
    {
        account_ui = GetComponent<AccountManagement_UI>();
        StartCoroutine(CheckVersionRequest());
        //LoadData();
        //Login("john404_test@gmail.com", "_aJ4JQr2faCyMiKzuXSBeLXeQTb5q5jha");
    }

    public void Login(string email, string password)
    {
        StartCoroutine(LoginRequest(email, password));
    }

    public void LoginSuccessed()
    {
        var userName = _userStats.GetScheme().UserName;
        Fishverse_Core.instance.account_username = userName;
        PlayerPrefs.SetString("username", userName);
        account_ui.OnLoginSuccess();
    }

    public void LoadData()
    {
        if (PlayerPrefs.HasKey("log_em"))
        {
            account_ui.input_email.text = PlayerPrefs.GetString("log_em");
            Fishverse_Core.instance.account_email = account_ui.input_email.text;
        }
        if (PlayerPrefs.HasKey("log_ps"))
        {
            account_ui.input_password.text = PlayerPrefs.GetString("log_ps");
        }
        if (PlayerPrefs.HasKey("log_auto"))
        {
            if (PlayerPrefs.GetInt("log_auto") == 1)
            {
                account_ui.panel_loading.SetActive(true);
                //AUTO LOGIN
                Login(account_ui.input_email.text, account_ui.input_password.text);
                return;
            }
        }
        
        if(_authManager.IsAuthorized(out _))
        {
            account_ui.panel_loading.SetActive(true);
            //AUTO LOGIN
            LoginSuccessed();
        }

    }

    public void LogOut()
    {
        PlayerPrefs.SetString("log_em", "");
        PlayerPrefs.SetString("log_ps", "");
        PlayerPrefs.SetInt("log_auto", 0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void SaveData(string email, string password)
    {
        PlayerPrefs.SetString("log_em", email);
        Fishverse_Core.instance.account_email = email;
        PlayerPrefs.SetString("log_ps", password);
        PlayerPrefs.SetInt("log_auto", 1);
    }

    IEnumerator LoginRequest(string email, string password)
    {
        var loginAsync = _authManager.LoginAsync(email, password, (x) => { }, (x) => { });

        yield return new WaitWhile(() => loginAsync.Status == UniTaskStatus.Pending);

        if (_authManager.IsAuthorized(out _))
        {
            SaveData(email, password);
            var userName = _userStats.GetScheme().UserName;
            Fishverse_Core.instance.account_username = userName;
            PlayerPrefs.SetString("username", userName); //adapter
            account_ui.OnLoginSuccess();
        }
        else
        {
            account_ui.OnLoginFailed(1);
        }

        yield break;

        //LOGIN DASHBOARD

        WWWForm login_dashboard_form = new WWWForm();
        login_dashboard_form.AddField("email", email);
        login_dashboard_form.AddField("password", password);

        UnityWebRequest dashboard_login_request =
            UnityWebRequest.Post(Fishverse_Core.instance.dashboard_server, login_dashboard_form);
        yield return dashboard_login_request.SendWebRequest();

        if (dashboard_login_request.error == null)
        {
            if (dashboard_login_request.downloadHandler.text != "")
            {
                JSON json_account_data = JSON.ParseString(dashboard_login_request.downloadHandler.text);
                string access_token;
                string username;

                access_token = json_account_data.GetString("access_token");
                username = json_account_data.GetJSON("user").GetString("username");

                //SAVE USERNAME
                Fishverse_Core.instance.account_username = username;

                if (access_token != null)
                {
                    //LOGIN DB

                    WWWForm login_form = new WWWForm();
                    login_form.AddField("apikey", Fishverse_Core.instance.api_key);
                    login_form.AddField("email", email);
                    login_form.AddField("username", username);

                    UnityWebRequest login_request =
                        UnityWebRequest.Post(Fishverse_Core.instance.server + "users/login.php", login_form);
                    yield return login_request.SendWebRequest();

                    if (login_request.error == null)
                    {
                        if (login_request.downloadHandler.text == "login-success")
                        {
                            //LOGIN SUCCESS
                            SaveData(email, password);
                            PlayerPrefs.SetString("username", username);
                            account_ui.OnLoginSuccess();
                        }
                        else
                        {
                            account_ui.OnLoginFailed(1);
                        }
                    }
                    else
                    {
                        account_ui.OnLoginFailed(2);
                    }
                }
                else
                {
                    account_ui.OnLoginFailed(3);
                }
            }
            else
            {
                account_ui.OnLoginFailed(4);
            }
        }
        else
        {
            account_ui.OnLoginFailed(5);
        }
    }

    private bool IsFastAuthCompleted()
    {
        if (_authManager == null)
            return false;

        var isAuthorized = _authManager.IsAuthorized(out _);
        return isAuthorized;
    }

    IEnumerator CheckVersionRequest()
    {
        //string result = "";
        //var loginProcess = _authManager.LoginAsync("john404test@gmail.com", "Bw2y7ZQMG7aJKABvFMXkd4pZLoqFCcAe", (x) => result = x, (x) => result = x);

        //yield return new WaitUntil(() => loginProcess.Status != UniTaskStatus.Pending);

        yield return new WaitUntil(IsFastAuthCompleted);

        //Debug.Log("Login test result: " + result);

        WWWForm login_form = new WWWForm();
        login_form.AddField("apikey", Fishverse_Core.instance.api_key);

        UnityWebRequest login_request =
            UnityWebRequest.Post(Fishverse_Core.instance.server + "app/get_version.php", login_form);
        yield return login_request.SendWebRequest();

        if (login_request.error == null)
        {
            if (login_request.downloadHandler.text != "")
            {
                Fishverse_Core.instance.app_version = login_request.downloadHandler.text;
                if(Fishverse_Core.instance.app_version != Fishverse_Core.instance.app_version_local)
                {
                    account_ui.ShowCrashPopup("Current version is outdated.\nTo use this application you must update it");
                }
                else
                {
                    LoadData();
                }
            }
            else
            {
                account_ui.ShowCrashPopup("Server connection error");
            }
        }
        else
        {
            account_ui.ShowCrashPopup("Server connection error");
        }
    }
}

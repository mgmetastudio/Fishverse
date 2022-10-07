using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Leguar.TotalJSON;

public class AccountManagement_API : MonoBehaviour
{
    private void Start()
    {
        Login("john404test@gmail.com", "aJ4JQr2faCyMiKzuXSBeLXeQTb5q5jha");
    }

    public void Login(string email, string password)
    {
        StartCoroutine(LoginRequest(email, password));
    }

    IEnumerator LoginRequest(string email, string password)
    {
        //LOGIN DASHBOARD

        WWWForm login_dashboard_form = new WWWForm();
        login_dashboard_form.AddField("email", email);
        login_dashboard_form.AddField("password", password);

        UnityWebRequest dashboard_login_request = UnityWebRequest.Post(Fishverse_Core.instance.dashboard_server, login_dashboard_form);
        yield return dashboard_login_request.SendWebRequest();

        if (dashboard_login_request.error == null)
        {
            if (dashboard_login_request.downloadHandler.text != "")
            {
                JSON json_account_data = JSON.ParseString(dashboard_login_request.downloadHandler.text);
                string access_token;
                access_token = json_account_data.GetString("access_token");
                print(access_token);
            }
        }


        /*WWWForm loginForm = new WWWForm();

        loginForm.AddField("apikey", OBS_Core.instance.appData.api_key);
        loginForm.AddField("email", email);
        loginForm.AddField("password", password);

        string adress = OBS_Core.instance.appData.server + "account_login.php";

        UnityWebRequest loginPostRequest = UnityWebRequest.Post(adress, loginForm);
        yield return loginPostRequest.SendWebRequest();

        if (loginPostRequest.error == null)
        {
            if (loginPostRequest.downloadHandler.text == "login-success")
            {
                account_ui.OnLoginSuccess();

                OBS_Core.instance.userData.password = password;
                OBS_Core.instance.userData.email = email;
                account_ui.OnProfileDataUpdate();
                SaveData(email, password);

                StartCoroutine(GetAccountDataRequest());

                GetComponent<EconomyManagement_API>().GetEconomyData();
            }
            else
            {
                switch (loginPostRequest.downloadHandler.text)
                {
                    case "user-does-not-exist":
                        account_ui.OnFail("This user not exist");
                        break;

                    case "wrong-password":
                        account_ui.OnFail("Wrong password");
                        break;

                    default:
                        account_ui.OnFail(loginPostRequest.downloadHandler.text);
                        break;
                }
            }
        }
        else
        {
            account_ui.OnFail(loginPostRequest.error);
        }
    }*/
    }
}

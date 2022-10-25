using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class MiniGameServer_API : MonoBehaviour
{
    public string[] leadearboard_string;
    public List<MiniGameLeaderboardUser> leaderboard_users;
    public MiniGameLeaderboard_UI leaderboard_UI;

    public void GetLeaderboard()
    {
        StartCoroutine(GetLeaderboardRequest());
    }

    public void SubmitScore(float score)
    {
        StartCoroutine(SubmitScoreRequest(score));
    }

    public void GetBestScore()
    {
        StartCoroutine(GetBestScoreRequest());
    }

    IEnumerator SubmitScoreRequest(float score)
    {
        WWWForm score_form = new WWWForm();
        score_form.AddField("apikey", Fishverse_Core.instance.api_key);
        score_form.AddField("email", Fishverse_Core.instance.account_email);
        score_form.AddField("new_score", score.ToString());

        UnityWebRequest score_form_request =
            UnityWebRequest.Post(Fishverse_Core.instance.server + "account_update_minigame_score.php", score_form);
        yield return score_form_request.SendWebRequest();

        if (score_form_request.error == null)
        {
            if (score_form_request.downloadHandler.text != "")
            {
                print(score_form_request.downloadHandler.text);
            }
        }
    }

    IEnumerator GetBestScoreRequest()
    {
        WWWForm score_form = new WWWForm();
        score_form.AddField("apikey", Fishverse_Core.instance.api_key);
        score_form.AddField("email", Fishverse_Core.instance.account_email);

        UnityWebRequest score_form_request =
            UnityWebRequest.Post(Fishverse_Core.instance.server + "account_minigame_getscore.php", score_form);
        yield return score_form_request.SendWebRequest();

        if (score_form_request.error == null)
        {
            if (score_form_request.downloadHandler.text != "")
            {
                if(GetComponent<MiniGame_API>())
                {
                    GetComponent<MiniGame_API>().best_score = Mathf.Round(float.Parse(score_form_request.downloadHandler.text));
                }
            }
        }
    }

    IEnumerator GetLeaderboardRequest()
    {
        WWWForm score_form = new WWWForm();
        score_form.AddField("apikey", Fishverse_Core.instance.api_key);

        UnityWebRequest score_form_request =
            UnityWebRequest.Post(Fishverse_Core.instance.server + "minigame_getscores_realtime.php", score_form);
        yield return score_form_request.SendWebRequest();

        if (score_form_request.error == null)
        {
            if (score_form_request.downloadHandler.text != "")
            {
                leaderboard_users.Clear();
                print(score_form_request.downloadHandler.text);

                leadearboard_string = score_form_request.downloadHandler.text.Remove(0,3).Split(":::");

                for(int i = 0; i < leadearboard_string.Length; i+= 2)
                {
                    MiniGameLeaderboardUser user = new MiniGameLeaderboardUser();
                    user.username = leadearboard_string[i];
                    user.score = int.Parse(leadearboard_string[i + 1]);
                    leaderboard_users.Add(user);

                    if(leaderboard_UI)
                    {
                        leaderboard_UI.GenerateLeaderboard(leaderboard_users);
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class MiniGameLeaderboardUser
    {
        public string username;
        public int score;
    }
}

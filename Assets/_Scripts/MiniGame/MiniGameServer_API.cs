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

    public void SubmitScore(int score)
    {
        StartCoroutine(SubmitScoreRequest(score));
    }

    public void GetBestScore()
    {
        StartCoroutine(GetBestScoreRequest());
    }

    IEnumerator SubmitScoreRequest(int score)
    {
        WWWForm score_form = new WWWForm();
        score_form.AddField("apikey", Fishverse_Core.instance.api_key);
        score_form.AddField("email", Fishverse_Core.instance.account_email);
        score_form.AddField("username", Fishverse_Core.instance.account_username);
        score_form.AddField("score", score.ToString());

        UnityWebRequest score_form_request =
            UnityWebRequest.Post(Fishverse_Core.instance.server + "minigame/set_score.php", score_form);
        yield return score_form_request.SendWebRequest();

        if (score_form_request.error == null)
        {
            if (score_form_request.downloadHandler.text != "")
            {
                print(score_form_request.downloadHandler.text);
                GetLeaderboard();
            }
        }
    }

    IEnumerator GetBestScoreRequest()
    {
        WWWForm score_form = new WWWForm();
        score_form.AddField("apikey", Fishverse_Core.instance.api_key);
        score_form.AddField("email", Fishverse_Core.instance.account_email);

        UnityWebRequest score_form_request =
            UnityWebRequest.Post(Fishverse_Core.instance.server + "minigame/get_score.php", score_form);
        yield return score_form_request.SendWebRequest();

        if (score_form_request.error == null)
        {
            if (score_form_request.downloadHandler.text != "")
            {
                if (GetComponent<MiniGame_Manager>())
                {
                    GetComponent<MiniGame_Manager>().best_score = int.Parse(score_form_request.downloadHandler.text);
                }
            }
        }
    }

    IEnumerator GetLeaderboardRequest()
    {
        WWWForm score_form = new WWWForm();
        score_form.AddField("apikey", Fishverse_Core.instance.api_key);

        UnityWebRequest score_form_request =
            UnityWebRequest.Post(Fishverse_Core.instance.server + "minigame/get_scores_unity.php", score_form);
        yield return score_form_request.SendWebRequest();

        if (score_form_request.error == null)
        {
            if (score_form_request.downloadHandler.text != "")
            {
                leaderboard_users.Clear();

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

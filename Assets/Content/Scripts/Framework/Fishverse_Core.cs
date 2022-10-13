using UnityEngine;

public class Fishverse_Core : MonoBehaviour
{
    public static Fishverse_Core instance;

    public string api_key = "fish_mIbtvtlo6E";
    public string server = "https://mglabs.tinymagicians.com/fishverse/";
    public string dashboard_server = "https://api-fisher.thefishverse.com/rest-auth/login/";

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }
}

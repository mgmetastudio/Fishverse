using UnityEngine;
using UnityEngine.Video;
using Zenject;
using LibEngine.Auth;
public class EntryControlManager : MonoBehaviour
{
    public EntryControlManagerUI EntryControlManagerUI;
    public AccountManagement_UI AccountManagement_UI;
    [Inject] protected IAuthManager _authManager;
    void Start()
    {
        EntryControlManagerUI.E_Panel.SetActive(false);
        if (!_authManager.IsAuthorized(out _))
        {
           EntryControlManagerUI.E_LoginPanel.SetActive(false);
           EntryControlManagerUI.E_FirstContentPanel.SetActive(true);
            EntryControlManagerUI.E_Panel.SetActive(true);
        }
        else
        {
            EntryControlManagerUI.E_LoginPanel.SetActive(true);
            EntryControlManagerUI.E_FirstContentPanel.SetActive(false);
            r_AudioController.instance.PlayBackground();
        }
        EntryControlManagerUI.E_Register.onClick.AddListener(() => { Register(); r_AudioController.instance.PlayClickSound(); });
        EntryControlManagerUI.E_Login.onClick.AddListener(() => { Login(); r_AudioController.instance.PlayClickSound(); });
        EntryControlManagerUI.E_SkipButton.onClick.AddListener(() => { r_AudioController.instance.PlayClickSound(); });

     /*   if (EntryControlManagerUI.E_Video != null)
        {
            EntryControlManagerUI.E_Video.loopPointReached += OnVideoEnd;
        }*/
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void Register()
    {
        Application.OpenURL("https://fisher.thefishverse.com/");
    }
    private void Login()
    {
        EntryControlManagerUI.E_LoginPanel.SetActive(true);
        EntryControlManagerUI.E_FirstContentPanel.SetActive(false);
    }
    private void SkipVideo()
    {
        EntryControlManagerUI.E_PanelWelcomeVideo.SetActive(false);
        EntryControlManagerUI.E_Video.Stop();
        EntryControlManagerUI.E_Panel.SetActive(true);
        r_AudioController.instance.PlayBackground();
    }
    private void OnVideoEnd(VideoPlayer vp)
    {
        // This method is called when the video reaches its end
       // SkipVideo();
    }


}

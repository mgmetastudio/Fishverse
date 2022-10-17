using UnityEngine;
using UnityEngine.UI;

public class Fishing_UI : MonoBehaviour
{
    public Button btn_fishing;
    public Image btn_fishing_img;
    public Sprite btn_fishing_sprite_cast;
    public Sprite btn_fishing_sprite_pull;
    public Slider slider_pull;
    public GameObject panel_success;
    public GameObject panel_failed;
    public GameObject[] ui_hide_when_fishing;

    private Fishing_API fishing_api;
    
    private void Start()
    {
        fishing_api = GetComponent<Fishing_API>();
    }

    public void Update_PullValue_UI()
    {
        slider_pull.value = fishing_api.pull_value;
    }

    public void Show_Pull_UI()
    {
        if (btn_fishing.interactable == false)
            btn_fishing.interactable = true;

        if(btn_fishing_img.sprite != btn_fishing_sprite_pull)
            btn_fishing_img.sprite = btn_fishing_sprite_pull;

        slider_pull.gameObject.SetActive(true);
    }

    public void Hide_Pull_UI()
    {
        if (btn_fishing.interactable == true)
            btn_fishing.interactable = false;

        slider_pull.gameObject.SetActive(false);
    }

    public void Show_StartFishingBtn()
    {
        if(btn_fishing.interactable == false)
            btn_fishing.interactable = true;

        if (btn_fishing_img.sprite != btn_fishing_sprite_cast)
            btn_fishing_img.sprite = btn_fishing_sprite_cast;
    }

    public void Hide_StartFishingBtn()
    {
        if (btn_fishing.interactable == true)
            btn_fishing.interactable = false;

        if (btn_fishing_img.sprite != btn_fishing_sprite_cast)
            btn_fishing_img.sprite = btn_fishing_sprite_cast;
    }

    public void Show_SuccessPanel()
    {
        panel_success.SetActive(true);
    }

    public void Show_FailedPanel()
    {
        panel_failed.SetActive(true);
    }

    public void Event_FishingAction()
    {
        if(fishing_api.fishing_state == Fishing_API.FishingState.None)
            fishing_api.StartFishing();
        if (fishing_api.fishing_state == Fishing_API.FishingState.Pulling)
            fishing_api.Add_Pull_Value();
    }

    public void OnStartFishing()
    {
        foreach (GameObject ui in ui_hide_when_fishing)
        {
            ui.SetActive(false);
        }
    }

    public void OnFinishFishing()
    {
        foreach (GameObject ui in ui_hide_when_fishing)
        {
            ui.SetActive(true);
        }
    }
}

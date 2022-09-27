using UnityEngine;
using UnityEngine.UI;

public class Fishing_UI : MonoBehaviour
{
    public GameObject btn_start_fishing;
    public Slider slider_pull;
    public GameObject btn_pull;
    public GameObject panel_success;
    public GameObject panel_failed;

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
        slider_pull.gameObject.SetActive(true);
        btn_pull.SetActive(true);
    }

    public void Hide_Pull_UI()
    {
        slider_pull.gameObject.SetActive(false);
        btn_pull.SetActive(false);
    }

    public void Show_StartFishingBtn()
    {
        if(btn_start_fishing.activeSelf == false)
            btn_start_fishing.SetActive(true);
    }

    public void Hide_StartFishingBtn()
    {
        if (btn_start_fishing.activeSelf == true)
            btn_start_fishing.SetActive(false);
    }

    public void Show_SuccessPanel()
    {
        panel_success.SetActive(true);
    }

    public void Show_FailedPanel()
    {
        panel_failed.SetActive(true);
    }

    public void Event_StartFishing()
    {
        fishing_api.StartFishing();
    }

    public void Event_PullFish()
    {
        fishing_api.Add_Pull_Value();
    }
}

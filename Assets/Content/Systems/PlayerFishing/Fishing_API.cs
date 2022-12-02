using UnityEngine;
using System.Collections;

public class Fishing_API : MonoBehaviour
{
    [Header("Core")]
    public CameraRaycaster raycaster;
    public FishingState fishing_state = FishingState.None;

    [Space(8)]
    [Header("Fishing Point")]
    public GameObject fishing_point;
    public GameObject fishing_point_graphics;
    public float fishing_point_smoothing = 2;
    public float show_floater_delay = 0.85f;
    public GameObject fishing_floater;

    [Space(8)]
    [Header("Pull Gameplay")]
    public float pull_value = 0.5f;
    public float pull_value_add_amount = 0.1f;
    public float pull_value_decrese_speed = 0.2f;
    public float pull_time = 8f;
    public float pull_time_current = 0;

    private Player_Root player_root;
    private Fishing_UI fishing_ui;
    private Fishing_Animations fishing_animations;
    public enum FishingState { None, Waiting, Pulling, Busy }

    private void Start()
    {
        player_root = GetComponent<Player_Root>();
        fishing_ui = GetComponent<Fishing_UI>();
        fishing_animations = GetComponent<Fishing_Animations>();
    }

    //UPDATE
    private void Update()
    {
        if (player_root.player_state == Player_Root.PlayerState.BoatWalking || player_root.player_state == Player_Root.PlayerState.GroundWalking)
        {
            if (fishing_state == FishingState.None)
            {
                raycaster.Cast_Water();
                Update_FishingPoint_Visibility();
                Update_UI_Visibility();
                Update_FishingPoint_Position();
            }
            else if (fishing_state == FishingState.Pulling)
            {
                Decrease_Pull_Value();

                if (pull_value < 0.01f || pull_value > 0.99f)
                {
                    EndFishing(false);
                }

                else
                {
                    pull_time_current += Time.deltaTime;
                    if (pull_time_current >= pull_time)
                    {
                        EndFishing(true);
                    }
                }
            }
        }
        else
        {
            fishing_ui.Hide_StartFishingBtn();
        }
    }

    private void Update_UI_Visibility()
    {
        if (raycaster.data_water.is_hitting && raycaster.isHit)
        {
            fishing_ui.Show_StartFishingBtn();
        }
        else
        {
            fishing_ui.Hide_StartFishingBtn();
        }
    }

    private void Update_FishingPoint_Visibility()
    {
        if (raycaster.data_water.is_hitting && raycaster.isHit)
        {
            ShowFishingPoint();
        }
        else
        {
            Hide_FishingPoint();
        }
    }

    private void Update_FishingPoint_Position(bool instant = false)
    {
        if (fishing_point.activeSelf)
        {
            if(!instant)
                fishing_point.transform.position = Vector3.Lerp(fishing_point_graphics.transform.position, raycaster.GetLastHitPosition(), Time.deltaTime * fishing_point_smoothing);
            else
                fishing_point.transform.position = raycaster.GetLastHitPosition();
        }
    }

    private void Decrease_Pull_Value()
    {
        pull_value = Mathf.Clamp(pull_value - (Time.deltaTime * pull_value_decrese_speed), 0, 1);
        fishing_ui.Update_PullValue_UI();
    }

    public void Add_Pull_Value()
    {
        pull_value = Mathf.Clamp(pull_value + pull_value_add_amount, 0, 1);
        fishing_ui.Update_PullValue_UI();
    }

    //API
    public void Hide_FishingPoint()
    {
        if (fishing_point_graphics.activeSelf == true)
        {
            fishing_point_graphics.SetActive(false);
        }
    }

    public void ShowFishingPoint()
    {
        if (fishing_point_graphics.activeSelf == false)
        {
            fishing_point_graphics.SetActive(true);
            Update_FishingPoint_Position(true);
        }
    }

    public void StartFishing()
    {
        if (fishing_state == FishingState.None)
        {
            fishing_state = FishingState.Waiting;
            fishing_ui.Hide_StartFishingBtn();
            fishing_ui.OnStartFishing();

            player_root.DisableMovement();
            fishing_animations.PlayAnim_StartFishing();

            pull_value = 0.5f;
            pull_time_current = 0f;

            Hide_FishingPoint();

            StartCoroutine(WaitForPull());
        }
    }

    public void EndFishing(bool success)
    {
        fishing_animations.PlayAnim_EndFishing();
        fishing_ui.Hide_Pull_UI();
        fishing_state = FishingState.Busy;
        fishing_floater.SetActive(false);

        StartCoroutine(EndFishing_Delay(success));
    }

    IEnumerator EndFishing_Delay(bool success)
    {
        yield return new WaitForSeconds(1f);

        if (success)
        {
            fishing_ui.Show_SuccessPanel();
        }
        else
        {
            fishing_ui.Show_FailedPanel();
        }

        fishing_ui.OnFinishFishing();

        yield return new WaitForSeconds(1f);
        fishing_state = FishingState.None;
        player_root.EnableMovement();
    }

    IEnumerator WaitForPull()
    {
        yield return new WaitForSeconds(show_floater_delay);

        fishing_floater.SetActive(true);

        yield return new WaitForSeconds(Random.Range(5f, 8f));

        fishing_ui.Show_Pull_UI();
        fishing_animations.PlayAnim_PullLoop();

        fishing_state = FishingState.Busy;

        yield return new WaitForSeconds(0.5f);

        fishing_state = FishingState.Pulling;
    }

    //UI Events
    public void Event_FishingBtn()
    {
        StartFishing();
    }
}

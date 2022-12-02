using UnityEngine;

public class Fishing_Animations : MonoBehaviour
{
    public Animator hand_anim_controller;
    public Animator fishing_point_anim_controller;

    public void PlayAnim_StartFishing()
    {
        hand_anim_controller.SetTrigger("StartFishing");
    }

    public void PlayAnim_PullLoop()
    {
        hand_anim_controller.SetTrigger("Pull");
        fishing_point_anim_controller.SetTrigger("Move");
    }

    public void PlayAnim_EndFishing()
    {
        hand_anim_controller.SetTrigger("EndFishing");
        fishing_point_anim_controller.SetTrigger("Idle");
    }
}

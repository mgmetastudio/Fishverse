using NullSave.TOCK.Inventory;
using UnityEngine;

public class SkillManager : MonoBehaviour
{

    public Animator playerAnimator;

    private void Awake()
    {
        if(playerAnimator == null)
        {
            playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Animator>();
        }
    }

    public void UseSkill(SkillSlotUI skillSlot)
    {
        if (skillSlot.AttachedSkillItem == null) return;

        switch(skillSlot.AttachedSkillItem.name)
        {
            case "Feather of Lightness":
                playerAnimator.SetTrigger("Jump");
                break;
        }

    }

}

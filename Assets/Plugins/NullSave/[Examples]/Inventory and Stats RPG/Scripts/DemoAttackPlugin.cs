using NullSave.TOCK.Character;
using NullSave.TOCK.Inventory;
using UnityEngine;

[CreateAssetMenu(menuName = "TOCK/Inventory/(Demo) Attack Plugin")]
public class DemoAttackPlugin : CharacterCogPlugin
{

    #region Variables

    public string attackButton = "Fire1";
    public bool allowRetrigger = false;
    public float extraStopMoveTime = 0.2f;

    private float preventMoveRemain;

    #endregion

    #region Plugin Methods

    public override void OnUpdate()
    {
#if INVENTORY_COG
        if (Character.Inventory.IsMenuOpen) return;
#endif

        if (preventMoveRemain > 0 && !Character.IsAttacking)
        {
            preventMoveRemain -= Time.deltaTime;
        }

        if (Character.PreventButtonUpdate || Character.InAction) return;

        if ((!Character.IsAttacking || allowRetrigger) && Character.GetButtonDown(attackButton) && Character.Animator.GetBool("IsHoldingWeapon"))
        {
#if INVENTORY_COG
            // Get the weapon used to see if we need and have ammo
            InventoryItem weapon = Character.Inventory.GetEquipPoint("LeftHand").EquipedOrStoredItem;
            if(weapon.usesAmmo)
            {
                if(Character.Inventory.GetEquippedAmmoCount(weapon.ammoType) < weapon.ammoPerUse)
                {
                    // We can't fire
                    return;
                }
            }
#endif
        
            // We're good to go
            Character.IsAttacking = true;
            Character.Animator.SetTrigger("Attack");
            preventMoveRemain = extraStopMoveTime;
        }
    }

    public override void PreMovement()
    {
        if (preventMoveRemain > 0)
        {
            Character.PreventMovement = true;
        }
    }

#endregion

}

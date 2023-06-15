using NullSave.TOCK.Character;
using NullSave.TOCK.Inventory;
using UnityEngine;

public class DemoProjectileAttackSMB : StateMachineBehaviour
{

    #region Variables

    public string equipPoint;

    public bool spawnProjectile;
    [Range(0f,1f)] public float spawnTime;

    public bool fireProjectile;
    [Range(0f, 1f)] public float fireTime;


    public bool releaseAttack = true;

    private bool wasSpawned, wasFired;

    #endregion

    /// <summary>
    /// Release attack lock
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="animatorStateInfo"></param>
    /// <param name="layerIndex"></param>
    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if (releaseAttack)
        {
            animator.GetComponent<CharacterCog>().IsAttacking = false;
        }

        wasSpawned = false;
        wasFired = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime > 1f) return;

        if (spawnProjectile && !wasSpawned && stateInfo.normalizedTime >= spawnTime)
        {
            EquipPoint ep = animator.GetComponent<InventoryCog>().GetEquipPoint(equipPoint);
            if (ep)
            {
                GameObject obj = ep.ObjectReference;
                if (obj)
                {
                    ProjectileWeapon pw = obj.GetComponentInChildren<ProjectileWeapon>();
                    if (pw != null)
                    {
                        pw.SpawnProjectile();
                        wasSpawned = true;
                    }
                }
            }
        }

        if (fireProjectile && !wasFired && stateInfo.normalizedTime >= fireTime)
        {
            EquipPoint ep = animator.GetComponent<InventoryCog>().GetEquipPoint(equipPoint);
            if (ep)
            {
                GameObject obj = ep.ObjectReference;
                if (obj)
                {
                    ProjectileWeapon pw = obj.GetComponentInChildren<ProjectileWeapon>();
                    if (pw != null)
                    {
                        pw.FireProjectile();
                        wasFired = true;
                    }
                }
            }
        }

        //if (!isActive && stateInfo.normalizedTime % 1 >= startDamage && stateInfo.normalizedTime % 1 <= endDamage)
        //{
        //    animator.GetComponent<CharacterCog>().SetDamageDealersActive(true);
        //    isActive = true;
        //}
        //else if (stateInfo.normalizedTime % 1 >= endDamage && isActive)
        //{
        //    animator.GetComponent<CharacterCog>().SetDamageDealersActive(false);
        //    isActive = false;
        //}
    }

}
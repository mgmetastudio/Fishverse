using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Inventory
{
    [RequireComponent(typeof(InventoryCog))]
    public class InventoryAttackManager : MonoBehaviour
    {

        #region Variables

        public string equipPointName = "WeaponEquipPoint";
        private EquipPoint equipPoint;

        public bool useButton = true;
        public string attackButton = "Fire1";

        public bool useKey = true;
        public KeyCode attackKey = KeyCode.LeftControl;

        // Animator requirements
        public List<AnimatorMod> requireMods;

        // Animator applications
        public List<AnimatorMod> applyMods;

        //Events
        public UnityEvent onAttackTriggered;

        #endregion

        #region Properties

        public Animator Animator { get; private set; }

        public InventoryCog Inventory { get; private set; }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            Animator = GetComponentInChildren<Animator>();
            Inventory = GetComponent<InventoryCog>();
            equipPoint = Inventory.GetEquipPoint(equipPointName);
            if(equipPoint == null)
            {
                Debug.LogWarning("No equip point named " + equipPointName);
                enabled = false;
            }

            if(Animator == null)
            {
                Debug.LogWarning("No animator found");
            }
        }

        private void Update()
        {
            if((useButton && InventoryCog.GetButtonDown(attackButton)) || (useKey && InventoryCog.GetKeyDown(attackKey)))
            {
                Attack();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Performs attack provided all requirements are met
        /// </summary>
        public void Attack()
        {
            Attack(equipPoint, equipPoint.EquipedOrStoredItem);
        }

        /// <summary>
        /// Attack using supplied weapon if possible
        /// </summary>
        /// <param name="item"></param>
        public void Attack(InventoryItem item)
        {
            if(item.CurrentEquipPoint != equipPoint)
            {
                Inventory.EquipItem(item, equipPoint.pointId);
            }

            Attack(item.CurrentEquipPoint, item);
        }

        #endregion

        #region Private Methods

        private void Attack(EquipPoint ep, InventoryItem weapon)
        {
            // Check primary conditions
            if (Inventory.IsMenuOpen || !ep.IsItemEquippedOrStored || weapon.itemType != ItemType.Weapon) return;

            // Check ammo condition
            if (weapon.usesAmmo)
            {
                if (Inventory.GetEquippedAmmoCount(weapon.ammoType) < weapon.ammoPerUse)
                {
                    // We can't fire
                    return;
                }
            }

            // Check animator conditions
            if (Animator != null)
            {
                foreach (AnimatorMod req in requireMods)
                {
                    switch (req.paramType)
                    {
                        case AnimatorParamType.Bool:
                            if (Animator.GetBool(req.keyName) != req.boolVal) return;
                            break;
                        case AnimatorParamType.Float:
                            if (Animator.GetFloat(req.keyName) != req.floatVal) return;
                            break;
                        case AnimatorParamType.Int:
                            if (Animator.GetInteger(req.keyName) != req.intVal) return;
                            break;
                    }
                }
            }

            // Unstore
            if (ep.IsItemStored)
            {
                ep.EquipItem(ep.Item);
            }

            // Apply modifiers
            if (Animator != null)
            {
                foreach (AnimatorMod mod in applyMods)
                {
                    mod.ApplyMod(Animator);
                }
            }

            // Fire event
            onAttackTriggered?.Invoke();
        }

        #endregion

    }
}
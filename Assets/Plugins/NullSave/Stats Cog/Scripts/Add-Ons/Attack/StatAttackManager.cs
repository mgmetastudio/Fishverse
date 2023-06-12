using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    [RequireComponent(typeof(StatsCog))]
    [RequireComponent(typeof(DamageDealer))]
    public class StatAttackManager : MonoBehaviour
    {

        #region Variables

        public List<StatAttack> availableAttacks;

        #endregion

        #region Properties

        public Animator Animator { get; private set; }

        public DamageDealer DamageDealer { get; private set; }

#if INVENTORY_COG

        public Inventory.InventoryCog InventorySource { get; private set; }

#endif

        public StatsCog StatSource { get; private set; }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            Animator = GetComponentInChildren<Animator>();
            StatSource = GetComponent<StatsCog>();
            DamageDealer = GetComponent<DamageDealer>();
#if INVENTORY_COG
            InventorySource = GetComponent<Inventory.InventoryCog>();
#endif
        }

        #endregion

        #region Public Methods

        public List<StatAttack> GetAvailableAttacks()
        {
            List<StatAttack> attacks = new List<StatAttack>();

            foreach (StatAttack attack in availableAttacks)
            {
                if (attack.IsAvailable(this))
                {
                    attacks.Add(attack);
                }
            }

            return attacks;
        }

        #endregion

    }
}
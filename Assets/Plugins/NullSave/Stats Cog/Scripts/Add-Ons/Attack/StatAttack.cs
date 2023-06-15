#if INVENTORY_COG
using NullSave.TOCK.Inventory;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    [CreateAssetMenu(menuName = "TOCK/Stats Cog/Stat Attack")]
    public class StatAttack : ScriptableObject
    {

        #region Variables

        [Header("Setup")]
        public string category;
        public string displayName;
        public StatConditionalBool unlocked = new StatConditionalBool() { value = true };
        public StatConditionalBool learned;
        public StatConditionalBool readied;

        [Header("Ready Conditions")]
        public List<string> requireFormulas;
#if INVENTORY_COG
        public List<AnimatorMod> requireAniMods;
        public List<InventoryItemCondition> requireItems;
        public List<string> requireEquipped;
#endif

        [Header("Targeting")]
        public bool targetsArea;
        public int minTargets;
        public int maxTargets;
        public LayerMask targetLayer;
        public float minRange;
        public float maxRange;
        public bool hasRadius;
        public float radius;
        public bool canTargetSelf;
        public GameObject spawnObject;

        [Header("Reload Time")]
        public bool mustReload;
        public bool startLoaded;
        public float reloadTime;

        [Header("Hit & Miss")]
        public DamageType damageType;
        public bool autoHit;
        public bool usesHitRoll = true;
        public string hitRoll = "1 + rnd_i(20)";
        public bool rollCanCrit = true;
        public string minCritValue = "20";
        public string hitCondition = "[roll] >= 1 + rnd_i(20) + target:AC";
        public string hitDamageAmount = "1";
        public string critDamageAmount = "1 * 2";
        public string missDamageAmount = "[hitDmg] / 2";
        public List<StatEffect> hitEffects;

        [Header("On Use (Self)")]
        public List<StatEffect> applyEffects;
        public List<StatModifier> applyModifiers;
#if INVENTORY_COG
        public List<AnimatorMod> applyAniMods;
#endif

        #endregion

        #region Public Methods

        public void Activate(StatAttackManager attackManager, List<StatsCog> targets)
        {
            if (!IsAvailable(attackManager)) return;

            if (targetsArea)
            {
                //!! TODO
            }
            else
            {
                bool hits;
                bool isCrit = false;
                float toHit = 0;
                float damage;
                if (usesHitRoll)
                {
                    toHit = attackManager.StatSource.GetExpressionValue(hitRoll);
                    Debug.Log("Rolled: " + toHit);
                    if (rollCanCrit && toHit >= attackManager.StatSource.GetExpressionValue(minCritValue))
                    {
                        isCrit = true;
                        Debug.Log("CRITHIT!");
                    }
                }

                foreach (StatsCog target in targets)
                {
                    hits = autoHit ? true : attackManager.StatSource.EvaluateCondition(hitCondition.Replace("[roll]", toHit.ToString()), target);
                    if (hits)
                    {
                        Debug.Log("HIT! " + target.gameObject.name);
                        damage = isCrit ? attackManager.StatSource.GetExpressionValue(critDamageAmount) : attackManager.StatSource.GetExpressionValue(hitDamageAmount);
                    }
                    else
                    {
                        Debug.Log("Miss! " + target.gameObject.name);
                        damage = attackManager.StatSource.GetExpressionValue(missDamageAmount);
                    }

                    Debug.Log("Damage: " + damage);
                    if (damage > 0)
                    {
                        attackManager.DamageDealer.damage = new List<Damage>();
                        attackManager.DamageDealer.damage.Add(new Damage() { baseAmount = damage.ToString(), damageType = damageType });
                        attackManager.DamageDealer.StatsSource = attackManager.StatSource;
                        attackManager.DamageDealer.effects = hits ? hitEffects : new List<StatEffect>();

                        target.GetComponentInChildren<DamageReceiver>().TakeDamage(attackManager.DamageDealer, attackManager.gameObject);
                    }
                }

                // Apply effects
                foreach (StatEffect effect in applyEffects)
                {
                    attackManager.StatSource.AddEffect(effect);
                }

                StatValue statValue;
                foreach (StatModifier mod in applyModifiers)
                {
                    statValue = attackManager.StatSource.FindStat(mod.affectedStat);
                    if (statValue != null)
                    {
                        statValue.AddModifier(mod);
                    }
                }

#if INVENTORY_COG
                foreach (AnimatorMod animatorMod in applyAniMods)
                {
                    animatorMod.ApplyMod(attackManager.Animator);
                }
#endif
            }
        }

        public bool IsAvailable(StatAttackManager attackManager)
        {
            if (!unlocked.GetValue(attackManager.StatSource) || !learned.GetValue(attackManager.StatSource) || !readied.GetValue(attackManager.StatSource)) return false;

            foreach (string formula in requireFormulas)
            {
                if (!attackManager.StatSource.EvaluateCondition(formula)) return false;
            }

#if INVENTORY_COG
            foreach (AnimatorMod mod in requireAniMods)
            {
                if (!mod.CheckMod(attackManager.Animator)) return false;
            }

            foreach (InventoryItemCondition item in requireItems)
            {
                if (!item.IsConditionMet(attackManager.InventorySource)) return false;
            }

            EquipPoint ep;
            foreach (string point in requireEquipped)
            {
                ep = attackManager.InventorySource.GetPointById(point);
                if (ep == null || !ep.IsItemEquipped) return false;
            }
#endif

            return true;
        }

        #endregion

    }
}
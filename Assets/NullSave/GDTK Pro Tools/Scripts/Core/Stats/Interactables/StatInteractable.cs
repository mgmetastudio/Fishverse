#if GDTK
using System.Collections.Generic;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    public class StatInteractable : InteractableObject
    {

        #region Fields

        [Tooltip("Stat Condition that must be true to be interactable")] public string requirements;
        [Tooltip("Action(s) to take on interact")] public InteractableActions actions;
        
        [Tooltip("Id of Background to set")] public string backgroundId;
        [Tooltip("Id of Race to set")] public string raceId;
        [Tooltip("Id of Class to add")] public string addClassId;
        [Tooltip("Id of Class to remove")] public string removeClassId;
        [Tooltip("Id of Status Condition to add")] public string addStatusConditionId;
        [Tooltip("Id of Status Condition to remove")] public string removeStatusConditionId;
        [Tooltip("List of Stat Modifiers to add")] public List<GDTKStatModifier> statModifiers;
        [Tooltip("Id of Status Effect to add")] public string addStatusEffectId;
        [Tooltip("Id of Status Effect to remove")] public string removeStatusEffectId;
        [Tooltip("Object to Activate")] public GameObject activateTarget;
        [Tooltip("Prefab to Spawn")] public GameObject prefab;
        [Tooltip("Parent Transform for spawned object")] public Transform parent;
        [Tooltip("Id of Event to raise")] public string raiseEventId;
        [Tooltip("Id of reward to grant")] public string rewardId;

        #endregion

        #region Properties

        public override bool IsInteractable
        {
            get
            {
                if (CurrentAgent != null)
                {
                    BasicStats bs = CurrentAgent.GetComponent<BasicStats>();
                    if(bs != null)
                    {
                        if (!bs.IsConditionTrue(requirements)) return false;
                    }
                }
                return base.IsInteractable;
            }
        }

        #endregion

        #region Public Methods

        public override bool Interact(Interactor source)
        {
            if (!IsInteractable) return false;

            BasicStats stats = source.GetInteractorComponent<BasicStats>();
            PlayerCharacterStats pc = stats is PlayerCharacterStats stats1 ? stats1 : null;

            if (stats == null) return false;
            if (!stats.IsConditionTrue(requirements)) return false;

            if (actions.HasFlag(InteractableActions.SetBackground))
            {
                pc?.SetBackground(backgroundId);
            }

            if (actions.HasFlag(InteractableActions.CustomReward))
            {
                pc?.GrantCustomReward(rewardId);
            }

            if (actions.HasFlag(InteractableActions.RemoveBackground))
            {
                pc?.SetBackground(string.Empty);
            }

            if(actions.HasFlag(InteractableActions.AddClass))
            {
                pc?.AddClass(addClassId);
            }

            if (actions.HasFlag(InteractableActions.RemoveClass))
            {
                pc?.RemoveClass(stats.database.GetClass(removeClassId));
            }

            if (actions.HasFlag(InteractableActions.AddStatEffect))
            {
                pc?.AddEffect(addStatusEffectId);
            }

            if (actions.HasFlag(InteractableActions.RemoveStatEffect))
            {
                pc?.RemoveEffect(removeStatusEffectId);
            }

            if (actions.HasFlag(InteractableActions.AddStatModifiers))
            {
                if(stats != null)
                {
                    foreach(GDTKStatModifier mod in statModifiers)
                    {
                        stats.AddStatModifier(mod);
                    }
                }
            }

            if (actions.HasFlag(InteractableActions.AddStatusCondition))
            {
                stats?.AddStatusCondition(addStatusConditionId);
            }

            if (actions.HasFlag(InteractableActions.RemoveStatusCondition))
            {
                stats?.RemoveStatusCondition(removeStatusConditionId);
            }

            if (actions.HasFlag(InteractableActions.SaveData))
            {
                pc?.DataSave(pc?.saveFilename);
            }

            if (actions.HasFlag(InteractableActions.LoadData))
            {
                pc?.DataLoad(pc?.saveFilename);
            }

            if (actions.HasFlag(InteractableActions.SetRace))
            {
                pc?.SetRace(raceId);
            }

            if (actions.HasFlag(InteractableActions.RemoveRace))
            {
                pc?.SetRace(string.Empty);
            }

            if (actions.HasFlag(InteractableActions.SpawnObject))
            {
                Instantiate(prefab, parent);
            }

            if (actions.HasFlag(InteractableActions.ActivateObject))
            {
                activateTarget.gameObject.SetActive(true);
            }

            if (actions.HasFlag(InteractableActions.RaiseEvent))
            {
                stats?.RaiseEvent(raiseEventId);
            }

            return base.Interact(source);
        }

        #endregion

    }
}
#endif
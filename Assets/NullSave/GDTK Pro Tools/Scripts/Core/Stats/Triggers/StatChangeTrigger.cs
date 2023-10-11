#if GDTK
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.GDTK.Stats
{
    [AutoDoc("This component allows you to respond to changes on a specific Stat.")]
    public class StatChangeTrigger : MonoBehaviour
    {

        #region Fields

        [Tooltip("Source of stats")] public StatSourceReference statSource;
        [Tooltip("Stat Source")] public BasicStats stats;
        [Tooltip("Key used to find source")] public string key;

        [Tooltip("Id of stat to subscribe to")] public string statId;
        [Tooltip("Raise event when minimum value changes")] public bool minChanged;
        [Tooltip("Raise event when maximum value changes")] public bool maxChanged;
        [Tooltip("Raise event when value changes")] public bool valueChanged;
        [Tooltip("Raise event when special value changes")] public bool specialChanged;

        [Tooltip("Modifiers to apply when the value changes")] public List<GDTKStatModifier> modifiers;

        [Tooltip("Event raised when changes meeting requirments occur")] public UnityEvent onChanged;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (statSource == StatSourceReference.FindInRegistry)
            {
                stats = ToolRegistry.GetComponent<BasicStats>(key);
            }

            GDTKStat stat = stats.GetStat(statId);
            if (minChanged) stat.expressions.minimum.onValueChanged += RaiseEvent;
            if (maxChanged) stat.expressions.maximum.onValueChanged += RaiseEvent;
            if (valueChanged) stat.expressions.value.onValueChanged += RaiseEvent;
            if (specialChanged) stat.expressions.special.onValueChanged += RaiseEvent;
        }

        private void Reset()
        {
            stats = GetComponent<BasicStats>();
        }

        #endregion

        #region Private Methoeds

        private void RaiseEvent()
        {
            onChanged?.Invoke();

            foreach(GDTKStatModifier mod in modifiers)
            {
                stats.AddStatModifier(mod, null);
            }

        }

        #endregion

    }
}
#endif
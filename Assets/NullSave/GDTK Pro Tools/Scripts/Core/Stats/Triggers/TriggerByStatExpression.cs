#if GDTK
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.GDTK.Stats
{
    [AutoDoc("This component allows you to monitor an expression and respond to it whenever one or more value in the expression changes.")]
    public class TriggerByStatExpression : MonoBehaviour
    {

        #region Fields

        [Tooltip("Source of stats")] public StatSourceReference statSource;
        [Tooltip("Stat Source")] public BasicStats stats;
        [Tooltip("Key used to find source")] public string key;

        [Tooltip("Exression to validate")] public string expression;

        [Tooltip("Event raised whenever expression is true")] public UnityEvent onMatch;
        [Tooltip("Event raised whenever expression is false")] public UnityEvent onMismatch;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (statSource == StatSourceReference.FindInRegistry)
            {
                stats = ToolRegistry.GetComponent<BasicStats>(key);
            }

            stats.onStatsReloaded += Resubscribe;
            Resubscribe();
        }

        private void Reset()
        {
            stats = GetComponent<BasicStats>();
        }

        #endregion

        #region Private Methoeds

        private void Resubscribe()
        {
            GDTKStatsManager.AutoSubscribe(expression, UpdateMatch, stats.source);
            UpdateMatch();
        }

        private void UpdateMatch()
        {
            if(GDTKStatsManager.IsConditionTrue(expression, stats.source))
            {
                onMatch?.Invoke();
            }
            else
            {
                onMismatch?.Invoke();
            }
        }

        #endregion

    }
}
#endif
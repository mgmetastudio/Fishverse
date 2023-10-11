#if GDTK
using System.Collections.Generic;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    public class RaiseStatEvent : MonoBehaviour
    {

        #region Fields

        [Tooltip("Determine if source is a Direct Reference or in the Tool Registry")] public StatSourceReference statSource;
        [Tooltip("Stat Source")] public BasicStats stats;
        [Tooltip("Key used to find source")] public string key;

        [Tooltip("Id of event to raise")] public string eventId;
        [Tooltip("Condition that must be true to raise event")] public string raiseWhen;

        private List<SimpleEvent> subscriptions;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (statSource == StatSourceReference.FindInRegistry)
            {
                stats = ToolRegistry.GetComponent<BasicStats>(key);
            }
        }

        private void OnEnable()
        {
            subscriptions = GDTKStatsManager.AutoSubscribe(raiseWhen, TriggerEvent, stats.source);
            TriggerEvent();
        }

        private void OnDisable()
        {
            if (subscriptions != null)
            {
                for (int i = 0; i < subscriptions.Count; i++)
                {
                    subscriptions[i] -= TriggerEvent;
                }
            }
        }

        private void Reset()
        {
            stats = GetComponent<BasicStats>();
        }

        #endregion

        #region Private Methods

        private void TriggerEvent()
        {
            if (GDTKStatsManager.IsConditionTrue(raiseWhen, stats.source))
            {
                stats.RaiseEvent(eventId);
            }
        }

        #endregion

    }
}
#endif
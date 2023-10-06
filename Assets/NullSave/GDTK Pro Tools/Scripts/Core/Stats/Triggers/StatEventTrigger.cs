#if GDTK
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.GDTK.Stats
{
    [AutoDoc("This component allows you to repond when a specific Stat Event occurs.")]
    public class StatEventTrigger : MonoBehaviour
    {

        #region Fields

        [Tooltip("Determine if source is a Direct Reference or in the Tool Registry")] public StatSourceReference statSource;
        [Tooltip("Stat Source")] public BasicStats stats;
        [Tooltip("Key used to find source")] public string key;

        [Tooltip("Id of event to listen for")] public string eventId;

        [Tooltip("Event raised whenever Stat Event triggers")] public UnityEvent onEventRaised;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (statSource == StatSourceReference.FindInRegistry)
            {
                stats = ToolRegistry.GetComponent<BasicStats>(key);
            }

            stats.onEventTriggered += CheckEvent;
        }

        private void Reset()
        {
            stats = GetComponent<BasicStats>();
        }

        #endregion

        #region Public Methods

        [AutoDoc("Cause the associated Stat Source to raise a specific Stat Event.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public StatEventTrigger source;<br/><br/>    public void ExampleMethod()<br/>    {<br/>        source.RaiseEvent(\"exampleId\");<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of the Stat Event to raise")]
        public void RaiseEvent(string id)
        {
            if (stats != null)
            {
                stats.RaiseEvent(id);
            }
        }

        #endregion

        #region Private Methods

        private void CheckEvent(GDTKEvent statEvent)
        {
            if (statEvent.info.id == eventId)
            {
                onEventRaised?.Invoke();
            }
        }

        #endregion

    }
}
#endif
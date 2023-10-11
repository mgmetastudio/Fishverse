#if GDTK
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.GDTK.Stats
{
    [AutoDoc("This UI component displays information about a specific Status Condition")]
    public class StatusConditionUI : statsInfoUI
    {

        #region Fields

        [Tooltip("Format to use when displaying Tooltip UI"), TextArea(2, 5)] public string tooltipFormat;

        [Tooltip("Event raised when Status Condition becomes active")] public UnityEvent onActivated;
        [Tooltip("Event raised when Status Condition becomes inactive")] public UnityEvent onDeactivated;

        private TooltipClient tooltip;

        #endregion

        #region Properties

        [AutoDoc("Gets the currently associated Status Condition", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    StatusConditionUI source;<br/><br/>    public void ExampleMethod()<br/>    {<br/>        GDTKStatusCondition condition = source.condition;<br/>    }<br/><br/>}")]
        public GDTKStatusCondition condition { get; private set; }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            tooltip = GetComponent<TooltipClient>();
        }

        private void OnDestroy()
        {
            if (condition != null)
            {
                condition.onActivated -= Activated;
                condition.onDeactivated -= Deactivated;
            }
        }

        private void Reset()
        {
            tooltipFormat = "<b>{title}</b>\r\n{description}";
        }

        #endregion

        #region Public Methods

        [AutoDoc("Loads a Status Condition for display", "using NullSave.GDTK;<br/>using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    StatusConditionUI source;<br/><br/>    public void ExampleMethod()<br/>    {<br/>        StatsDatabase database = ToolRegistry.GetComponent<StatsDatabase>();<br/>        GDTKStatusCondition condition = database.statusConditions[0];<br/>        source.Load(condition);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Status Condition to display")]
        public void Load(GDTKStatusCondition statusCondition)
        {
            condition = statusCondition;
            info = statusCondition.info;

            ApplyImage();

            foreach (TemplatedLabel label in labels)
            {
                label.target.text = FormatInfo(label.format);
            }

            condition.onActivated += Activated;
            condition.onDeactivated += Deactivated;

            if (tooltip != null)
            {
                tooltip.tooltip = tooltipFormat
                    .Replace("{id}", statusCondition.info.id)
                    .Replace("{title}", statusCondition.info.title)
                    .Replace("{abbr}", statusCondition.info.abbr)
                    .Replace("{description}", statusCondition.info.description)
                    .Replace("{group}", statusCondition.info.groupName)
                    ;
            }

            if (statusCondition.active)
            {
                Activated(statusCondition);
            }
            else
            {
                Deactivated(statusCondition);
            }
        }

        #endregion

        #region Private Methods

        private void Activated(GDTKStatusCondition condition)
        {
            onActivated?.Invoke();
        }

        private void Deactivated(GDTKStatusCondition condition)
        {
            onDeactivated?.Invoke();
        }

        #endregion

    }
}
#endif
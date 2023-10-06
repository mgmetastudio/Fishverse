#if GDTK
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [AutoDoc("This UI component displays information about a specific Stat.")]
    public class StatUI : statsInfoUI
    {

        #region Variables

        [Tooltip("Determine if source is a Direct Reference or in the Tool Registry")] public StatSourceReference source;
        [Tooltip("Stat Source")] [SerializeField] private BasicStats m_stats;
        [Tooltip("Key used to find source")] public string key;

        [Tooltip("Id of stat controlling value")] [SerializeField] private string m_statId;
        [Tooltip("Display values as ints")] public bool treatAsInt;

        private GDTKStat valueStat;

        #endregion

        #region Properties

        [AutoDoc("Gets/Sets id of Stat to display information for. On set the UI is automatically updated.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public StatUI source;<br/><br/>    public void ExampleMethod()<br/>    {<br/>        source.statId = \"exampleId\";<br/>    }<br/><br/>}")]
        public string statId
        {
            get { return m_statId; }
            set
            {
                if (m_statId == value) return;
                m_statId = value;
                UpdateBinding();
            }
        }

        [AutoDoc("Gets/Sets BasicStats providing class information. On set the UI is automatically updated.", "using NullSave.GDTK;<br/>using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public StatUI source;<br/><br/>    public void ExampleMethod()<br/>    {<br/>        source.statSource = ToolRegistry.GetComponent<BasicStats>(\"Player\");<br/>    }<br/><br/>}")]
        public BasicStats statSource
        {
            get { return m_stats; }
            set
            {
                if (m_stats == value) return;
                source = StatSourceReference.DirectReference;
                m_stats = value;
                UpdateBinding();
            }
        }

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            if (valueStat != null) return;

            if (source == StatSourceReference.FindInRegistry)
            {
                m_stats = ToolRegistry.GetComponent<BasicStats>(key);
            }

            if (m_stats != null)
            {
                UpdateBinding();
            }
        }

        #endregion

        #region Public Methods

        [AutoDoc("Updates both the Stat Id and source at once and refreshes UI.", "using NullSave.GDTK;<br/>using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public StatUI source;<br/><br/>    public void ExampleMethod()<br/>    {<br/>        source.SetClassIdAndSource(ToolRegistry.GetComponent<BasicStats>(\"Player\"), \"exampleId\");<br/>    }<br/><br/>}")]
        [AutoDocParameter("Source providing Class information")]
        [AutoDocParameter("Id of Stat to display information for")]
        public void SetStatIdAndSource(BasicStats source, string id)
        {
            this.source = StatSourceReference.DirectReference;
            m_stats = source;
            m_statId = id;
            UpdateBinding();
        }

        [AutoDoc("Updates the bindings and refreshes the UI", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public StatUI source;<br/><br/>    public void ExampleMethod()<br/>    {<br/>        source.UpdateBinding();<br/>    }<br/><br/>}")]
        public void UpdateBinding()
        {
            m_stats.onStatsReloaded -= UpdateBinding;
            m_stats.onStatsReloaded += UpdateBinding;

            if (valueStat != null)
            {
                valueStat.RemoveSubscription(StatBinding.Everything, UpdateUI);
                valueStat = null;
            }

            if (m_stats == null)
            {
                StringExtensions.LogWarning(name, "StatValueLabel", "No stat source supplied");
                return;
            }

            valueStat = m_stats.GetStat(statId);
            if (valueStat == null)
            {
                StringExtensions.LogWarning(name, "StatValueLabel", "Stat Id " + statId + " not found on source");
                return;
            }

            valueStat.AddSubscription(StatBinding.Everything, UpdateUI);
            UpdateUI();
        }

        #endregion

        #region Private Methods

        private void UpdateUI()
        {
            info = valueStat.info;

            ApplyImage();

            foreach (TemplatedLabel label in labels)
            {
                if (treatAsInt)
                {
                    label.target.text = FormatInfo(label.format)
                        .Replace("{value}", ((int)valueStat.value).ToString())
                        .Replace("{maximum}", ((int)valueStat.maximum).ToString())
                        .Replace("{minimum}", ((int)valueStat.minimum).ToString())
                        .Replace("{special}", ((int)valueStat.special).ToString())
                        .Replace("{valueMod}", ((int)valueStat.expressions.value.modifierTotal).ToString())
                        .Replace("{maximumMod}", ((int)valueStat.expressions.maximum.modifierTotal).ToString())
                        .Replace("{minimumMod}", ((int)valueStat.expressions.minimum.modifierTotal).ToString())
                        .Replace("{regenDelay}", ((int)valueStat.regenerationDelay).ToString())
                        .Replace("{regenRate}", ((int)valueStat.regenerationRate).ToString())
                        ;
                }
                else
                {
                    label.target.text = FormatInfo(label.format)
                        .Replace("{value}", valueStat.value.ToString())
                        .Replace("{maximum}", valueStat.maximum.ToString())
                        .Replace("{minimum}", valueStat.minimum.ToString())
                        .Replace("{special}", valueStat.special.ToString())
                        .Replace("{valueMod}", valueStat.expressions.value.modifierTotal.ToString())
                        .Replace("{maximumMod}", valueStat.expressions.maximum.modifierTotal.ToString())
                        .Replace("{minimumMod}", valueStat.expressions.minimum.modifierTotal.ToString())
                        .Replace("{regenDelay}", valueStat.regenerationDelay.ToString())
                        .Replace("{regenRate}", valueStat.regenerationRate.ToString())
                        ;
                }
            }
        }

        #endregion

    }
}
#endif
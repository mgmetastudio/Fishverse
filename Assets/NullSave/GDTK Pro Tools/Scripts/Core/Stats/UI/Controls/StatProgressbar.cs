#if GDTK
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.GDTK.Stats
{
    [AutoDoc("This UI control can be created via `UI/GDTK: Stats/Stat Progressbar` and allows you to display a progressbar for a Stat. The value will automatically update whenever the Stat changes.")]
    [RequireComponent(typeof(Progressbar))]
    public class StatProgressbar : MonoBehaviour, IStatClient
    {

        #region Variables

        [Tooltip("Determine if source is a Direct Reference or in the Tool Registry")] public StatSourceReference source;
        [Tooltip("Stat Source")] [SerializeField] private BasicStats m_stats;
        [Tooltip("Key used to find source")] public string key;

        [Tooltip("Id of stat controlling value")] public string valueId;
        [Tooltip("Id of stat controlling minimum (if different)")] public string minimumId;
        [Tooltip("Id of stat controlling maximum (if different)")] public string maximumId;

        [Tooltip("Image to display stat sprite")] public Image image;

        [Tooltip("Apply color to Image")] public bool colorImage;
        [Tooltip("Apply color to Progressbar Fill")] public bool colorProgress;

        private Progressbar Progressbar;
        private GDTKStat valueStat, minStat, maxStat;

        #endregion

        #region Properties

        [AutoDoc("Gets/Sets id of Stat to display information for. On set the UI is automatically updated.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public StatProgressbar source;<br/><br/>    public void ExampleMethod()<br/>    {<br/>        source.statId = \"exampleId\";<br/>    }<br/><br/>}")]
        public string statId
        {
            get { return valueId; }
            set
            {
                if (valueId == value) return;
                valueId = value;
                Rebind();
            }
        }

        [AutoDoc("Gets/Sets BasicStats providing class information. On set the UI is automatically updated.", "using NullSave.GDTK;<br/>using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public StatProgressbar source;<br/><br/>    public void ExampleMethod()<br/>    {<br/>        source.statSource = ToolRegistry.GetComponent<BasicStats>(\"Player\");<br/>    }<br/><br/>}")]
        public BasicStats statSource
        {
            get { return m_stats; }
            set
            {
                if (m_stats == value) return;
                if (m_stats != null)
                {
                    m_stats.onStatsReloaded -= Rebind;
                }
                source = StatSourceReference.DirectReference;
                m_stats = value;
                Rebind();
            }
        }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            Progressbar = GetComponent<Progressbar>();
        }

        private void OnEnable()
        {
            if (valueStat != null) return;

            if (source == StatSourceReference.FindInRegistry)
            {
                m_stats = ToolRegistry.GetComponent<BasicStats>(key);
            }

            Rebind();
        }

        #endregion

        #region Public Methods

        [AutoDoc("Updates both the Stat Id and source at once and refreshes UI.", "using NullSave.GDTK;<br/>using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public StatProgressbar source;<br/><br/>    public void ExampleMethod()<br/>    {<br/>        source.SetStatIdAndSource(ToolRegistry.GetComponent<BasicStats>(\"Player\"), \"exampleId\");<br/>    }<br/><br/>}")]
        [AutoDocParameter("Source providing Stat information")]
        [AutoDocParameter("Id of Stat to display information for")]
        public void SetStatIdAndSource(BasicStats source, string id)
        {
            valueId = id;
            statSource = source;
        }

        [AutoDoc("Updates the bindings and refreshes the UI", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public StatProgressbar source;<br/><br/>    public void ExampleMethod()<br/>    {<br/>        source.Rebind();<br/>    }<br/><br/>}")]
        public void Rebind()
        {
            if (m_stats == null) return;
            m_stats.onStatsReloaded -= Rebind;
            m_stats.onStatsReloaded += Rebind;

            if (valueStat != null)
            {
                valueStat.expressions.value.onValueChanged -= UpdateStat;
            }
            if (minStat != null)
            {
                minStat.expressions.value.onValueChanged -= UpdateStat;
            }
            if (maxStat != null)
            {
                maxStat.expressions.value.onValueChanged -= UpdateStat;
            }

            valueStat = m_stats.GetStat(valueId);
            if (valueStat == null)
            {
                StringExtensions.LogError(name, "StatProgressbar", valueId + " is not a known stat");
                enabled = false;
                return;
            }
            valueStat.expressions.value.onValueChanged += UpdateStat;
            if (image != null)
            {
                image.sprite = valueStat.info.image.GetImage();
            }

            if (!string.IsNullOrEmpty(minimumId))
            {
                minStat = m_stats.GetStat(minimumId);
                if (minStat == null)
                {
                    StringExtensions.LogError(name, "StatProgressbar", minimumId + " is not a known stat");
                    enabled = false;
                    return;
                }
            }

            if (!string.IsNullOrEmpty(maximumId))
            {
                maxStat = m_stats.GetStat(maximumId);
                if (maxStat == null)
                {
                    StringExtensions.LogError(name, "StatProgressbar", maximumId + " is not a known stat");
                    enabled = false;
                    return;
                }
            }

            if(colorImage && image != null)
            {
                image.color = valueStat.info.color;
            }

            if (colorProgress)
            {
                Progressbar.targetGraphic.color = valueStat.info.color;
            }


            UpdateStat();
        }

        #endregion

        #region Private Methods

        private void UpdateStat()
        {
            if (valueStat == null || Progressbar == null) return;

            if (minStat != null)
            {
                Progressbar.minValue = minStat.value;
            }
            else
            {
                Progressbar.minValue = valueStat.minimum;
            }
            if (maxStat != null)
            {
                Progressbar.maxValue = maxStat.value;
            }
            else
            {
                Progressbar.maxValue = valueStat.maximum;
            }

            Progressbar.value = valueStat.value;
        }

        #endregion

    }
}
#endif
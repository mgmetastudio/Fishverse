#if GDTK
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.GDTK.Stats
{
    [AutoDoc("This UI component displays a list of Stats for a Stat Source")]
    public class StatListUI : MonoBehaviour, IStatClient
    {

        #region Fields

        [Tooltip("Determine if source is a Direct Reference or in the Tool Registry")] public StatSourceReference source;
        [Tooltip("Stat Source")] [SerializeField] private BasicStats m_stats;
        [Tooltip("Key used to find source")] public string key;

        [Tooltip("Prefab to create for displaying stat")] public StatUI uiPrefab;
        [Tooltip("Transform to place prefabs inside")] public Transform content;

        [Tooltip("Event triggered whenever the list is updated")] public UnityEvent onListUpdated;

        private List<StatUI> loaded;
        private SimpleEvent subscription;

        #endregion

        #region Properties

        [AutoDoc("Unused", "")]
        public string statId
        {
            get { return null; }
            set { }
        }

        [AutoDoc("Gets/Sets the Stat Source for this object. On set UI is automatically updated.", "using NullSave.GDTK;<br/>using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    StatListUI source;<br/><br/>    public void ExampleMethod()<br/>    {<br/>        source.statSource = ToolRegistry.GetComponent<BasicStats>(\"Player\");<br/>    }<br/><br/>}")]
        public BasicStats statSource
        {
            get { return m_stats; }
            set
            {
                if (m_stats == value) return;
                source = StatSourceReference.DirectReference;
                m_stats = value;
                if(subscription != null)
                {
                    subscription -= UpdateBinding;
                }
                if(m_stats != null)
                {
                    m_stats.onStatsReloaded += UpdateBinding;
                    subscription = m_stats.onStatsReloaded;
                }
                UpdateBinding();
            }
        }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (uiPrefab != null && uiPrefab.gameObject.scene.buildIndex != -1)
            {
                uiPrefab.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            if (m_stats != null) return;

            if (source == StatSourceReference.FindInRegistry)
            {
                m_stats = ToolRegistry.GetComponent<BasicStats>(key);
            }

            UpdateBinding();
        }

        private void Start()
        {
            if (m_stats == null)
            {
                StringExtensions.LogWarning(name, "StatValueLabel", "No stat source supplied");
                return;
            }

            UpdateBinding();
        }

        private void Reset()
        {
            content = transform;
        }

        #endregion

        #region Public Methos

        [AutoDoc("Sets the Stat Source and updates the UI.", "using NullSave.GDTK;<br/>using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    StatListUI source;<br/><br/>    public void ExampleMethod()<br/>    {<br/>        source.SetStatIdAndSource(ToolRegistry.GetComponent<BasicStats>(\"Player\"), null);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Source of Stats")]
        [AutoDocParameter("Unused")]
        public void SetStatIdAndSource(BasicStats source, string id)
        {
            this.source = StatSourceReference.DirectReference;
            m_stats = source;
            UpdateBinding();
        }

        [AutoDoc("Updates the UI for current settings.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    StatListUI source;<br/><br/>    public void ExampleMethod()<br/>    {<br/>        source.UpdateBinding();<br/>    }<br/><br/>}")]
        public void UpdateBinding()
        {
            if(m_stats == null)
            {
                StringExtensions.LogError(name, "StatListUI", "No Stat Source provided.");
                enabled = false;
                return;
            }

            if (loaded == null)
            {
                StatUI ui;
                loaded = new List<StatUI>();

                foreach (var entry in m_stats.stats)
                {
                    if (!entry.Value.info.hidden)
                    {
                        ui = Instantiate(uiPrefab, content);
                        ui.SetStatIdAndSource(m_stats, entry.Value.info.id);
                        ui.gameObject.SetActive(true);
                        loaded.Add(ui);
                    }
                }

                onListUpdated?.Invoke();
                return;
            }

            foreach (StatUI ui in loaded)
            {
                ui.SetStatIdAndSource(m_stats, ui.statId);
            }
            onListUpdated?.Invoke();
        }

        #endregion

    }
}
#endif
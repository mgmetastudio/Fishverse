#if GDTK
using System.Collections.Generic;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [AutoDoc("Displays a list of all Status Conditions on a Stat Source.")]
    public class StatusConditionListUI : MonoBehaviour
    {

        #region Fields

        [Tooltip("Determine if source is a Direct Reference or in the Tool Registry")] public StatSourceReference source;
        [Tooltip("Stat Source")] [SerializeField] private StatsAndEffects m_stats;
        [Tooltip("Key used to find source")] public string key;

        [Tooltip("Prefab to create for displaying status conditons")] public StatusConditionUI uiPrefab;
        [Tooltip("Transform to place prefabs inside")] public Transform content;
        [Tooltip("Show conditions even when they are inactive")] public bool showInactive;

        private Dictionary<GDTKStatusCondition, StatusConditionUI> loaded;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            loaded = new Dictionary<GDTKStatusCondition, StatusConditionUI>();

            if (uiPrefab != null && uiPrefab.gameObject.scene.buildIndex != -1)
            {
                uiPrefab.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (m_stats != null)
            {
                m_stats.onStatusConditionAdded -= StatusConditionAdded;
                m_stats.onStatusConditionRemoved -= StatusConditionRemoved;
                m_stats.onStatsReloaded -= ResetLoaded;
            }
        }

        private void Start()
        {
            if (source == StatSourceReference.FindInRegistry)
            {
                m_stats = ToolRegistry.GetComponent<StatsAndEffects>(key);
            }

            if (m_stats == null)
            {
                StringExtensions.LogWarning(name, "StatusConditionListUI", "No stat source supplied");
                return;
            }

            m_stats.onStatusConditionAdded += StatusConditionAdded;
            m_stats.onStatusConditionRemoved += StatusConditionRemoved;
            m_stats.onStatsReloaded += ResetLoaded;

            // Get any already active attributes
            foreach (GDTKStatusCondition condition in m_stats.statusConditions)
            {
                StatusConditionAdded(condition);
            }
        }

        private void Reset()
        {
            content = transform;
        }

        #endregion

        #region Private Methods

        private void AddStatusCondition(GDTKStatusCondition condition)
        {
            if (loaded.ContainsKey(condition)) return;

            StatusConditionUI ui = Instantiate(uiPrefab, content);
            ui.Load(condition);
            ui.gameObject.SetActive(condition.active);
            loaded.Add(condition, ui);
        }

        private void RemoveStatusCondition(GDTKStatusCondition condition)
        {
            if (!loaded.ContainsKey(condition)) return;

            Destroy(loaded[condition].gameObject);
            loaded.Remove(condition);
        }

        private void ResetLoaded()
        {
            foreach (var item in loaded)
            {
                Destroy(item.Value.gameObject);
            }
            loaded.Clear();
        }

        private void StatusConditionActivated(GDTKStatusCondition condition)
        {
            AddStatusCondition(condition);
        }

        private void StatusConditionAdded(GDTKStatusCondition condition)
        {
            condition.onActivated += StatusConditionActivated;
            condition.onDeactivated += StatusConditionDeactivated;
            if(condition.active || showInactive)
            {
                AddStatusCondition(condition);
            }
        }

        private void StatusConditionDeactivated(GDTKStatusCondition condition)
        {
            if (!showInactive) RemoveStatusCondition(condition);
        }

        private void StatusConditionRemoved(GDTKStatusCondition condition)
        {
            condition.onActivated -= StatusConditionActivated;
            condition.onDeactivated -= StatusConditionDeactivated;
            RemoveStatusCondition(condition);
        }

        #endregion

    }
}
#endif
#if GDTK
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [AutoDoc("Displays a list of all Status Effects active on a Stat Source")]
    public class StatusEffectListUI : MonoBehaviour
    {

        #region Variables

        [Tooltip("Determine if source is a Direct Reference or in the Tool Registry")] public StatSourceReference source;
        [Tooltip("Stat Source")] [SerializeField] private StatsAndEffects m_stats;
        [Tooltip("Key used to find source")] public string key;

        [Tooltip("Prefab to create for displaying status effect")] public StatusEffectUI uiPrefab;
        [Tooltip("Transform to place prefabs inside")] public Transform content;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (uiPrefab != null && uiPrefab.gameObject.scene.buildIndex != -1)
            {
                uiPrefab.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if(m_stats != null)
            {
                m_stats.onEffectAdded -= EffectAdded;
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
                StringExtensions.LogWarning(name, "StatValueLabel", "No stat source supplied");
                return;
            }

            m_stats.onEffectAdded += EffectAdded;

            // Get any already active effects
            foreach(GDTKStatusEffect effect in m_stats.activeEffects)
            {
                EffectAdded(effect);
            }
        }

        private void Reset()
        {
            content = transform;
        }

        #endregion

        #region Private Methods

        private void EffectAdded(GDTKStatusEffect effect)
        {
            StatusEffectUI ui = Instantiate(uiPrefab, content);
            ui.gameObject.SetActive(true);
            ui.Load(effect);
        }

        #endregion

    }
}
#endif
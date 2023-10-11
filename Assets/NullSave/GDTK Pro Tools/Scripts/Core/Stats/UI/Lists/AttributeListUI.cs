#if GDTK
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.GDTK.Stats
{
    [AutoDoc("UI object displaying a list of all Attributes available on a Stat Source.")]
    public class AttributeListUI : MonoBehaviour
    {

        #region Fields

        [Tooltip("Determine if source is a Direct Reference or in the Tool Registry")] public StatSourceReference source;
        [Tooltip("Stat Source")] [SerializeField] private StatsAndEffects m_stats;
        [Tooltip("Key used to find source")] public string key;

        [Tooltip("Prefab to create for displaying attribute")] public AttributeUI uiPrefab;
        [Tooltip("Transform to place prefabs inside")] public Transform content;

        [Tooltip("Event triggered whenever the list is updated")] public UnityEvent onListUpdated;

        private List<string> loadedAttribs;
        private List<AttributeUI> loaded;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            loaded = new List<AttributeUI>();
            loadedAttribs = new List<string>();

            if (uiPrefab != null && uiPrefab.gameObject.scene.buildIndex != -1)
            {
                uiPrefab.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (m_stats != null)
            {
                m_stats.onAttributeAdded -= AttributeAdded;
                m_stats.onAttributeRemoved -= AttributeRemoved;
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
                StringExtensions.LogWarning(name, "AttributeListUI", "No stat source supplied");
                return;
            }

            m_stats.onAttributeAdded += AttributeAdded;
            m_stats.onAttributeRemoved += AttributeRemoved;

            // Get any already active attributes
            foreach (GDTKAttribute attribute in m_stats.attributes)
            {
                AttributeAdded(attribute);
            }
        }

        private void Reset()
        {
            content = transform;
        }

        #endregion

        #region Private Methods

        private void AttributeAdded(GDTKAttribute attribute)
        {
            if (loadedAttribs.Contains(attribute.info.id) || attribute.info.hidden) return;

            AttributeUI ui = Instantiate(uiPrefab, content);
            ui.Load(attribute);
            ui.gameObject.SetActive(true);
            loaded.Add(ui);
            loadedAttribs.Add(attribute.info.id);

            onListUpdated?.Invoke();
        }

        private void AttributeRemoved(GDTKAttribute attribute)
        {
            if (!loadedAttribs.Contains(attribute.info.id)) return;

            loadedAttribs.Remove(attribute.info.id);
            foreach (AttributeUI ui in loaded)
            {
                if (ui.info.id == attribute.info.id)
                {
                    Destroy(ui.gameObject);
                    loaded.Remove(ui);
                    onListUpdated?.Invoke();
                    return;
                }
            }
        }

        #endregion

    }
}
#endif
#if GDTK
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.GDTK.Stats
{
    [AutoDoc("Displays a list of all Perks on a Stat Source.")]
    public class PerkListUI : MonoBehaviour
    {

        #region Fields

        [Tooltip("Determine if source is a Direct Reference or in the Tool Registry")] public StatSourceReference source;
        [Tooltip("Stat Source")] [SerializeField] private PlayerCharacterStats m_stats;
        [Tooltip("Key used to find source")] public string key;

        [Tooltip("Prefab to create for displaying perk info")] public PerkUI uiPrefab;
        [Tooltip("Transform to place prefabs inside")] public Transform content;

        [Tooltip("Event triggered whenever the list is updated")] public UnityEvent onListUpdated;

        private List<string> loadedAttribs;
        private List<PerkUI> loaded;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            loaded = new List<PerkUI>();
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
                m_stats.onPerkAdded -= PerkAdded;
                m_stats.onPerkRemoved -= PerkRemoved;
            }
        }

        private void Start()
        {
            if (source == StatSourceReference.FindInRegistry)
            {
                m_stats = ToolRegistry.GetComponent<PlayerCharacterStats>(key);
            }

            if (m_stats == null)
            {
                StringExtensions.LogWarning(name, "PerkListUI", "No stat source supplied");
                return;
            }

            m_stats.onPerkAdded += PerkAdded;
            m_stats.onPerkRemoved += PerkRemoved;

            // Get any already active Perks
            foreach (GDTKPerk perk in m_stats.perks)
            {
                PerkAdded(perk);
            }

            onListUpdated?.Invoke();
        }

        private void Reset()
        {
            content = transform;
        }

        #endregion

        #region Private Methods

        private void PerkAdded(GDTKPerk Perk)
        {
            if (loadedAttribs.Contains(Perk.info.id) || Perk.info.hidden) return;

            PerkUI ui = Instantiate(uiPrefab, content);
            ui.Load(Perk);
            ui.gameObject.SetActive(true);
            loaded.Add(ui);
            loadedAttribs.Add(Perk.info.id);
            onListUpdated?.Invoke();
        }

        private void PerkRemoved(GDTKPerk Perk)
        {
            if (!loadedAttribs.Contains(Perk.info.id)) return;

            loadedAttribs.Remove(Perk.info.id);
            foreach (PerkUI ui in loaded)
            {
                if (ui.info.id == Perk.info.id)
                {
                    loaded.Remove(ui);
                    Destroy(ui.gameObject);
                    onListUpdated?.Invoke();
                    return;
                }
            }
        }

        #endregion

    }
}
#endif
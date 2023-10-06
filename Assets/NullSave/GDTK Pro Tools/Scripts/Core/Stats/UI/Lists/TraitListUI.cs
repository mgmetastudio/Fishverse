#if GDTK
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.GDTK.Stats
{
    public class TraitListUI : MonoBehaviour
    {

        #region Fields

        [Tooltip("Load from Stat Source")] public bool loadFromStats;
        [Tooltip("Determine if source is a Direct Reference or in the Tool Registry")] public StatSourceReference source;
        [Tooltip("Stat Source")] [SerializeField] private PlayerCharacterStats m_stats;
        [Tooltip("Key used to find source")] public string key;

        [Tooltip("Prefab to create for displaying Trait")] public TraitUI prefab;
        [Tooltip("Transform to place prefabs inside")] public Transform content;

        [Tooltip("Event triggered whenever the list is updated")] public UnityEvent onListUpdated;

        private List<TraitUI> loaded;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            loaded = new List<TraitUI>();

            if (source == StatSourceReference.FindInRegistry)
            {
                m_stats = ToolRegistry.GetComponent<PlayerCharacterStats>();
            }

            if (prefab != null && prefab.gameObject.scene.buildIndex != -1)
            {
                prefab.gameObject.SetActive(false);
            }

            if (loadFromStats)
            {
                Load(m_stats.traits.ToList());
                m_stats.onTraitAdded += TraitAdded;
                m_stats.onTraitRemoved += TraitRemoved;
                m_stats.onStatsReloaded += Clear;
            }
        }

        private void Reset()
        {
            content = transform;
        }

        #endregion

        #region Public Methods

        public void Clear()
        {
            if(loaded == null)
            {
                loaded = new List<TraitUI>();
                return;
            }

            foreach (TraitUI item in loaded)
            {
                Destroy(item.gameObject);
            }
            loaded.Clear();
            onListUpdated?.Invoke();
        }

        public void Load(List<GDTKTrait> traits)
        {
            Clear();

            foreach (GDTKTrait trait in traits)
            {
                TraitUI ui = Instantiate(prefab, content);
                ui.Load(trait);
                ui.gameObject.SetActive(true);
                loaded.Add(ui);
            }

            onListUpdated?.Invoke();
        }

        #endregion

        #region Private Methods

        private void TraitAdded(GDTKTrait trait)
        {
            TraitUI ui = Instantiate(prefab, content);
            ui.Load(trait);
            ui.gameObject.SetActive(true);
            loaded.Add(ui);

            onListUpdated?.Invoke();
        }

        private void TraitRemoved(GDTKTrait trait)
        {
            foreach(TraitUI ui in loaded)
            {
                if(ui.trait.info.id == trait.info.id)
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
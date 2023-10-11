#if GDTK
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.GDTK.Stats
{
    [AutoDoc("Displays a list of all Classes on a Stat Source.")]
    public class ClassListUI : MonoBehaviour
    {

        #region Fields

        [Tooltip("Determine if source is a Direct Reference or in the Tool Registry")] public StatSourceReference source;
        [Tooltip("Stat Source")] [SerializeField] private PlayerCharacterStats m_stats;
        [Tooltip("Key used to find source")] public string key;

        [Tooltip("Prefab to create for displaying class")] public ClassUI uiPrefab;
        [Tooltip("Transform to place prefabs inside")] public Transform content;

        [Tooltip("Event raised when there are no classes on the target")] public UnityEvent onNoClasses;
        [Tooltip("Event raised when target has 1 or more classes")] public UnityEvent onHasClasses;
        [Tooltip("Event raised when a change is made to classes")] public UnityEvent onClassesChanged;

        private List<ClassUI> loaded;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            loaded = new List<ClassUI>();

            if (uiPrefab != null && uiPrefab.gameObject.scene.buildIndex != -1)
            {
                uiPrefab.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (m_stats != null)
            {
                m_stats.onClassAdded -= ClassAdded;
                m_stats.onClassRemoved -= ClassRemoved;
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
                StringExtensions.LogWarning(name, "ClassListUI", "No stat source supplied");
                return;
            }

            if(m_stats.classes.Count == 0)
            {
                onNoClasses?.Invoke();
            }
            else
            {
                onHasClasses?.Invoke();
            }

            m_stats.onClassAdded += ClassAdded;
            m_stats.onClassRemoved += ClassRemoved;
            m_stats.onClassesChanged += () => onClassesChanged?.Invoke();

            // Get any already active attributes
            foreach (GDTKClass playerClass in m_stats.classes)
            {
                ClassAdded(playerClass);
            }
        }

        private void Reset()
        {
            content = transform;
        }

        #endregion

        #region Private Methods

        private void ClassAdded(GDTKClass playerClass)
        {
            foreach(ClassUI loadedUI in loaded)
            {
                if (loadedUI.id == playerClass.info.id) return;
            }

            ClassUI ui = Instantiate(uiPrefab, content);
            ui.Load(m_stats, playerClass.info.id);
            ui.gameObject.SetActive(true);
            loaded.Add(ui);

            onHasClasses?.Invoke();
        }

        private void ClassRemoved(GDTKClass playerClass)
        {

            if (m_stats.classes.Count == 0)
            {
                onNoClasses?.Invoke();
            }
            else
            {
                onHasClasses?.Invoke();
            }

            foreach (ClassUI loadedUI in loaded)
            {
                if (loadedUI.id == playerClass.info.id)
                {
                    Destroy(loadedUI.gameObject);
                    loaded.Remove(loadedUI);
                    return;
                }
            }
        }

        #endregion

    }
}
#endif
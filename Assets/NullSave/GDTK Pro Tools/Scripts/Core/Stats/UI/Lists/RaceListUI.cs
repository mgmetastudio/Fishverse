#if GDTK
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.GDTK.Stats
{
    [AutoDoc("This UI component displays a list of all available races.")]
    public class RaceListUI : MonoBehaviour
    {

        #region Fields

        [Tooltip("Prefab to create for displaying race")] public RaceUI uiPrefab;
        [Tooltip("Transform to place prefabs inside")] public Transform content;
        [Tooltip("Destory object on close event")] public bool destroyOnClose;

        [Tooltip("Automatically submit when a race is selected")] public bool autoSubmit;
        [Tooltip("Assign selected race to a Player Character Stats")] public bool assignSelected;
        [Tooltip("Determine if source is a Direct Reference or in the Tool Registry")] public StatSourceReference source;
        [Tooltip("Stat Source")] [SerializeField] private PlayerCharacterStats m_stats;
        [Tooltip("Key used to find source")] public string key;

        [Tooltip("Event raised when list opens")] public UnityEvent onOpen;
        [Tooltip("Event raised when list closes")] public UnityEvent onClose;
        [Tooltip("Event raised when selection is submitted")] public UnityEvent onSubmit;
        [Tooltip("Event raised when selection is changed")] public UnityEvent onSelectionChanged;

        private StatsDatabase db;
        private GDTKRace m_selected;
        private List<RaceUI> loaded;

        #endregion

        #region Properties


        public GDTKRace selected
        {
            get { return m_selected; }
            set
            {
                if (m_selected == value) return;
                m_selected = value;
                foreach (RaceUI ui in loaded)
                {
                    ui.selected = ui.race == value;
                }
                onSelectionChanged?.Invoke();
            }
        }


        #endregion

        #region Unity Methods

        private void Awake()
        {
            loaded = new List<RaceUI>();

            InterfaceManager.PreventInteractions = true;
            InterfaceManager.LockPlayerController = true;

            if (uiPrefab != null && uiPrefab.gameObject.scene.buildIndex != -1)
            {
                uiPrefab.gameObject.SetActive(false);
            }

            db = ToolRegistry.GetComponent<StatsDatabase>();
            if (db == null)
            {
                StringExtensions.LogError(name, "Awake", "No Stats Database found");
                return;
            }

            foreach (GDTKRace race in db.races)
            {
                RaceUI ui = Instantiate(uiPrefab, content);
                ui.Load(race);
                ui.gameObject.SetActive(true);
                ui.onClick.AddListener(() => SelectRace(ui));
                loaded.Add(ui);
            }
        }

        private void OnDisable()
        {
            InterfaceManager.PreventInteractions = false;
            InterfaceManager.LockPlayerController = false;
        }

        private void OnEnable()
        {
            if (m_selected != null)
            {
                foreach (RaceUI ui in loaded)
                {
                    ui.selected = false;
                }
                m_selected = null;
            }
            InterfaceManager.PreventInteractions = true;
            InterfaceManager.LockPlayerController = true;
        }

        private void Reset()
        {
            assignSelected = true;
            content = transform;
        }

        #endregion

        #region Public Methods

        public void Close()
        {
            if (destroyOnClose)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void Submit()
        {
            if (assignSelected)
            {
                if (source == StatSourceReference.FindInRegistry)
                {
                    m_stats = ToolRegistry.GetComponent<PlayerCharacterStats>(key);
                }
                m_stats.race = selected;
            }

            Close();
        }

        #endregion

        #region Private Methods

        private void SelectRace(RaceUI raceUI)
        {
            selected = raceUI.race;
            if (autoSubmit)
            {
                Submit();
            }
        }

        #endregion

    }
}
#endif
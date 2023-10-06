#if GDTK
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.GDTK.Stats
{
    public class BackgroundUI : statsInfoUI
    {

        #region Fields

        [Tooltip("Determine if source is a Direct Reference or in the Tool Registry")] public StatSourceReference source;
        [Tooltip("Stat Source")] [SerializeField] private PlayerCharacterStats m_stats;
        [Tooltip("Key used to find source")] public string key;

        [Tooltip("Show list of associated Traits")] public bool showTraits;
        [Tooltip("List used to display Traits")] public TraitListUI traitList;

        [Tooltip("Event fired when item is selected")] public UnityEvent onSelected;
        [Tooltip("Event fired when item is loses selection")] public UnityEvent onDeselected;

        private bool m_selected;

        #endregion

        #region Properties

        public GDTKBackground background { get; private set; }

        public bool selected
        {
            get { return m_selected; }
            set
            {
                if (m_selected == value) return;
                m_selected = value;
                if(m_selected)
                {
                    onSelected?.Invoke();
                }
                else
                {
                    onDeselected?.Invoke();
                }
            }
        }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (source == StatSourceReference.FindInRegistry)
            {
                m_stats = ToolRegistry.GetComponent<PlayerCharacterStats>(key);
            }

            if(m_stats != null)
            {
                m_stats.onBackgroundChanged += () => Load(m_stats.background);
                Load(m_stats.background);
            }

            onDeselected?.Invoke();
        }

        #endregion

        #region Public Methods

        public void Load(GDTKBackground background)
        {
            this.background = background;
            info = background?.info;

            ApplyImage();

            foreach (TemplatedLabel label in labels)
            {
                label.target.text = FormatInfo(label.format);
            }

            if (showTraits)
            {
                traitList?.Load(background?.traits);
            }
        }

        #endregion

    }
}
#endif
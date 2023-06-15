using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Stats
{
    public class InteractableTile : MonoBehaviour
    {

        #region Variables

        public float maxInteractDistance = 1;
        public GameObject interactIndicator;
        public bool hideAfterInteract = true;

        public StatConditionalBool canInteract = new StatConditionalBool() { value = true };
        public UnityEvent onInteract;

        private bool showInteractMarker;
        private GameObject spawnedIndicator;

        private FogOfWarTarget fow;

        #endregion

        #region Properties

        public bool ShowInteractMarker
        {
            get { return showInteractMarker; }
            set
            {
                if (fow != null && !fow.IsVisible) value = false;
                showInteractMarker = value;

                if (!showInteractMarker)
                {
                    if (spawnedIndicator != null) Destroy(spawnedIndicator);
                }
                else
                {
                    spawnedIndicator = Instantiate(interactIndicator, transform);
                }
            }
        }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            fow = GetComponentInChildren<FogOfWarTarget>();
        }

        #endregion

        #region Public Methods

        public virtual bool Interact(StatsCog statSource)
        {
            if (!canInteract.GetValue(statSource)) return false;

            if(hideAfterInteract)
            {
                ShowInteractMarker = false;
            }

            onInteract?.Invoke();
            return true;
        }

        #endregion

    }
}
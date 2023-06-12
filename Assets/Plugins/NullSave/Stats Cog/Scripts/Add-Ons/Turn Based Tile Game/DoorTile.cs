using UnityEngine.Events;

namespace NullSave.TOCK.Stats
{
    public class DoorTile : InteractableTile
    {

        #region Variables

        public bool isOpen;
        public StatConditionalBool canOpen, canClose = new StatConditionalBool() { value = true };
        public UnityEvent onOpen, onClose;

        #endregion

        #region Public Methods

        public override bool Interact(StatsCog statSource)
        {
            if (!canInteract.GetValue(statSource)) return false;

            if(isOpen)
            {
                if (!canClose.GetValue(statSource)) return false;
                isOpen = false;
                onClose?.Invoke();
            }
            else
            {
                if (!canOpen.GetValue(statSource)) return false;
                isOpen = true;
                onOpen?.Invoke();
            }

            // Update Vision
            FogOfWar3D fow = FindObjectOfType<FogOfWar3D>();
            if(fow != null)
            {
                fow.RefreshViewers();
            }

            if (hideAfterInteract)
            {
                ShowInteractMarker = false;
            }

            onInteract?.Invoke();
            return true;
        }

        #endregion

    }
}
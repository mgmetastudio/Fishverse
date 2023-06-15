using UnityEngine;

namespace NullSave.TOCK.Stats
{
    public class MoveableTile : MonoBehaviour
    {

        #region Variables

        public int movementCost = 5;
        public bool canWalk = true;

        public GameObject moveIndicator;
        public GameObject targetIndicator;

        private bool showMoveMarker;
        private bool showTargetMarker;
        private GameObject spawnedIndicator;

        private FogOfWarTarget fow;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            fow = GetComponentInChildren<FogOfWarTarget>();
        }

        #endregion

        #region Properties

        public bool ShowTargetMarker
        {
            get { return showTargetMarker; }
            set
            {
                if (fow != null && !fow.IsVisible) value = false;
                showTargetMarker = value;

                if (!showTargetMarker)
                {
                    if (spawnedIndicator != null) Destroy(spawnedIndicator);
                }
                else
                {
                    ShowMoveMarker = false;
                    spawnedIndicator = Instantiate(targetIndicator, transform);
                }
            }
        }

        public bool ShowMoveMarker
        {
            get { return showMoveMarker; }
            set
            {
                if (fow != null && !fow.IsVisible) value = false;
                showMoveMarker = value;

                if (!ShowMoveMarker)
                {
                    if (spawnedIndicator != null) Destroy(spawnedIndicator);
                }
                else
                {
                    spawnedIndicator = Instantiate(moveIndicator, transform);
                }
            }
        }

        #endregion

    }
}
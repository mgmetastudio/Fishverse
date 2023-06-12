using UnityEngine;

namespace NullSave.TOCK.Stats
{
    public class GlobalStats
    {

        #region Variables

        private static StatsCog globalStats;

        #endregion

        #region Properties

        public static StatsCog StatsCog
        {
            get { return globalStats; }
            set
            {
                if (globalStats == value) return;
                if(globalStats != null)
                {
                    Debug.Log("Replacing Global Stats (" + globalStats.name + ") with (" + value.name + ")");
                }
                globalStats = value;
            }
        }

        #endregion

    }
}
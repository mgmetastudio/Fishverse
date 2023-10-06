#if GDTK
using System;

namespace NullSave.GDTK.JSON
{
    [Serializable]
    public class jsonPlayerStatList : jsonNPCList
    {

        #region Fields

        public string levelId;
        public bool preventLevelGain;
        public string raceId;
        public string backgroundId;
        public string classId;

        #endregion

    }
}
#endif
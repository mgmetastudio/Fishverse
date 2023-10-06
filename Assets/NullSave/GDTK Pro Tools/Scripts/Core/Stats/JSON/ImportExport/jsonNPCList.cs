#if GDTK
using NullSave.GDTK.Stats;
using System;
using System.Collections.Generic;

namespace NullSave.GDTK.JSON
{
    [Serializable]
    public class jsonNPCList : jsonStatsList
    {

        #region Fields

        public bool respawnData;
        public string respawnCondition;
        public bool savePosition;
        public bool saveRotation;
        public List<GDTKStatModifier> respawnModifiers;
        public string saveFilename;

        #endregion

        #region Constructor

        public jsonNPCList()
        {
            respawnModifiers = new List<GDTKStatModifier>();
        }

        #endregion

    }
}
#endif
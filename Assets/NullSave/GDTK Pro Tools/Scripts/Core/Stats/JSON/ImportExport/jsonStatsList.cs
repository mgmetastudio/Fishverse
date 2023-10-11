#if GDTK
using NullSave.GDTK.Stats;
using System;
using System.Collections.Generic;

namespace NullSave.GDTK.JSON
{
    [Serializable]
    public class jsonStatsList
    {

        #region Fields

        public List<string> attributes;
        public List<string> conditions;
        public List<GDTKStat> stats;

        #endregion

        #region Constructor

        public jsonStatsList()
        {
            attributes = new List<string>();
            conditions = new List<string>();
            stats = new List<GDTKStat>();
        }
        
        #endregion

    }
}
#endif
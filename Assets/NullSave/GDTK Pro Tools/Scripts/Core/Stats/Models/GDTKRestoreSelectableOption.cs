#if GDTK
using System;

namespace NullSave.GDTK.Stats
{
    public class GDTKRestoreSelectableOption : ISelectableOption
    {

        #region Properties

        public BasicInfo optionInfo { get; set; }

        public Type type { get; set; }

        #endregion

        #region Constructor

        public GDTKRestoreSelectableOption(string id, string typeName)
        {
            optionInfo = new BasicInfo();
            optionInfo.id = id;
            type = Type.GetType(typeName);
        }

        #endregion

    }
}
#endif
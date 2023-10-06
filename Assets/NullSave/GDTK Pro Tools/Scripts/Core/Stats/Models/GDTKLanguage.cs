#if GDTK
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [Serializable]
    public class GDTKLanguage : ISelectableOption
    {

        #region Fields

        [Tooltip("Info about this object")] public BasicInfo info;
        [Tooltip("List of creatures that normally speak this language")] public List<string> typicalSpeakers;
        [Tooltip("Script associated with this language")] public string script;

        public string sourceId;

        #endregion

        #region Properties

        public BasicInfo optionInfo { get { return info; } }

        public Type type { get { return GetType(); } }

        #endregion

        #region Constructor

        public GDTKLanguage()
        {
            info = new BasicInfo();
            typicalSpeakers = new List<string>();
        }

        #endregion

        #region Public Methods

        public GDTKLanguage Clone()
        {
            GDTKLanguage result = new GDTKLanguage();

            result.info = info.Clone();
            result.typicalSpeakers = typicalSpeakers.ToList();
            result.script = script;

            return result;
        }

        #endregion

    }
}
#endif
#if GDTK
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [Serializable]
    public class AddOnPluginChoice
    {

        #region Fields

        [Tooltip("Information about object")] public BasicInfo info;
        [Tooltip("List of choices")] public List<UniversalPluginWrapper<AddOnPlugin>> pickFrom;

#if UNITY_EDITOR
        [SerializeField] private bool z_expanded;
#endif

        #endregion

        #region Constructor

        public AddOnPluginChoice()
        {
            info = new BasicInfo();
            pickFrom = new List<UniversalPluginWrapper<AddOnPlugin>>();
        }

        #endregion

        #region Public Methods

        public AddOnPluginChoice Clone()
        {
            AddOnPluginChoice result = new AddOnPluginChoice();
            result.info = info.Clone();
            foreach (UniversalPluginWrapper<AddOnPlugin> plugin in pickFrom)
            {
                result.pickFrom.Add(plugin.Clone());
            }
            return result;
        }

        #endregion

    }
}
#endif
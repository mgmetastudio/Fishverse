using System;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [Serializable]
    public class GDTKUnlocking
    {

        #region Fields

        [Tooltip("Sets when this item is unlocked")] public Unlocking unlock;
        [Tooltip("Level required to unlock")] public int level;
        [Tooltip("Id of the associated Class")] public string classId;
        [Tooltip("Condition that must be true to be unlocked")] public string expression;

#if UNITY_EDITOR
        [SerializeField] private bool z_expanded;
#endif

        #endregion

        #region Public Methods

        public GDTKUnlocking Clone()
        {
            GDTKUnlocking result = new GDTKUnlocking();

            result.unlock = unlock;
            result.level = level;
            result.expression = expression;
            result.classId = classId;

            return result;
        }

        #endregion

    }
}

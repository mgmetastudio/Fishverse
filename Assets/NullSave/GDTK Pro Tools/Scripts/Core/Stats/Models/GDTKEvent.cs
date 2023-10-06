#if GDTK
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [Serializable]
    public class GDTKEvent
    {

        #region Fields

        public BasicInfo info;

        [Tooltip("Raise token heartbeat with this event")] public bool raiseTokenHeartbeat;
        [Tooltip("Number of tokens to send")] public float tokens;
        [Tooltip("Stat Modifiers to apply when event is raised")] public List<GDTKStatModifier> statModifiers;

        #endregion

        #region Constructor

        public GDTKEvent()
        {
            info = new BasicInfo();
            statModifiers = new List<GDTKStatModifier>();
        }

        #endregion

        #region Public Methods

        public GDTKEvent Clone()
        {
            GDTKEvent result = new GDTKEvent();

            result.info = info.Clone();
            result.raiseTokenHeartbeat = raiseTokenHeartbeat;
            result.tokens = tokens;
            foreach (GDTKStatModifier mod in statModifiers) result.statModifiers.Add(mod.Clone());

            return result;
        }

        #endregion

    }
}
#endif
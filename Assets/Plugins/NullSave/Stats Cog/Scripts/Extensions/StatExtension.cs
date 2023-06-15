using System;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    public class StatExtension : ScriptableObject
    {

        #region Public Methods

        public virtual bool AllowAddToParent(Type parentType) { throw new NotImplementedException(); }

        public virtual void OnAdded(GameObject host) { throw new NotImplementedException(); }

        public virtual void OnEnded(GameObject host) { throw new NotImplementedException(); }

        public virtual void OnRemoved(GameObject host) { throw new NotImplementedException(); }

        #endregion

    }
}
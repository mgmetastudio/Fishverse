#if GDTK
using System;
using System.IO;

namespace NullSave.GDTK.Stats
{
    [Serializable]
    public class StatsPlugin : UniversalPlugin
    {

        #region Properties

        public BasicStats host { get; private set; }

        #endregion

        #region Public Methods

        public virtual void DataLoad(Stream stream) { }

        public virtual void DataSave(Stream stream) { }

        public virtual void Initialize(BasicStats host) { this.host = host; }

        public virtual void Shutdown() { }

        #endregion

    }
}
#endif
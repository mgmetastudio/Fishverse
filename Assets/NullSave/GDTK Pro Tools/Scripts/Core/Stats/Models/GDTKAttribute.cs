#if GDTK
using System;
using System.IO;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [Serializable]
    public class GDTKAttribute : ISelectableOption
    {

        #region Fields

        [Tooltip("Information about object")] public BasicInfo info;

        public string sourceId;

        #endregion

        #region Properties

        public BasicInfo optionInfo { get { return info; } }

        public Type type { get { return GetType(); } }

        #endregion

        #region Constructor

        public GDTKAttribute()
        {
            info = new BasicInfo();
        }

        #endregion

        #region Public Methods

        public GDTKAttribute Clone()
        {
            GDTKAttribute result = new GDTKAttribute();

            result.info = info.Clone();

            return result;
        }

        public void DataLoad(Stream stream, int version)
        {
            if (version > BasicStats.STATS_FILE_VERSION)
            {
                throw new NotSupportedException("Invalid file version");
            }

            info.DataLoad(stream, 1);
            sourceId = stream.ReadStringPacket();
        }

        public void DataSave(Stream stream, int version)
        {
            if (version > BasicStats.STATS_FILE_VERSION)
            {
                throw new NotSupportedException("Invalid file version");
            }

            info.DataSave(stream, 1);
            stream.WriteStringPacket(sourceId);
        }

        #endregion

    }
}
#endif
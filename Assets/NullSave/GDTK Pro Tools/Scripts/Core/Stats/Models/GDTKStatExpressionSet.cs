#if GDTK
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [Serializable]
    public class GDTKStatExpressionSet
    {

        #region Fields

        [Tooltip("Minimum value")] public GDTKStatValue minimum;
        [Tooltip("Maximum value")] public GDTKStatValue maximum;
        [Tooltip("Actual value")] public GDTKStatValue value;
        [Tooltip("Special value not bound by minimum or maximum")] public GDTKStatValue special;

#if UNITY_EDITOR
        [SerializeField] private bool z_expanded;
#endif

        #endregion

        #region Properties

        public bool initialized { get; private set; }

        #endregion

        #region Constructor

        public GDTKStatExpressionSet()
        {
            minimum = new GDTKStatValue();
            maximum = new GDTKStatValue();
            value = new GDTKStatValue();
            special = new GDTKStatValue();
#if UNITY_EDITOR
            z_expanded = true;
            bool x = z_expanded;    // Remove stupid unity warning
#endif
        }

        #endregion

        #region Public Methods

        public GDTKStatExpressionSet Clone()
        {
            GDTKStatExpressionSet result = new GDTKStatExpressionSet();

            result.minimum = minimum.Clone();
            result.maximum = maximum.Clone();
            result.value = value.Clone();
            result.special = special.Clone();

            return result;
        }

        public void CopyFrom(GDTKStatExpressionSet data)
        {
            minimum.CopyFrom(data.minimum);
            maximum.CopyFrom(data.maximum);
            value.CopyFrom(data.value);
            special.CopyFrom(data.special);
        }

        public void DataLoad(Stream stream, string statId, int version)
        {
            if (version > BasicStats.STATS_FILE_VERSION)
            {
                throw new NotSupportedException("Invalid file version");
            }

            LoadData(new Data.ExpressionSetData(stream, statId, version));
        }

        public void DataSave(Stream stream, int version)
        {
            if (version > BasicStats.STATS_FILE_VERSION)
            {
                throw new NotSupportedException("Invalid file version");
            }

            GenerateData().Write(stream, version);
        }

        public void Initialize(GDTKStat stat)
        {
            if (initialized) return;

            minimum.Initialize(stat);
            maximum.Initialize(stat);
            if (stat.startMaxed)
            {
                value.valueExpression = maximum.value.ToString();
            }
            value.Initialize(stat);
            special.Initialize(stat);

            initialized = true;
        }

        public bool Matches(GDTKStatExpressionSet source)
        {
            return source.maximum == maximum &&
                source.minimum == minimum &&
                source.value == value &&
                source.special == special;
        }

        public void Shutdown()
        {
            minimum.Shutdown();
            maximum.Shutdown();
            value.Shutdown();
            special.Shutdown();
        }

        #endregion

        #region Private Methods

        protected internal void FinalizeLoading(string statId, StatSource host)
        {
            if (initialized) return;

            minimum.FinalizeLoading(statId, host);
            maximum.FinalizeLoading(statId, host);
            value.FinalizeLoading(statId, host);
            special.FinalizeLoading(statId, host);

            initialized = true;
        }

        private Data.ExpressionSetData GenerateData()
        {
            Data.ExpressionSetData data = new Data.ExpressionSetData();
            data.maximum = maximum;
            data.minimum = minimum;
            data.special = special;
            data.value = value;
            return data;
        }

        private void LoadData(Data.ExpressionSetData data)
        {
            minimum.CopyFrom(data.minimum);
            maximum.CopyFrom(data.maximum);
            value.CopyFrom(data.value);
            special.CopyFrom(data.special);
        }

        #endregion

    }
}
#endif
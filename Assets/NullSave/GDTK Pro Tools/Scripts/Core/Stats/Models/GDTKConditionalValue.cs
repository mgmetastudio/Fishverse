#if GDTK
using System;
using System.IO;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [Serializable]
    public class GDTKConditionalValue
    {

        #region Fields

        [Tooltip("Expression to check")] public string condition;
        [Tooltip("Value to use if condition is true")] public string value;

        #endregion

        #region Constructor

        public GDTKConditionalValue()
        {
            condition = "1 < 2";
            value = "1";
        }

        #endregion

        #region Public Methods

        public GDTKConditionalValue Clone()
        {
            GDTKConditionalValue result = new GDTKConditionalValue();

            result.condition = condition;
            result.value = value;

            return result;
        }

        public virtual void DataSave(Stream stream, int version)
        {
            GenerateData().Write(stream, version);
        }

        public virtual void DataLoad(Stream stream, int version)
        {
            LoadData(new Data.ConditionalValueData(stream, version));
        }

        #endregion

        #region Private Methods

        private Data.ConditionalValueData GenerateData()
        {
            Data.ConditionalValueData data = new Data.ConditionalValueData();
            data.condition = condition;
            data.value = value;
            return data;
        }

        private void LoadData(Data.ConditionalValueData data)
        {
            condition = data.condition;
            value = data.value;
        }

        #endregion

    }
}
#endif
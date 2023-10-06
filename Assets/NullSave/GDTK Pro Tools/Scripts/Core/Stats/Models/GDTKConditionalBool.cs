#if GDTK
using System;
using System.IO;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [Serializable]
    public class GDTKConditionalBool
    {

        #region Fields

        [Tooltip("Value")] public bool value;
        [Tooltip("Use expression to determine value")] public bool useExpression;
        [Tooltip("Expression used to determin value")] public string expression = "1 = 0";

        #endregion

        #region Public Methods

        public GDTKConditionalBool Clone()
        {
            GDTKConditionalBool newObj = new GDTKConditionalBool();
            newObj.value = value;
            newObj.expression = expression;
            newObj.useExpression = useExpression;
            return newObj;
        }

        public virtual void DataLoad(Stream stream, int version)
        {
            if (version > BasicStats.STATS_FILE_VERSION)
            {
                throw new NotSupportedException("Invalid file version");
            }

            LoadData(new Data.ConditionalBoolData(stream, version));
        }

        public virtual void DataSave(Stream stream, int version)
        {
            if (version > BasicStats.STATS_FILE_VERSION)
            {
                throw new NotSupportedException("Invalid file version");
            }

            GenerateData().Write(stream, version);
        }

        public static GDTKConditionalBool FromString(string value)
        {
            GDTKConditionalBool cb = new GDTKConditionalBool();

            if (value != null)
            {
                switch (value.ToLower())
                {
                    case "0":
                    case "-1":
                    case "false":
                        cb.useExpression = false;
                        cb.value = false;
                        break;
                    case "true":
                    case "1":
                        cb.useExpression = false;
                        cb.value = true;
                        break;
                    default:
                        cb.useExpression = true;
                        cb.expression = value;
                        break;
                }
            }

            return cb;
        }

        public override string ToString()
        {
            if (!useExpression) return value.ToString();
            return expression;
        }

        #endregion

        #region Private Methods

        private Data.ConditionalBoolData GenerateData()
        {
            return new Data.ConditionalBoolData
            {
                value = value,
                useExpression = useExpression,
                expression = expression
            };
        }

        private void LoadData(Data.ConditionalBoolData data)
        {
            value = data.value;
            useExpression = data.useExpression;
            expression = data.expression;
        }

        #endregion

    }
}
#endif
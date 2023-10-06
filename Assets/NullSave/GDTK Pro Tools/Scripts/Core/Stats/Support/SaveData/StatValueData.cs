#if GDTK
using System.Collections.Generic;
using System.IO;

namespace NullSave.GDTK.Stats.Data
{
    public class StatValueData
    {

        #region Fields

        public ValueType valueType;
        public string value;
        public string randomMin;
        public string randomMax;
        public List<GDTKConditionalValue> conditionalValues;
        public double initialValue;

        public double modifierTotal;
        public double valueTotal;
        public bool isExpressionNumeric;

        #endregion

        #region Constructors

        public StatValueData()
        {
            conditionalValues = new List<GDTKConditionalValue>();
        }

        public StatValueData(Stream stream, int version)
        {
            if (version > BasicStats.STATS_FILE_VERSION)
            {
                throw new System.NotSupportedException("Invalid file version");
            }

            conditionalValues = new List<GDTKConditionalValue>();

            valueType = (ValueType)stream.ReadInt();
            value = stream.ReadStringPacket();
            randomMin = stream.ReadStringPacket();
            randomMax = stream.ReadStringPacket();

            int count = stream.ReadInt();
            for (int i = 0; i < count; i++)
            {
                GDTKConditionalValue condition = new GDTKConditionalValue();
                condition.DataLoad(stream, version);
                conditionalValues.Add(condition);
            }

            initialValue = stream.ReadDouble();
            modifierTotal = stream.ReadDouble();
            valueTotal = stream.ReadDouble();
            isExpressionNumeric = stream.ReadBool();
        }

        #endregion

        #region Public Methods

        public void Write(Stream stream, int version)
        {
            stream.WriteInt((int)valueType);
            stream.WriteStringPacket(value);
            stream.WriteStringPacket(randomMin);
            stream.WriteStringPacket(randomMax);
            stream.WriteInt(conditionalValues.Count);
            foreach (GDTKConditionalValue condition in conditionalValues)
            {
                condition.DataSave(stream, BasicStats.STATS_FILE_VERSION);
            }
            stream.WriteDouble(initialValue);
            stream.WriteDouble(modifierTotal);
            stream.WriteDouble(valueTotal);
            stream.WriteBool(isExpressionNumeric);
        }

        #endregion

    }
}
#endif
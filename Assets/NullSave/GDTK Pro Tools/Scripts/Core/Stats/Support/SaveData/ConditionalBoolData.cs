#if GDTK
using System.IO;

namespace NullSave.GDTK.Stats.Data
{
    public class ConditionalBoolData
    {

        #region Fields

        public bool value;
        public bool useExpression;
        public string expression;

        #endregion

        #region Constructor

        public ConditionalBoolData() { }

        public ConditionalBoolData(Stream stream, int version)
        {
            value = stream.ReadBool();
            useExpression = stream.ReadBool();
            expression = stream.ReadStringPacket();
        }

        #endregion

        #region Public Methods

        public void Write(Stream stream, int version)
        {
            stream.WriteBool(value);
            stream.WriteBool(useExpression);
            stream.WriteStringPacket(expression);
        }

        #endregion

    }
}
#endif
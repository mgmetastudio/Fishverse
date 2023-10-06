#if GDTK
using System.IO;

namespace NullSave.GDTK.Stats.Data
{
    public class ConditionalValueData
    {

        #region Fields

        public string condition;
        public string value;

        #endregion

        #region Constructors

        public ConditionalValueData() { }

        public ConditionalValueData(Stream stream, int version)
        {
            if (version > BasicStats.STATS_FILE_VERSION)
            {
                throw new System.NotSupportedException("Invalid file version");
            }

            condition = stream.ReadStringPacket();
            value = stream.ReadStringPacket();
        }

        #endregion

        #region Public Methods

        public void Write(Stream stream, int version)
        {
            stream.WriteStringPacket(condition);
            stream.WriteStringPacket(value);
        }

        #endregion

    }
}
#endif
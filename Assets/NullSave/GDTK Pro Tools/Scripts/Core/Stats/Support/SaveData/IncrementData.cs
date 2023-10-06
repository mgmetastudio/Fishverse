#if GDTK
using System.IO;

namespace NullSave.GDTK.Stats.Data
{
    public class IncrementData
    {

        #region Fields

        public GDTKConditionalBool enabled;
        public string incrementWhen;
        public GDTKStatValue incrementAmount;

        #endregion

        #region Constructors

        public IncrementData() { }

        public IncrementData(Stream stream, string statId, int version)
        {
            enabled = new GDTKConditionalBool();
            enabled.DataLoad(stream, version);

            incrementWhen = stream.ReadStringPacket();

            incrementAmount = new GDTKStatValue();
            incrementAmount.DataLoad(stream, statId, version);
        }

        #endregion

        #region Public Methods

        public void Write(Stream stream, int version)
        {
            enabled.DataSave(stream, version);
            stream.WriteStringPacket(incrementWhen);
            incrementAmount.DataSave(stream, version);
        }

        #endregion

    }
}
#endif
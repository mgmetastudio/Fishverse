#if GDTK
using System.IO;

namespace NullSave.GDTK.Stats.Data
{
    public class RegenerationData
    {

        #region Fields

        public GDTKConditionalBool enabled;
        public GDTKStatValue delay;
        public GDTKStatValue rate;
        public bool wholeIncrements;

        public bool enabledState;
        public bool regenerating;
        public float clock;

        #endregion

        #region Constructors

        public RegenerationData() { }

        public RegenerationData(Stream stream, string statId, int version)
        {
            enabled = new GDTKConditionalBool();
            enabled.DataLoad(stream, version);

            delay = new GDTKStatValue();
            delay.DataLoad(stream, statId, version);

            rate = new GDTKStatValue();
            rate.DataLoad(stream, statId, version);

            wholeIncrements = stream.ReadBool();
            enabledState = stream.ReadBool();
            regenerating = stream.ReadBool();
            clock = stream.ReadFloat();
        }

        #endregion

        #region Public Methods

        public void Write(Stream stream, int version)
        {
            enabled.DataSave(stream, version);
            delay.DataSave(stream, version);
            rate.DataSave(stream, version);
            stream.WriteBool(wholeIncrements);
            stream.WriteBool(enabledState);
            stream.WriteBool(regenerating);
            stream.WriteFloat(clock);
        }

        #endregion

    }
}
#endif
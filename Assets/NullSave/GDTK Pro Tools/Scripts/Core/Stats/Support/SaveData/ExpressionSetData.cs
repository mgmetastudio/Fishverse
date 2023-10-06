#if GDTK
using System.IO;

namespace NullSave.GDTK.Stats.Data
{
    public class ExpressionSetData
    {

        #region Fields

        public GDTKStatValue minimum;
        public GDTKStatValue maximum;
        public GDTKStatValue value;
        public GDTKStatValue special;

        #endregion

        #region Constructors

        public ExpressionSetData() { }

        public ExpressionSetData(Stream stream, string statId, int version)
        {
            minimum = new GDTKStatValue();
            minimum.DataLoad(stream, statId, version);

            maximum = new GDTKStatValue();
            maximum.DataLoad(stream, statId, version);

            value = new GDTKStatValue();
            value.DataLoad(stream, statId, version);

            special = new GDTKStatValue();
            special.DataLoad(stream, statId, version);
        }

        #endregion

        #region Public Methods

        public void Write(Stream stream, int version)
        {
            minimum.DataSave(stream, version);
            maximum.DataSave(stream, version);
            value.DataSave(stream, version);
            special.DataSave(stream, version);
        }

        #endregion

    }
}
#endif
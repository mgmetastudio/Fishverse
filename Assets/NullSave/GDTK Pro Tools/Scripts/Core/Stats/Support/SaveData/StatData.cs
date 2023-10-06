#if GDTK
using System.Collections.Generic;
using System.IO;

namespace NullSave.GDTK.Stats.Data
{
    public class StatData
    {

        #region Fields

        public BasicInfo info;
        public GDTKStatExpressionSet expressions;
        public GDTKRegenerationSettings regeneration;
        public GDTKIncrementSettings incrementation;
        public bool startMaxed;
        public List<GDTKStatModifier> activeModifiers;

        #endregion

        #region Constructors

        public StatData() { }

        public StatData(Stream stream, int version)
        {
            info = new BasicInfo();
            expressions = new GDTKStatExpressionSet();
            regeneration = new GDTKRegenerationSettings();
            incrementation = new GDTKIncrementSettings();
            activeModifiers = new List<GDTKStatModifier>();

            info.DataLoad(stream, 1);
            expressions.DataLoad(stream, info.id, version);
            regeneration.DataLoad(stream, info.id, version);
            incrementation.DataLoad(stream, info.id, version);
            startMaxed = stream.ReadBool();

            activeModifiers = new List<GDTKStatModifier>();
            int count = stream.ReadInt();
            for (int i = 0; i < count; i++)
            {
                GDTKStatModifier mod = new GDTKStatModifier();
                mod.DataLoad(stream, version);
                activeModifiers.Add(mod);
            }
        }

        #endregion

        #region Public Methods

        public void Write(Stream stream, int version)
        {
            info.DataSave(stream, 1);
            expressions.DataSave(stream, version);
            regeneration.DataSave(stream, version);
            incrementation.DataSave(stream, version);
            stream.WriteBool(startMaxed);

            stream.WriteInt(activeModifiers.Count);
            foreach (GDTKStatModifier mod in activeModifiers)
            {
                mod.DataSave(stream, version);
            }
        }

        #endregion

    }
}
#endif
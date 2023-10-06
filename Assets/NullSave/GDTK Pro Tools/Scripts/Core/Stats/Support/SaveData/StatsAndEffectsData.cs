#if GDTK
using System.Collections.Generic;
using System.IO;

namespace NullSave.GDTK.Stats.Data
{
    public class StatsAndEffectsData : BasicStatsData
    {

        #region Fields

        public List<GDTKStatusEffect> activeEffects;
        public List<string> forbiddenIds;

        #endregion

        #region Constructors

        public StatsAndEffectsData() { }

        public StatsAndEffectsData(Stream stream) : base(stream)
        {
            int count;

            activeEffects = new List<GDTKStatusEffect>();
            forbiddenIds = new List<string>();

            count = stream.ReadInt();
            for (int i = 0; i < count; i++)
            {
                GDTKStatusEffect effect = new GDTKStatusEffect();
                effect.DataLoad(stream, version);
                activeEffects.Add(effect);
            }

            count = stream.ReadInt();
            forbiddenIds = new List<string>();
            for (int i = 0; i < count; i++)
            {
                forbiddenIds.Add(stream.ReadStringPacket());
            }
        }

        #endregion

        #region Public Methods

        public override void Write(Stream stream, int version)
        {
            base.Write(stream, version);

            stream.WriteInt(activeEffects.Count);
            foreach (GDTKStatusEffect effect in activeEffects)
            {
                effect.DataSave(stream, version);
            }

            stream.WriteInt(forbiddenIds.Count);
            foreach (string id in forbiddenIds)
            {
                stream.WriteStringPacket(id);
            }
        }

        #endregion

    }
}
#endif
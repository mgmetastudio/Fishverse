#if GDTK
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NullSave.GDTK.Stats.Data
{
    public class PlayerCharacterStatsData : NPCStatsData
    {

        #region Fields

        public bool preventLevelGain;
        public RaceData race;
        public BackgroundData background;
        public List<GDTKClass> classes;
        public List<GDTKPerk> perks;
        public string levelId;

        #endregion

        #region Constructors

        public PlayerCharacterStatsData() { }

        public PlayerCharacterStatsData(Stream stream) : base(stream)
        {
            StatsDatabase database = ToolRegistry.GetComponent<StatsDatabase>();
            classes = new List<GDTKClass>();
            perks = new List<GDTKPerk>();

            preventLevelGain = stream.ReadBool();

            race = stream.ReadBool() ? new RaceData(stream, version) : null;

            background = stream.ReadBool() ? new BackgroundData(stream, version) : null;

            GDTKClass pc;
            int count = stream.ReadInt();
            for (int i = 0; i < count; i++)
            {
                pc = database.GetClass(stream.ReadStringPacket());
                classes.Add(pc);
                pc.DataLoad(stream, version);
            }

            GDTKPerk perk;
            count = stream.ReadInt();
            for (int i = 0; i < count; i++)
            {
                perk = database.GetPerk(stream.ReadStringPacket());
                perks.Add(perk);
                perk.DataLoad(stream, version);
            }
            levelId = stream.ReadStringPacket();
        }

        #endregion

        #region Public Methods

        public override void Write(Stream stream, int version)
        {
            base.Write(stream, version);

            stream.WriteBool(preventLevelGain);

            stream.WriteBool(race != null);
            if (race != null) race.Write(stream, version);

            stream.WriteBool(background != null);
            if (background != null) background.Write(stream, version);

            stream.WriteInt(classes.Count);
            foreach (GDTKClass pc in classes)
            {
                pc.DataSave(stream, version);
            }

            stream.WriteInt(perks.Count);
            foreach (GDTKPerk perk in perks)
            {
                perk.DataSave(stream, version);
            }

            stream.WriteStringPacket(levelId);
        }

        #endregion

    }
}
#endif
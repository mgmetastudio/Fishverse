#if GDTK
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NullSave.GDTK.Stats.Data
{
    public class RaceData
    {

        #region Fields

        public string id;
        public string instanceId;
        public List<GDTKTrait> traits;
        [JsonDoNotSerialize] public GDTKRace race;

        #endregion

        #region Constructors

        public RaceData() { }

        public RaceData(Stream stream, int version)
        {
            StatsDatabase database = ToolRegistry.GetComponent<StatsDatabase>();

            id = stream.ReadStringPacket();
            instanceId = stream.ReadStringPacket();

            race = database.GetRace(id);
            traits = race.traits.ToList();

            foreach (GDTKTrait trait in traits)
            {
                trait.DataLoad(stream, version);
            }

            race.LoadData(this);
        }

        #endregion

        #region Public Methods

        public void Write(Stream stream, int version)
        {
            stream.WriteStringPacket(id);
            stream.WriteStringPacket(instanceId);

            foreach (GDTKTrait trait in traits)
            {
                trait.DataSave(stream, version);
            }
        }

        #endregion

    }
}
#endif
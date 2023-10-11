#if GDTK
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NullSave.GDTK.Stats.Data
{
    public class BackgroundData
    {

        #region Fields

        public string id;
        public string instanceId;
        public List<GDTKTrait> traits;
        [JsonDoNotSerialize] public GDTKBackground background;

        #endregion

        #region Constructor

        public BackgroundData() { }

        public BackgroundData(Stream stream, int version)
        {
            StatsDatabase database = ToolRegistry.GetComponent<StatsDatabase>();

            id = stream.ReadStringPacket();
            instanceId = stream.ReadStringPacket();

            background = database.GetBackground(id);
            traits = background.traits.ToList();

            foreach (GDTKTrait trait in traits)
            {
                trait.DataLoad(stream, version);
            }

            background.LoadData(this);
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
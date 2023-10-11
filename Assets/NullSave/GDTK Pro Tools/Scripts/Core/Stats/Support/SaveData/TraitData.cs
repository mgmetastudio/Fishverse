#if GDTK
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NullSave.GDTK.Stats.Data
{
    public class TraitData
    {

        #region Fields

        public string instanceId;
        public string sourceId;
        public bool applied;
        public List<UniversalPluginWrapper<AddOnPlugin>> addOnPlugins;

        #endregion

        #region Constructors

        public TraitData() { }

        public TraitData(Stream stream, int version)
        {
            addOnPlugins = new List<UniversalPluginWrapper<AddOnPlugin>>();

            instanceId = stream.ReadStringPacket();
            sourceId = stream.ReadStringPacket();
            applied = stream.ReadBool();

            int count = stream.ReadInt();
            for(int i=0; i < count; i++)
            {
                UniversalPluginWrapper<AddOnPlugin> plugin = new UniversalPluginWrapper<AddOnPlugin>();
                plugin.DataLoad(stream);
                addOnPlugins.Add(plugin);
            }
        }

        #endregion

        #region Public Methods

        public void Write(Stream stream, int version)
        {
            stream.WriteStringPacket(instanceId);
            stream.WriteStringPacket(sourceId);
            stream.WriteBool(applied);

            stream.WriteInt(addOnPlugins.Count);
            foreach (UniversalPluginWrapper<AddOnPlugin> addon in addOnPlugins)
            {
                addon.DataSave(stream);
            }

        }

        #endregion

    }
}
#endif
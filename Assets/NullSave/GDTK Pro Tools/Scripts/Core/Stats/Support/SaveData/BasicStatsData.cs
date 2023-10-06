#if GDTK
using System.Collections.Generic;
using System.IO;

namespace NullSave.GDTK.Stats.Data
{
    public class BasicStatsData
    {

        #region Fields

        public int version;
        public List<GDTKStat> stats;
        public List<GDTKStatusCondition> statusConditions;
        public List<GDTKAttribute> attributes;
        public List<UniversalPluginWrapper<StatsPlugin>> plugins;

        #endregion

        #region Constructors

        public BasicStatsData() { }

        public BasicStatsData(Stream stream)
        {
            int count;
            stats = new List<GDTKStat>();
            statusConditions = new List<GDTKStatusCondition>();
            attributes = new List<GDTKAttribute>();
            plugins = new List<UniversalPluginWrapper<StatsPlugin>>();

            version = stream.ReadInt();

            // Stats
            count = stream.ReadInt();
            for (int i = 0; i < count; i++)
            {
                GDTKStat stat = new GDTKStat();
                stat.DataLoad(stream, version);
                stats.Add(stat);
            }

            // Status Conditions
            count = stream.ReadInt();
            for (int i = 0; i < count; i++)
            {
                GDTKStatusCondition conditon = new GDTKStatusCondition();
                conditon.DataLoad(stream, version);
                statusConditions.Add(conditon);
            }

            // Attributes
            count = stream.ReadInt();
            for (int i = 0; i < count; i++)
            {
                GDTKAttribute attribute = new GDTKAttribute();
                attribute.DataLoad(stream, version);
                attributes.Add(attribute);
            }

            // Plugins
            foreach (var wrapper in plugins)
            {
                wrapper.plugin.DataLoad(stream);
            }
        }

        #endregion

        #region Public Methods

        public virtual void Write(Stream stream, int version)
        {
            stream.WriteInt(version);

            // Stats
            stream.WriteInt(stats.Count);
            foreach (var entry in stats)
            {
                entry.DataSave(stream, version);
            }

            // Status Conditions
            stream.WriteInt(statusConditions.Count);
            foreach (GDTKStatusCondition condition in statusConditions)
            {
                condition.DataSave(stream, version);
            }

            // Attributes
            stream.WriteInt(attributes.Count);
            foreach (GDTKAttribute attribute in attributes)
            {
                attribute.DataSave(stream, version);
            }

            // Plugins
            foreach (var wrapper in plugins)
            {
                wrapper.plugin.DataSave(stream);
            }
        }

        #endregion

    }
}
#endif
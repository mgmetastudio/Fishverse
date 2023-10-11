#if GDTK
using System.Collections.Generic;

namespace NullSave.GDTK.Stats
{
    public class jsonBasicStats
    {

        #region Fields

        public bool register;
        public string registryKey;

        public List<UniversalPluginWrapper<StatsPlugin>> plugins;

        public Dictionary<string, GDTKStat> initializedStats;
        public Dictionary<string, IUniquelyIdentifiable> identifiableSources;

        public List<GDTKStat> m_stats;
        public List<GDTKStatusCondition> m_statusConditions;
        public List<string> m_attributeIds;
        public List<string> m_conditionIds;

        public long[] testingOnly = new long[] { 1, 2, 3, 42, 69, 420 };

        #endregion

        #region Constructor

        public jsonBasicStats()
        {
            plugins = new List<UniversalPluginWrapper<StatsPlugin>>();
            initializedStats = new Dictionary<string, GDTKStat>();
            identifiableSources = new Dictionary<string, IUniquelyIdentifiable>();
            m_stats = new List<GDTKStat>();
            m_statusConditions = new List<GDTKStatusCondition>();
            m_attributeIds = new List<string>();
            m_conditionIds = new List<string>();
        }

        #endregion

    }
}
#endif
#if GDTK
using NullSave.GDTK.Stats;
using System;
using System.Collections.Generic;

namespace NullSave.GDTK.JSON
{
    [Serializable]
    public class jsonStatsDatabase
    {

        #region Fields

        public DataSource dataSource;
        public string bundleName;
        public string assetName;
        public string path;
        public bool loadOnAwake;

        public List<ActionSequenceList> actionSequences;
        public List<GDTKAttribute> attributes;
        public List<GDTKBackground> backgrounds;
        public List<GDTKClass> classes;
        public List<GDTKStatusEffect> effects;
        public List<GDTKEvent> events;
        public List<GDTKLanguage> languages;
        public List<GDTKPerk> perks;
        public List<GDTKRace> races;
        public List<GDTKStatusCondition> statusConditions;
        public List<GDTKLevelReward> levelRewards;
        public List<GDTKLevelReward> customRewards;

        #endregion

    }
}
#endif
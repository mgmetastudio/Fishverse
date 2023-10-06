#if GDTK
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [Serializable]
    public class GDTKPerk : ISelectableOption, IUniquelyIdentifiable
    {

        #region Fields

        [Tooltip("Information about object")] public BasicInfo info;
        [Tooltip("Unlocking conditions")] public GDTKUnlocking unlocking;
        [Tooltip("List of attribute ids to add")] public List<string> attributeIds;
        [Tooltip("List of condition ids to add")] public List<string> conditionIds;
        [Tooltip("List of stat modifiers to add")] public List<GDTKStatModifier> statModifiers;

        public string sourceId;

        private string m_instanceId;

        #endregion

        #region Properties

        public string instanceId
        {
            get
            {
                if (string.IsNullOrEmpty(m_instanceId))
                {
                    m_instanceId = Guid.NewGuid().ToString().Replace("-", "");
                }

                return m_instanceId;
            }
        }

        public BasicInfo optionInfo { get { return info; } }

        public Type type { get { return GetType(); } }

        #endregion

        #region Constructor

        public GDTKPerk()
        {
            info = new BasicInfo();
            unlocking = new GDTKUnlocking();
            attributeIds = new List<string>();
            conditionIds = new List<string>();
            statModifiers = new List<GDTKStatModifier>();
        }

        #endregion

        #region Public Methods

        public GDTKPerk Clone()
        {
            GDTKPerk result = new GDTKPerk();

            result.info = info.Clone();
            result.unlocking = unlocking.Clone();
            result.attributeIds = attributeIds.ToList();
            result.conditionIds = conditionIds.ToList();
            foreach (GDTKStatModifier mod in statModifiers)
            {
                result.statModifiers.Add(mod.Clone());
            }

            return result;
        }

        public void DataLoad(Stream stream, int version)
        {
            if (version > BasicStats.STATS_FILE_VERSION)
            {
                throw new NotSupportedException("Invalid file version");
            }

            m_instanceId = stream.ReadStringPacket();
            sourceId = stream.ReadStringPacket();
        }

        public void DataSave(Stream stream, int version)
        {
            if (version > BasicStats.STATS_FILE_VERSION)
            {
                throw new NotSupportedException("Invalid file version");
            }

            stream.WriteStringPacket(info.id);
            stream.WriteStringPacket(m_instanceId);
            stream.WriteStringPacket(sourceId);
        }

        public bool IsUnlocked(PlayerCharacterStats statSource)
        {
            return unlocking.unlock switch
            {
                Unlocking.AtCharacterLevel => statSource.GetCharacterLevel() >= unlocking.level,
                Unlocking.AtClassLevel => statSource.GetClassLevel(unlocking.classId) >= unlocking.level,
                Unlocking.ByExpression => GDTKStatsManager.IsConditionTrue(unlocking.expression, statSource.source),
                _ => true,
            };
        }

        #endregion

    }
}
#endif
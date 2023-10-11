#if GDTK
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [Serializable]
    public class GDTKClass : ISelectableOption, IUniquelyIdentifiable
    {

        #region Fields

        [Tooltip("Info about this class")] public BasicInfo info;
        [Tooltip("Determines when to unlock this class")] public GDTKUnlocking unlocking;
        [Tooltip("Parent Class Id, used for sub-classing")] public string parentId;
        [Tooltip("Traits to apply with this class")] public List<GDTKTrait> traits;
        [Tooltip("Level of class")] [SerializeField] [JsonSerializeAs("level")] private int m_level;
        [Tooltip("Sets if Class is playable")] public bool playable;

        [JsonDoNotSerialize] public SimpleEvent onLevelChanged;

        [JsonSerializeAs("instanceId")] private string m_instanceId;

        #endregion

        #region Properties

        public string instanceId
        {
            get
            {
                if (string.IsNullOrEmpty(m_instanceId))
                {
                    m_instanceId = Guid.NewGuid().ToString();
                }

                return m_instanceId;
            }
        }

        [JsonDoNotSerialize] public int level
        {
            get { return m_level; }
            set
            {
                if (m_level == value) return;
                m_level = value;
                onLevelChanged?.Invoke();
            }
        }

        public BasicInfo optionInfo { get { return info; } }

        public Type type { get { return GetType(); } }

        #endregion

        #region Constructor

        public GDTKClass()
        {
            info = new BasicInfo();
            unlocking = new GDTKUnlocking();
            traits = new List<GDTKTrait>();
            playable = true;
        }

        #endregion

        #region Public Methods

        public GDTKClass Clone()
        {
            GDTKClass result = new GDTKClass();

            result.info = info.Clone();
            result.unlocking = unlocking.Clone();
            result.parentId = parentId;
            foreach (GDTKTrait trait in traits) result.traits.Add(trait.Clone());
            result.level = level;
            result.playable = playable;

            return result;
        }

        public void DataLoad(Stream stream, int version)
        {
            if (version > BasicStats.STATS_FILE_VERSION)
            {
                throw new NotSupportedException("Invalid file version");
            }

            m_instanceId = stream.ReadStringPacket();
            m_level = stream.ReadInt();
            playable = stream.ReadBool();

            foreach (GDTKTrait trait in traits)
            {
                trait.DataLoad(stream, version);
            }
        }

        public void DataSave(Stream stream, int version)
        {
            if (version > BasicStats.STATS_FILE_VERSION)
            {
                throw new NotSupportedException("Invalid file version");
            }

            stream.WriteStringPacket(info.id);
            stream.WriteStringPacket(m_instanceId);
            stream.WriteInt(m_level);
            stream.WriteBool(playable);

            foreach (GDTKTrait trait in traits)
            {
                trait.DataSave(stream, version);
            }
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

        public void Shutdown()
        {
            onLevelChanged = null;
        }

        #endregion

        #region Private Methods

        protected internal void FinalizeLoading(PlayerCharacterStats host)
        {
            foreach (GDTKTrait trait in traits)
            {
                trait.FinalizeLoading(host);
            }
        }

        #endregion

    }
}
#endif
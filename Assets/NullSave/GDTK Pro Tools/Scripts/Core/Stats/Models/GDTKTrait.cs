#if GDTK
using NullSave.GDTK.JSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [Serializable]
    public class GDTKTrait : IUniquelyIdentifiable, ISerializationCallbackReceiver
    {

        #region Fields

        [Tooltip("Info about this object")] public BasicInfo info;
        [Tooltip("Items of times in this list prevent the trait from having affect when equipped")] public List<string> forbidEquipmentTypes;
        [Tooltip("Defines how/when this trait is unlocked")] public GDTKUnlocking unlocking;
        [Tooltip("Modifiers to apply while this condition is active")] public List<GDTKStatModifier> statModifiers;


        public string sourceId;
        private string m_instanceId;
        [SerializeField] private bool m_initialized;
        [SerializeField] private bool m_applied;

        [NonSerialized] private PlayerCharacterStats statSource;
        [NonSerialized] private GlobalStats globalStats;

#if UNITY_EDITOR
        [SerializeField] private bool z_expanded;
        [SerializeField] private bool z_info_expanded;
#endif

        private List<SimpleEvent> subscriptions;

        private List<UniversalPluginWrapper<AddOnPlugin>> m_addOnPlugins;
        [SerializeField] [JsonSerializeAs("addOnPlugins")] private List<jsonUniversalPluginWrapper> jsonAddOnPlugins;
        [SerializeField] private bool needsRefresh;

        #endregion

        #region Properties

        public List<UniversalPluginWrapper<AddOnPlugin>> addOnPlugins
        {
            get
            {
                if (m_addOnPlugins == null || needsRefresh)
                {
                    m_addOnPlugins = new List<UniversalPluginWrapper<AddOnPlugin>>();
                    foreach (jsonUniversalPluginWrapper aow in jsonAddOnPlugins)
                    {
                        m_addOnPlugins.Add(new UniversalPluginWrapper<AddOnPlugin>(aow.id, aow.serializationNamespace, aow.serializationType, aow.serializationData));
                    }

                    needsRefresh = false;
                }

                return m_addOnPlugins;
            }
        }

        public bool initialized
        {
            get { return m_initialized; }
        }

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

        public bool isApplied
        {
            get { return m_applied; }
        }

#if UNITY_EDITOR

        public object addOnPlugList { get; set; }

        public object modifierList { get; set; }

#endif

        #endregion

        #region Constructor

        public GDTKTrait()
        {
            info = new BasicInfo();
            forbidEquipmentTypes = new List<string>();
            unlocking = new GDTKUnlocking();
            statModifiers = new List<GDTKStatModifier>();
            jsonAddOnPlugins = new List<jsonUniversalPluginWrapper>();
            m_addOnPlugins = new List<UniversalPluginWrapper<AddOnPlugin>>();
        }

        #endregion

        #region Public Methods

        public GDTKTrait Clone()
        {
            GDTKTrait result = new GDTKTrait();

            result.info = info.Clone();
            result.forbidEquipmentTypes = forbidEquipmentTypes.ToList();
            result.unlocking = unlocking.Clone();
            foreach (GDTKStatModifier mod in statModifiers) result.statModifiers.Add(mod.Clone());
            foreach (UniversalPluginWrapper<AddOnPlugin> plugin in addOnPlugins) result.addOnPlugins.Add(plugin);

            return result;
        }

        public void DataLoad(Stream stream, int version)
        {
            if (version > BasicStats.STATS_FILE_VERSION)
            {
                throw new NotSupportedException("Invalid file version");
            }

            ReleaseSubscriptions();
            m_initialized = false;

            LoadData(new Data.TraitData(stream, version));
        }

        public void DataSave(Stream stream, int version)
        {
            if (version > BasicStats.STATS_FILE_VERSION)
            {
                throw new NotSupportedException("Invalid file version");
            }

            GenerateData().Write(stream, version);
        }

        public void Initialize(IUniquelyIdentifiable source, PlayerCharacterStats statSource)
        {
            if (m_initialized) return;

            this.statSource = statSource;
            globalStats = statSource.globalStats;
            sourceId = source?.instanceId;

            m_initialized = true;
            Subscribe();
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

        #region Private Methods

        protected internal void FinalizeLoading(PlayerCharacterStats host)
        {
            statSource = host;
            globalStats = statSource.globalStats;
            host.RegisterSource(this);

            if (!m_applied)
            {
                Subscribe();
            }
            else
            {
                host.AddTrait(this, host.GetSource(sourceId));
            }
        }

        private Data.TraitData GenerateData()
        {
            OnBeforeSerialize();
            return new Data.TraitData
            {
                instanceId = m_instanceId,
                sourceId = sourceId,
                applied = m_applied,
                addOnPlugins = m_addOnPlugins.ToList(),
            };
        }

        private void LoadData(Data.TraitData data)
        {
            m_instanceId = data.instanceId;
            sourceId = data.sourceId;
            m_applied = data.applied;

            jsonAddOnPlugins = new List<jsonUniversalPluginWrapper>();
            for (int i = 0; i < data.addOnPlugins.Count(); i++)
            {
                jsonAddOnPlugins.Add(data.addOnPlugins[i].ToJSON());
            }

            needsRefresh = true;
            m_initialized = true;

            // Force instancing plugins
            _ = addOnPlugins.Count;
        }

        private void ReleaseSubscriptions()
        {
            if (subscriptions != null)
            {
                for (int i = 0; i < subscriptions.Count; i++)
                {
                    subscriptions[i] -= UnlockAndApply;
                }
                subscriptions.Clear();
            }

            if (statSource != null)
            {
                statSource.onLevelChanged -= UnlockAndApply;
                statSource.onClassesChanged -= UnlockAndApply;
            }
        }

        private void Subscribe()
        {
            switch (unlocking.unlock)
            {
                case Unlocking.AtCharacterLevel:
                    if (statSource.GetCharacterLevel() >= unlocking.level)
                    {
                        statSource.AddTrait(this, statSource.GetSource(sourceId));
                        m_applied = true;
                    }
                    else
                    {
                        statSource.onLevelChanged += UnlockAndApply;
                    }
                    break;
                case Unlocking.AtClassLevel:
                    if (statSource.GetClassLevel(unlocking.classId) >= unlocking.level)
                    {
                        statSource.AddTrait(this, statSource.GetSource(sourceId));
                        m_applied = true;
                    }
                    else
                    {
                        statSource.onClassesChanged += UnlockAndApply;
                    }
                    break;
                case Unlocking.ByExpression:
                    if (GDTKStatsManager.IsConditionTrue(unlocking.expression, statSource.source))
                    {
                        statSource.AddTrait(this, statSource.GetSource(sourceId));
                        m_applied = true;
                    }
                    else
                    {
                        subscriptions = GDTKStatsManager.AutoSubscribe(unlocking.expression, UnlockAndApply, statSource.source);
                    }
                    break;
                case Unlocking.Immediately:
                default:
                    statSource.AddTrait(this, statSource.GetSource(sourceId));
                    m_applied = true;
                    break;
            }
        }

        private void UnlockAndApply()
        {
            if (m_applied)
            {
                ReleaseSubscriptions();
                return;
            }

            switch (unlocking.unlock)
            {
                case Unlocking.AtCharacterLevel:
                    if (statSource.GetCharacterLevel() >= unlocking.level)
                    {
                        statSource.AddTrait(this, statSource.GetSource(sourceId));
                        m_applied = true;
                    }
                    break;
                case Unlocking.AtClassLevel:
                    if (statSource.GetClassLevel(unlocking.classId) >= unlocking.level)
                    {
                        statSource.AddTrait(this, statSource.GetSource(sourceId));
                        m_applied = true;
                    }
                    break;
                case Unlocking.ByExpression:
                    if (GDTKStatsManager.IsConditionTrue(unlocking.expression, statSource.source))
                    {
                        statSource.AddTrait(this, statSource.GetSource(sourceId));
                        m_applied = true;
                    }
                    break;
            }
        }

        #endregion

        #region Serialization

        [JsonAfterDeserialization]
        public void OnAfterDeserialize()
        {
            needsRefresh = true;
        }

        [JsonBeforeSerialization]
        public void OnBeforeSerialize()
        {
            if (needsRefresh) return;
            jsonAddOnPlugins = new List<jsonUniversalPluginWrapper>();
            foreach (UniversalPluginWrapper<AddOnPlugin> addOn in addOnPlugins)
            {
                jsonAddOnPlugins.Add(addOn.ToJSON());
            }
        }

        #endregion

    }
}
#endif
#if GDTK
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [Serializable]
    public class GDTKStatusCondition : IUniquelyIdentifiable, ISelectableOption
    {

        #region Fields

        [Tooltip("Information about this Status Condition")] public BasicInfo info;
        [Tooltip("Trigger for starting Status Condtion")] public ConditionStartMode activateWhen;
        [Tooltip("Expression that must be true to activate Status Condition")] public string startCondition;
        [Tooltip("Event used activate Status Condition")] public string startEvent;
        [Tooltip("Trigger for ending Status Condtion")] public ConditionEndMode deactivateWhen;
        [Tooltip("Expression that must be true to deactivate Status Condition")] public string endCondition;
        [Tooltip("Event used deactivate Status Condition")] public string endEvent;
        [Tooltip("Seconds/Turns to wait before deactivating Status Condition")] public float endTime;

        [Tooltip("Modifiers to apply while this condition is active")] public List<GDTKStatModifier> statModifiers;
        [Tooltip("Attributes to add while effect is active")] public List<string> attributeIds;
        [Tooltip("Item to spawn")] public GDTKSpawnInfo spawnInfo;

        public string sourceId;

        [JsonDoNotSerialize] public StatusConditionEvent onActivated, onDeactivated;

        [NonSerialized] private BasicStats target;
        private List<SimpleEvent> subscriptions;
        private GlobalStats globalStats;
        private float timeRemaining;

        private string m_instanceId;

        private string debugId;

        #endregion

        #region Properties

        public bool active { get; private set; }

        public bool initialized { get; private set; }

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

        public BasicInfo optionInfo { get { return info; } }

        public Type type { get { return GetType(); } }

        #endregion

        #region Constructor

        public GDTKStatusCondition()
        {
            info = new BasicInfo();
            statModifiers = new List<GDTKStatModifier>();
            attributeIds = new List<string>();
            spawnInfo = new GDTKSpawnInfo();
            debugId = Guid.NewGuid().ToString();
        }

        #endregion

        #region Public Methods

        public GDTKStatusCondition Clone()
        {
            GDTKStatusCondition result = new GDTKStatusCondition();

            result.activateWhen = activateWhen;
            result.deactivateWhen = deactivateWhen;
            result.endCondition = endCondition;
            result.endEvent = endEvent;
            result.endTime = endTime;
            result.startCondition = startCondition;
            result.startEvent = startEvent;
            result.info = info.Clone();
            foreach (GDTKStatModifier mod in statModifiers) result.statModifiers.Add(mod.Clone());
            result.attributeIds = attributeIds;
            result.spawnInfo = spawnInfo.Clone();

            return result;
        }

        public void DataLoad(Stream stream, int version)
        {
            if (version > BasicStats.STATS_FILE_VERSION)
            {
                throw new NotSupportedException("Invalid file version");
            }

            initialized = false;
            LoadData(new Data.StatusConditionData(stream, version));
        }

        public void DataSave(Stream stream, int version)
        {
            if (version > BasicStats.STATS_FILE_VERSION)
            {
                throw new NotSupportedException("Invalid file version");
            }

            GenerateData().Write(stream, version);
        }

        public void Initialize(BasicStats target)
        {
            if (initialized) return;

            this.target = target;
            globalStats = ToolRegistry.GetComponent<GlobalStats>();
            ReleaseSubscriptions();

            switch (activateWhen)
            {
                case ConditionStartMode.ConditionTrue:
                    BasicStats statSource = target;
                    subscriptions = GDTKStatsManager.AutoSubscribe(startCondition, UpdateStateFromCondition, target.source);
                    break;
                case ConditionStartMode.EventTriggered:
                    target.SubscribeToEventTriggers(EventTriggered);
                    break;
            }

            switch (deactivateWhen)
            {
                case ConditionEndMode.ConditionTrue:
                    subscriptions = GDTKStatsManager.AutoSubscribe(endCondition, UpdateStateFromCondition, target.source);
                    break;
                case ConditionEndMode.EventTriggered:
                    target.SubscribeToEventTriggers(EventTriggered);
                    break;
                case ConditionEndMode.TimeElapsed:
                    target.SubscribeToHeartbeat(UpdateEndFromHeartbeat);
                    break;
                case ConditionEndMode.TurnsElapsed:
                    target.SubscribeToTokenHeartbeat(UpdateEndFromHeartbeat);
                    break;
            }

            initialized = true;
        }

        public void Shutdown()
        {
            initialized = false;

            Deactivate();
            ReleaseSubscriptions();
            target.UnsubscribeFromHeartbeat(UpdateEndFromHeartbeat);
            target.UnsubscribeFromTokenHeartbeat(UpdateEndFromHeartbeat);
            target.UnsubscribeFromEventTriggers(EventTriggered);
        }

        #endregion

        #region Private Methods

        private void Activate()
        {
            if (!initialized || active) return;

            foreach (GDTKStatModifier modifier in statModifiers)
            {
                target.AddStatModifier(modifier, this);
            }

            foreach (string attrib in attributeIds)
            {
                target.AddAttribute(attrib, this);
            }


            spawnInfo.Spawn(target.transform);

            timeRemaining = endTime;
            active = true;
            onActivated?.Invoke(this);
        }

        private void Deactivate()
        {
            if (!initialized || !active) return;

            target.RemoveStatModifiersFromSource(this);
            target.RemoveAttributesFromSource(this);

            spawnInfo.DestroySpawn();

            active = false;
            onDeactivated?.Invoke(this);
        }

        private void EventTriggered(GDTKEvent statEvent)
        {
            if (activateWhen == ConditionStartMode.EventTriggered && statEvent.info.id == startEvent)
            {
                Activate();
            }

            if (deactivateWhen == ConditionEndMode.EventTriggered && statEvent.info.id == endEvent)
            {
                Deactivate();
            }
        }

        protected internal void FinalizeLoading(BasicStats host)
        {
            host.RegisterSource(this);
            Initialize(host);
        }

        private Data.StatusConditionData GenerateData()
        {
            return new Data.StatusConditionData
            {
                instanceId = instanceId,
                info = info,
                active = active,
                activateWhen = activateWhen,
                startCondition = startCondition,
                startEvent = startEvent,
                deactivateWhen = deactivateWhen,
                endCondition = endCondition,
                endEvent = endEvent,
                endTime = endTime,
                statModifiers = statModifiers.ToList(),
                attributeIds = attributeIds.ToList(),
                spawnInfo = spawnInfo,
                sourceId = sourceId,
                timeRemaining = timeRemaining,
            };
        }

        private void LoadData(Data.StatusConditionData data)
        {
            m_instanceId = data.instanceId;
            info = data.info;
            active = data.active;
            activateWhen = data.activateWhen;
            startCondition = data.startCondition;
            startEvent = data.startEvent;
            deactivateWhen = data.deactivateWhen;
            endCondition = data.endCondition;
            endEvent = data.endEvent;
            endTime = data.endTime;
            statModifiers = data.statModifiers.ToList();
            attributeIds = data.attributeIds.ToList();
            spawnInfo = data.spawnInfo;
            sourceId = data.sourceId;
            timeRemaining = data.timeRemaining;
        }

        private void ReleaseSubscriptions()
        {
            if (subscriptions == null)
            {
                subscriptions = new List<SimpleEvent>(0);
                return;
            }

            for (int i = 0; i < subscriptions.Count; i++)
            {
                subscriptions[i] -= UpdateStateFromCondition;
            }
            subscriptions.Clear();
        }

        private void UpdateStateFromCondition()
        {
            if (!active && activateWhen == ConditionStartMode.ConditionTrue)
            {
                if (GDTKStatsManager.IsConditionTrue(startCondition, target.source))
                {
                    Activate();
                }
            }

            if (active && deactivateWhen == ConditionEndMode.ConditionTrue)
            {
                if (GDTKStatsManager.IsConditionTrue(endCondition, target.source))
                {
                    Deactivate();
                }
            }
        }

        private void UpdateEndFromHeartbeat(float amount)
        {
            if (!active) return;
            timeRemaining -= amount;
            if (timeRemaining <= 0)
            {
                Deactivate();
            }
        }

        #endregion

    }
}
#endif
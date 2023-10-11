#if GDTK
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [Serializable]
    public class GDTKRegenerationSettings
    {

        #region Fields

        [Tooltip("Enable regeneration")] public GDTKConditionalBool enabled;
        [Tooltip("Seconds to wait before regeneration starts")] public GDTKStatValue delay;
        [Tooltip("Amount to add each second")] public GDTKStatValue rate;
        [Tooltip("If true regeneration is applied at the end of each second, otherwise it is applied over the course of each second")] public bool wholeIncrements;

        [NonSerialized] private GDTKStat owner;
        private string statId;
        [JsonSerializeAs("enabledState")] private bool m_enabled;
        [JsonSerializeAs("regenerating")] private bool m_regenerating;
        [JsonSerializeAs("clock")] private float lastRegenCheck;

#if UNITY_EDITOR
        [SerializeField] private bool z_expanded;
#endif

        #endregion

        #region Properties

        public bool isEnabled
        {
            get { return m_enabled; }
            set
            {
                if (m_enabled == value) return;
                m_enabled = value;
                timeWaited = 0;
                secondProgress = 0;
                lastRegenCheck = owner.value;
                SetSubscriptions();
            }
        }

        public bool isRegenerating
        {
            get { return m_regenerating; }
            set
            {
                if (m_regenerating == value) return;
                m_regenerating = value;
                if (m_regenerating)
                {
                    owner.owner.SubscribeToHeartbeat(UpdateRegeneration);
                }
                else
                {
                    owner.owner.UnsubscribeFromHeartbeat(UpdateRegeneration);
                }
            }
        }

        public bool initialized { get; private set; }

        private float secondProgress { get; set; }

        public float timeWaited { get; private set; }

        #endregion

        #region Constructors

        public GDTKRegenerationSettings()
        {
            enabled = new GDTKConditionalBool();
            delay = new GDTKStatValue();
            rate = new GDTKStatValue();
        }

        #endregion

        #region Public Methods

        public GDTKRegenerationSettings Clone()
        {
            GDTKRegenerationSettings result = new GDTKRegenerationSettings();

            result.enabled = enabled.Clone();
            result.delay = delay.Clone();
            result.rate = rate.Clone();
            result.wholeIncrements = wholeIncrements;

            return result;
        }

        public void CopyFrom(GDTKRegenerationSettings data)
        {
            statId = data.statId;
            enabled = data.enabled;
            delay.CopyFrom(data.delay);
            rate.CopyFrom(data.rate);
            wholeIncrements = data.wholeIncrements;
            m_enabled = data.m_enabled;
            m_regenerating = data.m_regenerating;
            lastRegenCheck = data.lastRegenCheck;
        }

        public void DataLoad(Stream stream, string statId, int version)
        {
            if (version > BasicStats.STATS_FILE_VERSION)
            {
                throw new NotSupportedException("Invalid file version");
            }

            LoadData(statId, new Data.RegenerationData(stream, statId, version));
        }

        public void DataSave(Stream stream, int version)
        {
            if (version > BasicStats.STATS_FILE_VERSION)
            {
                throw new NotSupportedException("Invalid file version");
            }

            GenerateData().Write(stream, version);
        }

        public void Initialize(GDTKStat stat)
        {
            if (initialized) return;

            owner = stat;

            if (enabled.useExpression)
            {
                GDTKStatsManager.AutoSubscribe(enabled.expression, UpdateStateFromSubscription, owner.owner, null);
            }

            UpdateEnabledState();
            delay.Initialize(stat);
            rate.Initialize(stat);

            initialized = true;
        }

        public bool Matches(GDTKRegenerationSettings source)
        {
            return enabled == source.enabled &&
                delay == source.delay &&
                rate == source.rate &&
                wholeIncrements == source.wholeIncrements;
        }

        public void ResetRegeneration()
        {
            timeWaited = 0;
            secondProgress = 0;
            lastRegenCheck = owner.value;
        }

        public void Shutdown()
        {
            delay.Shutdown();
            rate.Shutdown();
        }

        #endregion

        #region Private Methods

        private void CheckRegenerationNeed()
        {
            if (!isRegenerating && (owner.value < owner.maximum))
            {
                isRegenerating = true;
                lastRegenCheck = owner.value;
                return;
            }

            if (owner.value < lastRegenCheck)
            {
                timeWaited = 0;
                secondProgress = 0;
            }

            lastRegenCheck = owner.value;
        }

        protected internal void FinalizeLoading(string statId, StatSource host)
        {
            if (initialized) return;

            this.statId = statId;
            owner = host.GetStat(statId);

            if (enabled.useExpression)
            {
                GDTKStatsManager.AutoSubscribe(enabled.expression, UpdateStateFromSubscription, host, null);
            }

            if (m_regenerating)
            {
                host.SubscribeToHeartbeat(UpdateRegeneration);
            }

            delay.FinalizeLoading(statId, host);
            rate.FinalizeLoading(statId, host);

            SetSubscriptions();

            initialized = true;
        }

        private Data.RegenerationData GenerateData()
        {
            return new Data.RegenerationData
            {
                enabled = enabled,
                delay = delay,
                rate = rate,
                wholeIncrements = wholeIncrements,
                enabledState = m_enabled,
                regenerating = m_regenerating,
                clock = lastRegenCheck
            };
        }

        private void LoadData(string statId, Data.RegenerationData data)
        {
            this.statId = statId;
            enabled = data.enabled;
            delay.CopyFrom(data.delay);
            rate.CopyFrom(data.rate);
            wholeIncrements = data.wholeIncrements;
            m_enabled = data.enabledState;
            m_regenerating = data.regenerating;
            lastRegenCheck = data.clock;
        }

        private void SetSubscriptions()
        {
            if (m_enabled)
            {
                owner.expressions.value.onValueChanged += CheckRegenerationNeed;
                owner.expressions.maximum.onValueChanged += CheckRegenerationNeed;
                CheckRegenerationNeed(); // Always check immediately on change
            }
            else
            {
                owner.expressions.value.onValueChanged -= CheckRegenerationNeed;
                owner.expressions.maximum.onValueChanged -= CheckRegenerationNeed;
                isRegenerating = false;
            }
        }

        private void UpdateEnabledState()
        {
            if (!enabled.useExpression)
            {
                isEnabled = enabled.value;
            }
            else
            {
                isEnabled = GDTKStatsManager.IsConditionTrue(enabled.expression, owner.owner, null);
                //!!isEnabled = GDTKStatsManager.IsConditionTrue(enabled.expression, owner.owner, owner.globalStats);
            }
        }

        private void UpdateRegeneration(float time)
        {
            // Wait first
            if (timeWaited <= owner.regenerationDelay)
            {
                timeWaited += time;
                if (timeWaited > owner.regenerationDelay)
                {
                    time = timeWaited - (float)owner.regenerationDelay;
                    timeWaited = (float)owner.regenerationDelay;
                }
                else
                {
                    return;
                }
            }

            // Add value
            if (!wholeIncrements)
            {
                owner.value += owner.regenerationRate * time;
            }
            else
            {
                secondProgress += time;
                while (secondProgress >= 1)
                {
                    secondProgress -= 1;
                    owner.value += owner.regenerationRate;
                }
            }

            if (owner.value == owner.maximum)
            {
                isRegenerating = false;
                timeWaited = 0;
                secondProgress = 0;
                lastRegenCheck = owner.value;
            }
        }

        private void UpdateStateFromSubscription()
        {
            isEnabled = GDTKStatsManager.IsConditionTrue(enabled.expression, owner.owner, null);
            //isEnabled = GDTKStatsManager.IsConditionTrue(enabled.expression, owner.owner, owner.globalStats);
        }

        #endregion

    }
}
#endif
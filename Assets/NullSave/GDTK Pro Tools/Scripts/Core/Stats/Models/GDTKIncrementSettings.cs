#if GDTK
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [Serializable]
    public class GDTKIncrementSettings
    {

        #region Fields

        [Tooltip("Enable incrementing")] public GDTKConditionalBool enabled;
        [Tooltip("Increment value when this condition is true")] public string incrementWhen;
        [Tooltip("Amount to increment by")] public GDTKStatValue incrementAmount;

        private List<SimpleEvent> subscriptions;
        [NonSerialized] private GDTKStat owner;
        private string statId;

#if UNITY_EDITOR
        [SerializeField] private bool z_expanded;
#endif

        #endregion

        #region Properties

        public bool initialized { get; private set; }

        #endregion

        #region Constructor

        public GDTKIncrementSettings()
        {
            enabled = new GDTKConditionalBool();
            incrementAmount = new GDTKStatValue();
            incrementWhen = "1 = 0";
        }

        #endregion

        #region Public Methods

        public GDTKIncrementSettings Clone()
        {
            GDTKIncrementSettings result = new GDTKIncrementSettings();

            result.enabled = enabled.Clone();
            result.incrementAmount = incrementAmount.Clone();
            result.incrementWhen = incrementWhen;

            return result;
        }

        public void CopyFrom(GDTKIncrementSettings data)
        {
            statId = data.statId;
            enabled = data.enabled;
            incrementWhen = data.incrementWhen;
            incrementAmount.CopyFrom(data.incrementAmount);
        }

        public void DataLoad(Stream stream, string statId, int version)
        {
            if (version > BasicStats.STATS_FILE_VERSION)
            {
                throw new NotSupportedException("Invalid file version");
            }

            LoadData(statId, new Data.IncrementData(stream, statId, version));
        }

        public void DataSave(Stream stream, int version)
        {
            if (version > BasicStats.STATS_FILE_VERSION)
            {
                throw new NotSupportedException("Invalid file version");
            }

            GenerateData().Write(stream, version);
        }

        public void Initialize(GDTKStat owner)
        {
            if (initialized) return;

            subscriptions = new List<SimpleEvent>();
            this.owner = owner;
            incrementAmount.Initialize(owner);
            GenerateSubscriptions();

            initialized = true;
        }

        public bool Matches(GDTKIncrementSettings source)
        {
            return enabled == source.enabled &&
                incrementAmount == source.incrementAmount &&
                incrementWhen == source.incrementWhen;
        }

        public void Shutdown()
        {
            incrementAmount.Shutdown();
            ReleaseSubscriptions();
        }

        #endregion

        #region Private Methods

        protected internal void FinalizeLoading(string statId, StatSource host)
        {
            if (initialized) return;
            this.statId = statId;
            owner = host.GetStat(statId);
            incrementAmount.FinalizeLoading(statId, host);
            GenerateSubscriptions();
            initialized = true;
        }

        private Data.IncrementData GenerateData()
        {
            return new Data.IncrementData
            {
                enabled = enabled,
                incrementWhen = incrementWhen,
                incrementAmount = incrementAmount
            };
        }

        private void LoadData(string statId, Data.IncrementData data)
        {
            this.statId = statId;
            enabled = data.enabled;
            incrementWhen = data.incrementWhen;
            incrementAmount.CopyFrom(data.incrementAmount);
        }

        private void GenerateSubscriptions()
        {
            ReleaseSubscriptions();

            subscriptions = GDTKStatsManager.AutoSubscribe(incrementWhen, UpdateValueFromSubscription, owner.owner, null);
        }

        private void ReleaseSubscriptions()
        {
            if (subscriptions == null)
            {
                return;
            }

            for (int i = 0; i < subscriptions.Count; i++)
            {
                subscriptions[i] -= UpdateValueFromSubscription;
            }
            subscriptions.Clear();
        }

        private void UpdateValueFromSubscription()
        {
            if (!owner.IsConditionTrue(incrementWhen)) return;

            owner.value += (float)incrementAmount.value;
        }

        #endregion

    }
}
#endif
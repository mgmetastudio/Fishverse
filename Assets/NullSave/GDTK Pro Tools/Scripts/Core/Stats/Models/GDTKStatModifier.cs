#if GDTK
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [Serializable]
    public class GDTKStatModifier
    {

        #region Fields

        [Tooltip("Stat to target with this modifier")] public string affectsStatId;
        [Tooltip("If not blank this condition must be true in order to apply the modifier")] public string requirements;

        [Tooltip("Set how to apply this modifier")] public ModifierApplication applies;
        [Tooltip("Lifetime in seconds/turns")] public float lifespan;
        [Tooltip("If checked change is applied only after a full second/turn, otherwise it is applied over the course of each second/turn")] public bool wholeIncrements;

        [Tooltip("Set which part of the stat to change")] public ModifierTarget target;
        [Tooltip("Set how to change the targeted value")] public ModifierChangeType changeType;

        [Tooltip("Value to use for change")] public GDTKStatValue value;

        public string sourceId;
        [JsonDoNotSerialize] public StatModifierEvent onDeactivated;
        [JsonDoNotSerialize] public SimpleEvent onLifeChanged;

        [JsonSerialize] private double remainingLife;
        [NonSerialized] private GDTKStat stat;

#if UNITY_EDITOR
        [SerializeField] private bool z_expanded;
#endif

        #endregion

        #region Properties

        [JsonSerialize] public double appliedValue { get; set; }

        private bool initialized { get; set; }

        [JsonSerialize] public string instanceId { get; private set; }

        [JsonDoNotSerialize]
        public double lifeRemaining
        {
            get { return remainingLife; }
            set
            {
                remainingLife = value;
                if (value <= 0 && stat != null)
                {
                    stat.RemoveModifier(this);
                }
            }
        }

        public string originalSetValue { get; set; }

        [JsonSerialize] private float secondProgress { get; set; }

        #endregion

        #region Constructor

        public GDTKStatModifier()
        {
            value = new GDTKStatValue();
            instanceId = Guid.NewGuid().ToString().Replace("-", "");
        }

        #endregion

        #region Public Methods

        public void Activate(GDTKStat owner)
        {
            if (initialized) return;

            stat = owner;
            lifeRemaining = lifespan;
            secondProgress = 0;
            switch (applies)
            {
                case ModifierApplication.RecurringOverSeconds:
                    owner.owner.SubscribeToHeartbeat(UpdateRecurrence);
                    break;
                case ModifierApplication.RecurringOverTurns:
                    owner.owner.SubscribeToTokenHeartbeat(UpdateRecurrence);
                    break;
            }

            initialized = true;
        }

        public GDTKStatModifier Clone()
        {
            GDTKStatModifier result = new GDTKStatModifier();

            result.affectsStatId = affectsStatId;
            result.applies = applies;
            result.changeType = changeType;
            result.lifespan = lifespan;
            result.requirements = requirements;
            result.target = target;
            result.value = value.Clone();

            return result;
        }

        public virtual void DataLoad(Stream stream, int version)
        {
            if (version > BasicStats.STATS_FILE_VERSION)
            {
                throw new NotSupportedException("Invalid file version");
            }

            LoadData(new Data.StatModifierData(stream, version));
        }

        public virtual void DataSave(Stream stream, int version)
        {
            if (version > BasicStats.STATS_FILE_VERSION)
            {
                throw new NotSupportedException("Invalid file version");
            }

            GenerateData().Write(stream, version);
        }

        public void Deactivate()
        {
            switch (applies)
            {
                case ModifierApplication.RecurringOverSeconds:
                    stat.owner.UnsubscribeFromHeartbeat(UpdateRecurrence);
                    break;
                case ModifierApplication.RecurringOverTurns:
                    stat.owner.UnsubscribeFromTokenHeartbeat(UpdateRecurrence);
                    break;
            }

            value.Shutdown();
            Shutdown();
        }

        public string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(affectsStatId);

            switch (target)
            {
                case ModifierTarget.Maximum:
                    sb.Append(".maximum");
                    break;
                case ModifierTarget.Minimum:
                    sb.Append(".minimum");
                    break;
                case ModifierTarget.RegenerationDelay:
                    sb.Append(".regenerationDelay");
                    break;
                case ModifierTarget.RegenerationRate:
                    sb.Append(".regenerationRate");
                    break;
                case ModifierTarget.Special:
                    sb.Append(".special");
                    break;
                case ModifierTarget.Value:
                    sb.Append(".value");
                    break;
            }

            switch (applies)
            {
                case ModifierApplication.SetValueOnce:
                    sb.Append(" = ");
                    break;
                case ModifierApplication.SetValueUntilRemoved:
                    sb.Append(" = ");
                    break;
                default:
                    switch (changeType)
                    {
                        case ModifierChangeType.Add:
                            sb.Append(" += ");
                            break;
                        case ModifierChangeType.AddMultiplier:
                            sb.Append(" += ");
                            sb.Append(affectsStatId);
                            sb.Append(" * ");
                            break;
                        case ModifierChangeType.Subtract:
                            sb.Append(" -= ");
                            break;
                        case ModifierChangeType.SubtractMultiplier:
                            sb.Append(" -= ");
                            sb.Append(affectsStatId);
                            sb.Append(" * ");
                            break;
                    }
                    break;
            }

            sb.Append(value.ToExpression());

            switch (applies)
            {
                case ModifierApplication.SetValueOnce:
                case ModifierApplication.Immediately:
                    sb.Append(" at once");
                    break;
                case ModifierApplication.RecurringOverSeconds:
                    sb.Append(" over ");
                    sb.Append(lifespan);
                    sb.Append(" second(s)");
                    break;
                case ModifierApplication.RecurringOverTurns:
                    sb.Append(" over ");
                    sb.Append(lifespan);
                    sb.Append(" turn(s)");
                    break;
                case ModifierApplication.SetValueUntilRemoved:
                case ModifierApplication.UntilRemoved:
                    sb.Append(" until removed");
                    break;
            }

            return sb.ToString();
        }

        public string GetDisplayValue(bool showPlus)
        {
            double result;

            switch (applies)
            {
                case ModifierApplication.UntilRemoved:
                    switch (changeType)
                    {
                        case ModifierChangeType.Add:
                        case ModifierChangeType.AddMultiplier:
                            result = appliedValue;
                            break;
                        default:
                            result = -appliedValue;
                            break;

                    }
                    break;
                default:
                    switch (changeType)
                    {
                        case ModifierChangeType.Add:
                        case ModifierChangeType.AddMultiplier:
                            result = value.value;
                            break;
                        default:
                            result = -value.value;
                            break;
                    }
                    break;
            }

            if (showPlus && result > 0) return "+" + result.ToString();
            return result.ToString();
        }

        public GDTKStat GetStat()
        {
            return stat;
        }

        public bool Matches(GDTKStatModifier source)
        {
            return affectsStatId == source.affectsStatId &&
                requirements == source.requirements &&
                applies == source.applies &&
                lifespan == source.lifespan &&
                wholeIncrements == source.wholeIncrements &&
                target == source.target &&
                value == source.value;
        }

        public void Shutdown()
        {
            onDeactivated?.Invoke(this);
            onDeactivated = null;
            onLifeChanged = null;
            initialized = false;
        }

        public void ResetLifespan()
        {
            if (applies != ModifierApplication.RecurringOverSeconds && applies != ModifierApplication.RecurringOverTurns) return;
            lifeRemaining = lifespan;
            secondProgress = 0;
        }

        #endregion

        #region Private Methods

        private void ApplyChange(double value)
        {
            switch (target)
            {
                case ModifierTarget.Maximum:
                    stat.maximum = GetChangeValue(stat.maximum, value);
                    break;
                case ModifierTarget.Minimum:
                    stat.minimum = GetChangeValue(stat.minimum, value);
                    break;
                case ModifierTarget.RegenerationRate:
                    stat.regenerationRate = GetChangeValue(stat.regenerationRate, value);
                    break;
                case ModifierTarget.RegenerationDelay:
                    stat.regenerationDelay = GetChangeValue(stat.regenerationDelay, value);
                    break;
                case ModifierTarget.Special:
                    stat.special = GetChangeValue(stat.special, value);
                    break;
                case ModifierTarget.Value:
                    stat.value = GetChangeValue(stat.value, value);
                    break;
            }
        }

        protected internal void FinalizeLoading(StatSource host)
        {
            if (initialized) return;

            stat = host.GetStat(affectsStatId);

            if (stat != null)
            {
                switch (applies)
                {
                    case ModifierApplication.RecurringOverSeconds:
                        host.SubscribeToHeartbeat(UpdateRecurrence);
                        break;
                    case ModifierApplication.RecurringOverTurns:
                        host.SubscribeToTokenHeartbeat(UpdateRecurrence);
                        break;
                }
            }

            value.FinalizeLoading(affectsStatId, host);
            initialized = true;
        }

        private Data.StatModifierData GenerateData()
        {
            return new Data.StatModifierData
            {
                instanceId = instanceId,
                affectsStatId = affectsStatId,
                requirements = requirements,
                applies = applies,
                lifespan = lifespan,
                wholeIncrements = wholeIncrements,
                target = target,
                changeType = changeType,
                value = value,
                remainingLife = remainingLife,
                secondProgress = secondProgress,
                sourceId = sourceId,
                appliedValue = appliedValue,
                originalSetValue = originalSetValue,
            };
        }

        private float GetChangeValue(double originalValue, double changeAmount)
        {
            return changeType switch
            {
                ModifierChangeType.AddMultiplier => (float)(originalValue + (originalValue * changeAmount)),
                ModifierChangeType.Subtract => (float)(originalValue - changeAmount),
                ModifierChangeType.SubtractMultiplier => (float)(originalValue - (originalValue * changeAmount)),
                _ => (float)(originalValue + changeAmount),
            };
        }

        private void LoadData(Data.StatModifierData data)
        {
            instanceId = data.instanceId;
            affectsStatId = data.affectsStatId;
            requirements = data.requirements;
            applies = data.applies;
            lifespan = data.lifespan;
            wholeIncrements = data.wholeIncrements;
            target = data.target;
            changeType = data.changeType;
            value.CopyFrom(data.value);
            remainingLife = data.remainingLife;
            secondProgress = data.secondProgress;
            sourceId = data.sourceId;
            appliedValue = data.appliedValue;
            originalSetValue = data.originalSetValue;
        }

        private void UpdateRecurrence(float time)
        {
            if (wholeIncrements)
            {
                secondProgress += time;
                while (secondProgress >= 1)
                {
                    secondProgress -= 1;
                    ApplyChange(value.value);
                }
            }
            else
            {
                ApplyChange(value.value * time);
            }

            lifeRemaining -= time;
            onLifeChanged?.Invoke();
        }

        #endregion

    }
}
#endif
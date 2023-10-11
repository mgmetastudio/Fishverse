#if GDTK
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [Serializable]
    public class GDTKStat : IUniquelyIdentifiable
    {

        #region Fields

        [Tooltip("Information about this Stat")] public BasicInfo info;
        [Tooltip("Expressions used to generate values for this stat")] public GDTKStatExpressionSet expressions;
        [Tooltip("Settings for regenerating the stat's value")] public GDTKRegenerationSettings regeneration;
        [Tooltip("Settings for auto-incrementing the stat's value")] public GDTKIncrementSettings incrementation;
        [Tooltip("Start with maximum value")] public bool startMaxed;

#if UNITY_EDITOR
        [SerializeField] private bool z_expanded;
#endif

        [NonSerialized] public StatSource owner;

        #endregion

        #region Properties

        [JsonSerialize] public List<GDTKStatModifier> activeModifiers { get; private set; }

        public bool initialized { get; private set; }

        public bool initializing { get; private set; }

        public string instanceId { get { return info.id; } }

        [JsonDoNotSerialize]
        public float maximum
        {
            get { return (float)expressions.maximum.valueWithModifiers; }
            set
            {
                if (value < minimum) value = minimum;
                if (this.value > value) this.value = value;
                expressions.maximum.value = value;
            }
        }

        [JsonDoNotSerialize]
        public float minimum
        {
            get { return (float)expressions.minimum.valueWithModifiers; }
            set
            {
                if (value > maximum) value = maximum;
                if (this.value < value) this.value = value;
                expressions.minimum.value = value;
            }
        }

        [JsonDoNotSerialize]
        public float regenerationDelay
        {
            get { return (float)regeneration.delay.valueWithModifiers; }
            set
            {
                regeneration.delay.value = value;
            }
        }

        [JsonDoNotSerialize]
        public float regenerationRate
        {
            get { return (float)regeneration.rate.valueWithModifiers; }
            set
            {
                regeneration.rate.value = value;
            }
        }

        [JsonDoNotSerialize]
        public float special
        {
            get { return (float)expressions.special.valueWithModifiers; }
            set
            {
                expressions.special.value = value;
            }
        }

        [JsonDoNotSerialize]
        public float value
        {
            get { return (float)expressions.value.valueWithModifiers; }
            set
            {
                if (value < minimum) value = minimum;
                if (value > maximum) value = maximum;
                expressions.value.value = value;
            }
        }

        #endregion

        #region Constructors

        public GDTKStat()
        {
            info = new BasicInfo();
            expressions = new GDTKStatExpressionSet();
            regeneration = new GDTKRegenerationSettings();
            incrementation = new GDTKIncrementSettings();
        }

        public GDTKStat (string title, string id, string minimum, string maximum, string value, ImageInfo image = null)
        {
            info = new BasicInfo() { title = title, id = id };
            if(image != null)
            {
                info.image = image.Clone();
            }
            expressions = new GDTKStatExpressionSet();
            expressions.maximum.valueExpression = maximum;
            expressions.minimum.valueExpression = minimum;
            expressions.value.valueExpression = value;
            regeneration = new GDTKRegenerationSettings();
            incrementation = new GDTKIncrementSettings();
        }

        #endregion

        #region Public Methods

        public void AddModifier(GDTKStatModifier modifier, Dictionary<string, StatSource> sources)
        {
            if (!initialized)
            {
                StringExtensions.LogError("GDTKStat." + info.id, "AddModifier", "Stat is not initialized");
                return;
            }

            if (activeModifiers.Contains(modifier)) return;

            modifier.value.Initialize(this, sources, modifier.applies != ModifierApplication.Immediately && modifier.applies != ModifierApplication.SetValueOnce);

            switch (modifier.applies)
            {
                case ModifierApplication.Immediately:
                    AddInstantValue(modifier);
                    return;
                case ModifierApplication.RecurringOverSeconds:
                case ModifierApplication.RecurringOverTurns:
                    modifier.Activate(this);
                    break;
                case ModifierApplication.SetValueOnce:
                    AddSetValueOnce(modifier);
                    return;
                case ModifierApplication.SetValueUntilRemoved:
                    AddSetValueUntilRemoved(modifier);
                    break;
                case ModifierApplication.UntilRemoved:
                    AddValueUntilRemoved(modifier);
                    break;
            }

            activeModifiers.Add(modifier);
        }

        public void AddSubscription(StatBinding subscribeFlags, SimpleEvent subscriptionCallback)
        {
            if (subscribeFlags.HasFlag(StatBinding.Maximum) || subscribeFlags == StatBinding.Everything) expressions.maximum.onValueChanged += subscriptionCallback;
            if (subscribeFlags.HasFlag(StatBinding.Minimum) || subscribeFlags == StatBinding.Everything) expressions.minimum.onValueChanged += subscriptionCallback;
            if (subscribeFlags.HasFlag(StatBinding.RegenerationDelay) || subscribeFlags == StatBinding.Everything) regeneration.delay.onValueChanged += subscriptionCallback;
            if (subscribeFlags.HasFlag(StatBinding.RegenerationRate) || subscribeFlags == StatBinding.Everything) regeneration.rate.onValueChanged += subscriptionCallback;
            if (subscribeFlags.HasFlag(StatBinding.Special) || subscribeFlags == StatBinding.Everything) expressions.special.onValueChanged += subscriptionCallback;
            if (subscribeFlags.HasFlag(StatBinding.Value) || subscribeFlags == StatBinding.Everything) expressions.value.onValueChanged += subscriptionCallback;
        }

        public GDTKStat Clone()
        {
            GDTKStat result = new GDTKStat();

            result.info = info.Clone();
            result.expressions = expressions.Clone();
            result.regeneration = regeneration.Clone();
            result.incrementation = incrementation.Clone();

            return result;
        }

        public void DataLoad(Stream stream, int version)
        {
            if (version > BasicStats.STATS_FILE_VERSION)
            {
                throw new NotSupportedException("Invalid file version");
            }

            initialized = false;
            initializing = true;

            LoadData(new Data.StatData(stream, version));
        }

        public void DataSave(Stream stream, int version)
        {
            if (version > BasicStats.STATS_FILE_VERSION)
            {
                throw new NotSupportedException("Invalid file version");
            }

            GenerateData().Write(stream, version);
        }

        public double GetValue(string formula, StatSource other = null)
        {
            return GDTKStatsManager.GetValue(formula, owner, other);
        }

        public void Initialize(StatSource owner)
        {
            if (initialized || initializing) return;
            initializing = true;

            this.owner = owner;

            activeModifiers = new List<GDTKStatModifier>();
            expressions.Initialize(this);
            regeneration.Initialize(this);
            incrementation.Initialize(this);

            expressions.value.onValueChanged += LockRangeValue;
            expressions.minimum.onValueChanged += LockRangeMinimum;
            expressions.maximum.onValueChanged += LockRangeMaximum;

            initializing = false;
            initialized = true;
        }

        public bool IsConditionTrue(string condition, StatSource other = null)
        {
            return GDTKStatsManager.IsConditionTrue(condition, owner, other);
        }

        public bool Matches(GDTKStat stat)
        {
            if (!info.Matches(stat.info)) return false;
            if (!expressions.Matches(stat.expressions)) return false;
            if (!incrementation.Matches(stat.incrementation)) return false;
            if (!regeneration.Matches(stat.regeneration)) return false;
            return startMaxed == stat.startMaxed;
        }

        public void RemoveModifier(GDTKStatModifier modifier)
        {
            foreach (GDTKStatModifier mod in activeModifiers)
            {
                if (mod.instanceId == modifier.instanceId)
                {
                    switch (mod.applies)
                    {
                        case ModifierApplication.RecurringOverSeconds:
                        case ModifierApplication.RecurringOverTurns:
                            activeModifiers.Remove(mod);
                            mod.Deactivate();
                            break;
                        case ModifierApplication.SetValueUntilRemoved:
                            RemoveSetValueRemoved(mod);
                            activeModifiers.Remove(mod);
                            break;
                        case ModifierApplication.UntilRemoved:
                            RemoveValue(mod);
                            activeModifiers.Remove(mod);
                            break;
                    }

                    return;
                }
            }
        }

        public void RemoveModifierFromSource(IUniquelyIdentifiable source)
        {
            foreach (GDTKStatModifier mod in activeModifiers.ToArray())
            {
                if (mod.sourceId == source?.instanceId)
                {
                    RemoveModifier(mod);
                }
            }
        }

        public void RemoveSubscription(StatBinding subscribeFlags, SimpleEvent subscriptionCallback)
        {
            if (subscribeFlags.HasFlag(StatBinding.Maximum) || subscribeFlags == StatBinding.Everything) expressions.maximum.onValueChanged -= subscriptionCallback;
            if (subscribeFlags.HasFlag(StatBinding.Minimum) || subscribeFlags == StatBinding.Everything) expressions.minimum.onValueChanged -= subscriptionCallback;
            if (subscribeFlags.HasFlag(StatBinding.RegenerationDelay) || subscribeFlags == StatBinding.Everything) regeneration.delay.onValueChanged -= subscriptionCallback;
            if (subscribeFlags.HasFlag(StatBinding.RegenerationRate) || subscribeFlags == StatBinding.Everything) regeneration.rate.onValueChanged -= subscriptionCallback;
            if (subscribeFlags.HasFlag(StatBinding.Special) || subscribeFlags == StatBinding.Everything) expressions.special.onValueChanged -= subscriptionCallback;
            if (subscribeFlags.HasFlag(StatBinding.Value) || subscribeFlags == StatBinding.Everything) expressions.value.onValueChanged -= subscriptionCallback;
        }

        public void Shutdown()
        {
            expressions.Shutdown();
            regeneration.Shutdown();
            incrementation.Shutdown();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Adds a modifier that applies instantly and cannot be undone
        /// </summary>
        /// <param name="modifier"></param>
        private void AddInstantValue(GDTKStatModifier modifier)
        {
            switch (modifier.target)
            {
                case ModifierTarget.Maximum:
                    maximum = (float)GetChangeValue(maximum, modifier);
                    break;
                case ModifierTarget.Minimum:
                    minimum = (float)GetChangeValue(minimum, modifier);
                    break;
                case ModifierTarget.RegenerationRate:
                    regenerationRate = (float)GetChangeValue(regenerationRate, modifier);
                    break;
                case ModifierTarget.RegenerationDelay:
                    regenerationDelay = (float)GetChangeValue(regenerationDelay, modifier);
                    break;
                case ModifierTarget.Special:
                    special = (float)GetChangeValue(special, modifier);
                    break;
                case ModifierTarget.Value:
                    value = (float)GetChangeValue(value, modifier);
                    break;
            }
        }

        /// <summary>
        /// Apply an instant baseValue set
        /// </summary>
        /// <param name="modifier"></param>
        private void AddSetValueOnce(GDTKStatModifier modifier)
        {
            switch (modifier.target)
            {
                case ModifierTarget.Maximum:
                    expressions.maximum.value = modifier.value.value;
                    break;
                case ModifierTarget.Minimum:
                    expressions.minimum.value = modifier.value.value;
                    break;
                case ModifierTarget.RegenerationDelay:
                    regeneration.delay.value = modifier.value.value;
                    break;
                case ModifierTarget.RegenerationRate:
                    regeneration.rate.value = modifier.value.value;
                    break;
                case ModifierTarget.Special:
                    expressions.special.value = modifier.value.value;
                    break;
                case ModifierTarget.Value:
                    expressions.value.value = modifier.value.value;
                    break;
            }
        }

        /// <summary>
        /// Set baseValue and retain original for removal
        /// </summary>
        /// <param name="modifier"></param>
        private void AddSetValueUntilRemoved(GDTKStatModifier modifier)
        {
            switch (modifier.target)
            {
                case ModifierTarget.Maximum:
                    modifier.originalSetValue = GetOriginalValue(modifier.target, expressions.maximum.valueExpression);
                    expressions.maximum.valueExpression = modifier.value.valueExpression;
                    break;
                case ModifierTarget.Minimum:
                    modifier.originalSetValue = GetOriginalValue(modifier.target, expressions.minimum.valueExpression);
                    expressions.minimum.valueExpression = modifier.value.valueExpression;
                    break;
                case ModifierTarget.RegenerationDelay:
                    modifier.originalSetValue = GetOriginalValue(modifier.target, regeneration.delay.valueExpression);
                    regeneration.delay.valueExpression = modifier.value.valueExpression;
                    break;
                case ModifierTarget.RegenerationRate:
                    modifier.originalSetValue = GetOriginalValue(modifier.target, regeneration.rate.valueExpression);
                    regeneration.rate.valueExpression = modifier.value.valueExpression;
                    break;
                case ModifierTarget.Special:
                    modifier.originalSetValue = GetOriginalValue(modifier.target, expressions.special.valueExpression);
                    expressions.special.valueExpression = modifier.value.valueExpression;
                    break;
                case ModifierTarget.Value:
                    modifier.originalSetValue = GetOriginalValue(modifier.target, expressions.value.valueExpression);
                    expressions.value.valueExpression = modifier.value.valueExpression;
                    break;
            }
        }

        private void AddValueUntilRemoved(GDTKStatModifier modifier)
        {
            switch (modifier.target)
            {
                case ModifierTarget.Maximum:
                    modifier.appliedValue = GetChangeUntilRemovedValue(maximum, modifier);
                    expressions.maximum.modifierTotal += modifier.appliedValue;
                    modifier.value.onValueChanged += () => ResetModifierTotalsFromEvent(modifier.target, expressions.maximum);
                    expressions.maximum.onValueChanged?.Invoke();
                    break;
                case ModifierTarget.Minimum:
                    modifier.appliedValue = GetChangeUntilRemovedValue(minimum, modifier);
                    expressions.minimum.modifierTotal += modifier.appliedValue;
                    modifier.value.onValueChanged += () => ResetModifierTotalsFromEvent(modifier.target, expressions.minimum);
                    expressions.minimum.onValueChanged?.Invoke();
                    break;
                case ModifierTarget.RegenerationDelay:
                    modifier.appliedValue = GetChangeUntilRemovedValue(regenerationDelay, modifier);
                    regeneration.delay.modifierTotal += modifier.appliedValue;
                    modifier.value.onValueChanged += () => ResetModifierTotalsFromEvent(modifier.target, regeneration.delay);
                    regeneration.delay.onValueChanged?.Invoke();
                    break;
                case ModifierTarget.RegenerationRate:
                    modifier.appliedValue = GetChangeUntilRemovedValue(regenerationRate, modifier);
                    regeneration.rate.modifierTotal += modifier.appliedValue;
                    modifier.value.onValueChanged += () => ResetModifierTotalsFromEvent(modifier.target, regeneration.rate);
                    regeneration.rate.onValueChanged?.Invoke();
                    break;
                case ModifierTarget.Special:
                    modifier.appliedValue = GetChangeUntilRemovedValue(special, modifier);
                    expressions.special.modifierTotal += modifier.appliedValue;
                    modifier.value.onValueChanged += () => ResetModifierTotalsFromEvent(modifier.target, expressions.special);
                    expressions.special.onValueChanged?.Invoke();
                    break;
                case ModifierTarget.Value:
                    modifier.appliedValue = GetChangeUntilRemovedValue(value, modifier);
                    expressions.value.modifierTotal += modifier.appliedValue;
                    modifier.value.onValueChanged += () => ResetModifierTotalsFromEvent(modifier.target, expressions.value);
                    expressions.value.onValueChanged?.Invoke();
                    break;
            }
        }

        protected internal void FinalizeLoading(StatSource host)
        {
            if (initialized) return;

            owner = host;
            
            expressions.FinalizeLoading(info.id, host);
            regeneration.FinalizeLoading(info.id, host);
            incrementation.FinalizeLoading(info.id, host);
            foreach(GDTKStatModifier mod in activeModifiers)
            {
                mod.FinalizeLoading(host);
            }

#if UNITY_EDITOR
            // Debug allows for bypassing the lock, so secure it it
            expressions.value.onValueChanged += LockRangeValue;
            expressions.minimum.onValueChanged += LockRangeMinimum;
            expressions.maximum.onValueChanged += LockRangeMaximum;
#endif

            initialized = true;
            initializing = false;
        }

        private Data.StatData GenerateData()
        {
            return new Data.StatData
            {
                info = info,
                expressions = expressions,
                regeneration = regeneration,
                incrementation = incrementation,
                startMaxed = startMaxed,
                activeModifiers = activeModifiers,
            };
        }

        private double GetChangeValue(double originalValue, GDTKStatModifier modifier)
        {
            return modifier.changeType switch
            {
                ModifierChangeType.AddMultiplier => originalValue + (originalValue * modifier.value.value),
                ModifierChangeType.Subtract => originalValue - modifier.value.value,
                ModifierChangeType.SubtractMultiplier => originalValue - (originalValue * modifier.value.value),
                _ => originalValue + modifier.value.value,
            };
        }

        private double GetChangeUntilRemovedValue(double originalValue, GDTKStatModifier modifier)
        {
            return modifier.changeType switch
            {
                ModifierChangeType.AddMultiplier => originalValue * modifier.value.value,
                ModifierChangeType.SubtractMultiplier => -(originalValue * modifier.value.value),
                ModifierChangeType.Subtract => -modifier.value.value,
                _ => modifier.value.value,
            };
        }

        private string GetOriginalValue(ModifierTarget target, string current)
        {
            foreach (GDTKStatModifier modifier in activeModifiers)
            {
                if (modifier.applies == ModifierApplication.SetValueUntilRemoved && modifier.target == target)
                {
                    return modifier.originalSetValue;
                }
            }

            return current;
        }

        private string GetRestoreValue(ModifierTarget target, string presented)
        {
            foreach (GDTKStatModifier modifier in activeModifiers)
            {
                if (modifier.applies == ModifierApplication.SetValueUntilRemoved && modifier.target == target)
                {
                    presented = modifier.originalSetValue;
                }
            }

            return presented;
        }

        private void LoadData(Data.StatData data)
        {
            initializing = true;

            info = data.info;
            expressions.CopyFrom(data.expressions);
            regeneration.CopyFrom(data.regeneration);
            incrementation.CopyFrom(data.incrementation);
            startMaxed = data.startMaxed;
            activeModifiers = data.activeModifiers.ToList();

            initializing = false;
        }

        private void LockRangeMaximum()
        {
            if (expressions.maximum.value < minimum)
            {
                maximum = minimum;
            }
            LockRangeValue();
        }

        private void LockRangeMinimum()
        {
            if (expressions.minimum.value > maximum)
            {
                minimum = maximum;
            }
            LockRangeValue();
        }

        private void LockRangeValue()
        {
            if (expressions.value.value < minimum)
            {
                value = minimum;
            }
            else if (expressions.value.value > maximum)
            {
                value = maximum;
            }
        }

        private void RemoveSetValueRemoved(GDTKStatModifier modifier)
        {
            switch (modifier.target)
            {
                case ModifierTarget.Maximum:
                    expressions.maximum.valueExpression = GetRestoreValue(modifier.target, expressions.maximum.valueExpression);
                    break;
                case ModifierTarget.Minimum:
                    expressions.minimum.valueExpression = GetRestoreValue(modifier.target, expressions.minimum.valueExpression);
                    break;
                case ModifierTarget.RegenerationDelay:
                    regeneration.delay.valueExpression = GetRestoreValue(modifier.target, regeneration.delay.valueExpression);
                    break;
                case ModifierTarget.RegenerationRate:
                    regeneration.rate.valueExpression = GetRestoreValue(modifier.target, regeneration.rate.valueExpression);
                    break;
                case ModifierTarget.Special:
                    expressions.special.valueExpression = GetRestoreValue(modifier.target, expressions.special.valueExpression);
                    break;
                case ModifierTarget.Value:
                    expressions.value.valueExpression = GetRestoreValue(modifier.target, expressions.value.valueExpression);
                    break;
            }
        }

        private void RemoveValue(GDTKStatModifier modifier)
        {
            switch (modifier.target)
            {
                case ModifierTarget.Maximum:
                    expressions.maximum.modifierTotal -= modifier.appliedValue;
                    if (value > maximum)
                    {
                        value = maximum;
                    }
                    break;
                case ModifierTarget.Minimum:
                    expressions.minimum.modifierTotal -= modifier.appliedValue;
                    if (value < minimum)
                    {
                        value = minimum;
                    }
                    break;
                case ModifierTarget.RegenerationDelay:
                    regeneration.delay.modifierTotal -= modifier.appliedValue;
                    break;
                case ModifierTarget.RegenerationRate:
                    regeneration.rate.modifierTotal -= modifier.appliedValue;
                    break;
                case ModifierTarget.Special:
                    expressions.special.modifierTotal -= modifier.appliedValue;
                    break;
                case ModifierTarget.Value:
                    expressions.value.modifierTotal -= modifier.appliedValue;
                    if (value > maximum)
                    {
                        value = maximum;
                    }
                    else if (value < minimum)
                    {
                        value = minimum;
                    }
                    break;
            }
        }

        private void ResetModifierTotalsFromEvent(ModifierTarget target, GDTKStatValue value)
        {
            double modTotal = 0;

            foreach (GDTKStatModifier mod in activeModifiers)
            {
                if (mod.applies == ModifierApplication.UntilRemoved && mod.target == target)
                {
                    modTotal += mod.value.value;
                }
            }

            value.modifierTotal = modTotal;
        }

        #endregion

    }
}
#endif
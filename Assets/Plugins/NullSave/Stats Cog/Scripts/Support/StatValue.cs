using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Stats
{
    [CreateAssetMenu(menuName = "TOCK/Stats Cog/Stat Value", order = 2)]
    public class StatValue : ScriptableObject
    {

        #region Variables

        // UI
        [Tooltip("Category to place stat")] public string category;
        [Tooltip("Icon for stat")] public Sprite icon;
        [Tooltip("Color to use for Icon in UI")] public Color iconColor = Color.white;
        [Tooltip("Name to display for stat in UI")] public string displayName;
        [Tooltip("Color to use for Display Name in UI")] public Color textColor = Color.white;
        [Tooltip("If not checked, stat will be hidden from UI")] public bool displayInList = true;

        // Value range
        [Tooltip("Value for stat. (Simple float or formula)")] public string value = "0";
        [Tooltip("Minimum value for stat. (Simple float or formula)")] public string minValue = "0";
        [Tooltip("Maximum value for stat. (Simple float or formula)")] public string maxValue = "100";
        [Tooltip("When checked the value will automatically be set to the same as the maximum value (numeric values only)")] public bool startWithMaxValue = false;
        public bool treatAsInt = false;

        // Regeneration
        [Tooltip("Enable regeneration")] public bool enableRegen = false;
        [Tooltip("Number of seconds to wait after last time value changed before regenerating")] public string regenDelay = "1";
        [Tooltip("Amount to regenerate each second. (Applied as a fraction each frame)")] public string regenPerSecond = "1";

        // Incrementation
        [Tooltip("Enable incrementing")] public bool enableIncrement = false;
        [Tooltip("Condition that must be met to trigger an increment")] public string incrementWhen = "1 > 2";
        [Tooltip("Amount to increment (when triggered)")] public string incrementAmount = "0";
        [Tooltip("Commands to run on increment")] public string[] incrementCommand;

        // Events
        public ValueChanged onBaseMinValueChanged, onBaseValueChanged, onBaseMaxValueChanged;
        public ValueChanged onMinValueChanged, onValueChanged, onMaxValueChanged;
        public UnityEvent onInit;

        // Internal current values
        private float curBaseValue, curBaseMin, curBaseMax, curBaseRegenAmount, curBaseRegenDelay;
        private float curValue, curMin, curMax, curRegenAmount, curRegenDelay;

        private float nextRegen;
        private bool processingIncrement, wantsReprocess;

        // Monitored modifiers
        private List<StatModifier> monitoredMods;

        #endregion

        #region Properties

        /// <summary>
        /// Returns a list of active modifiers on this Stat
        /// </summary>
        public List<StatModifier> ActiveModifiers { get; private set; }

        /// <summary>
        /// Current maximum value (without modifiers)
        /// </summary>
        public float CurrentBaseMaximum
        {
            get { return curBaseMax; }
            private set
            {
                if (treatAsInt) value = Mathf.Round(value);
                if (curBaseMax == value) return;

                float oldValue = curBaseMax;
                curBaseMax = value;
                onBaseMaxValueChanged?.Invoke(oldValue, value);

                ReCalcCurrentMax();
            }
        }

        /// <summary>
        /// Current minimum value (without modifiers)
        /// </summary>
        public float CurrentBaseMinimum
        {
            get { return curBaseMin; }
            private set
            {
                if (treatAsInt) value = Mathf.Round(value);
                if (curBaseMin == value) return;

                float oldValue = curBaseMin;
                curBaseMin = value;
                onBaseMinValueChanged?.Invoke(oldValue, value);

                ReCalcCurrentMin();
            }
        }

        /// <summary>
        /// Current regeneration amount (without modifiers)
        /// </summary>
        public float CurrentBaseRegenAmount
        {
            get { return curBaseRegenAmount; }
            private set
            {
                if (treatAsInt) value = Mathf.Round(value);
                if (curBaseRegenAmount == value) return;
                curBaseRegenAmount = value;

                // Apply modifiers
                float v = curBaseRegenAmount;
                foreach (StatModifier mod in ActiveModifiers)
                {
                    if (mod.valueTarget == EffectValueTarget.RegenAmount)
                    {
                        v += mod.AppliedValue;
                    }
                }
                CurrentRegenAmount = v;
            }
        }

        /// <summary>
        /// Current regeneration dealy (without modifiers)
        /// </summary>
        public float CurrentBaseRegenDelay
        {
            get { return curBaseRegenDelay; }
            private set
            {
                if (treatAsInt) value = Mathf.Round(value);
                if (curBaseRegenDelay == value) return;
                curBaseRegenDelay = value;

                // Apply modifiers
                float v = curBaseRegenDelay;
                foreach (StatModifier mod in ActiveModifiers)
                {
                    if (mod.valueTarget == EffectValueTarget.RegenDelay)
                    {
                        v += mod.AppliedValue;
                    }
                }
                CurrentRegenDelay = v;
            }
        }

        /// <summary>
        /// Current value (without modifiers)
        /// </summary>
        public float CurrentBaseValue
        {
            get { return curBaseValue; }
            private set
            {
                if (treatAsInt) value = Mathf.Round(value);
                float unclamped = value;
                value = Mathf.Clamp(value, curMin, curMax);
                if (curBaseValue == value) return;

                float oldValue = curBaseValue;
                curBaseValue = value;
                onBaseValueChanged?.Invoke(oldValue, value);

                if (enableRegen && oldValue > curBaseValue)
                {
                    nextRegen = CurrentRegenDelay;
                }

                if (curValue != unclamped)
                {
                    ReCalcCurrentValue(unclamped);
                }

            }
        }

        /// <summary>
        /// Current maximum value (with modifiers)
        /// </summary>
        public float CurrentMaximum
        {
            get { return curMax; }
            private set
            {
                if (treatAsInt) value = Mathf.Round(value);
                if (curMax == value) return;

                float oldValue = curMax;
                curMax = value;
                onMaxValueChanged?.Invoke(oldValue, value);
            }
        }

        /// <summary>
        /// Current minimum value (with modifiers)
        /// </summary>
        public float CurrentMinimum
        {
            get { return curMin; }
            private set
            {
                if (treatAsInt) value = Mathf.Round(value);
                if (curMin == value) return;

                float oldValue = curMin;
                curMin = value;
                onMinValueChanged?.Invoke(oldValue, value);
            }
        }

        /// <summary>
        /// Current regeneration amount (with modifiers)
        /// </summary>
        public float CurrentRegenAmount { get; private set; }

        /// <summary>
        /// Current regeneration delay (with modifiers)
        /// </summary>
        public float CurrentRegenDelay { get; private set; }

        /// <summary>
        /// Current value (with modifiers)
        /// </summary>
        public float CurrentValue
        {
            get { return curValue; }
            private set
            {
                if (treatAsInt) value = Mathf.Round(value);
                value = Mathf.Clamp(value, curMin, curMax);
                if (curValue == value) return;

                float oldValue = curValue;
                curValue = value;
                onValueChanged?.Invoke(oldValue, value);
            }
        }

        public bool Initialized { get; private set; }

        public StatsCog Parent { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add an instant modifier x times
        /// </summary>
        /// <param name="mod">modifier</param>
        /// <param name="count">times to apply</param>
        public void AddInstantModifier(StatModifier mod, int count)
        {
            switch (mod.effectType)
            {
                case EffectTypes.Instant:
                    if (!value.IsNumeric())
                    {
                        Debug.Log(name + " only sustained modifiers can be used on stats with non-numeric values");
                        return;
                    }

                    mod.Initialize(this);
                    float change = mod.AppliedValue * count;
                    switch (mod.valueTarget)
                    {
                        case EffectValueTarget.MaximumValue:
                            CurrentBaseMaximum += change;
                            break;
                        case EffectValueTarget.MinimumValue:
                            CurrentBaseMinimum += change;
                            break;
                        case EffectValueTarget.RegenAmount:
                            CurrentBaseRegenAmount += change;
                            break;
                        case EffectValueTarget.RegenDelay:
                            CurrentBaseRegenDelay += change;
                            break;
                        case EffectValueTarget.Value:
                            CurrentBaseValue += change;
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// Add a new modifer 
        /// </summary>
        /// <param name="mod"></param>
        public void AddModifier(StatModifier mod)
        {
            switch (mod.effectType)
            {
                case EffectTypes.Instant:
                    if(mod.valueType == EffectValueTypes.ValueSet)
                    {
                        switch (mod.valueTarget)
                        {
                            case EffectValueTarget.MaximumValue:
                                CurrentBaseMaximum = Parent.GetExpressionValue(mod.value);
                                break;
                            case EffectValueTarget.MinimumValue:
                                CurrentBaseMinimum = Parent.GetExpressionValue(mod.value);
                                break;
                            case EffectValueTarget.RegenAmount:
                                CurrentBaseRegenAmount = Parent.GetExpressionValue(mod.value);
                                break;
                            case EffectValueTarget.RegenDelay:
                                CurrentBaseRegenDelay = Parent.GetExpressionValue(mod.value);
                                break;
                            case EffectValueTarget.Value:
                                CurrentBaseValue = Parent.GetExpressionValue(mod.value);
                                break;
                        }
                        return;
                    }

                    if (!value.IsNumeric())
                    {
                        Debug.Log(name + " only sustained modifiers can be used on stats with non-numeric values");
                        return;
                    }

                    mod.Initialize(this);
                    switch (mod.valueTarget)
                    {
                        case EffectValueTarget.MaximumValue:
                            CurrentBaseMaximum += mod.AppliedValue;
                            break;
                        case EffectValueTarget.MinimumValue:
                            CurrentBaseMinimum += mod.AppliedValue;
                            break;
                        case EffectValueTarget.RegenAmount:
                            CurrentBaseRegenAmount += mod.AppliedValue;
                            break;
                        case EffectValueTarget.RegenDelay:
                            CurrentBaseRegenDelay += mod.AppliedValue;
                            break;
                        case EffectValueTarget.Value:
                            CurrentBaseValue += mod.AppliedValue;
                            break;
                    }
                    break;
                case EffectTypes.Recurring:
                    if (!value.IsNumeric())
                    {
                        Debug.Log(name + " only sustained modifiers can be used on stats with non-numeric values");
                        return;
                    }
                    mod.Initialize(this);
                    ActiveModifiers.Add(mod);
                    monitoredMods.Add(mod);
                    break;
                case EffectTypes.Sustained:
                    mod.Initialized = false;
                    ActiveModifiers.Add(mod);
                    switch (mod.valueTarget)
                    {
                        case EffectValueTarget.MaximumValue:
                            ReCalcCurrentMax();
                            break;
                        case EffectValueTarget.MinimumValue:
                            ReCalcCurrentMin();
                            break;
                        case EffectValueTarget.Value:
                            ReCalcCurrentValue(CurrentBaseValue);
                            break;
                        case EffectValueTarget.RegenAmount:
                            ReCalcCurrentRegenAmount();
                            break;
                        case EffectValueTarget.RegenDelay:
                            ReCalcCurrentRegenDelay();
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// Initialize Stat Value
        /// </summary>
        /// <param name="owner"></param>
        public void Initialize(StatsCog owner)
        {
            ActiveModifiers = new List<StatModifier>();
            monitoredMods = new List<StatModifier>();

            // Set parent stats cog
            Parent = owner;
            Parent.onUpdateMods.AddListener(UpdateMods);

            // Subscribe
            Subscribe();

            // Set Base values
            UpdateMinValue(0, 0);
            UpdateMaxValue(0, 0);
            UpdateValue(0, 0);
            UpdateRegenAmount(0, 0);
            UpdateRegenDelay(0, 0);
            nextRegen = -1;

            // Max
            if (startWithMaxValue && value.IsNumeric())
            {
                CurrentBaseValue = CurrentBaseMaximum;
            }

            // Finalize
            Initialized = true;
            if (onInit != null) onInit.Invoke();
        }

        /// <summary>
        /// Calculate the change in value when replacing one modifier with another
        /// </summary>
        /// <param name="original">Original Stat Modifier</param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public float GetModifierChange(StatModifier original, StatModifier replacement)
        {
            float val1 = 0;
            if (original != null)
            {
                val1 = original.AppliedValue;
            }

            float val2 = 0;
            if (replacement != null)
            {
                val2 = replacement.CalculateAppliedValue(this);
            }

            return val2 - val1;
        }

        /// <summary>
        /// Load StatValue data from stream
        /// </summary>
        /// <param name="stream"></param>
        public void Load(Stream stream, float version)
        {
            if (version != 1.4f) return;

            name = stream.ReadStringPacket();
            curBaseValue = stream.ReadFloat();
            curBaseMin = stream.ReadFloat();
            curBaseMax = stream.ReadFloat();
            curBaseRegenAmount = stream.ReadFloat();
            curBaseRegenDelay = stream.ReadFloat();
            curValue = stream.ReadFloat();
            curMin = stream.ReadFloat();
            curMax = stream.ReadFloat();
            curRegenAmount = stream.ReadFloat();
            curRegenDelay = stream.ReadFloat();
            nextRegen = stream.ReadFloat();

            ActiveModifiers.Clear();
            int count = stream.ReadInt();
            for (int i = 0; i < count; i++)
            {
                StatModifier sm = new StatModifier();
                sm.Load(stream);
                ActiveModifiers.Add(sm);
            }

            // Raise events
            if (onInit != null) onInit.Invoke();
        }

        /// <summary>
        /// Removes modifier from StatValue
        /// </summary>
        /// <param name="mod"></param>
        public void RemoveModifier(StatModifier mod)
        {
            if (mod.effectType != EffectTypes.Instant)
            {
                RemoveMod(mod);
            }
        }

        /// <summary>
        /// Save StatValue data to stream
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            stream.WriteStringPacket(name);
            stream.WriteFloat(curBaseValue);
            stream.WriteFloat(curBaseMin);
            stream.WriteFloat(curBaseMax);
            stream.WriteFloat(curBaseRegenAmount);
            stream.WriteFloat(curBaseRegenDelay);
            stream.WriteFloat(curValue);
            stream.WriteFloat(curMin);
            stream.WriteFloat(curMax);
            stream.WriteFloat(curRegenAmount);
            stream.WriteFloat(curRegenDelay);
            stream.WriteFloat(nextRegen);
            stream.WriteInt(ActiveModifiers.Count);

            foreach (StatModifier modifier in ActiveModifiers)
            {
                modifier.Save(stream);
            }
        }

        /// <summary>
        /// Relay command to parent for evaluation
        /// </summary>
        /// <param name="command"></param>
        public void SendCommand(string command)
        {
            Parent.StartCoroutine(DoSendCommand(command));
        }

        public void SetMaximum(float value)
        {
            CurrentBaseMaximum = value;
        }

        public void SetMinimum(float value)
        {
            CurrentBaseMinimum = value;
        }

        public void SetValue(float value)
        {
            value = Mathf.Clamp(value, CurrentBaseMinimum, CurrentBaseMaximum);
            if (enableRegen && value < CurrentBaseValue)
            {
                nextRegen = CurrentRegenDelay;
            }
            CurrentBaseValue = value;
        }

        public void SetToMaximum()
        {
            CurrentBaseValue = CurrentMaximum;
        }

        public bool Validate(StatsCog validator, out string error)
        {
            if (!validator.ValidateExpression(value))
            {
                error = "Invalid value expression";
                return false;
            }

            error = null;
            return true;
        }

        #endregion

        #region Private Methods

        private IEnumerator DoSendCommand(string command)
        {
            yield return new WaitForEndOfFrame();
            Parent.SendCommand(command);
        }

        private void ReCalcCurrentMax()
        {
            // Apply modifiers
            float v = curBaseMax;
            foreach (StatModifier mod in ActiveModifiers)
            {
                if (mod.effectType == EffectTypes.Sustained)
                {
                    if (!mod.Initialized) mod.Initialize(this);
                    if (mod.valueTarget == EffectValueTarget.MaximumValue)
                    {
                        v += mod.AppliedValue;
                    }
                }
            }
            CurrentMaximum = v;
            ReCalcCurrentValue(CurrentBaseValue);
        }

        private void ReCalcCurrentMin()
        {
            // Apply modifiers
            float v = curBaseMin;
            foreach (StatModifier mod in ActiveModifiers)
            {
                if (mod.effectType == EffectTypes.Sustained)
                {
                    if (!mod.Initialized) mod.Initialize(this);
                    if (mod.valueTarget == EffectValueTarget.MinimumValue)
                    {
                        v += mod.AppliedValue;
                    }
                }
            }
            CurrentMinimum = v;
            ReCalcCurrentValue(CurrentBaseValue);
        }

        private void ReCalcCurrentRegenAmount()
        {
            // Apply modifiers
            float v = curBaseRegenAmount;
            foreach (StatModifier mod in ActiveModifiers)
            {
                if (mod.effectType == EffectTypes.Sustained)
                {
                    if (!mod.Initialized) mod.Initialize(this);
                    if (mod.valueTarget == EffectValueTarget.RegenAmount)
                    {
                        v += mod.AppliedValue;
                    }
                }
            }

            if (v < 0) v = 0;
            CurrentRegenAmount = v;
        }

        private void ReCalcCurrentRegenDelay()
        {
            // Apply modifiers
            float v = curBaseRegenDelay;
            foreach (StatModifier mod in ActiveModifiers)
            {
                if (mod.effectType == EffectTypes.Sustained)
                {
                    if (!mod.Initialized) mod.Initialize(this);
                    if (mod.valueTarget == EffectValueTarget.RegenDelay)
                    {
                        v += mod.AppliedValue;
                    }
                }
            }

            if (v < 0) v = 0;
            CurrentRegenDelay = v;
        }

        private void ReCalcCurrentValue(float value)
        {
            // Apply modifiers
            float v = value;
            foreach (StatModifier mod in ActiveModifiers)
            {
                if (mod.effectType == EffectTypes.Sustained)
                {
                    if (!mod.Initialized) mod.Initialize(this);
                    if (mod.valueTarget == EffectValueTarget.Value)
                    {
                        v += mod.AppliedValue;
                    }
                }
            }
            CurrentValue = v;
        }

        private void RemoveMod(StatModifier modifier)
        {
            foreach (StatModifier mod in ActiveModifiers)
            {
                if (mod.id == modifier.id)
                {
                    ActiveModifiers.Remove(mod);
                    monitoredMods.Remove(mod);
                    if (mod.effectType == EffectTypes.Sustained)
                    {
                        switch (mod.valueTarget)
                        {
                            case EffectValueTarget.MaximumValue:
                                ReCalcCurrentMax();
                                CurrentBaseValue = curBaseValue;
                                break;
                            case EffectValueTarget.MinimumValue:
                                ReCalcCurrentMin();
                                CurrentBaseValue = curBaseValue;
                                break;
                            case EffectValueTarget.Value:
                                ReCalcCurrentValue(curBaseValue);
                                break;
                            case EffectValueTarget.RegenAmount:
                                ReCalcCurrentRegenAmount();
                                break;
                            case EffectValueTarget.RegenDelay:
                                ReCalcCurrentRegenDelay();
                                break;
                        }
                    }
                    return;
                }
            }
        }

        private void Subscribe()
        {
            StatValue stat;
            List<StatValue> subs;
            List<string> req;

            // Min Value
            subs = new List<StatValue>();
            req = Parent.GetSubscriptionRequirements(minValue);
            foreach (string statName in req)
            {
                stat = Parent.FindStat(statName);
                if (stat != null && !subs.Contains(stat))
                {
                    stat.onValueChanged.AddListener(UpdateMinValue);
                    subs.Add(stat);
                }
            }

            // Max Value
            subs = new List<StatValue>();
            req = Parent.GetSubscriptionRequirements(maxValue);
            foreach (string statName in req)
            {
                stat = Parent.FindStat(statName);
                if (stat != null && !subs.Contains(stat))
                {
                    stat.onValueChanged.AddListener(UpdateMaxValue);
                    subs.Add(stat);
                }
            }

            // Value
            subs = new List<StatValue>();
            req = Parent.GetSubscriptionRequirements(value);
            foreach (string statName in req)
            {
                stat = Parent.FindStat(statName);
                if (stat != null && !subs.Contains(stat))
                {
                    stat.onValueChanged.AddListener(UpdateValue);
                    subs.Add(stat);
                }
            }

            // Regen Delay
            subs = new List<StatValue>();
            req = Parent.GetSubscriptionRequirements(regenDelay);
            foreach (string statName in req)
            {
                stat = Parent.FindStat(statName);
                if (stat != null && !subs.Contains(stat))
                {
                    stat.onValueChanged.AddListener(UpdateRegenDelay);
                    subs.Add(stat);
                }
            }

            // Regen Amount
            subs = new List<StatValue>();
            req = Parent.GetSubscriptionRequirements(regenPerSecond);
            foreach (string statName in req)
            {
                stat = Parent.FindStat(statName);
                if (stat != null && !subs.Contains(stat))
                {
                    stat.onValueChanged.AddListener(UpdateRegenAmount);
                    subs.Add(stat);
                }
            }

            // Increment
            subs = new List<StatValue>();
            req = Parent.GetSubscriptionRequirements(incrementWhen);
            foreach (string statName in req)
            {
                stat = Parent.FindStat(statName);
                if (stat != null && !subs.Contains(stat))
                {
                    stat.onValueChanged.AddListener(UpdateIncrement);
                    subs.Add(stat);
                }
            }
        }

        private void UpdateIncrement(float oldValue, float newValue)
        {
            if (processingIncrement)
            {
                wantsReprocess = true;
                return;
            }

            if (Parent.EvaluateCondition(incrementWhen))
            {
                processingIncrement = true;
                CurrentBaseValue += Parent.GetExpressionValue(incrementAmount);
                if (incrementCommand != null)
                {
                    foreach (string command in incrementCommand)
                    {
                        Parent.SendCommand(command);
                    }
                }
                processingIncrement = false;
                if (wantsReprocess)
                {
                    wantsReprocess = false;
                    UpdateIncrement(0, 0);
                }
            }
        }

        private void UpdateMaxValue(float oldValue, float newValue)
        {
            CurrentBaseMaximum = Parent.GetExpressionValue(maxValue);
        }

        private void UpdateMinValue(float oldValue, float newValue)
        {
            CurrentBaseMinimum = Parent.GetExpressionValue(minValue);
        }

        /// <summary>
        /// Update StatValue
        /// </summary>
        private void UpdateMods()
        {
            float preMod = CurrentValue;

            // Regenerate
            if (enableRegen)
            {
                if (CurrentValue < CurrentMaximum)
                {
                    if (nextRegen == -1)
                    {
                        nextRegen = CurrentRegenDelay;
                    }
                    if (nextRegen > 0)
                    {
                        nextRegen -= Time.deltaTime;
                    }
                    else
                    {
                        CurrentBaseValue += CurrentRegenAmount * Time.deltaTime;
                    }
                }
                else if (CurrentValue == CurrentMaximum)
                {
                    nextRegen = -1;
                }
            }

            // Recurring modifiers
            foreach (StatModifier modifier in monitoredMods)
            {
                switch (modifier.valueTarget)
                {
                    case EffectValueTarget.MaximumValue:
                        CurrentBaseMaximum += modifier.AppliedValue * Time.deltaTime;
                        break;
                    case EffectValueTarget.MinimumValue:
                        CurrentBaseMinimum += modifier.AppliedValue * Time.deltaTime;
                        break;
                    case EffectValueTarget.RegenAmount:
                        CurrentBaseRegenAmount += modifier.AppliedValue * Time.deltaTime;
                        break;
                    case EffectValueTarget.RegenDelay:
                        CurrentBaseRegenDelay += modifier.AppliedValue * Time.deltaTime;
                        break;
                    case EffectValueTarget.Value:
                        CurrentBaseValue += modifier.AppliedValue * Time.deltaTime;
                        break;
                }
            }

            if(name == Parent.healthStat && CurrentValue < preMod)
            {
                Parent.onDamageTaken?.Invoke(preMod - CurrentValue, null, null);
            }
        }

        private void UpdateRegenAmount(float oldValue, float newValue)
        {
            CurrentBaseRegenAmount = Parent.GetExpressionValue(regenPerSecond);
        }

        private void UpdateRegenDelay(float oldValue, float newValue)
        {
            CurrentBaseRegenDelay = Parent.GetExpressionValue(regenDelay);
        }

        private void UpdateValue(float oldValue, float newValue)
        {
            CurrentBaseValue = Parent.GetExpressionValue(value);
        }

        #endregion

    }
}
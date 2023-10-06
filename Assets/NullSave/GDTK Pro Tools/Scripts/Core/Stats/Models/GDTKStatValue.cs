#if GDTK
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NullSave.GDTK.Stats
{
    [Serializable]
    public class GDTKStatValue
    {

        #region Fields

        [SerializeField] [JsonSerializeAs("valueType")] [Tooltip("Type of value for this stat")] private ValueType m_valueType;
        [SerializeField] [JsonSerializeAs("value")] [Tooltip("Value")] private string m_value;
        [SerializeField] [JsonSerializeAs("randomMin")] [Tooltip("Minimum random value")] private string m_randomMin;
        [SerializeField] [JsonSerializeAs("randomMax")] [Tooltip("Maximum random value")] private string m_randomMax;
        [SerializeField] [JsonSerializeAs("conditionalValues")] [Tooltip("Conditinal values")] private List<GDTKConditionalValue> m_conditions;

        public SimpleEvent onValueChanged;

        [JsonSerializeAs("initialValue")] private double valueInit;
        [JsonSerializeAs("modifierTotal")] private double modTotal;
        [JsonSerializeAs("valueTotal")] private double valueTotal;

        private List<SimpleEvent> subscriptions;
        [NonSerialized] private GDTKStat owner;
        [NonSerialized] private StatSource other;
        [NonSerialized] private StatSource ownerProvider;
        [NonSerialized] private Dictionary<string, StatSource> sources;

        private bool m_locked;
        private List<double> pendingUpdates;

        #endregion

        #region Properties

        public IReadOnlyList<GDTKConditionalValue> conditions
        {
            get
            {
                if (m_conditions == null) m_conditions = new List<GDTKConditionalValue>();
                return m_conditions;
            }
        }

        public bool initialized { get; private set; }

        [JsonSerialize] public bool isExpressionNumeric { get; private set; }

        public bool locked
        {
            get { return m_locked; }
            set
            {
                if (m_locked == value) return;
                m_locked = value;

                if (m_locked)
                {
                    pendingUpdates = new List<double>();
                }
                else
                {
                    if (m_valueType == ValueType.Conditional)
                    {
                        foreach (double pendingValue in pendingUpdates)
                        {
                            UpdateConditionalValue(pendingValue);
                        }
                    }
                    else
                    {
                        foreach (double pendingValue in pendingUpdates)
                        {
                            this.value = pendingValue;
                        }
                    }
                    pendingUpdates = null;
                }
            }
        }

        [JsonDoNotSerialize]
        public double modifierTotal
        {
            get { return modTotal; }
            set
            {
                if (modTotal == value) return;
                modTotal = value;
                valueTotal = valueInit + modTotal;
                if (initialized)
                {
                    onValueChanged?.Invoke();
                }
            }
        }

        private StatSource provider
        {
            get
            {
                if (owner != null) return owner.owner;
                return ownerProvider;
            }
        }

        public double randomMaximum { get; private set; }

        public double randomMinimum { get; private set; }

        [JsonDoNotSerialize]
        public double value
        {
            get { return valueInit; }
            set
            {
                if (m_valueType == ValueType.Conditional) return;
                if (valueInit == value) return;

                if (m_locked)
                {
                    pendingUpdates.Add(value);
                    return;
                }

                valueInit = value;
                valueTotal = valueInit + modTotal;
                if (initialized) onValueChanged?.Invoke();
            }
        }

        [JsonDoNotSerialize]
        public string valueExpression
        {
            get { return m_value; }
            set
            {
                if (m_valueType != ValueType.Standard) return;
                if (m_value == value) return;
                m_value = value;
                if (initialized)
                {
                    ReleaseSubscriptions();
                    GenerateSubscriptions();
                    UpdateValueFromSubscription();
                    onValueChanged?.Invoke();
                }
            }
        }

        [JsonDoNotSerialize]
        public ValueType valueType
        {
            get { return m_valueType; }
        }

        public double valueWithModifiers
        {
            get
            {
                return valueTotal;
            }
        }

        #endregion

        #region Constructor

        public GDTKStatValue()
        {
            m_value = "0";
            m_randomMax = "1";
            m_randomMin = "0";
            m_conditions = new List<GDTKConditionalValue>();
            subscriptions = new List<SimpleEvent>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clone object
        /// </summary>
        /// <returns></returns>
        public GDTKStatValue Clone()
        {
            GDTKStatValue result = new GDTKStatValue();

            result.m_valueType = m_valueType;
            result.m_value = m_value;
            result.m_randomMax = m_randomMax;
            result.m_randomMin = m_randomMin;
            foreach (GDTKConditionalValue cond in m_conditions) result.m_conditions.Add(cond.Clone());

            return result;
        }

        public void CopyFrom(GDTKStatValue source)
        {
            owner = source.owner;
            m_valueType = source.valueType;
            m_value = source.m_value;
            m_randomMin = source.m_randomMin;
            m_randomMax = source.m_randomMax;
            m_conditions = source.m_conditions.ToList();
            valueInit = source.valueInit;
            modTotal = source.modifierTotal;
            valueTotal = source.valueTotal;
            isExpressionNumeric = source.isExpressionNumeric;
        }

        public void DataLoad(Stream stream, string statId, int version)
        {
            if (version > BasicStats.STATS_FILE_VERSION)
            {
                throw new NotSupportedException("Invalid file version");
            }

            LoadData(new Data.StatValueData(stream, version));
        }

        public void DataSave(Stream stream, int version)
        {
            GenerateData().Write(stream, version);
        }

        /// <summary>
        /// Create object from string expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static GDTKStatValue FromExpression(string expression)
        {
            GDTKStatValue result = new GDTKStatValue();

            if (expression != null)
            {
                string conCheck = expression.ToLower();

                if (conCheck.StartsWith("range:"))
                {
                    string[] parts = expression.Substring(6).Split('|');
                    result.m_randomMin = parts[0];
                    result.m_randomMax = parts[1];
                    result.m_valueType = ValueType.RandomRange;
                }
                else if (conCheck.StartsWith("conditional:"))
                {
                    result.m_valueType = ValueType.Conditional;
                    string[] parts;
                    foreach (string part in expression.Substring(12).Split(','))
                    {
                        if (!string.IsNullOrEmpty(part))
                        {
                            parts = part.Split('|');
                            result.m_conditions.Add(new GDTKConditionalValue { condition = parts[0], value = parts[1] });
                        }
                    }
                }
                else
                {
                    result.m_valueType = ValueType.Standard;
                    result.m_value = expression;
                }
            }

            return result;
        }

        public double GetUninitializedValue(StatSource statSource, StatSource global, StatSource other)
        {
            if (initialized) return value;

            switch (m_valueType)
            {
                case ValueType.Conditional:
                    foreach (GDTKConditionalValue condition in m_conditions)
                    {
                        if (string.IsNullOrEmpty(condition.condition) ||
                            GDTKStatsManager.IsConditionTrue(condition.condition, statSource, other))
                        {
                            return provider.GetValue(condition.value);
                        }
                    }
                    return 0;
                case ValueType.RandomRange:
                    randomMaximum = GDTKStatsManager.GetValue(m_randomMax, statSource, other);
                    randomMinimum = GDTKStatsManager.GetValue(m_randomMin, statSource, other);
                    return Random.Range((float)randomMinimum, (float)randomMaximum);
                case ValueType.Standard:
                default:
                    return GDTKStatsManager.GetValue(m_value, statSource, other);
            }
        }

        public void Initialize(GDTKStat stat, bool generateSubscriptions = true, StatSource other = null)
        {
            if (initialized) return;

            owner = stat;
            this.other = other;
            ownerProvider = owner.owner;

            if (generateSubscriptions)
            {
                GenerateSubscriptions();
            }

            switch (m_valueType)
            {
                case ValueType.Conditional:
                    UpdateValueFromSubscription();
                    break;
                case ValueType.RandomRange:
                    randomMaximum = provider.GetValue(m_randomMax, other);
                    randomMinimum = provider.GetValue(m_randomMin, other);
                    valueInit = Random.Range((float)randomMinimum, (float)randomMaximum);
                    break;
                case ValueType.Standard:
                    isExpressionNumeric = m_value.IsNumeric();
                    valueInit = provider.GetValue(m_value, other);
                    break;
            }

            valueTotal = valueInit;

            initialized = true;
        }

        public void Initialize(GDTKStat stat, Dictionary<string, StatSource> sources, bool generateSubscriptions = true)
        {
            if (initialized) return;

            owner = stat;
            this.sources = sources;
            ownerProvider = owner.owner;

            if (generateSubscriptions)
            {
                GenerateSubscriptions();
            }

            switch (m_valueType)
            {
                case ValueType.Conditional:
                    UpdateValueFromSubscription();
                    break;
                case ValueType.RandomRange:
                    randomMaximum = provider.GetValue(m_randomMax, sources);
                    randomMinimum = provider.GetValue(m_randomMin, sources);
                    valueInit = Random.Range((float)randomMinimum, (float)randomMaximum);
                    break;
                case ValueType.Standard:
                    isExpressionNumeric = m_value.IsNumeric();
                    valueInit = provider.GetValue(m_value, sources);
                    break;
            }

            valueTotal = valueInit;

            initialized = true;
        }

        public void Shutdown()
        {
            onValueChanged = null;
            ReleaseSubscriptions();
            initialized = false;
        }

        /// <summary>
        /// Export object to a string expression
        /// </summary>
        /// <returns></returns>
        public string ToExpression()
        {
            switch (m_valueType)
            {
                case ValueType.Conditional:
                    bool isFirst = true;
                    StringBuilder sb = new StringBuilder("conditional:");
                    foreach (GDTKConditionalValue condition in m_conditions)
                    {
                        if (isFirst)
                        {
                            isFirst = false;
                        }
                        else
                        {
                            sb.Append(",");
                        }
                        sb.Append(condition.condition);
                        sb.Append("|");
                        sb.Append(condition.value);
                    }
                    return sb.ToString();
                case ValueType.RandomRange:
                    return "range:" + m_randomMin + "|" + m_randomMax;
                default:    // Standard
                    return m_value;
            }
        }

        #endregion

        #region Private Methods

        protected internal void FinalizeLoading(string statId, StatSource host)
        {
            if (initialized) return;

            ownerProvider = host;
            owner = host.GetStat(statId);
            GenerateSubscriptions();

            initialized = true;
        }

        private Data.StatValueData GenerateData()
        {
            Data.StatValueData data = new Data.StatValueData();
            data.valueType = valueType;
            data.value = m_value;
            data.randomMax = m_randomMax;
            data.randomMin = m_randomMin;
            data.conditionalValues = m_conditions.ToList();
            data.initialValue = valueInit;
            data.modifierTotal = modTotal;
            data.valueTotal = valueTotal;
            data.isExpressionNumeric = isExpressionNumeric;
            return data;
        }

        private void GenerateSubscriptions()
        {
            ReleaseSubscriptions();

            string checkValue = string.Empty;
            switch (m_valueType)
            {
                case ValueType.Conditional:
                    foreach (GDTKConditionalValue value in m_conditions)
                    {
                        checkValue += value.condition + " ";
                    }
                    break;
                case ValueType.Standard:
                    checkValue = m_value;
                    break;
            }

            if (checkValue == string.Empty) return;

            subscriptions = GDTKStatsManager.AutoSubscribe(checkValue, UpdateValueFromSubscription, provider, other);
        }

        private void LoadData(Data.StatValueData data)
        {
            m_valueType = data.valueType;
            m_value = data.value;
            m_randomMin = data.randomMin;
            m_randomMax = data.randomMax;
            m_conditions = data.conditionalValues.ToList();
            valueInit = data.initialValue;
            modTotal = data.modifierTotal;
            valueTotal = data.valueTotal;
            isExpressionNumeric = data.isExpressionNumeric;
        }

        private void ReleaseSubscriptions()
        {
            for (int i = 0; i < subscriptions.Count; i++)
            {
                subscriptions[i] -= UpdateValueFromSubscription;
            }
            subscriptions.Clear();
        }

        private void UpdateConditionalValue(double change)
        {
            if (m_locked)
            {
                if (!pendingUpdates.Contains(change))
                {
                    pendingUpdates.Add(change);
                }
                return;
            }

            valueInit = change;
            valueTotal = valueInit + modTotal;
            if (initialized) onValueChanged?.Invoke();
        }

        private void UpdateValueFromSubscription()
        {
            switch (m_valueType)
            {
                case ValueType.Conditional:
                    foreach (GDTKConditionalValue condition in m_conditions)
                    {
                        if (string.IsNullOrEmpty(condition.condition) || provider.IsConditionTrue(condition.condition, other))
                        {
                            // Conditional values cannot be set through value property
                            double change = provider.GetValue(condition.value, other);
                            if (valueInit != change)
                            {
                                UpdateConditionalValue(change);
                            }
                            return;
                        }
                    }

                    value = 0;
                    onValueChanged?.Invoke();
                    break;
                case ValueType.Standard:
                    value = owner.GetValue(m_value);
                    break;
            }
        }

        #endregion

#if UNITY_EDITOR

        public static GDTKStatValue FromProp(UnityEditor.SerializedProperty source)
        {
            GDTKStatValue result = new GDTKStatValue();

            result.m_valueType = (ValueType)source.FindPropertyRelative("m_valueType").intValue;
            result.m_value = source.FindPropertyRelative("m_value").stringValue;
            result.m_randomMin = source.FindPropertyRelative("m_randomMin").stringValue;
            result.m_randomMax = source.FindPropertyRelative("m_randomMax").stringValue;
            result.m_conditions = new List<GDTKConditionalValue>();
            UnityEditor.SerializedProperty list = source.FindPropertyRelative("m_conditions");
            for (int i = 0; i < list.arraySize; i++)
            {
                UnityEditor.SerializedProperty entry = list.GetArrayElementAtIndex(i);
                GDTKConditionalValue cv = new GDTKConditionalValue();
                cv.condition = entry.FindPropertyRelative("condition").stringValue;
                cv.value = entry.FindPropertyRelative("value").stringValue;
                result.m_conditions.Add(cv);
            }

            return result;
        }

        public void ToProp(UnityEditor.SerializedProperty target)
        {
            target.FindPropertyRelative("m_valueType").intValue = (int)m_valueType;
            target.FindPropertyRelative("m_value").stringValue = m_value;
            target.FindPropertyRelative("m_randomMin").stringValue = m_randomMin;
            target.FindPropertyRelative("m_randomMax").stringValue = m_randomMax;

            UnityEditor.SerializedProperty list = target.FindPropertyRelative("m_conditions");
            list.arraySize = m_conditions.Count;
            for (int i = 0; i < list.arraySize; i++)
            {
                UnityEditor.SerializedProperty entry = list.GetArrayElementAtIndex(i);
                entry.FindPropertyRelative("condition").stringValue = m_conditions[i].condition;
                entry.FindPropertyRelative("value").stringValue = m_conditions[i].value;
            }
        }

#endif

    }
}
#endif
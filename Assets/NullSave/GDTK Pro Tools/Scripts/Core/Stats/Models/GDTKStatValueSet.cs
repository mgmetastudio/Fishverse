#if GDTK
using System;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [Serializable]
    public class GDTKStatValueSet
    {

        #region Fields

        [Tooltip("Minimum value"), SerializeField] private double m_minimum;
        [Tooltip("Maximum value"), SerializeField] private double m_maximum;
        [Tooltip("Actual value"), SerializeField] private double m_value;
        [Tooltip("Special value not bound by minimum or maximum"), SerializeField] private double m_special;

        [JsonDoNotSerialize] public SimpleEvent onMinimumChanged, onMaximumChanged, onValueChanged, onSpecialChanged;

        #endregion

        #region Properties

        public bool initialized { get; private set; }

        public double maximum
        {
            get { return m_maximum; }
            set
            {
                if (m_maximum == value) return;
                m_maximum = value;
                onMaximumChanged?.Invoke();
                if (this.value > value) this.value = value;
            }
        }

        public double minimum
        {
            get { return m_minimum; }
            set
            {
                if (m_minimum == value) return;
                m_minimum = value;
                onMinimumChanged?.Invoke();
                if (this.value < value) this.value = value;
            }
        }

        public double special
        {
            get { return m_special; }
            set
            {
                if (m_special == value) return;
                m_special = value;
                onSpecialChanged?.Invoke();
            }
        }

        public double value
        {
            get { return m_value; }
            set
            {
                if (value < minimum) value = minimum;
                if (value > maximum) value = maximum;
                if (m_value == value) return;
                m_value = value;

                onValueChanged?.Invoke();
            }
        }

        #endregion

        #region Public Methods

        public GDTKStatValueSet Clone()
        {
            GDTKStatValueSet result = new GDTKStatValueSet();

            result.m_minimum = m_minimum;
            result.m_maximum = m_maximum;
            result.m_value = m_value;
            result.m_special = m_special;

            return result;
        }

        public void Initialize(GDTKStat owner)
        {
            if (initialized) return;

            m_maximum = ResolveStatValue(owner, owner.expressions.maximum);
            m_minimum = ResolveStatValue(owner, owner.expressions.minimum);
            m_value = ResolveStatValue(owner, owner.expressions.value);
            m_special = ResolveStatValue(owner, owner.expressions.special);

            initialized = true;
        }

        #endregion

        #region Private Methods

        private double ResolveStatValue(GDTKStat owner, GDTKStatValue statValue)
        {
            double result = 0;

            //!!
            //switch(statValue.valueType)
            //{
            //    case ValueType.Conditional:
            //        break;
            //    case ValueType.RandomRange:
            //        expression = statValue.randomMaximum;

            //        break;
            //    case ValueType.Standard:
            //        expression = statValue.valueExpression;
            //        result = GDTKStatsManager.GetValue(expression, owner.parent);
            //        Debug.Log("Standard Result: " + result);
            //        break;
            //}

            return result;
        }

        #endregion

    }
}
#endif
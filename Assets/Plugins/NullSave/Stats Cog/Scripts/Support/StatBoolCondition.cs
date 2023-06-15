using System;

namespace NullSave.TOCK.Stats
{
    [Serializable]
    public class StatConditionalBool
    {

        #region Variables

        public ConditionalValueSource valueSource = ConditionalValueSource.Static;
        public bool value;
        public string valueFromStat = "1 = 0";

        #endregion

        #region Public Methods

        public StatConditionalBool Clone()
        {
            StatConditionalBool newObj = new StatConditionalBool();
            newObj.value = value;
            newObj.valueFromStat = valueFromStat;
            newObj.valueSource = valueSource;
            return newObj;
        }

        public bool GetValue(StatsCog statsCog)
        {
            if (valueSource == ConditionalValueSource.Static) return value;
            return statsCog.EvaluateCondition(valueFromStat);
        }

        public bool Validate(StatsCog validator, out string error)
        {
            if (valueSource == ConditionalValueSource.StatsCog)
            {
                if (!validator.ValidateExpression(valueFromStat))
                {
                    error = "Invalid bool expression";
                    return false;
                }
            }

            error = null;
            return true;
        }

        #endregion

    }
}

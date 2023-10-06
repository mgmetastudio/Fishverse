#if GDTK
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [Serializable]
    public class GDTKStatMapperTarget
    {

        #region Fields

        [Tooltip("Component to send Stat to")] public Component target;
        [Tooltip("Field to bind to Stat")] public string fieldTarget;
        [Tooltip("Id of Stat to bind")] public string statName;
        [Tooltip("Expression used to bind")] public string expression;
        [Tooltip("Update Stat with value from component")] public bool biDirectional;
        [Tooltip("Source providing Stats")] public BasicStats statSource;

        public GDTKStat stat;
        public FieldInfo field;

#if UNITY_EDITOR
        public string[] options;
        public int selOption;
#endif

        #endregion

        #region Properties

        public bool isBool
        {
            get
            {
                if (field == null) return false;
                return field.FieldType == typeof(bool);
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(BasicStats stats)
        {
            statSource = stats;
            field = target.GetType().GetField(fieldTarget);

            stats.onStatsReloaded += Rebind;
            Rebind();
        }

        public void Update()
        {
            if (!isBool)
            {
                float val = (float)field.GetValue(target);
                if (stat.value != val)
                {
                    if (stat.regeneration.isRegenerating && val < stat.value)
                    {
                        stat.regeneration.ResetRegeneration();
                    }
                    stat.value = val;

                }
            }
        }

#if UNITY_EDITOR
        public void UpdateOptions()
        {
            if (target == null)
            {
                options = new string[0];
                selOption = -1;
                fieldTarget = string.Empty;
            }
            else
            {
                FieldInfo[] fields = target.GetType().GetFields();
                List<string> availOptions = new List<string>();
                foreach (FieldInfo field in fields)
                {
                    if (field.FieldType == typeof(int) ||
                     field.FieldType == typeof(Single) ||
                     field.FieldType == typeof(double) ||
                     field.FieldType == typeof(float) ||
                     field.FieldType == typeof(long) ||
                     field.FieldType == typeof(bool)
                     )
                    {
                        availOptions.Add(field.Name);
                    }
                }

                options = availOptions.ToArray();
                for (int i = 0; i < options.Length; i++)
                {
                    if (options[i] == fieldTarget)
                    {
                        selOption = i;
                        field = target.GetType().GetField(fieldTarget);
                    }
                }
            }
        }

#endif
        #endregion

        #region Private Methods

        private void Rebind()
        {
            if (isBool)
            {
                GDTKStatsManager.AutoSubscribe(expression, SetValueByExpression, statSource.source);
                SetValueByExpression();
            }
            else
            {
                stat = statSource.GetStat(statName);
                stat.AddSubscription(StatBinding.Value, SetValue);
                SetValue();
            }
        }

        private void SetValue()
        {
            field.SetValue(target, Convert.ChangeType(stat.value, field.FieldType));
        }

        private void SetValueByExpression()
        {
            field.SetValue(target, statSource.IsConditionTrue(expression));
        }

        #endregion

    }
}
#endif
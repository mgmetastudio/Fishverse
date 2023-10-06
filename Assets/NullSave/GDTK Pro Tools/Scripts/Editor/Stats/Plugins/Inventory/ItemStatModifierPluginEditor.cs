#if GDTK_Inventory2
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [CustomUniversalPluginEditor(typeof(StatModifierPlugin))]
    public class StatModifierPluginEditor : UniversalPluginEditor
    {

        #region Fields

        private List<GDTKStatModifier> statModifiers;
        private Type modType;
        private Type valType;

        #endregion

        #region Overrides

        public override void OnEnable()
        {
            statModifiers = (List<GDTKStatModifier>)PropertyObjectValue("statModifiers");
            modType = typeof(GDTKStatModifier);
            valType = typeof(GDTKStatValue);
        }

        public override void OnInspectorGUI()
        {
            DrawStatModifierList();
        }

        #endregion

        #region Private Methods

        private bool DrawStatModifier(GDTKStatModifier modifier)
        {
            FieldInfo fiExpanded = modType.GetField("z_expanded", BindingFlags.Instance | BindingFlags.NonPublic);
            bool expanded = (bool)fiExpanded.GetValue(modifier);
            bool result;
            ModifierApplication application = modifier.applies;
            bool wantsDelete = false;

            // Draw plugin bar
            GUILayout.BeginHorizontal();

            result = GDTKEditor.DrawToggleBarTitleLeft(expanded, modifier.GetDescription());
            if (result != expanded)
            {
                //!!modifier.z_expanded = result;
                fiExpanded.SetValue(modifier, result);
            }

            // Delete
            if (GDTKEditor.DrawBarIcon(GDTKEditor.Styles.ButtonRight, GDTKEditor.GetIcon("icons/trash")))
            {
                wantsDelete = true;
            }

            GUILayout.EndHorizontal();

            // Draw Expanded item
            if (expanded)
            {
                GUILayout.Space(-4);
                GUILayout.BeginVertical("box");

                EditorGUI.BeginChangeCheck();
                GUILayout.Label("Behavior", SubHeaderStyle);
                modifier.affectsStatId = EditorGUILayout.TextField("Affects Stat Id", modifier.affectsStatId);
                modifier.requirements = EditorGUILayout.TextField("Requirements", modifier.requirements);
                modifier.applies = (ModifierApplication)EditorGUILayout.EnumPopup("Applies", modifier.applies);
                if (application == ModifierApplication.RecurringOverSeconds || application == ModifierApplication.RecurringOverTurns)
                {
                    modifier.lifespan = EditorGUILayout.FloatField("Lifespan", modifier.lifespan);
                    modifier.wholeIncrements = EditorGUILayout.Toggle("Whole Increments", modifier.wholeIncrements);
                }

                modifier.target = (ModifierTarget)EditorGUILayout.EnumPopup("Target", modifier.target);
                if (application != ModifierApplication.SetValueOnce && application != ModifierApplication.SetValueUntilRemoved)
                {
                    modifier.changeType = (ModifierChangeType)EditorGUILayout.EnumPopup("Change Type", modifier.changeType);
                }
                DrawStatValue(modifier.value);

                if (EditorGUI.EndChangeCheck())
                {
                    PropertyObjectValue("statModifiers", statModifiers, true);
                }

                GUILayout.EndVertical();
            }

            return wantsDelete;
        }

        public void DrawStatModifierList()
        {
            int removeAt = -1;

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(EditorGUIUtility.labelWidth));
            GUILayout.Label("Stat Modifiers");
            GUILayout.EndVertical();

            GUILayout.Label("(" + statModifiers.Count + ")");

            GUILayout.EndHorizontal();

            for (int i = 0; i < statModifiers.Count; i++)
            {
                if (DrawStatModifier(statModifiers[i]))
                {
                    removeAt = i;
                }
            }

            if (removeAt > -1)
            {
                GUI.FocusControl(null);
                statModifiers.RemoveAt(removeAt);
                PropertyObjectValue("statModifiers", statModifiers, true);
            }

            if (GUILayout.Button(new GUIContent("Add Modifier", GDTKEditor.GetIcon("icons/add-small")), GUILayout.Height(24)))
            {
                statModifiers.Add(new GDTKStatModifier());
                PropertyObjectValue("statModifiers", statModifiers, true);
            }
        }

        private void DrawStatValue(GDTKStatValue statValue)
        {
            ValueType valueType = (ValueType)EditorGUILayout.EnumPopup("Value Type", statValue.valueType);
            if (valueType != statValue.valueType)
            {
                FieldInfo fiValueType = valType.GetField("m_valueType", BindingFlags.Instance | BindingFlags.NonPublic);
                fiValueType.SetValue(statValue, valueType);
            }

            switch (statValue.valueType)
            {
                case ValueType.RandomRange:
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Value");

                    FieldInfo fiMin = valType.GetField("m_randomMin", BindingFlags.Instance | BindingFlags.NonPublic);
                    FieldInfo fiMax = valType.GetField("m_randomMax", BindingFlags.Instance | BindingFlags.NonPublic);
                    string val;

                    val = (string)fiMin.GetValue(statValue);
                    string rmin = EditorGUILayout.TextField(val);
                    if (rmin != val)
                    {
                        fiMin.SetValue(statValue, rmin.ToString());
                    }

                    val = (string)fiMax.GetValue(statValue);
                    string rmax = EditorGUILayout.TextField(val);
                    if (rmax != val)
                    {
                        fiMax.SetValue(statValue, rmax.ToString());
                    }

                    GUILayout.EndHorizontal();
                    break;
                case ValueType.Standard:
                    string value = EditorGUILayout.TextField("Value", statValue.valueExpression);
                    if (value != statValue.valueExpression)
                    {
                        FieldInfo fiValue = valType.GetField("m_value", BindingFlags.Instance | BindingFlags.NonPublic);
                        fiValue.SetValue(statValue, value);
                    }
                    break;
                case ValueType.Conditional:
                    GUILayout.Label("Conditional value not support inside plugin");
                    break;
            }
        }

        #endregion

    }
}
#endif
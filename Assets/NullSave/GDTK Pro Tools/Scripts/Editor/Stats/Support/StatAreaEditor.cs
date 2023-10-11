#if GDTK
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [CustomEditor(typeof(StatArea))]
    [CanEditMultipleObjects]
    public class StatAreaEditor : GDTKStatsEditor
    {

        #region Fields

        StatsDatabase db;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            db = FindObjectOfType<StatsDatabase>();
        }

        public override void OnInspectorGUI()
        {
            MainContainerBegin();

            SectionHeader("Editor");
            SimpleProperty("visualize");

            SectionHeader("Targeting");
            SimpleProperty("layerMask");
            SimpleProperty("detection");
            switch ((DetectionType)SimpleValue<int>("detection"))
            {
                case DetectionType.ColliderHit:
                    break;
                case DetectionType.CubeCast:
                    SimpleProperty("extends");
                    break;
                case DetectionType.RayCast:
                    SimpleProperty("range");
                    break;
                case DetectionType.SphereCast:
                    SimpleProperty("radius");
                    break;
                case DetectionType.SphereConeCast:
                    SimpleProperty("radius");
                    SimpleProperty("coneAngle", "Angle");
                    SimpleProperty("range");
                    break;
            }
            SimpleProperty("applyContinually");

            //SectionHeader("Actions");
            GUILayout.Space(12);
            SimpleEnumFlagsContext(serializedObject.FindProperty("actions"), typeof(InteractableActions));

            if (SimpleValue<int>("actions") > 0)
            {
                InteractableActions actions = (InteractableActions)SimpleValue<int>("actions");

                SectionHeader("Actions");
                if (actions.HasFlag(InteractableActions.SetBackground))
                {
                    DynamicDBField("backgroundId", BuildBackgrounds);
                }
                if (actions.HasFlag(InteractableActions.SetRace))
                {
                    DynamicDBField("raceId", BuildRaces);
                }
                if (actions.HasFlag(InteractableActions.AddClass))
                {
                    DynamicDBField("addClassId", BuildClasses);
                }
                if (actions.HasFlag(InteractableActions.RemoveClass))
                {
                    DynamicDBField("removeClassId", BuildClasses);
                }
                if (actions.HasFlag(InteractableActions.AddStatusCondition))
                {
                    DynamicDBField("addStatusConditionId", BuildStatusConditions);
                }
                if (actions.HasFlag(InteractableActions.RemoveStatusCondition))
                {
                    DynamicDBField("removeStatusConditionId", BuildStatusConditions);
                }
                if (actions.HasFlag(InteractableActions.AddStatEffect))
                {
                    DynamicDBField("addStatusEffectId", BuildStatEffects);
                }
                if (actions.HasFlag(InteractableActions.RemoveStatEffect))
                {
                    DynamicDBField("removeStatusEffectId", BuildStatEffects);
                }
                if (actions.HasFlag(InteractableActions.ActivateObject))
                {
                    SimpleProperty("activateTarget");
                }
                if (actions.HasFlag(InteractableActions.SpawnObject))
                {
                    SimpleProperty("prefab");
                    SimpleProperty("parent");
                }
                if (actions.HasFlag(InteractableActions.RaiseEvent))
                {
                    DynamicDBField("raiseEventId", BuildEvents);
                }
                if (actions.HasFlag(InteractableActions.AddStatModifiers))
                {
                    DrawStatModifierList(serializedObject.FindProperty("statModifiers"), false);
                }
            }

            MainContainerEnd();
        }

        #endregion

        #region Private Methods

        private void BuildBackgrounds(List<string> output)
        {
            output.AddRange(db.backgrounds.Select(x => x.info.id).ToList());
        }

        private void BuildClasses(List<string> output)
        {
            output.AddRange(db.classes.Select(x => x.info.id).ToList());
        }

        private void BuildEvents(List<string> output)
        {
            output.AddRange(db.events.Select(x => x.info.id).ToList());
        }

        private void BuildRaces(List<string> output)
        {
            output.AddRange(db.races.Select(x => x.info.id).ToList());
        }

        private void BuildStatEffects(List<string> output)
        {
            output.AddRange(db.effects.Select(x => x.info.id).ToList());
        }

        private void BuildStatusConditions(List<string> output)
        {
            output.AddRange(db.statusConditions.Select(x => x.info.id).ToList());
        }

        private void DynamicDBField(string propertyName, Action<List<string>> optionBuilder)
        {
            if (db == null)
            {
                SimpleProperty(propertyName);
            }
            else
            {
                SimpleStringSearchProperty(serializedObject.FindProperty(propertyName),
                     ObjectNames.NicifyVariableName(propertyName), optionBuilder);
            }
        }

        #endregion

    }
}
#endif
#if GDTK
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    public class StatsExportWindow : GDTKEditorWindow
    {

        #region Constants

        private const string TITLE = "Exporting Stats";

        #endregion

        #region Fields

        private BasicStats source;
        private Vector2 scrollPos;
        private Dictionary<int, Tuple<string, bool>> selections;
        private bool includeStats;
        
        private Dictionary<int, Tuple<string, bool>> conditions;
        private bool includeConditions;

        private Dictionary<int, Tuple<string, bool>> attributes;
        private bool includeAttributes;

        private bool includeLevel;
        private bool includeRace;
        private bool includeBackground;
        private bool includeClass;
        private bool includeRespawn;

        #endregion

        #region Unity Methods

        private void OnGUI()
        {
            GUILayout.Space(18);
            GUILayout.Label("Items to Export", EditorStyles.boldLabel);
            GUILayout.Space(18);

            bool res;
            bool statRes = includeStats;
            bool condRes = includeConditions;
            bool condAttr = includeAttributes;
            scrollPos = GUILayout.BeginScrollView(scrollPos, EditorStyles.helpBox, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            // Stats
            if (selections.Count > 0)
            {
                statRes = GUILayout.Toggle(includeStats, "Stats");
                GUILayout.Space(2);
                GUILayout.BeginHorizontal();
                GUILayout.Space(18);
                GUILayout.BeginVertical();
                for (int i = 0; i < selections.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    res = GUILayout.Toggle(selections[i].Item2, new GUIContent(" " + selections[i].Item1, GDTKEditor.GetIcon("icons/stats")), GUILayout.Height(12));
                    if (res != selections[i].Item2)
                    {
                        selections[i] = new Tuple<string, bool>(selections[i].Item1, res);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(2);
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }

            // Conditions
            if (conditions.Count > 0)
            {
                condRes = GUILayout.Toggle(includeConditions, "Conditions");
                    GUILayout.Space(2);
                GUILayout.BeginHorizontal();
                GUILayout.Space(18);
                GUILayout.BeginVertical();
                for (int i = 0; i < conditions.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    res = GUILayout.Toggle(conditions[i].Item2, new GUIContent(" " + conditions[i].Item1, GDTKEditor.GetIcon("icons/animate")), GUILayout.Height(12));
                    if (res != conditions[i].Item2)
                    {
                        conditions[i] = new Tuple<string, bool>(conditions[i].Item1, res);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(2);
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }

            // Attributes
            if (attributes.Count > 0)
            {
                condAttr = GUILayout.Toggle(includeAttributes, "Attributes");
                    GUILayout.Space(2);
                GUILayout.BeginHorizontal();
                GUILayout.Space(18);
                GUILayout.BeginVertical();
                for (int i = 0; i < attributes.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    res = GUILayout.Toggle(attributes[i].Item2, new GUIContent(" " + attributes[i].Item1, GDTKEditor.GetIcon("icons/tag")), GUILayout.Height(12));
                    if (res != attributes[i].Item2)
                    {
                        attributes[i] = new Tuple<string, bool>(attributes[i].Item1, res);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(2);
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }

            // Player Character
            if (source is PlayerCharacterStats)
            {
                includeRace = GUILayout.Toggle(includeRace, "Race Id");
                GUILayout.Space(2);
                includeBackground = GUILayout.Toggle(includeBackground, "Background Id");
                GUILayout.Space(2);
                includeClass = GUILayout.Toggle(includeClass, "Class Id");
                GUILayout.Space(2);
                includeLevel = GUILayout.Toggle(includeLevel, "Level Id");
                GUILayout.Space(2);
                includeRespawn = GUILayout.Toggle(includeRespawn, "Respawn Data");
                GUILayout.Space(2);
            }
            else if (source is NPCStats)
            {
                includeRespawn = GUILayout.Toggle(includeRespawn, "Respawn Data");
                GUILayout.Space(2);
            }

            GUILayout.EndScrollView();
            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            GUILayout.Space(8);
            if (GUILayout.Button("All", GUILayout.Width(50), GUILayout.Height(19)))
            {
                for (int i = 0; i < selections.Count; i++) selections[i] = new Tuple<string, bool>(selections[i].Item1, true);
                for (int i = 0; i < conditions.Count; i++) conditions[i] = new Tuple<string, bool>(conditions[i].Item1, true);
                for (int i = 0; i < attributes.Count; i++) attributes[i] = new Tuple<string, bool>(attributes[i].Item1, true);
                includeRace = true;
                includeBackground = true;
                includeClass = true;
                includeLevel = true;
                includeRespawn = true;
                statRes = true;
                includeStats = true;
                condRes = true;
                includeConditions = true;
                condAttr = true;
                includeAttributes = true;
            }
            GUILayout.Space(4);
            if (GUILayout.Button("None", GUILayout.Width(50), GUILayout.Height(19)))
            {
                for (int i = 0; i < selections.Count; i++) selections[i] = new Tuple<string, bool>(selections[i].Item1, false);
                for (int i = 0; i < conditions.Count; i++) conditions[i] = new Tuple<string, bool>(conditions[i].Item1, false);
                for (int i = 0; i < attributes.Count; i++) attributes[i] = new Tuple<string, bool>(attributes[i].Item1, false);
                includeRace = false;
                includeBackground = false;
                includeClass = false;
                includeLevel = false;
                includeRespawn = false;
                statRes = false;
                includeStats = false;
                condRes = false;
                includeConditions = false;
                condAttr = false;
                includeAttributes = false;
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Export...", GUILayout.Width(56), GUILayout.Height(19)))
            {
                string exportPath = EditorUtility.SaveFilePanel("Export Stats", Application.dataPath, "Stats", "json");
                if (!string.IsNullOrEmpty(exportPath))
                {
                    string json;

                    List<int> statIndexes = new List<int>();
                    for (int i = 0; i < selections.Count; i++) { if (selections[i].Item2) statIndexes.Add(i); }

                    List<int> conditionIndexes = new List<int>();
                    for (int i = 0; i < conditions.Count; i++) { if (conditions[i].Item2) conditionIndexes.Add(i); }

                    List<int> attribIndexes = new List<int>();
                    for (int i = 0; i < attributes.Count; i++) { if (attributes[i].Item2) attribIndexes.Add(i); }

                    if (source is PlayerCharacterStats pcs)
                    {
                        json = pcs.JSONExport(statIndexes, conditionIndexes, attribIndexes, includeRace, includeBackground, includeClass, includeLevel, includeRespawn);
                    }
                    else if (source is NPCStats npc)
                    {
                        json = npc.JSONExport(statIndexes, conditionIndexes, attribIndexes, includeRespawn);
                    }
                    else
                    {
                        json = source.JSONExport(statIndexes, conditionIndexes, attribIndexes);
                    }

                    File.WriteAllText(exportPath, json);
                    AssetDatabase.Refresh();
                    Close();
                }
            }
            GUILayout.Space(8);
            GUILayout.EndHorizontal();
            GUILayout.Space(8);
            GUILayout.BeginVertical();

            GUILayout.EndVertical();

            if(statRes != includeStats)
            {
                includeStats = statRes;
                for (int i = 0; i < selections.Count; i++)
                {
                    selections[i] = new Tuple<string, bool>(selections[i].Item1, includeStats);
                }
            }

            if (condRes != includeConditions)
            {
                includeConditions = condRes;
                for (int i = 0; i < conditions.Count; i++)
                {
                    conditions[i] = new Tuple<string, bool>(conditions[i].Item1, includeConditions);
                }
            }

            if (condAttr != includeAttributes)
            {
                includeAttributes = condAttr;
                for (int i = 0; i < attributes.Count; i++)
                {
                    attributes[i] = new Tuple<string, bool>(attributes[i].Item1, includeAttributes);
                }
            }

        }


        #endregion

        #region Public Methods

        public static void ShowWindow(BasicStats source)
        {
            StatsExportWindow w = GetWindow<StatsExportWindow>(TITLE);
            w.titleContent = new GUIContent(TITLE, GDTKEditor.GetIcon("Icons/stats-window-icon"));
            w.wantsMouseMove = true;
            w.source = source;
            w.minSize = new Vector2(686, 711);
            w.maxSize = new Vector2(686, 711);

            int index = 0;

            // Stats
            Dictionary<int, Tuple<string, bool>> selections = new Dictionary<int, Tuple<string, bool>>();
            foreach(var option in source.stats)
            {
                selections.Add(index++, new Tuple<string, bool>(option.Value.info.id, true));
            }
            w.selections = selections;
            w.includeStats = true;

            // Conditions
            index = 0;
            Dictionary<int, Tuple<string, bool>> conditions = new Dictionary<int, Tuple<string, bool>>();
            foreach (var option in source.startingConditionIds)
            {
                conditions.Add(index++, new Tuple<string, bool>(option, true));
            }
            w.conditions = conditions;
            w.includeConditions = true;

            // Attributes
            index = 0;
            Dictionary<int, Tuple<string, bool>> attributes = new Dictionary<int, Tuple<string, bool>>();
            foreach (var option in source.startingAttributeIds)
            {
                attributes.Add(index++, new Tuple<string, bool>(option, true));
            }
            w.attributes = attributes;
            w.includeAttributes = true;

            w.includeBackground = true;
            w.includeClass = true;
            w.includeLevel = true;
            w.includeRace = true;
            w.includeRespawn = true;

            w.Show();
        }

        #endregion

    }
}
#endif
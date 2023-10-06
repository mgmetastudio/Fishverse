#if GDTK
using NullSave.GDTK.JSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    public class StatsImportWindow : GDTKEditorWindow
    {

        #region Constants

        private const string TITLE = "Importing Stats";

        #endregion

        #region Enumerations

        private enum ItemState
        {
            Matches,
            New,
            Changed
        }

        #endregion

        #region Fields

        private BasicStats source;
        private jsonPlayerStatList data;
        private Vector2 scrollPos;

        private List<GDTKStat> statKeys;
        private Dictionary<GDTKStat, Tuple<ItemState, bool>> stats;
        private bool includeStats;
        private bool hasChangedStats;

        private List<string> condKeys;
        private Dictionary<string, Tuple<ItemState, bool>> conditions;
        private bool includeConditions;
        private bool hasChangedConditions;

        private List<string> attribKeys;
        private Dictionary<string, Tuple<ItemState, bool>> attributes;
        private bool includeAttributes;
        private bool hasChangedAttributes;

        private Tuple<ItemState, bool> level;
        private Tuple<ItemState, bool> race;
        private Tuple<ItemState, bool> background;
        private Tuple<ItemState, bool> playerClass;
        private Tuple<ItemState, bool> respawn;

        private bool showRespawn;
        private bool showLevel;
        private bool showRace;
        private bool showBackground;
        private bool showClass;

        #endregion

        #region Unity Methods

        private void OnGUI()
        {
            GUILayout.Space(18);
            GUILayout.Label("Items to Import", EditorStyles.boldLabel);
            GUILayout.Space(18);

            bool res;
            bool statRes = includeStats;
            bool condRes = includeConditions;
            bool attribRes = includeAttributes;

            scrollPos = GUILayout.BeginScrollView(scrollPos, EditorStyles.helpBox, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            // Stats
            if (stats.Count > 0)
            {
                if (hasChangedStats)
                {
                    statRes = GUILayout.Toggle(includeStats, "Stats");
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(16);
                    GUILayout.Label("Stats");
                    GUILayout.EndHorizontal();
                }
                GUILayout.Space(2);
                GUILayout.BeginHorizontal();
                GUILayout.Space(18);
                GUILayout.BeginVertical();

                for (int i = 0; i < stats.Count; i++)
                {
                    switch (stats[statKeys[i]].Item1)
                    {
                        case ItemState.New:
                        case ItemState.Changed:
                            GUILayout.BeginHorizontal();
                            res = GUILayout.Toggle(stats[statKeys[i]].Item2, new GUIContent(" " + statKeys[i].info.id, GDTKEditor.GetIcon("icons/stats")), GUILayout.Height(12));
                            GUILayout.FlexibleSpace();
                            GUILayout.Label(new GUIContent(GDTKEditor.GetIcon(stats[statKeys[i]].Item1 == ItemState.Changed ? "icons/changed-entry" : "icons/new-entry")));
                            if (res != stats[statKeys[i]].Item2)
                            {
                                stats[statKeys[i]] = new Tuple<ItemState, bool>(stats[statKeys[i]].Item1, res);
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.Space(2);
                            break;
                        case ItemState.Matches:
                            GUILayout.BeginHorizontal();
                            GUI.enabled = false;
                            GUILayout.Space(18);
                            GUILayout.Label(new GUIContent(statKeys[i].info.id, GDTKEditor.GetIcon("icons/stats")), GUILayout.Height(15));
                            GUI.enabled = true;
                            GUILayout.EndHorizontal();
                            GUILayout.Space(4);
                            break;
                    }
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }

            // Conditions
            if (conditions.Count > 0)
            {
                if (hasChangedConditions)
                {
                    condRes = GUILayout.Toggle(condRes, "Conditions");
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(16);
                    GUILayout.Label("Conditions");
                    GUILayout.EndHorizontal();
                }
                GUILayout.Space(2);
                GUILayout.BeginHorizontal();
                GUILayout.Space(18);
                GUILayout.BeginVertical();

                foreach (var conditionEntry in conditions)
                {
                    switch (conditionEntry.Value.Item1)
                    {
                        case ItemState.New:
                        case ItemState.Changed:
                            GUILayout.BeginHorizontal();
                            res = GUILayout.Toggle(conditionEntry.Value.Item2, new GUIContent(" " + conditionEntry.Key, GDTKEditor.GetIcon("icons/animate")), GUILayout.Height(12));
                            GUILayout.FlexibleSpace();
                            GUILayout.Label(new GUIContent(GDTKEditor.GetIcon(conditionEntry.Value.Item1 == ItemState.Changed ? "icons/changed-entry" : "icons/new-entry")));
                            if (res != conditionEntry.Value.Item2)
                            {
                                conditions[conditionEntry.Key] = new Tuple<ItemState, bool>(conditionEntry.Value.Item1, res);
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.Space(2);
                            break;
                        case ItemState.Matches:
                            GUILayout.BeginHorizontal();
                            GUI.enabled = false;
                            GUILayout.Space(18);
                            GUILayout.Label(new GUIContent(conditionEntry.Key, GDTKEditor.GetIcon("icons/animate")), GUILayout.Height(15));
                            GUI.enabled = true;
                            GUILayout.EndHorizontal();
                            GUILayout.Space(4);
                            break;
                    }
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }

            // Attributes
            if (attributes.Count > 0)
            {
                if (hasChangedAttributes)
                {
                    attribRes = GUILayout.Toggle(attribRes, "Attributes");
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(16);
                    GUILayout.Label("Attributes");
                    GUILayout.EndHorizontal();
                }
                GUILayout.Space(2);
                GUILayout.BeginHorizontal();
                GUILayout.Space(18);
                GUILayout.BeginVertical();

                foreach (var attributeEntry in attributes)
                {
                    switch (attributeEntry.Value.Item1)
                    {
                        case ItemState.New:
                        case ItemState.Changed:
                            GUILayout.BeginHorizontal();
                            res = GUILayout.Toggle(attributeEntry.Value.Item2, new GUIContent(" " + attributeEntry.Key, GDTKEditor.GetIcon("icons/tag")), GUILayout.Height(12));
                            GUILayout.FlexibleSpace();
                            GUILayout.Label(new GUIContent(GDTKEditor.GetIcon(attributeEntry.Value.Item1 == ItemState.Changed ? "icons/changed-entry" : "icons/new-entry")));
                            if (res != attributeEntry.Value.Item2)
                            {
                                attributes[attributeEntry.Key] = new Tuple<ItemState, bool>(attributeEntry.Value.Item1, res);
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.Space(2);
                            break;
                        case ItemState.Matches:
                            GUILayout.BeginHorizontal();
                            GUI.enabled = false;
                            GUILayout.Space(18);
                            GUILayout.Label(new GUIContent(attributeEntry.Key, GDTKEditor.GetIcon("icons/tag")), GUILayout.Height(15));
                            GUI.enabled = true;
                            GUILayout.EndHorizontal();
                            GUILayout.Space(4);
                            break;
                    }
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }

            if (showRace)
            {
                if (race.Item1 == ItemState.Matches)
                {
                    GUILayout.BeginHorizontal();
                    GUI.enabled = false;
                    GUILayout.Space(18);
                    GUILayout.Label("Race Id", GUILayout.Height(15));
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();
                    GUILayout.Space(4);
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    res = GUILayout.Toggle(race.Item2, "Race Id");
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(new GUIContent(GDTKEditor.GetIcon("icons/changed-entry")));
                    GUILayout.EndHorizontal();
                    GUILayout.Space(2);
                    if (res != race.Item2)
                    {
                        race = new Tuple<ItemState, bool>(race.Item1, res);
                    }
                }
            }

            if (showBackground)
            {
                if (background.Item1 == ItemState.Matches)
                {
                    GUILayout.BeginHorizontal();
                    GUI.enabled = false;
                    GUILayout.Space(18);
                    GUILayout.Label("Background Id", GUILayout.Height(15));
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();
                    GUILayout.Space(4);
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    res = GUILayout.Toggle(background.Item2, "Background Id");
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(new GUIContent(GDTKEditor.GetIcon("icons/changed-entry")));
                    GUILayout.EndHorizontal();
                    GUILayout.Space(2);
                    if (res != background.Item2)
                    {
                        background = new Tuple<ItemState, bool>(background.Item1, res);
                    }
                }
            }

            if (showClass)
            {
                if (playerClass.Item1 == ItemState.Matches)
                {
                    GUILayout.BeginHorizontal();
                    GUI.enabled = false;
                    GUILayout.Space(18);
                    GUILayout.Label("Class Id", GUILayout.Height(15));
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();
                    GUILayout.Space(4);
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    res = GUILayout.Toggle(playerClass.Item2, "Class Id");
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(new GUIContent(GDTKEditor.GetIcon("icons/changed-entry")));
                    GUILayout.EndHorizontal();
                    GUILayout.Space(2);
                    if (res != playerClass.Item2)
                    {
                        playerClass = new Tuple<ItemState, bool>(playerClass.Item1, res);
                    }
                }
            }

            if (showLevel)
            {
                if (level.Item1 == ItemState.Matches)
                {
                    GUILayout.BeginHorizontal();
                    GUI.enabled = false;
                    GUILayout.Space(18);
                    GUILayout.Label("Level Id", GUILayout.Height(15));
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();
                    GUILayout.Space(4);
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    res = GUILayout.Toggle(level.Item2, "Level Id");
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(new GUIContent(GDTKEditor.GetIcon("icons/changed-entry")));
                    GUILayout.EndHorizontal();
                    GUILayout.Space(2);
                    if (res != level.Item2)
                    {
                        level = new Tuple<ItemState, bool>(level.Item1, res);
                    }
                }
            }

            if (showRespawn)
            {
                if (respawn.Item1 == ItemState.Matches)
                {
                    GUILayout.BeginHorizontal();
                    GUI.enabled = false;
                    GUILayout.Space(18);
                    GUILayout.Label("Respawn Data", GUILayout.Height(15));
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();
                    GUILayout.Space(4);
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    res = GUILayout.Toggle(respawn.Item2, "Respawn Data");
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(new GUIContent(GDTKEditor.GetIcon("icons/changed-entry")));
                    GUILayout.EndHorizontal();
                    GUILayout.Space(2);
                    if (res != respawn.Item2)
                    {
                        respawn = new Tuple<ItemState, bool>(respawn.Item1, res);
                    }
                }
            }

            GUILayout.EndScrollView();
            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            GUILayout.Space(8);
            if (GUILayout.Button("All", GUILayout.Width(50), GUILayout.Height(19)))
            {
                if (showRace) race = new Tuple<ItemState, bool>(race.Item1, true);
                if (showBackground) background = new Tuple<ItemState, bool>(background.Item1, true);
                if (showClass) playerClass = new Tuple<ItemState, bool>(playerClass.Item1, true);
                if (showLevel) level = new Tuple<ItemState, bool>(level.Item1, true);
                if (showRespawn) respawn = new Tuple<ItemState, bool>(respawn.Item1, true);
                statRes = true;
                includeStats = true;
                condRes = true;
                includeConditions = true;
                attribRes = true;
                includeAttributes = true;
                for (int i = 0; i < stats.Count; i++) stats[statKeys[i]] = new Tuple<ItemState, bool>(stats[statKeys[i]].Item1, includeStats);
                for (int i = 0; i < condKeys.Count; i++) conditions[condKeys[i]] = new Tuple<ItemState, bool>(conditions[condKeys[i]].Item1, includeConditions);
                for (int i = 0; i < attribKeys.Count; i++) attributes[attribKeys[i]] = new Tuple<ItemState, bool>(attributes[attribKeys[i]].Item1, includeAttributes);
            }
            GUILayout.Space(4);
            if (GUILayout.Button("None", GUILayout.Width(50), GUILayout.Height(19)))
            {
                if (showRace) race = new Tuple<ItemState, bool>(race.Item1, false);
                if (showBackground) background = new Tuple<ItemState, bool>(background.Item1, false);
                if (showClass) playerClass = new Tuple<ItemState, bool>(playerClass.Item1, false);
                if (showLevel) level = new Tuple<ItemState, bool>(level.Item1, false);
                if (showRespawn) respawn = new Tuple<ItemState, bool>(respawn.Item1, false);
                statRes = false;
                includeStats = false;
                condRes = false;
                includeConditions = false;
                attribRes = false;
                includeAttributes = false;

                for (int i = 0; i < statKeys.Count; i++) stats[statKeys[i]] = new Tuple<ItemState, bool>(stats[statKeys[i]].Item1, includeStats);
                for (int i = 0; i < condKeys.Count; i++) conditions[condKeys[i]] = new Tuple<ItemState, bool>(conditions[condKeys[i]].Item1, includeConditions);
                for (int i = 0; i < attribKeys.Count; i++) attributes[attribKeys[i]] = new Tuple<ItemState, bool>(attributes[attribKeys[i]].Item1, includeAttributes);
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Import", GUILayout.Width(56), GUILayout.Height(19)))
            {
                jsonPlayerStatList importList = new jsonPlayerStatList();
                Tuple<ItemState, bool> check;

                // Attributes
                foreach (var value in data.attributes)
                {
                    check = attributes[value];
                    if (check.Item1 != ItemState.Matches && check.Item2)
                    {
                        importList.attributes.Add(value);
                    }
                }

                // Conditions
                foreach (var value in data.conditions)
                {
                    check = conditions[value];
                    if (check.Item1 != ItemState.Matches && check.Item2)
                    {
                        importList.attributes.Add(value);
                    }
                }

                // Stats
                foreach (var value in data.stats)
                {
                    check = stats[value];
                    if (check.Item1 != ItemState.Matches && check.Item2)
                    {
                        importList.stats.Add(value);
                    }
                }

                importList.raceId = (showRace && race.Item2) ? data.raceId : "!!IGNORE!!";
                importList.backgroundId = (showBackground && background.Item2) ? data.backgroundId : "!!IGNORE!!";
                importList.classId = (showClass && playerClass.Item2) ? data.classId : "!!IGNORE!!";
                importList.levelId = (showLevel && level.Item2) ? data.levelId : "!!IGNORE!!";
                if (showRespawn && respawn.Item2)
                {
                    importList.respawnData = true;
                    importList.respawnCondition = data.respawnCondition;
                    importList.saveFilename = data.saveFilename;
                    importList.savePosition = data.savePosition;
                    importList.saveRotation = data.saveRotation;
                    importList.respawnModifiers = data.respawnModifiers;
                }
                else
                {
                    importList.respawnData = false;
                }

                if (source is PlayerCharacterStats pcs)
                {
                    pcs.JSONImportSmart(importList);
                }
                else if (source is NPCStats npc)
                {
                    npc.JSONImportSmart(importList);
                }
                else
                {
                    source.JSONImportSmart(importList);
                }
                Close();
            }
            GUILayout.Space(8);
            GUILayout.EndHorizontal();
            GUILayout.Space(8);
            GUILayout.BeginVertical();

            GUILayout.EndVertical();

            if (statRes != includeStats)
            {
                includeStats = statRes;
                for (int i = 0; i < statKeys.Count; i++) stats[statKeys[i]] = new Tuple<ItemState, bool>(stats[statKeys[i]].Item1, includeStats);
            }

            if (condRes != includeConditions)
            {
                includeConditions = condRes;
                for (int i = 0; i < condKeys.Count; i++) conditions[condKeys[i]] = new Tuple<ItemState, bool>(conditions[condKeys[i]].Item1, includeConditions);
            }

            if (attribRes != includeAttributes)
            {
                includeAttributes = attribRes;
                for (int i = 0; i < attribKeys.Count; i++) attributes[attribKeys[i]] = new Tuple<ItemState, bool>(attributes[attribKeys[i]].Item1, includeAttributes);
            }
        }


        #endregion

        #region Public Methods

        public static void ShowWindow(BasicStats source, string filename)
        {
            StatsImportWindow w = GetWindow<StatsImportWindow>(TITLE);
            w.titleContent = new GUIContent(TITLE, GDTKEditor.GetIcon("Icons/stats-window-icon"));
            w.wantsMouseMove = true;
            w.source = source;
            w.minSize = new Vector2(686, 711);
            w.maxSize = new Vector2(686, 711);

            w.data = SimpleJson.FromJSON<jsonPlayerStatList>(File.ReadAllText(filename));

            ItemState state;

            // Stats
            Dictionary<GDTKStat, Tuple<ItemState, bool>> stats = new Dictionary<GDTKStat, Tuple<ItemState, bool>>();
            w.statKeys = new List<GDTKStat>();
            foreach (GDTKStat stat in w.data.stats)
            {
                if (stats.ContainsKey(stat))
                {
                    Debug.LogWarning("Duplicate found: " + stat.info.id + " (Skipped)");
                }
                else
                {
                    w.statKeys.Add(stat);
                    state = GetStatState(stat, source);
                    stats.Add(stat, new Tuple<ItemState, bool>(state, true));
                    if (state != ItemState.Matches) w.hasChangedStats = true;
                }
            }
            w.stats = stats;
            w.includeStats = true;

            // Conditions
            w.condKeys = new List<string>();
            Dictionary<string, Tuple<ItemState, bool>> conditions = new Dictionary<string, Tuple<ItemState, bool>>();
            foreach (string cond in w.data.conditions)
            {
                if (conditions.ContainsKey(cond))
                {
                    Debug.LogWarning("Duplicate found: " + cond + " (Skipped)");
                }
                else
                {
                    w.condKeys.Add(cond);
                    state = source.startingConditionIds.Contains(cond) ? ItemState.Matches : ItemState.New;
                    conditions.Add(cond, new Tuple<ItemState, bool>(state, true));
                    if (state != ItemState.Matches) w.hasChangedConditions = true;
                }
            }
            w.conditions = conditions;
            w.includeConditions = true;

            // Attributes
            w.attribKeys = new List<string>();
            Dictionary<string, Tuple<ItemState, bool>> attributes = new Dictionary<string, Tuple<ItemState, bool>>();
            foreach (string cond in w.data.attributes)
            {
                if (attributes.ContainsKey(cond))
                {
                    Debug.LogWarning("Duplicate found: " + cond + " (Skipped)");
                }
                else
                {
                    w.attribKeys.Add(cond);
                    state = source.startingAttributeIds.Contains(cond) ? ItemState.Matches : ItemState.New;
                    attributes.Add(cond, new Tuple<ItemState, bool>(state, true));
                    if (state != ItemState.Matches) w.hasChangedAttributes = true;
                }
            }
            w.attributes = attributes;
            w.includeAttributes = true;

            if (source is PlayerCharacterStats pcs)
            {
                w.showRespawn = w.data.respawnData;
                w.respawn = new Tuple<ItemState, bool>(GetRespawnState(w.data, pcs), true);

                if (w.data.levelId != "!!IGNORE!!")
                {
                    w.showLevel = true;
                    w.level = new Tuple<ItemState, bool>(w.data.levelId == pcs.levelId ? ItemState.Matches : ItemState.Changed, true);
                }

                if (w.data.raceId != "!!IGNORE!!")
                {
                    w.showRace = true;
                    w.race = new Tuple<ItemState, bool>(w.data.raceId == pcs.startingRaceId ? ItemState.Matches : ItemState.Changed, true);
                }

                if (w.data.backgroundId != "!!IGNORE!!")
                {
                    w.showBackground = true;
                    w.background = new Tuple<ItemState, bool>(w.data.backgroundId == pcs.startingBackgroundId ? ItemState.Matches : ItemState.Changed, true);
                }

                if (w.data.classId != "!!IGNORE!!")
                {
                    w.showClass = true;
                    w.playerClass = new Tuple<ItemState, bool>(w.data.classId == pcs.startingClassId ? ItemState.Matches : ItemState.Changed, true);
                }
            }
            else if (source is NPCStats npc)
            {
                w.showRespawn = w.data.respawnData;
                w.respawn = new Tuple<ItemState, bool>(GetRespawnState(w.data, npc), true);
            }
            else
            {
                w.showRespawn = false;
            }

            w.Show();
        }

        #endregion

        #region Private Methods

        private static ItemState GetRespawnState(jsonPlayerStatList data, NPCStats source)
        {
            if ((data.respawnModifiers == null && source.respawnModifiers != null) ||
                (data.respawnModifiers != null && source.respawnModifiers == null) ||
                data.respawnModifiers.Count != source.respawnModifiers.Count) return ItemState.Changed;
            for (int i = 0; i < data.respawnModifiers.Count; i++)
            {
                if (!data.respawnModifiers[i].Matches(source.respawnModifiers[i])) return ItemState.Changed;
            }

            return (data.saveFilename == source.saveFilename &&
                data.savePosition == source.savePosition &&
                data.saveRotation == source.saveRotation &&
                data.respawnCondition == source.respawnCondition) ? ItemState.Matches : ItemState.Changed;
        }

        private static ItemState GetStatState(GDTKStat stat, BasicStats source)
        {
            GDTKStat existingStat = GetStat(stat.info.id, source);
            if (existingStat == null) return ItemState.New;

            if (!stat.Matches(existingStat)) return ItemState.Changed;
            return ItemState.Matches;
        }

        private static GDTKStat GetStat(string id, BasicStats source)
        {
            foreach (var stat in source.stats)
            {
                if (stat.Value.info.id == id)
                {
                    return stat.Value;
                }
            }

            return null;
        }

        #endregion

    }
}
#endif
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(StatsCog))]
    public class StatsCogEditor : TOCKEditorV2
    {

        #region Enumerations

        private enum DisplayFlags
        {
            None = 0,
            Stats = 1,
            Effects = 2,
            Combat = 4,
            Events = 8,
            Debug = 16,
            DamageDealers = 32,
            DamageReceivers = 64,
            Evaluate = 128,
            Traits = 256,
            Attributes = 512,
            GlobalStats = 1024,
        }

        #endregion

        #region Variables

        private StatsCog myTarget;
        private bool showStatsDebug = true;
        private bool showEffectsDebug = true;
        private int selStat;
        private string command = string.Empty;
        private Vector2 scroll;
        private DisplayFlags displayFlags;
        private Vector2 statSP, startingSP, resistSP;
        private string[] statOptions;
        private List<int> expandedStats;

        private int boneIndex1, boneIndex2;
        private readonly string[] directions = new string[] { "FrontLeft", "FrontCenter", "FrontRight", "Left", "Right", "BackLeft", "BackCenter", "BackRight" };
        private string lastResult;

        private Dictionary<SerializedProperty, StatValueEditor> statEditors;
        private StatEffectListEditor effectListEditor;

        private List<DamageType> dmgTypes;
        private Dictionary<DamageType, DamageTypeEditor> dmgEditors;
        private List<DamageType> expandedDmgs;

        private Dictionary<Trait, TraitEditor> traitEditors;
        private List<Trait> expandedTraits;

        private List<DamageModifier> dmgMods;
        private Dictionary<DamageModifier, DamageModifierEditor> dmgModEditors;
        private List<int> expandedDmgMods;

        private Color ok = new Color(0.427f, 0.722f, 0.4f);
        private Color fail = new Color(0.72f, 0.4f, 0.4f);

        private string lastDmgDir;

        private List<int> debugStats;
        private string debugCommand;

        // Stat Drag
        private bool dragStat;
        private Vector2 startPos;
        private int startIndex;
        private int curIndex;

        private GUIStyle wrapTextStyle;

        #endregion

        #region Properties

        private GUIStyle WrapTextStyle
        {
            get
            {
                if (wrapTextStyle == null)
                {
                    wrapTextStyle = new GUIStyle(GUI.skin.label);
                    wrapTextStyle.wordWrap = true;
                }

                return wrapTextStyle;
            }
        }

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            if (target is StatsCog)
            {
                myTarget = (StatsCog)target;
            }
            expandedStats = new List<int>();
            statEditors = new Dictionary<SerializedProperty, StatValueEditor>();
            dmgTypes = new List<DamageType>();
            dmgEditors = new Dictionary<DamageType, DamageTypeEditor>();
            expandedDmgs = new List<DamageType>();
            dmgMods = new List<DamageModifier>();
            dmgModEditors = new Dictionary<DamageModifier, DamageModifierEditor>();
            expandedDmgMods = new List<int>();
            traitEditors = new Dictionary<Trait, TraitEditor>();
            expandedTraits = new List<Trait>();
            debugStats = new List<int>();
            UpdateDamageTypesList();
        }

        public override bool RequiresConstantRepaint()
        {
            return dragStat || Application.isPlaying;
        }

        public override void OnInspectorGUI()
        {
            bool addPrim = false;
            bool addSec = false;
            Color resContent = GUI.contentColor;
            displayFlags = (DisplayFlags)serializedObject.FindProperty("z_display_flags").intValue;
            MainContainerBeginSlim();

            if (SectionToggle((int)displayFlags, (int)DisplayFlags.GlobalStats, "Global Stats", GetIcon("D6", "Icons/d6")))
            {
                GUILayout.BeginVertical(SubSectionBox);
                GUILayout.Label("One instance of Stats Cog can be marked as 'Global' in a scene. This will let all other instances reference stats here using the 'global:' prefix. Global Stats needs to be the 1st instance in your scene.\r\n(example: global:difficulty)", WrapTextStyle);
                GUILayout.EndVertical();
                SimpleProperty("markGlobal");
                if (SimpleBool("markGlobal"))
                {
                    SimpleProperty("doNotDestroy");
                }
            }

            bool buttonPressed = false;
            if (SectionDropToggleWithButton((int)displayFlags, (int)DisplayFlags.Stats, " New ", out buttonPressed, "Stats", GetIcon("Stats", "Icons/tock-stats"), "stats", typeof(StatValue)))
            {
                StatEditorHelper.DrawStatsList(serializedObject.FindProperty("stats"), Application.isPlaying ? myTarget.Stats : myTarget.stats, debugStats, expandedStats, statEditors, myTarget, ref dragStat, ref curIndex, ref startIndex, ref startPos, ref debugCommand, this);

                // Check new drag index
                if (dragStat)
                {
                    if (Event.current.type == EventType.MouseDrag)
                    {
                        curIndex = startIndex + (int)((Event.current.mousePosition.y - startPos.y) / 20);
                        if (curIndex < 0) curIndex = 0;
                        if (curIndex >= myTarget.stats.Count) curIndex = myTarget.stats.Count - 1;
                    }
                    else if (Event.current.type == EventType.MouseUp)
                    {
                        dragStat = false;
                        if (startIndex != curIndex)
                        {
                            StatValue stat = myTarget.stats[startIndex];
                            myTarget.stats.RemoveAt(startIndex);
                            myTarget.stats.Insert(curIndex, stat);
                        }
                        Repaint();
                    }
                }

                // Create New Stat
                if (buttonPressed)
                {
                    ScriptableObject newItem = CreateNew("Stat Value", typeof(StatValue));
                    if (newItem != null)
                    {
                        myTarget.stats.Add((StatValue)newItem);
                    }

                    buttonPressed = false;
                }

                if (GUILayout.Button("Clear"))
                {
                    myTarget.stats.Clear();
                }
            }

            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Traits, "Traits", GetIcon("Trait", "Icons/tock-animate")))
            {
                if (myTarget.primaryTrait == null)
                {
                    GUILayout.BeginHorizontal();
                    SimpleProperty("primaryTrait", "Primary");

                    GUI.contentColor = EditorColor;
                    GUILayout.Label(new GUIContent(GetIcon("Add", "Icons/tock-add"), "Create New"), GUILayout.Width(16), GUILayout.Height(16));
                    GUI.contentColor = resContent;
                    if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    {
                        addPrim = true;
                    }

                    GUILayout.EndHorizontal();
                }
                else
                {
                    if (DrawTrait("Primary Trait:", myTarget.primaryTrait)) myTarget.primaryTrait = null;
                }

                if (myTarget.secondaryTrait == null)
                {
                    GUILayout.BeginHorizontal();
                    SimpleProperty("secondaryTrait", "Secondary");

                    GUI.contentColor = EditorColor;
                    GUILayout.Label(new GUIContent(GetIcon("Add", "Icons/tock-add"), "Create New"), GUILayout.Width(16), GUILayout.Height(16));
                    GUI.contentColor = resContent;
                    if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    {
                        addSec = true;
                    }

                    GUILayout.EndHorizontal();
                }
                else
                {
                    if (DrawTrait("Secondary Trait:", myTarget.secondaryTrait)) myTarget.secondaryTrait = null;
                }

                if (addPrim || addSec)
                {
                    ScriptableObject newItem = CreateNew("Trait", typeof(Trait));
                    if (newItem != null)
                    {
                        if(addPrim)
                        {
                            myTarget.primaryTrait = (Trait)newItem;
                        }
                        else
                        {
                            myTarget.secondaryTrait = (Trait)newItem;
                        }
                    }

                }

                SimpleProperty("lockTraits", "Lock After Set");
                DrawTraits();
            }

            // Create New Trait
            if (buttonPressed)
            {
                ScriptableObject newItem = CreateNew("Trait", typeof(Trait));
                if (newItem != null)
                {
                    myTarget.additionalTraits.Add((Trait)newItem);
                }
            }

            // Attributes
            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Attributes, "Attributes", GetIcon("Tag", "Icons/tock-tag")))
            {
                SimpleList("attributes", true);
            }

            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Effects, "Effects", GetIcon("Event", "Icons/tock-event")))
            {
                if (DrawEffects())
                {
                    MainContainerEnd();
                    return;
                }
            }

            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Combat, "Combat", GetIcon("Combat", "Icons/statscog-combat")))
            {
                DrawCombat();
            }

            if (!SimpleBool("externalCombat"))
            {
                if (SectionToggle((int)displayFlags, (int)DisplayFlags.DamageDealers, "Damage Dealers", GetIcon("Weapon", "Icons/weapon")))
                {
                    DamageDealer[] dealers = myTarget.GetComponentsInChildren<DamageDealer>();
                    foreach (DamageDealer dealer in dealers)
                    {
                        EditorGUILayout.ObjectField(dealer, typeof(DamageDealer), true);
                    }
                    if (dealers.Length == 0)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(24);
                        GUILayout.Label("{None}", Skin.GetStyle("SubHeader"));
                        GUILayout.EndHorizontal();
                    }

                    Animator anim = myTarget.GetComponentInChildren<Animator>();
                    if (anim != null && anim.isHuman)
                    {
                        SubHeader("Dynamic Creation");
                        boneIndex1 = EditorGUILayout.Popup("Add to Bone", boneIndex1, bones, GUILayout.ExpandWidth(true));
                        if (GUILayout.Button("Add"))
                        {
                            Transform target = anim.GetBoneTransform((HumanBodyBones)boneIndex1);
                            if (target == null)
                            {
                                EditorUtility.DisplayDialog("Stats Cog", "The requested bone '" + bones[boneIndex1] + "' could not be found on the selected rig.", "OK");
                            }
                            else
                            {
                                GameObject newDD = new GameObject();
                                newDD.name = "DamageDealer_" + bones[boneIndex1];
                                newDD.AddComponent<SphereCollider>().isTrigger = true;
                                newDD.AddComponent<DamageDealer>();
                                newDD.transform.SetParent(target);
                                newDD.transform.localPosition = Vector3.zero;
                                Selection.activeGameObject = newDD;
                            }
                        }
                    }
                }

                if (SectionToggle((int)displayFlags, (int)DisplayFlags.DamageReceivers, "Damage Receivers", GetIcon("DmgReceiver", "Icons/damage")))
                {
                    DamageReceiver[] receivers = myTarget.GetComponentsInChildren<DamageReceiver>();
                    foreach (DamageReceiver Receiver in receivers)
                    {
                        EditorGUILayout.ObjectField(Receiver, typeof(DamageReceiver), true);
                    }
                    if (receivers.Length == 0)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(24);
                        GUILayout.Label("{None}", Skin.GetStyle("SubHeader"));
                        GUILayout.EndHorizontal();
                    }

                    Animator anim = myTarget.GetComponentInChildren<Animator>();
                    if (anim != null && anim.isHuman)
                    {
                        SubHeader("Dynamic Creation");
                        boneIndex2 = EditorGUILayout.Popup("Add to Bone", boneIndex2, bones, GUILayout.ExpandWidth(true));
                        if (GUILayout.Button("Add"))
                        {
                            Transform target = anim.GetBoneTransform((HumanBodyBones)boneIndex2);
                            if (target == null)
                            {
                                EditorUtility.DisplayDialog("Stats Cog", "The requested bone '" + bones[boneIndex2] + "' could not be found on the selected rig.", "OK");
                            }
                            else
                            {
                                GameObject newDD = new GameObject();
                                newDD.name = "DamageReceiver_" + bones[boneIndex2];
                                newDD.AddComponent<CapsuleCollider>().isTrigger = true;
                                newDD.AddComponent<DamageReceiver>();
                                newDD.transform.SetParent(target);
                                newDD.transform.localPosition = Vector3.zero;
                                Selection.activeGameObject = newDD;
                            }
                        }
                    }
                }
            }

            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Events, "Events", GetIcon("Event", "Icons/tock-event")))
            {
                DrawEvents();
            }

            // Evaluator
            if (SectionToggle((int)displayFlags, (int)DisplayFlags.Evaluate, "Evaluator", GetIcon("Prompt", "Icons/prompt-small")))
            {
                SimpleProperty("debugFormula", "Formula");

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Evaluate"))
                {
                    try
                    {
                        lastResult = SimpleString("debugFormula") + " = " + myTarget.GetExpressionValue(SimpleString("debugFormula"));
                    }
                    catch
                    {
                        lastResult = "FAILED: " + SimpleString("debugFormula");
                    }
                }

                if(Application.isPlaying && GUILayout.Button("Send As Command"))
                {
                    myTarget.SendCommand(SimpleString("debugFormula"));
                }
                GUILayout.EndHorizontal();

                if (!string.IsNullOrEmpty(lastResult))
                {

                    GUILayout.Space(8);
                    GUILayout.Label(lastResult);
                }

            }


            MainContainerEnd();
        }

        #endregion

        #region Private Methods

        internal void DrawCombat()
        {
            SectionHeader("External Combat Manager");
            SimpleProperty("externalCombat", "Enable");
            if (SimpleBool("externalCombat")) return;

            int curOption = -1;
            if (statOptions == null || statOptions.Length != myTarget.stats.Count)
            {
                if (myTarget.stats != null)
                {
                    statOptions = new string[myTarget.stats.Count];

                    for (int i = 0; i < statOptions.Length; i++)
                    {
                        statOptions[i] = myTarget.stats[i].name;
                    }
                }
            }

            if (statOptions != null)
            {
                for (int i = 0; i < statOptions.Length; i++)
                {
                    if (statOptions[i] == myTarget.healthStat)
                    {
                        curOption = i;
                    }
                }
            }

            SectionHeader("Health");
            if (statOptions == null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(5);
                GUILayout.Label("Add stats to select health", Skin.GetStyle("SubHeader"));
                GUILayout.EndHorizontal();
            }
            else
            {
                curOption = EditorGUILayout.Popup("Health Stat", curOption, statOptions);
                if (curOption != -1)
                {
                    serializedObject.FindProperty("healthStat").stringValue = statOptions[curOption];
                }
            }
            SimpleProperty("damageValue");
            SimpleProperty("hitDirTolerance");
            SimpleProperty("ignoreLayer");

            int immune = SimpleInt("directionImmunity");
            serializedObject.FindProperty("directionImmunity").intValue = EditorGUILayout.MaskField("Direction Immunity", immune, directions);

            SimpleProperty("immunityAfterHit");

            SimpleProperty("destroyOnDeath");
            SimpleProperty("spawnOnDeath");


            bool buttonPressed;
            SectionHeaderWithButton("Damage Types", "New", out buttonPressed);
            if (buttonPressed)
            {
                if (string.IsNullOrWhiteSpace(lastDmgDir))
                {
                    lastDmgDir = Application.dataPath;
                }

                string path = EditorUtility.SaveFilePanelInProject("Create New Damage Type", "New Damage Type", "asset", "Select a location to create damage type.", lastDmgDir);
                if (path.Length != 0)
                {
                    lastDmgDir = Path.GetDirectoryName(path) + "/";
                    DamageType addItem = (DamageType)ScriptableObject.CreateInstance(typeof(DamageType));
                    addItem.name = Path.GetFileNameWithoutExtension(path);
                    AssetDatabase.CreateAsset(addItem, path);
                    AssetDatabase.Refresh();
                    UpdateDamageTypesList();
                }
            }

            foreach (DamageType dmgType in dmgTypes)
            {
                DrawDamageType(dmgType);
            }

            SectionHeaderWithButton("Damage Modifiers", "New", out buttonPressed, "damageModifiers", typeof(DamageModifier));
            if (buttonPressed)
            {
                if (string.IsNullOrWhiteSpace(lastDmgDir))
                {
                    lastDmgDir = Application.dataPath;
                }

                string path = EditorUtility.SaveFilePanelInProject("Create New Damage Modifier", "New Damage Modifier", "asset", "Select a location to create damage modifier.", lastDmgDir);
                if (path.Length != 0)
                {
                    lastDmgDir = Path.GetDirectoryName(path) + "/";
                    DamageModifier addItem = (DamageModifier)ScriptableObject.CreateInstance(typeof(DamageModifier));
                    addItem.name = Path.GetFileNameWithoutExtension(path);
                    AssetDatabase.CreateAsset(addItem, path);
                    AssetDatabase.Refresh();
                    myTarget.damageModifiers.Add(addItem);
                }
            }

            SerializedProperty list = serializedObject.FindProperty("damageModifiers");
            for (int i = 0; i < list.arraySize; i++)
            {
                if (DrawDamageModifier(list.GetArrayElementAtIndex(i), i, list.arraySize))
                {

                }
            }
        }

        private bool DrawDamageModifier(SerializedProperty dmgMod, int index, int length)
        {
            DamageModifier reference = myTarget.damageModifiers[index];
            Rect clickRect;

            GUILayout.Space(-4);

            GUILayout.BeginVertical(BoxBlue, GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();

            GUILayout.Label(reference.name, Skin.GetStyle("PanelText"));
            GUILayout.FlexibleSpace();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(reference, typeof(DamageModifier), false, GUILayout.MaxWidth(64));
            EditorGUI.EndDisabledGroup();

            bool delAtEnd;
            FoldoutTrashOnly(out delAtEnd, false);

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.EndVertical();
            clickRect = GUILayoutUtility.GetLastRect();

            if (expandedDmgMods.Contains(index))
            {
                GUILayout.Space(-6);
                GUILayout.BeginVertical(BoxWhite, GUILayout.ExpandWidth(true));

                if (!dmgModEditors.ContainsKey(reference))
                {
                    dmgModEditors.Add(reference, (DamageModifierEditor)Editor.CreateEditor(reference, typeof(DamageModifierEditor)));
                }
                dmgModEditors[reference].serializedObject.Update();
                dmgModEditors[reference].DrawInspector();
                dmgModEditors[reference].serializedObject.ApplyModifiedProperties();

                GUILayout.Space(4);
                GUILayout.EndVertical();
            }

            if (!delAtEnd && Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                if (expandedDmgMods.Contains(index))
                {
                    expandedDmgMods.Remove(index);
                }
                else
                {
                    expandedDmgMods.Add(index);
                }

                Repaint();
            }

            if (delAtEnd)
            {
                myTarget.damageModifiers.RemoveAt(index);
                return true;
            }

            return false;
        }

        private void DrawDamageType(DamageType dmgType)
        {
            Rect clickRect;
            GUILayout.BeginVertical(BoxBlue, GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();

            GUILayout.Label(dmgType.name, Skin.GetStyle("PanelText"));
            GUILayout.FlexibleSpace();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(dmgType, typeof(DamageType), false, GUILayout.MaxWidth(64));
            EditorGUI.EndDisabledGroup();

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.EndVertical();
            clickRect = GUILayoutUtility.GetLastRect();

            if (expandedDmgs.Contains(dmgType))
            {
                GUILayout.Space(-6);
                GUILayout.BeginVertical(BoxWhite, GUILayout.ExpandWidth(true));

                if (!dmgEditors.ContainsKey(dmgType))
                {
                    dmgEditors.Add(dmgType, (DamageTypeEditor)Editor.CreateEditor(dmgType, typeof(DamageTypeEditor)));
                }

                dmgEditors[dmgType].serializedObject.Update();
                dmgEditors[dmgType].DrawInspector();
                dmgEditors[dmgType].serializedObject.ApplyModifiedProperties();

                GUILayout.Space(4);
                GUILayout.EndVertical();
            }

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                if (expandedDmgs.Contains(dmgType))
                {
                    expandedDmgs.Remove(dmgType);
                }
                else
                {
                    expandedDmgs.Add(dmgType);
                }

                Repaint();
            }
        }

        private void DrawDebug()
        {
            bool tmp = showStatsDebug;
            showStatsDebug = SectionGroup("Stat Values", showStatsDebug);
            if (tmp)
            {
                if (myTarget.stats == null || myTarget.stats.Count == 0)
                {
                    EditorGUILayout.LabelField("No StatValues available to be monitored.", Skin.GetStyle("WrapText"));
                }
                else
                {
                    string[] options = new string[myTarget.Stats.Count];
                    for (int i = 0; i < options.Length; i++) options[i] = myTarget.stats[i].name;
                    if (selStat > options.Length - 1) selStat = options.Length - 1;
                    selStat = EditorGUILayout.Popup("Stat Value", selStat, options);

                    SectionHeader("Current Values");
                    if (myTarget.Stats != null && myTarget.Stats.Count > 0)
                    {
                        StatValue stat = myTarget.Stats[selStat];
                        EditorGUILayout.LabelField("Value", stat.CurrentValue.ToString());
                        EditorGUILayout.LabelField("Min", stat.CurrentMinimum.ToString());
                        EditorGUILayout.LabelField("Max", stat.CurrentMaximum.ToString());
                        if (stat.enableRegen)
                        {
                            EditorGUILayout.LabelField("Regen", stat.CurrentRegenAmount.ToString());
                            EditorGUILayout.LabelField("Regen Delay", stat.CurrentRegenDelay.ToString());
                        }

                        SectionHeader("Base Values");
                        EditorGUILayout.LabelField("Value", stat.CurrentBaseValue.ToString());
                        EditorGUILayout.LabelField("Min", stat.CurrentBaseMinimum.ToString());
                        EditorGUILayout.LabelField("Max", stat.CurrentBaseMaximum.ToString());
                        if (stat.enableRegen)
                        {
                            EditorGUILayout.LabelField("Regen", stat.CurrentBaseRegenAmount.ToString());
                            EditorGUILayout.LabelField("Regen Delay", stat.CurrentBaseRegenDelay.ToString());
                        }

                        SectionHeader("Active Modifiers");
                        if (stat.ActiveModifiers != null && stat.ActiveModifiers.Count == 0)
                        {
                            EditorGUILayout.LabelField("No modifiers active.", Skin.GetStyle("WrapText"));
                        }
                        else
                        {
                            for (int i = 0; i < stat.ActiveModifiers.Count; i++)
                            {
                                EditorGUILayout.LabelField("[" + i + "]", stat.ActiveModifiers[i].AppliedValue.ToString());
                            }
                        }
                    }
                }
            }

            tmp = showEffectsDebug;
            showEffectsDebug = SectionGroup("Active Effects", showEffectsDebug);
            if (tmp)
            {
                if (myTarget.Effects == null || myTarget.Effects.Count == 0)
                {
                    EditorGUILayout.LabelField("There are no active effects.", Skin.GetStyle("WrapText"));
                }
                else
                {
                    for (int i = 0; i < myTarget.Effects.Count; i++)
                    {
                        EditorGUILayout.LabelField("[" + i + "] " + myTarget.Effects[i].displayName);
                    }
                }
            }

            SectionHeader("Console");
            scroll = EditorGUILayout.BeginScrollView(scroll);
            command = EditorGUILayout.TextArea(command, GUILayout.Height(30));
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Process Command"))
            {
                myTarget.SendCommand(command);
                command = string.Empty;
                scroll = Vector2.zero;
                EditorUtility.SetDirty(target);
                Repaint();
            }
        }

        private bool DrawEffects()
        {
            GUILayout.BeginHorizontal();
            SimpleProperty("effectList");
            if (myTarget.effectList == null)
            {
                if (GUILayout.Button(" New ", GUILayout.Width(48), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                {
                    ScriptableObject newItem = CreateNew("Effect List", typeof(StatEffectList));
                    if (newItem != null)
                    {
                        myTarget.effectList = (StatEffectList)newItem;
                    }
                }
            }
            GUILayout.EndHorizontal();

            if (myTarget.effectList != null)
            {
                if(effectListEditor == null)
                {
                    effectListEditor = (StatEffectListEditor)Editor.CreateEditor(myTarget.effectList, typeof(StatEffectListEditor));
                }
                effectListEditor.serializedObject.Update();
                effectListEditor.DrawEffects(myTarget);
                effectListEditor.serializedObject.ApplyModifiedProperties();
            }

            SectionHeader("Starting Effects", "startingEffects", typeof(StatEffect));
            startingSP = SimpleList("startingEffects", startingSP, 120, 1);

            SectionHeader("Resistance");
            resistSP = SimpleList("effectResistances", resistSP, 120, 1);

            return false;
        }

        private void DrawEvents()
        {
            VerticalSpace(6);

            SimpleProperty("onEffectAdded");
            SimpleProperty("onEffectEnded");
            SimpleProperty("onEffectRemoved");
            SimpleProperty("onEffectResisted");

            VerticalSpace(6);

            SimpleProperty("onDamageTaken");
            SimpleProperty("onImmuneToDamage");
            SimpleProperty("onDeath");
        }

        private void UpdateDamageTypesList()
        {
            List<DamageType> foundTags = FindAssetsByType<DamageType>();
            dmgTypes = foundTags.OrderBy(_ => _.name).ToList();
        }

        #endregion

        #region Window Methods

        internal void DrawEffectsList()
        {
            SimpleProperty("effectList");
            if (myTarget.effectList == null)
            {
                if (GUILayout.Button("Create New Effect List"))
                {
                    if (string.IsNullOrWhiteSpace(myTarget.effectFolder))
                    {
                        myTarget.effectFolder = Application.dataPath;
                    }
                    string path = EditorUtility.SaveFilePanelInProject("Save Category", "New Effect List", "asset", "Select a location to save the Effect List", myTarget.effectFolder);
                    if (path.Length != 0)
                    {
                        myTarget.effectFolder = Path.GetDirectoryName(path);

                        StatEffectList effectList = (StatEffectList)ScriptableObject.CreateInstance(typeof(StatEffectList));
                        effectList.name = Path.GetFileNameWithoutExtension(path);
                        AssetDatabase.CreateAsset(effectList, path);
                        AssetDatabase.SaveAssets();

                        myTarget.effectList = effectList;
                    }
                }
            }
        }

        private bool DrawTrait(Trait trait)
        {
            if (trait == null) return true;

            bool delAtEnd = false;
            Rect clickRect;

            GUILayout.Space(-4);

            GUILayout.BeginVertical(BoxGreen, GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();

            GUILayout.Label(GetIcon("OKWhite", "Skins/ok-circle"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
            GUILayout.Label(trait.name, Skin.GetStyle("PanelText"));
            GUILayout.FlexibleSpace();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(trait, typeof(Trait), false, GUILayout.MaxWidth(64));
            EditorGUI.EndDisabledGroup();
            FoldoutTrashOnly(out delAtEnd);

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.EndVertical();
            clickRect = GUILayoutUtility.GetLastRect();

            if (expandedTraits.Contains(trait))
            {
                GUILayout.Space(-6);
                GUILayout.BeginVertical(BoxWhite, GUILayout.ExpandWidth(true));

                if (!traitEditors.ContainsKey(trait))
                {
                    traitEditors.Add(trait, (TraitEditor)Editor.CreateEditor(trait, typeof(TraitEditor)));
                }

                traitEditors[trait].serializedObject.Update();
                traitEditors[trait].DrawInspector(myTarget);
                traitEditors[trait].serializedObject.ApplyModifiedProperties();

                GUILayout.Space(6);
                GUILayout.EndVertical();
            }

            if (!delAtEnd && Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                if (expandedTraits.Contains(trait))
                {
                    expandedTraits.Remove(trait);
                }
                else
                {
                    expandedTraits.Add(trait);
                }

                Repaint();
            }

            if (delAtEnd)
            {
                return true;
            }

            return false;
        }

        private bool DrawTrait(string title, Trait trait)
        {
            bool isExpanded = expandedTraits.Contains(trait);
            bool toggle = false;
            bool delAtEnd = false;

            GUILayout.BeginHorizontal();
            GUILayout.Label(title + " " + trait.displayName, isExpanded ? ButtonLeftPressed : ButtonLeft, GUILayout.Height(21));
            GUILayout.BeginVertical(ButtonMid, GUILayout.MaxWidth(64), GUILayout.Height(21));
            EditorGUILayout.ObjectField(trait, typeof(Trait), false);
            GUILayout.EndVertical();

            Color c = GUI.contentColor;
            GUI.contentColor = EditorColor;
            GUILayout.Label(new GUIContent(GetIcon("TrashWhite", "Skins/trash-white"), "Delete"), ButtonRight, GUILayout.Height(21), GUILayout.Width(21));
            GUI.contentColor = c;

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                delAtEnd = true;
            }


            GUILayout.EndHorizontal();

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                toggle = true;
            }

            if (isExpanded)
            {
                GUILayout.Space(-2);
                GUILayout.BeginVertical(BoxWhite, GUILayout.ExpandWidth(true));

                traitEditors[trait].serializedObject.Update();
                traitEditors[trait].DrawInspector(myTarget);
                traitEditors[trait].serializedObject.ApplyModifiedProperties();

                GUILayout.Space(4);
                GUILayout.EndVertical();
            }

            if (toggle)
            {
                if(isExpanded)
                {
                    expandedTraits.Remove(trait);
                    traitEditors.Remove(trait);
                }
                else
                {
                    expandedTraits.Add(trait);
                    traitEditors.Add(trait, (TraitEditor)Editor.CreateEditor(trait, typeof(TraitEditor)));
                }
            }

            return delAtEnd;
        }

        internal bool DrawTraits()
        {
            bool createNew = false;
            Rect clickRect;
            Color restore = GUI.color;
            SerializedProperty list = serializedObject.FindProperty("additionalTraits");

            GUILayout.BeginVertical(BoxSub, GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();

            GUILayout.Label("Additional Traits");

            GUI.color = Color.black;
            GUILayout.Label(GetIcon("Drop", "Icons/tock-drop"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
            GUI.color = restore;

            GUILayout.FlexibleSpace();

            GUI.color = Color.black;
            GUILayout.Label(GetIcon("Add", "Icons/tock-add"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
            clickRect = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                createNew = true;
            }

            GUILayout.Label(GetIcon("X", "Icons/tock-x"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
            clickRect = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                list.ClearArray();
                expandedTraits.Clear();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                return true;
            }
            GUI.color = restore;

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.EndVertical();

            HandleDragDrop(list, typeof(Trait));

            if (list.arraySize > 0)
            {
                bool removeItem = false;
                Trait targetTrait = null;
                int index = 0;
                for (int i = 0; i < list.arraySize; i++)
                {
                    Trait trait = (Trait)list.GetArrayElementAtIndex(i).objectReferenceValue;
                    if (DrawTrait(trait))
                    {
                        removeItem = true;
                        targetTrait = trait;
                        index = i;
                    }
                }

                if (removeItem)
                {
                    list.GetArrayElementAtIndex(index).objectReferenceValue = null;
                    list.DeleteArrayElementAtIndex(index);
                    serializedObject.ApplyModifiedProperties();
                    expandedTraits.Clear();
                    Repaint();
                    return true;
                }
            }

            if (createNew)
            {
                ScriptableObject newItem = CreateNew("Trait", typeof(Trait));
                if (newItem != null)
                {
                    list.arraySize++;
                    list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = newItem;

                    serializedObject.ApplyModifiedProperties();
                    Repaint();
                    return true;
                }
            }

            return false;
        }

        #endregion

    }
}
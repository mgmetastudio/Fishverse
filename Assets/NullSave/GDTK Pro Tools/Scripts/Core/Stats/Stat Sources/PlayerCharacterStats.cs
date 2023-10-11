#if GDTK
using NullSave.GDTK.JSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [AutoDoc("This component inherits from NPCStats and adds classes, race, background and level to an object. It provides access to Stats, Attributes, Status Conditions, Stat Events, Status Effects, Languages, Respawn, Race, Classes, Level, Background, and Plugins.")]
    [DefaultExecutionOrder(-510)]
    public class PlayerCharacterStats : NPCStats
    {

        #region Fields

        [Tooltip("Prevent level from increasing, level will update to proper state once this value is false")] [SerializeField] private bool m_preventLevelGain;
        [SerializeField] [Tooltip("Id of Race to use at start")] private string m_raceId;
        [SerializeField] [Tooltip("Id of Background to use at start")] private string m_backgroundId;
        [SerializeField] [Tooltip("Id of Stat providing Level")] private string m_levelId;
        [SerializeField] [Tooltip("Id of Class to use at start")] private string m_classId;

        [Tooltip("Prefab used for selectioning Add-On Choices")] public AddOnChoiceListUI selectOptionUI;

        [Tooltip("Event raised when a Class is added")] public ClassEvent onClassAdded;
        [Tooltip("Event raised when a Class is removed")] public ClassEvent onClassRemoved;
        [Tooltip("Event raised when a Class is added *or* a Class' level changes")] public SimpleEvent onClassesChanged;
        [Tooltip("Event raised when the player's level changes")] public SimpleEvent onLevelChanged;
        [Tooltip("Event raised when the Race is changed")] public SimpleEvent onRaceChanged;
        [Tooltip("Event raice when the Background is changed")] public SimpleEvent onBackgroundChanged;
        [Tooltip("Event raised when a Perk is added")] public PerkEvent onPerkAdded;
        [Tooltip("Event raised when a Perk is removed")] public PerkEvent onPerkRemoved;
        [Tooltip("Event raised when a Perk is added *or* removed")] public SimpleEvent onPerksChanged;
        [Tooltip("Event raised when a Trait is added")] public TraitEvent onTraitAdded;
        [Tooltip("Event raised when a Trait is removed")] public TraitEvent onTraitRemoved;

        private GDTKRace m_race;
        private GDTKBackground m_background;

        private List<GDTKClass> m_classes;
        private List<GDTKTrait> m_traits;
        private List<GDTKPerk> m_perks;

        private List<AddOnPlugin> m_addOnPlugins;

        private GDTKStat levelStat;

        #endregion

        #region Properties

        [AutoDoc("Gets/Sets the Bacground for the object", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(PlayerCharacterStats source)<br/>    {<br/>        GDTKBackground background = source.database.backgrounds[0];<br/>        if(source.background != background)<br/>        {<br/>            source.background = background;<br/>        }<br/>    }<br/><br/>}")]
        public GDTKBackground background
        {
            get { return m_background; }
            set
            {
                if (m_background == value) return;
                RemoveBackground();
                SetBackground(value);
                onBackgroundChanged?.Invoke();
            }
        }

        [AutoDoc("Returns a read-only list of Classes assigned to the object", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(PlayerCharacterStats source)<br/>    {<br/>        GDTKClass playerClass = source.classes[0];<br/>        source.RemoveClass(playerClass);<br/>    }<br/><br/>}")]
        public IReadOnlyList<GDTKClass> classes
        {
            get { return m_classes; }
        }

        public string levelId { get { return m_levelId; } }

        [AutoDoc("Returns a read-only list of Perks assigned to the object", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(PlayerCharacterStats source)<br/>    {<br/>        GDTKPerk perk = source.perks[0];<br/>        source.RemovePerk(perk);<br/>    }<br/><br/>}")]
        public IReadOnlyList<GDTKPerk> perks
        {
            get { return m_perks; }
        }

        [AutoDoc("Gets/Sets whether to suspend Level Gaining. When changing from false to true any and all Levels that were suspended will be added in sequence.", "using NullSave.GDTK.Stats;<br/>using System.Collections;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public PlayerCharacterStats source;<br/><br/>    public IEnumerator ExampleMethod()<br/>    {<br/>        source.preventLevelGain = true;<br/>        yield return new WaitForSeconds(30);<br/>        source.preventLevelGain = false;<br/>    }<br/><br/>}")]
        public bool preventLevelGain
        {
            get { return m_preventLevelGain; }
            set
            {
                if (m_preventLevelGain == value) return;
                m_preventLevelGain = value;

                if (levelStat != null)
                {
                    levelStat.expressions.value.locked = value;
                }
            }
        }

        [AutoDoc("Gets/Sets the Race for the object", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(PlayerCharacterStats source)<br/>    {<br/>        GDTKRace race = source.database.races[0];<br/>        if(source.race != race)<br/>        {<br/>            source.race = race;<br/>        }<br/>    }<br/><br/>}")]
        public GDTKRace race
        {
            get { return m_race; }
            set
            {
                if (m_race == value) return;
                RemoveRace();
                SetRace(value);
                onRaceChanged?.Invoke();
            }
        }

        public string startingBackgroundId { get { return m_backgroundId; } }

        public string startingClassId { get { return m_classId; } }

        public string startingRaceId { get { return m_raceId; } }

        [AutoDoc("Returns a read-only list of Traits assigned to the object", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(PlayerCharacterStats source)<br/>    {<br/>        GDTKTrait trait = source.traits[0];<br/>        source.RemoveTraitsFromSource(trait.source);<br/>    }<br/><br/>}")]
        public IReadOnlyList<GDTKTrait> traits
        {
            get { return m_traits; }
        }

        #endregion

        #region Unity Methods

        [AutoDocSuppress]
        public override void Awake()
        {
            m_race = new GDTKRace();
            m_traits = new List<GDTKTrait>();
            m_perks = new List<GDTKPerk>();
            m_classes = new List<GDTKClass>();
            m_addOnPlugins = new List<AddOnPlugin>();

            base.Awake();
        }

        #endregion

        #region Public Methods

        [AutoDoc("Adds one or more Add-On Plugin", "using NullSave.GDTK;<br/>using NullSave.GDTK.Stats;<br/>using System;<br/>using System.Collections;<br/>using System.Collections.Generic;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public IEnumerator ExampleMethod(PlayerCharacterStats source, GDTKLevelReward reward)<br/>    {<br/>        bool canContinue;<br/><br/>        // Clone item to ensure we don't set database data<br/>        GDTKLevelReward rewardClone = reward.Clone();<br/><br/>        // Create a callback<br/>        Action<List<ISelectableOption>> callback = l =><br/>        {<br/>            canContinue = true;<br/>        };<br/><br/>        // Add Add-Ons<br/>        foreach (UniversalPluginWrapper<AddOnPlugin> wrapper in rewardClone.addOnPlugins)<br/>        {<br/>            canContinue = false;<br/>            source.AddAddOnPlugins(wrapper.plugin, null, callback);<br/>            while (!canContinue) yield return new WaitForEndOfFrame();<br/>        }<br/>    }<br/><br/>}")]
        [AutoDocParameter("Plugin to add")]
        [AutoDocParameter("Source associated with add-on")]
        [AutoDocParameter("Callback to invoke with results upon completion")]
        public virtual void AddAddOnPlugins(AddOnPlugin addOn, BasicInfo info, IUniquelyIdentifiable source, Action<List<ISelectableOption>> resultsCallback = null)
        {
            addOn.Initialize(this);

            if (!addOn.hasUnused)
            {
                resultsCallback?.Invoke(new List<ISelectableOption>());
                return;
            }

            addOn.source = source;
            RegisterSource(source);

            if (addOn.needsUI)
            {
                AddOnChoiceListUI spawn = InterfaceManager.ObjectManagement.InstantiateObject(selectOptionUI, InterfaceManager.UICanvas.transform);
                spawn.LoadChoices(addOn, info, this, resultsCallback);
                spawn.gameObject.SetActive(true);
            }
            else
            {
                addOn.Apply(this, globalStats);
                resultsCallback?.Invoke(addOn.selectedOptions.ToList());
            }

            m_addOnPlugins.Add(addOn);
        }

        [AutoDoc("Adds a Class to the object. If that Class already exists its level is increased by 1.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(PlayerCharacterStats source)<br/>    {<br/>        string id = source.database.classes[0].info.id;<br/>        source.AddClass(id);<br/>        // Adding a second time increases the class' level<br/>        source.AddClass(id);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of class to add")]
        [AutoDocParameter("Source associated with request")]
        public virtual GDTKClass AddClass(string id, IUniquelyIdentifiable source = null)
        {
            // Check for level up
            foreach (GDTKClass playerClass in classes)
            {
                if (playerClass.info.id == id)
                {
                    playerClass.level += 1;
                    onClassesChanged?.Invoke();
                    return playerClass;
                }
            }

            GDTKClass pc = database.GetClass(id);
            if (pc == null) return null;

            pc = pc.Clone();
            pc.level = 1;
            m_classes.Add(pc);
            RegisterSource(pc);

            foreach (GDTKTrait trait in pc.traits)
            {
                trait.Initialize(pc, this);
                //AddTrait(trait, pc);
            }

            onClassAdded?.Invoke(pc);
            onClassesChanged?.Invoke();

            return pc;
        }

        [AutoDoc("Adds a Perk to the object", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(PlayerCharacterStats source)<br/>    {<br/>        string id = source.database.perks[0].info.id;<br/>        source.AddPerk(id);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of the Perk to add")]
        [AutoDocParameter("Source associated with request")]
        public virtual GDTKPerk AddPerk(string id, IUniquelyIdentifiable source = null)
        {
            RegisterSource(source);

            foreach (GDTKPerk value in database.perks)
            {
                if (value.info.id == id)
                {
                    GDTKPerk clone = value.Clone();
                    clone.sourceId = source?.instanceId;
                    m_perks.Add(clone);

                    foreach (string attributeId in clone.attributeIds)
                    {
                        AddAttribute(attributeId, clone);
                    }

                    foreach (string conditionId in clone.conditionIds)
                    {
                        AddStatusCondition(conditionId, clone);
                    }

                    foreach (GDTKStatModifier statMod in clone.statModifiers)
                    {
                        AddStatModifier(statMod, clone);
                    }

                    onPerkAdded?.Invoke(clone);
                    onPerksChanged?.Invoke();
                    return clone;
                }
            }

            return null;
        }

        [AutoDoc("Add a Trait to the object", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(PlayerCharacterStats source)<br/>    {<br/>        // Create a modifier that applies 1 time immediately<br/>        // Modifier changes the value of a Stat with the Id<br/>        // of 'HP' by adding 2<br/>        GDTKStatModifier modifier = new GDTKStatModifier();<br/>        modifier.target = ModifierTarget.Value;<br/>        modifier.applies = ModifierApplication.Immediately;<br/>        modifier.affectsStatId = \"HP\";<br/>        modifier.changeType = ModifierChangeType.Add;<br/>        modifier.value.valueExpression = \"2\";<br/><br/>        // Create trait and add modifier<br/>        GDTKTrait trait = new GDTKTrait();<br/>        trait.statModifiers.Add(modifier);<br/><br/>        // Add trait<br/>        source.AddTrait(trait, null);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of the Trait to add")]
        [AutoDocParameter("Source associated with request")]
        public virtual void AddTrait(GDTKTrait trait, IUniquelyIdentifiable source = null)
        {
            if (!trait.initialized)
            {
                // Initializing the trait will add it as needed
                trait.Initialize(source, this);
                return;
            }

            if (!trait.IsUnlocked(this))
            {
                StringExtensions.LogError(name, "AddTrait", "Specified trait is not unlocked.");
                return;
            }

            trait.sourceId = source?.instanceId;
            RegisterSource(source);

            foreach (UniversalPluginWrapper<AddOnPlugin> wrapper in trait.addOnPlugins)
            {
                AddAddOnPlugins(wrapper.plugin, trait.info, trait);
            }

            foreach (GDTKStatModifier statMod in trait.statModifiers)
            {
                AddStatModifier(statMod, trait);
            }

            m_traits.Add(trait);
            onTraitAdded?.Invoke(trait);
        }

        [AutoDocSuppress]
        public override int DataLoad(Stream stream)
        {
            Data.PlayerCharacterStatsData data = new Data.PlayerCharacterStatsData(stream);
            LoadData(data);
            return data.version;
        }

        public override void DataLoadJSON(string json)
        {
            Data.PlayerCharacterStatsData data = SimpleJson.FromJSON<Data.PlayerCharacterStatsData>(json);

            // Special Case (RACE)
            if(data.race != null)
            {
                data.race.race = database.GetRace(data.race.id);
                data.race.race.traits = data.race.traits.ToList();
            }

            // Special Case (BACKGROUND)
            if (data.background != null)
            {
                data.background.background = database.GetBackground(data.background.id);
                data.background.background.traits = data.background.traits.ToList();
            }

            LoadData(data);
        }

        public override void DataLoadJSONFile(string filename)
        {
            DataLoadJSON(File.ReadAllText(Path.Combine(Application.persistentDataPath, filename)));
        }

        [AutoDocSuppress]
        public override void DataSave(Stream stream)
        {
            GenerateData().Write(stream, STATS_FILE_VERSION);
        }

        public override string DataSaveJSON(bool removeNulls = true, bool readable = false)
        {
            Data.PlayerCharacterStatsData data = (Data.PlayerCharacterStatsData)GenerateData();
            return SimpleJson.ToJSON(data, removeNulls, readable);
        }

        public override void DataSaveJSON(string filename)
        {
            using (FileStream fs = new FileStream(Path.Combine(Application.persistentDataPath, filename), FileMode.Create, FileAccess.Write))
            {
                fs.WriteString(DataSaveJSON());
            }
        }

        [AutoDoc("Returns the current Character level", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(PlayerCharacterStats source)<br/>    {<br/>        Debug.Log(\"Player Level: \" + source.GetCharacterLevel());<br/>    }<br/><br/>}")]
        public virtual int GetCharacterLevel()
        {
            if (levelStat == null) return 0;
            return (int)levelStat.value;
        }

        [AutoDoc("Get a Class available on this object by id", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(PlayerCharacterStats source)<br/>    {<br/>        string id = source.database.classes[0].info.id;<br/>        GDTKClass playerClass = source.GetClass(id);<br/>        if(playerClass != null)<br/>        {<br/>            source.RemoveClass(playerClass);<br/>        }<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of the Class to find")]
        public virtual GDTKClass GetClass(string id)
        {
            foreach (GDTKClass val in classes)
            {
                if (val.info.id == id) return val;
            }

            return null;
        }

        [AutoDoc("Get the level of a Class available on this object by id", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(PlayerCharacterStats source)<br/>    {<br/>        string id = source.database.classes[0].info.id;<br/>        Debug.Log(\"Class Level: \" + source.GetClassLevel(id));<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of the Class to get level for")]
        public virtual int GetClassLevel(string id)
        {
            foreach (GDTKClass val in classes)
            {
                if (val.info.id == id) return val.level;
            }

            return 0;
        }

        [AutoDoc("Grants player a custom reward", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(PlayerCharacterStats source)<br/>    {<br/>        string id = source.database.customRewards[0].info.id;<br/>        source.GrantCustomReward(id);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of the Custom Reward to grant")]
        public virtual void GrantCustomReward(string id)
        {
            foreach (GDTKLevelReward reward in database.customRewards)
            {
                if (reward.info.id == id)
                {
                    GrantCustomReward(reward);
                    return;
                }
            }
        }

        [AutoDoc("Grants player a custom reward", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(PlayerCharacterStats source)<br/>    {<br/>        GDTKLevelReward reward = source.database.customRewards[0];<br/>        source.GrantCustomReward(reward);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Custom Reward to grant")]
        public virtual void GrantCustomReward(GDTKLevelReward reward)
        {
            if (reward == null || !GDTKStatsManager.IsConditionTrue(reward.requirements, source)) return;
            StartCoroutine(ApplyCustomReward(reward));
        }

        [AutoDoc("Check if object has a specific Class", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(PlayerCharacterStats source)<br/>    {<br/>        string id = source.database.classes[0].info.id;<br/>        if(source.HasClass(id))<br/>        {<br/>            Debug.Log(\"Class is present\");<br/>        }<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of the Class to find")]
        public virtual bool HasClass(string id)
        {
            foreach (GDTKClass val in classes)
            {
                if (val.info.id == id) return true;
            }

            return false;
        }

        [AutoDoc("Check if object has a specific Perk", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(PlayerCharacterStats source)<br/>    {<br/>        string id = source.database.perks[0].info.id;<br/>        if(source.HasPerk(id))<br/>        {<br/>            Debug.Log(\"Perk is present\");<br/>        }<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of the Perk to find")]
        public virtual bool HasPerk(string id)
        {
            foreach (GDTKPerk val in perks)
            {
                if (val.info.id == id) return true;
            }

            return false;
        }

        [AutoDocSuppress]
        public override void Initialize(bool reset)
        {
            base.Initialize(reset);

            // Add Actions
            source.RegisterAction("classesChanged", Source_ClassesChanged);
            source.RegisterAction("getClass", Source_GetClass);
            source.RegisterAction("hasPerk", Source_HasPerk);
            source.RegisterAction("isRace", Source_IsRace);
            source.RegisterAction("perksChanged", Source_PerksChanged);
            source.RegisterAction("raceChanged", Source_RaceChanged);

            if (database != null)
            {
                SetRace(database.GetRace(m_raceId));
            }

            // Get Level
            levelStat = GetStat(m_levelId);
            if (levelStat != null)
            {
                levelStat.expressions.value.locked = m_preventLevelGain;
                levelStat.expressions.value.onValueChanged += UpdateLevel;
            }

            // Set Class
            if (!string.IsNullOrEmpty(m_classId))
            {
                AddClass(m_classId, null);
            }
        }

        [AutoDoc("Check if object is a specific Race", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(PlayerCharacterStats source)<br/>    {<br/>        string id = source.database.races[0].info.id;<br/>        if(source.IsRace(id))<br/>        {<br/>            Debug.Log(\"Race matches request\");<br/>        }<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of the Race to check")]
        public virtual bool IsRace(string id)
        {
            return m_raceId == id;
        }

        public string JSONExport(List<int> statIndexes, List<int> conditionIndexes, List<int> attributeIndexes, bool includeRace, bool includeBackground, bool includeClass, bool includeLevel, bool includeRespawn)
        {
            jsonPlayerStatList result = new jsonPlayerStatList();

            foreach (int index in attributeIndexes) result.attributes.Add(startingAttributeIds[index]);
            foreach (int index in conditionIndexes) result.conditions.Add(startingConditionIds[index]);

            int si = 0;
            foreach (var stat in stats)
            {
                if (statIndexes.Contains(si))
                {
                    result.stats.Add(stat.Value);
                }
                si++;
            }

            if (includeRespawn)
            {
                result.respawnData = true;
                result.respawnCondition = respawnCondition;
                result.saveFilename = saveFilename;
                result.savePosition = savePosition;
                result.saveRotation = saveRotation;
                foreach (GDTKStatModifier val in respawnModifiers) result.respawnModifiers.Add(val);
            }

            result.levelId = includeLevel ? m_levelId : "!!IGNORE!!";
            result.raceId = includeRace ? m_raceId : "!!IGNORE!!";
            result.backgroundId = includeBackground ? m_backgroundId : "!!IGNORE!!";
            result.classId = includeClass ? m_classId : "!!IGNORE!!";

            return SimpleJson.ToJSON(result);
        }

        public override string JSONExport()
        {
            jsonPlayerStatList result = new jsonPlayerStatList();

            foreach (string val in startingAttributeIds) result.attributes.Add(val);
            foreach (string val in startingConditionIds) result.conditions.Add(val);
            foreach (var val in stats) result.stats.Add(val.Value);

            result.respawnData = true;
            result.respawnCondition = respawnCondition;
            result.saveFilename = saveFilename;
            result.savePosition = savePosition;
            result.saveRotation = saveRotation;
            foreach (GDTKStatModifier val in respawnModifiers) result.respawnModifiers.Add(val);

            result.levelId = m_levelId;
            result.raceId = m_raceId;
            result.backgroundId = m_backgroundId;
            result.classId = m_classId;

            return SimpleJson.ToJSON(result);
        }

        public override void JSONImport(string json, bool clearExisting)
        {
            JSONImport(SimpleJson.FromJSON<jsonPlayerStatList>(json), clearExisting);
        }

        public void JSONImport(jsonPlayerStatList source, bool clearExisting)
        {
            JSONImport((jsonNPCList)source, clearExisting);

            m_levelId = source.levelId;
            m_preventLevelGain = source.preventLevelGain;
            m_raceId = source.raceId;
            m_backgroundId = source.backgroundId;
            m_classId = source.classId;
        }

        public void JSONImportSmart(jsonPlayerStatList source)
        {
            JSONImportSmart((jsonNPCList)source);

            if (source.levelId != "!!IGNORE!!") m_levelId = source.levelId;
            if (source.raceId != "!!IGNORE!!") m_raceId = source.raceId;
            if (source.backgroundId != "!!IGNORE!!") m_backgroundId = source.backgroundId;
            if (source.classId != "!!IGNORE!!") m_classId = source.classId;
        }

        [AutoDoc("Remove all Add-Ons from a specific source", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(PlayerCharacterStats source)<br/>    {<br/>        // Remove all Add-On with no source provided<br/>        source.RemoveAddOnsFromSource(null);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Source associated with request")]
        public virtual void RemoveAddOnsFromSource(IUniquelyIdentifiable source)
        {
            foreach (AddOnPlugin option in m_addOnPlugins.ToArray())
            {
                if (option.source == source)
                {
                    RemoveAttributesFromSource(option);
                    RemoveLanguagesFromSource(option);
                    RemoveStatusConditionsFromSource(option);
                    RemovePerkFromSource(option);
                    m_addOnPlugins.Remove(option);
                    RemoveSource(option);
                }
            }
        }

        [AutoDoc("Remove a Class from object", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(PlayerCharacterStats source)<br/>    {<br/>        GDTKClass playerClass = source.classes[0];<br/>        source.RemoveClass(playerClass);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of Class to Remove")]
        [AutoDocParameter("If false Class' level will be reduce by 1 and only removed if the level reaches 0")]
        public virtual void RemoveClass(GDTKClass playerClass, bool fullyRemove = false)
        {
            if (!classes.Contains(playerClass)) return;

            if (playerClass.level == 1 || fullyRemove)
            {
                m_classes.Remove(playerClass);
                onClassRemoved?.Invoke(playerClass);
                onClassesChanged?.Invoke();
            }
            else
            {
                playerClass.level -= 1;
            }
        }

        [AutoDoc("Remove a Perk from the object", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(PlayerCharacterStats source)<br/>    {<br/>        GDTKPerk perk = source.perks[0];<br/>        source.RemovePerk(perk);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Perk to remove")]
        public virtual void RemovePerk(GDTKPerk perk)
        {
            RemoveAttributesFromSource(perk);
            RemoveStatusConditionsFromSource(perk);
            RemoveStatModifiersFromSource(perk);
            m_perks.Remove(perk);
            onPerkRemoved?.Invoke(perk);
            onPerksChanged?.Invoke();
        }

        [AutoDoc("Remove all Perks from a specific source", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(PlayerCharacterStats source)<br/>    {<br/>        // Remove all Perks with no source provided<br/>        source.RemovePerkFromSource(null);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Source associated with request")]
        public virtual void RemovePerkFromSource(IUniquelyIdentifiable source)
        {
            foreach (GDTKPerk perk in m_perks.ToArray())
            {
                if (perk.sourceId == source?.instanceId)
                {
                    RemovePerk(perk);
                }
            }
        }

        [AutoDoc("Remove all Traits from a specific source", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(PlayerCharacterStats source)<br/>    {<br/>        GDTKTrait trait = source.traits[0];<br/>        source.RemoveTraitsFromSource(trait.source);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Source associated with request")]
        public virtual void RemoveTraitsFromSource(IUniquelyIdentifiable source)
        {
            foreach (GDTKTrait trait in m_traits.ToArray())
            {
                if (trait.sourceId == source?.instanceId)
                {
                    RemoveAttributesFromSource(trait);
                    RemoveLanguagesFromSource(trait);
                    RemoveStatusConditionsFromSource(trait);
                    RemovePerkFromSource(trait);
                    RemoveStatModifiersFromSource(trait);
                    RemoveAddOnsFromSource(trait);
                    m_traits.Remove(trait);
                    onTraitRemoved?.Invoke(trait);
                    RemoveSource(trait);
                }
            }
        }

        [AutoDoc("Set the Background of this object", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(PlayerCharacterStats source)<br/>    {<br/>        string id = source.database.backgrounds[0].info.id;<br/>        source.SetBackground(id);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of the Background to set")]
        public virtual void SetBackground(string id)
        {
            background = database.GetBackground(id);
        }

        [AutoDoc("Set the Race of this object", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(PlayerCharacterStats source)<br/>    {<br/>        string id = source.database.races[0].info.id;<br/>        source.SetRace(id);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of the Race to set")]
        public virtual void SetRace(string id)
        {
            race = database.GetRace(id);
        }

        [AutoDocSuppress]
        public override void Shutdown(bool fullShutdown)
        {
            base.Shutdown(fullShutdown);

            m_race = null;
            m_background = null;

            foreach (GDTKTrait trait in m_traits)
            {
                onTraitRemoved?.Invoke(trait);
            }
            m_traits.Clear();

            m_addOnPlugins.Clear();

            foreach (GDTKPerk perk in m_perks)
            {
                onPerkRemoved?.Invoke(perk);
            }
            m_perks.Clear();

            foreach (GDTKClass pc in m_classes)
            {
                pc.Shutdown();
                onClassRemoved?.Invoke(pc);
            }
            m_classes.Clear();

            if (fullShutdown)
            {
                onClassAdded = null;
                onClassRemoved = null;
                onClassesChanged = null;
                onLevelChanged = null;
                onPerksChanged = null;
                onPerkAdded = null;
                onPerkRemoved = null;
            }
        }

        #endregion

        #region Broadcast Methods

        [AutoDocSuppress]
        public override void PublicBroadcastReceived(object sender, string message)
        {
            StatsDebugManager.PlayerCharacterStatsRequest(this, gameObject, message);
        }

        #endregion

        #region Private Methods

        private IEnumerator ApplyCustomReward(GDTKLevelReward reward)
        {
            bool canContinue;
            AddOnPluginChoice selectedPluginChoice = null;

            GDTKLevelReward rewardClone = reward.Clone();

            void callback(List<ISelectableOption> l)
            {
                canContinue = true;
            }

            void pluginListCallback(AddOnPluginChoice pl)
            {
                selectedPluginChoice = pl;
                canContinue = true;
            }

            foreach (UniversalPluginWrapper<AddOnPlugin> wrapper in rewardClone.addOnPlugins)
            {
                canContinue = false;
                AddAddOnPlugins(wrapper.plugin, reward.info, null, callback);
                while (!canContinue) yield return new WaitForEndOfFrame();
            }

            if (rewardClone.pluginChoices.Count > 0)
            {
                canContinue = false;
                AddOnChoiceListUI spawn = InterfaceManager.ObjectManagement.InstantiateObject(selectOptionUI, InterfaceManager.UICanvas.transform);
                spawn.LoadChoices(rewardClone.pluginChoices, reward.info, this, pluginListCallback);
                spawn.gameObject.SetActive(true);
                while (!canContinue) yield return new WaitForEndOfFrame();
                if (selectedPluginChoice != null)
                {
                    foreach (UniversalPluginWrapper<AddOnPlugin> addOn in selectedPluginChoice.pickFrom)
                    {
                        canContinue = false;
                        AddAddOnPlugins(addOn.plugin, reward.info, null, callback);
                        while (!canContinue) yield return new WaitForEndOfFrame();
                    }
                }
            }

            foreach (GDTKStatModifier modifier in rewardClone.statModifiers)
            {
                AddStatModifier(modifier, null);
            }
        }

        internal override Data.BasicStatsData GenerateData()
        {
            Data.NPCStatsData baseData = (Data.NPCStatsData)base.GenerateData();

            return new Data.PlayerCharacterStatsData
            {
                stats = baseData.stats,
                statusConditions = baseData.statusConditions,
                attributes = baseData.attributes,
                plugins = baseData.plugins,
                activeEffects = baseData.activeEffects,
                forbiddenIds = baseData.forbiddenIds,
                languages = baseData.languages,
                respawnCondition = baseData.respawnCondition,
                savePosition = baseData.savePosition,
                position = baseData.position,
                saveRotation = baseData.saveRotation,
                rotation = baseData.rotation,
                preventLevelGain = m_preventLevelGain,
                race = m_race == null ? null : m_race.GenerateData(),
                background = m_background == null ? null : m_background.GenerateData(),
                classes = m_classes.ToList(),
                perks = m_perks.ToList(),
                levelId = levelId,
            };
        }

        internal override void LoadData(Data.BasicStatsData data)
        {
            base.LoadData(data);

            Data.PlayerCharacterStatsData sdata = (Data.PlayerCharacterStatsData)data;

            m_preventLevelGain = sdata.preventLevelGain;

            if(sdata.race != null)
            {
                RemoveRace();
                m_race = sdata.race.race;
                if (m_race != null)
                {
                    RegisterSource(m_race);
                    m_race.FinalizeLoading(this);
                }
            }
            onRaceChanged?.Invoke();

            if (sdata.background != null)
            {
                background = sdata.background.background;
                if(m_background != null)
                {
                    RegisterSource(m_background);
                    m_background.FinalizeLoading(this);
                }
            }
            onBackgroundChanged?.Invoke();

            foreach (GDTKClass pc in sdata.classes)
            {
                m_classes.Add(pc);
                RegisterSource(pc);
                pc.FinalizeLoading(this);
                onClassAdded?.Invoke(pc);
            }

            foreach (GDTKPerk perk in sdata.perks)
            {
                m_perks.Add(perk);
                onPerkAdded?.Invoke(perk);
            }

            m_levelId = sdata.levelId;

            // Get Level
            levelStat = GetStat(m_levelId);
            if (levelStat != null)
            {
                levelStat.expressions.value.locked = m_preventLevelGain;
                levelStat.expressions.value.onValueChanged += UpdateLevel;
            }
        }

        private IEnumerator ProcessLevelChange()
        {
            bool canContinue;
            AddOnPluginChoice selectedPluginChoice = null;

            void callback(List<ISelectableOption> l)
            {
                canContinue = true;
            }

            void pluginListCallback(AddOnPluginChoice pl)
            {
                selectedPluginChoice = pl;
                canContinue = true;
            }

            // Level Rewards
            foreach (GDTKLevelReward reward in database.levelRewards)
            {
                if (GDTKStatsManager.IsConditionTrue(reward.requirements, source))
                {
                    GDTKLevelReward rewardClone = reward.Clone();

                    foreach (UniversalPluginWrapper<AddOnPlugin> wrapper in rewardClone.addOnPlugins)
                    {
                        canContinue = false;
                        AddAddOnPlugins(wrapper.plugin, reward.info, null, callback);
                        while (!canContinue) yield return new WaitForEndOfFrame();
                    }

                    if (rewardClone.pluginChoices.Count > 0)
                    {
                        canContinue = false;
                        AddOnChoiceListUI spawn = InterfaceManager.ObjectManagement.InstantiateObject(selectOptionUI, InterfaceManager.UICanvas.transform);
                        spawn.LoadChoices(rewardClone.pluginChoices, reward.info, this, pluginListCallback);
                        spawn.gameObject.SetActive(true);
                        while (!canContinue) yield return new WaitForEndOfFrame();
                        if (selectedPluginChoice != null)
                        {
                            foreach (UniversalPluginWrapper<AddOnPlugin> addOn in selectedPluginChoice.pickFrom)
                            {
                                canContinue = false;
                                AddAddOnPlugins(addOn.plugin, reward.info, null, callback);
                                while (!canContinue) yield return new WaitForEndOfFrame();
                            }
                        }
                    }

                    foreach (GDTKStatModifier modifier in rewardClone.statModifiers)
                    {
                        AddStatModifier(modifier, null);
                    }
                }
            }

            onLevelChanged?.Invoke();
        }

        private void RemoveBackground()
        {
            if (m_background == null) return;

            RemoveTraitsFromSource(m_background);
            RemoveSource(m_background);
        }

        private void RemoveRace()
        {
            if (m_race == null) return;

            RemoveTraitsFromSource(m_race);
            RemoveSource(m_race);
        }

        private void SetBackground(GDTKBackground background)
        {
            if (background != null)
            {
                RegisterSource(background);

                foreach (GDTKTrait trait in background.traits)
                {
                    AddTrait(trait, background);
                }
            }

            m_backgroundId = background == null ? null : background.info.id;
            m_background = background;
        }

        private void SetRace(GDTKRace race)
        {
            if (race != null)
            {
                race = race.Clone();
                RegisterSource(race);

                foreach (GDTKTrait trait in race.traits)
                {
                    trait.Initialize(race, this);
                }
            }

            m_raceId = race == null ? null : race.info.id;
            m_race = race;
            onRaceChanged?.Invoke();
        }

        private void Source_ClassesChanged(object[] args, ref object result)
        {
            onClassesChanged += (SimpleEvent)args[0];
            result = onClassesChanged;
        }

        private void Source_GetClass(object[] args, ref object result)
        {
            result = GetClass((string)args[0]);
        }

        private void Source_HasPerk(object[] args, ref object result)
        {
            result = HasPerk((string)args[0]);
        }

        private void Source_IsRace(object[] args, ref object result)
        {
            result = IsRace((string)args[0]);
        }

        private void Source_PerksChanged(object[] args, ref object result)
        {
            onPerksChanged += (SimpleEvent)args[0];
            result = onPerksChanged;
        }

        private void Source_RaceChanged(object[] args, ref object result)
        {
            onRaceChanged += (SimpleEvent)args[0];
            result = onRaceChanged;
        }

        private void UpdateLevel()
        {
            StartCoroutine(ProcessLevelChange());
        }

        #endregion

    }
}
#endif
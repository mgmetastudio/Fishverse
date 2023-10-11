#if GDTK
using NullSave.GDTK.JSON;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NullSave.GDTK.Stats
{

    [AutoDoc("This component adds basic stats to an object. It provides access to Stats, Attributes, Status Conditions, Stat Events, and Plugins.")]
    [DefaultExecutionOrder(-510)]
    public class BasicStats : InteractorComponent, IBroadcastReceiver
    {

        #region Constants

        public const int STATS_FILE_VERSION = 2;

        #endregion

        #region Fields

        [Tooltip("Place in the Tool Registry so it can be found easily by other components")] public bool register;
        [Tooltip("Key to associate with this instance")] public string registryKey;

        [Tooltip("List of plugins to run in sequence")] public List<UniversalPluginWrapper<StatsPlugin>> plugins;

        [Tooltip("Event raised when a Stat Event is triggered")] public EventTrigger onEventTriggered;
        [Tooltip("Event raised when an Attribute is added")] public AttributeEvent onAttributeAdded;
        [Tooltip("Event raised when an Attribute is removed")] public AttributeEvent onAttributeRemoved;
        [Tooltip("Event raised when a Status Condition is added")] public StatusConditionEvent onStatusConditionAdded;
        [Tooltip("Event raised when a Status Condition is removed")] public StatusConditionEvent onStatusConditionRemoved;
        [Tooltip("Event raised when Stats are reloaded")] public SimpleEvent onStatsReloaded;

        [Tooltip("Event raised every Update")] public HeartbeatEvent onHeartbeat;
        [Tooltip("Event raised when a Token Heartbeat is invoked")] public HeartbeatEvent onTokenHeartbeat;

        private Dictionary<string, List<SimpleEvent>> attribSubs;

        private StatsDatabase db;
        [SerializeField] private int z_display_flags;

        [SerializeField] [JsonSerialize] private List<GDTKStatusCondition> m_statusConditions;
        [SerializeField] [JsonSerialize] private List<string> m_attributeIds;
        [SerializeField] [JsonSerialize] private List<string> m_conditionIds;
        private List<GDTKAttribute> m_attributes;

        // Upgrading
        [SerializeField] private StatSource statSource;

        // Obsoleting
        [SerializeField] [JsonSerialize] private List<GDTKStat> m_stats;
        
        #endregion

        #region Properties

        [AutoDoc("Returns a read-only list of Attributes on the object.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        foreach(GDTKAttribute attribute in source.attributes)<br/>        {<br/>            Debug.Log(attribute.info.title);<br/>        }<br/>    }<br/><br/>}")]
        public IReadOnlyList<GDTKAttribute> attributes
        {
            get { return m_attributes; }
        }

        [AutoDoc("True if the system is currently loading or saving data.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        if (!source.busy)<br/>        {<br/>            source.DataSave(\"test.sav\");<br/>        }<br/>    }<br/><br/>}")]
        public bool busy { get; private set; }

        [AutoDoc("Returns a reference to the Stats Database in the scene.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        if(source.database != null)<br/>        {<br/>            Debug.Log(\"Known Attributes: \" + source.database.attributes.Count);<br/>        }<br/>        else<br/>        {<br/>            Debug.Log(\"No database present in scene\");<br/>        }<br/>    }<br/><br/>}")]
        public StatsDatabase database
        {
            get
            {
                if (db == null)
                {
                    db = ToolRegistry.GetComponent<StatsDatabase>();
                }

                return db;
            }
        }

        [AutoDoc("Returns a reference to the Global Stats in the scene.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        if(source.globalStats != null)<br/>        {<br/>            Debug.Log(\"Global Stats: \" + source.globalStats.stats.Count);<br/>        }<br/>        else<br/>        {<br/>            Debug.Log(\"No Global Stats present in scene\");<br/>        }<br/>    }<br/><br/>}")]
        public GlobalStats globalStats { get; private set; }

        [AutoDoc("True if object has been initialized.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        Debug.Log(\"Ready: \" + source.initialized);<br/>    }<br/><br/>}")]
        public bool initialized { get; private set; }

        [AutoDoc("Returns a list of registered IUniquelyIdentifiable sources.", "using NullSave.GDTK;<br/>using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        IUniquelyIdentifiable uid = source.registeredSources[0];<br/>        source.RemoveAttributesFromSource(uid);<br/>    }<br/><br/>}")]
        public IReadOnlyList<IUniquelyIdentifiable> registeredSources
        {
            get => source.registeredSources;
        }

        public StatSource source
        {
            get { return statSource; }
        }

        public IReadOnlyList<string> startingAttributeIds { get { return m_attributeIds; } }

        public IReadOnlyList<string> startingConditionIds { get { return m_conditionIds; } }

        [AutoDoc("Returns a list of initialized stats.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        Debug.Log(\"Stats: \" + source.stats.Count);<br/>    }<br/><br/>}")]
        public IReadOnlyDictionary<string, GDTKStat> stats
        {
            get
            {
                return statSource.stats;
            }
        }

        [AutoDoc("Returns a read-only list of initialized Status Conditions.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        // Manually subscribe to the first condition<br/>        source.statusConditions[0].onActivated += StatusConditionActivated;<br/>    }<br/><br/>    private void StatusConditionActivated(GDTKStatusCondition source)<br/>    {<br/>        Debug.Log(source.info.title + \" has been activated.\");<br/>    }<br/><br/>}")]
        public IReadOnlyList<GDTKStatusCondition> statusConditions
        {
            get { return m_statusConditions; }
        }

        #endregion

        #region Unity Methods

        [AutoDocSuppress]
        public virtual void Awake()
        {
            globalStats = ToolRegistry.GetComponent<GlobalStats>();

            if (register)
            {
                ToolRegistry.RegisterComponent(this, registryKey);
            }

            Broadcaster.SubscribeToPublic(this);

            m_statusConditions = new List<GDTKStatusCondition>();
            m_attributes = new List<GDTKAttribute>();
            attribSubs = new Dictionary<string, List<SimpleEvent>>();

            source.onStatsReloaded += () => onStatsReloaded?.Invoke();

            Initialize(true);
        }

        [AutoDocSuppress]
        public virtual void Reset() { }

        [AutoDocSuppress]
        public virtual void Update()
        {
            source.RaiseHeartbeat(Time.deltaTime);
            onHeartbeat?.Invoke(Time.deltaTime);
        }

        #endregion

        #region Public Methods

        [AutoDoc("Add an Attribute to the object.", "using NullSave.GDTK;<br/>using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour, IUniquelyIdentifiable<br/>{<br/><br/>    // Implment the IUniquelyIdentifiable interface to call RemoveXBySource<br/>    public string instanceId<br/>    {<br/>        get { return \"exampleInstance\"; }<br/>    }<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        source.AddAttribute(source.database.attributes[0].info.id, this);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of the attribute to add")]
        [AutoDocParameter("Object adding this Attribute")]
        public GDTKAttribute AddAttribute(string id, IUniquelyIdentifiable source = null)
        {
            RegisterSource(source);

            foreach (GDTKAttribute value in database.attributes)
            {
                if (value.info.id == id)
                {
                    GDTKAttribute clone = value.Clone();
                    clone.sourceId = source?.instanceId;
                    m_attributes.Add(clone);
                    RaiseAttributeChange(id);
                    onAttributeAdded?.Invoke(clone);
                    return clone;
                }
            }

            return null;
        }

        [AutoDoc("Adds a new Stat to the object. Cannot be done while object is initialized.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        // We cannot add stats while initialized<br/>        if(source.initialized)<br/>        {<br/>            source.Shutdown(true);<br/>        }<br/><br/>        // Create Stat<br/>        GDTKStat stat = new GDTKStat();<br/>        stat.expressions.minimum.valueExpression = \"0\";<br/>        stat.expressions.maximum.valueExpression = \"10 + 1\";<br/>        stat.expressions.value.valueExpression = \"rnd_i[11]\";<br/><br/>        // Add Stat<br/>        source.AddStat(stat);<br/><br/>        // Re-initialize system<br/>        source.Initialize(true);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Stat to add to object")]
        public virtual void AddStat(GDTKStat stat)
        {
            if (initialized)
            {
                StringExtensions.LogError(name, "AddStat", "Cannot modify stats while initialized, call Shutdown() first");
                return;
            }
            m_stats.Add(stat);
        }

        [AutoDoc("Adds a new Stat Modifier to the object.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        // Create a modifier that applies 1 time immediately<br/>        // Modifier changes the value of a Stat with the Id<br/>        // of 'HP' by adding 2<br/>        GDTKStatModifier modifier = new GDTKStatModifier();<br/>        modifier.target = ModifierTarget.Value;<br/>        modifier.applies = ModifierApplication.Immediately;<br/>        modifier.affectsStatId = \"HP\";<br/>        modifier.changeType = ModifierChangeType.Add;<br/>        modifier.value.valueExpression = \"2\";<br/><br/>        source.AddStatModifier(modifier);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Modifier to add to the object")]
        [AutoDocParameter("Object adding this Stat Modifier")]
        [AutoDocParameter("Stats associated with the Source, only required if the Source is outside this object **and** values on that object are referenced by the Stat Modifier.")]
        public virtual bool AddStatModifier(GDTKStatModifier modifier, IUniquelyIdentifiable source = null, StatSource other = null)
        {
            return this.source.AddStatModifier(modifier, source, other);
        }

        [AutoDoc("Adds a Status Condition to the object.", "using NullSave.GDTK;<br/>using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour, IUniquelyIdentifiable<br/>{<br/><br/>    // Implment the IUniquelyIdentifiable interface to call RemoveXBySource<br/>    public string instanceId<br/>    {<br/>        get<br/>        {<br/>            return \"exampleInstance\";<br/>        }<br/>    }<br/><br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        source.AddStatusCondition(source.database.statusConditions[0].info.id, this);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of the Status Condition to add")]
        [AutoDocParameter("Object adding this Status Condition")]
        public GDTKStatusCondition AddStatusCondition(string id, IUniquelyIdentifiable source = null)
        {
            RegisterSource(source);

            foreach (GDTKStatusCondition value in database.statusConditions)
            {
                if (value.info.id == id)
                {
                    GDTKStatusCondition clone = value.Clone();
                    clone.sourceId = source?.instanceId;
                    m_statusConditions.Add(clone);
                    if (initialized)
                    {
                        clone.Initialize(this);
                    }
                    onStatusConditionAdded?.Invoke(clone);
                    return clone;
                }
            }

            return null;
        }

        [AutoDoc("Removes all Identifiable Sources. Cannot be done while initialized.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        // We cannot clear sources while initialized<br/>        if(source.initialized)<br/>        {<br/>            source.Shutdown(true);<br/>        }<br/><br/>        source.ClearSources();<br/><br/>        // Re-initialize<br/>        source.Initialize(true);<br/>    }<br/><br/>}")]
        public virtual void ClearSources()
        {
            statSource.ClearSources();
        }

        [AutoDoc("Removes all Stats. Cannot be done while initialized.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        // We cannot clear sources while initialized<br/>        if(source.initialized)<br/>        {<br/>            source.Shutdown(true);<br/>        }<br/><br/>        source.ClearStats();<br/><br/>        // Re-initialize<br/>        source.Initialize(true);<br/>    }<br/><br/>}")]
        public virtual void ClearStats()
        {
            statSource.ClearStats();
        }

        [AutoDoc("Loads data from a specified file", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        if (!source.busy)<br/>        {<br/>            source.DataLoad(\"test.sav\");<br/>        }<br/>    }<br/><br/>}")]
        [AutoDocParameter("Name of file to load. Application.persistentDataPath is automatically prepended.")]
        public virtual void DataLoad(string filename)
        {
            string path = Path.Combine(Application.persistentDataPath, filename);
            if (!File.Exists(path)) return;

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                DataLoad(fs);
            }
        }

        [AutoDoc("Loads data to a specified stream", "using NullSave.GDTK.Stats;<br/>using System.IO;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        // LOad data<br/>        using (FileStream fs = new FileStream(Path.Combine(Application.persistentDataPath, \"example.sav\"), FileMode.OpenOrCreate, FileAccess.Read))<br/>        {<br/>            source.DataLoad(fs);<br/>        }<br/><br/>    }<br/><br/>}")]
        [AutoDocParameter("Stream used to load data.")]
        public virtual int DataLoad(Stream stream)
        {
            if (busy) return -1;

            Data.BasicStatsData data = new Data.BasicStatsData(stream);
            LoadData(data);

            busy = false;

            return data.version;
        }

        public virtual void DataLoadJSON(string json)
        {
            Data.BasicStatsData data = SimpleJson.FromJSON<Data.BasicStatsData>(json);
            LoadData(data);
        }

        public virtual void DataLoadJSONFile(string filename)
        {
            DataLoadJSON(File.ReadAllText(Path.Combine(Application.persistentDataPath, filename)));
        }

        [AutoDoc("Saves data to a specified file", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        if (!source.busy)<br/>        {<br/>            source.DataSave(\"test.sav\");<br/>        }<br/>    }<br/><br/>}")]
        [AutoDocParameter("Name of file to create/update. Application.persistentDataPath is automatically prepended.")]
        public virtual void DataSave(string filename)
        {
            using (FileStream fs = new FileStream(Path.Combine(Application.persistentDataPath, filename), FileMode.OpenOrCreate, FileAccess.Write))
            {
                DataSave(fs);
            }
        }

        [AutoDoc("Saves data to a stream.", "using NullSave.GDTK.Stats;<br/>using System.IO;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        // Save data<br/>        using (FileStream fs = new FileStream(Path.Combine(Application.persistentDataPath, \"example.sav\"), FileMode.OpenOrCreate, FileAccess.Write))<br/>        {<br/>            source.DataSave(fs);<br/>        }<br/><br/>    }<br/><br/>}")]
        [AutoDocParameter("Stream used to save data")]
        public virtual void DataSave(Stream stream)
        {
            if (busy) return;

            GenerateData().Write(stream, STATS_FILE_VERSION);
        }

        public virtual string DataSaveJSON(bool removeNulls = true, bool readable = false)
        {
            Data.BasicStatsData data = GenerateData();
            return SimpleJson.ToJSON(data, removeNulls, readable);
        }

        public virtual void DataSaveJSON(string filename)
        {
            using (FileStream fs = new FileStream(Path.Combine(Application.persistentDataPath, filename), FileMode.Create, FileAccess.Write))
            {
                fs.WriteString(DataSaveJSON());
            }
        }

        /// <summary>
        /// Get a plugin of specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T GetPlugin<T>()
        {
            foreach (var wrapper in plugins)
            {
                if (wrapper.plugin is T typedValue)
                {
                    return typedValue;
                }
            }

            return default;
        }

        [AutoDoc("Returns an Identifiable Source by id", "using NullSave.GDTK;<br/>using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        string id = \"exampleInstance\";<br/>        IUniquelyIdentifiable identifiableSource = source.GetSource(id);<br/>        source.RemoveAttributesFromSource(identifiableSource);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id to find")]
        public virtual IUniquelyIdentifiable GetSource(string uniqueId)
        {
            return source.GetSource(uniqueId);
        }

        [AutoDoc("Get a Stat by id.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        GDTKStat stat = source.GetStat(\"HP\");<br/>        stat.value += 2;<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of the Stat to find")]
        public virtual GDTKStat GetStat(string statId)
        {
            return statSource.GetStat(statId);
        }

        [AutoDoc("Get the value of a formula/expression.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        // Get the maximum value of a stat name 'HP' and add 3<br/>        float value = source.GetValue(\"HP:max + 3\");<br/>    }<br/><br/>}")]
        [AutoDocParameter("Formula used to get value")]
        [AutoDocParameter("Source used when `other:` is requested")]
        public virtual float GetValue(string formula, StatSource other = null)
        {
            return statSource.GetValue(formula, other);
        }

        [AutoDoc("True if object has an Attribute with the provided id.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        if(source.HasAttribute(\"testAttribute\"))<br/>        {<br/>            Debug.Log(\"Attribute found\");<br/>        }<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of the Attribute to check for")]
        public bool HasAttribute(string id)
        {
            foreach (GDTKAttribute attrib in attributes)
            {
                if (attrib.info.id == id) return true;
            }

            return false;
        }

        [AutoDoc("Initialize the object. This is done automatically on Awake.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        // Re-initialize object<br/>        source.Initialize(true);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Reset the values of any previously initialized Stats.")]
        public virtual void Initialize(bool reset)
        {
            if (reset)
            {
                initialized = false;
            }
            else if (initialized)
            {
                return;
            }

            // Add Actions
            source.RegisterAction("subscribeToAttributeChange", Source_SubscribeToAttributeChange);
            source.RegisterAction("hasAttribute", Source_HasAttribute);

            // Initialize stat source
            if (m_stats.Count > 0)
            {
                if(reset)
                {
                    statSource.Shutdown();
                }
                statSource.Initialize(m_stats);
            }
            else
            {
                statSource.Initialize(reset);
            }

            // Initialize entries
            foreach (string attribute in m_attributeIds)
            {
                AddAttribute(attribute, null);
            }

            foreach (string condition in m_conditionIds)
            {
                AddStatusCondition(condition, null);
            }

            foreach (GDTKStatusCondition conditon in m_statusConditions)
            {
                conditon.Initialize(this);
            }

            foreach (UniversalPluginWrapper<StatsPlugin> wrapper in plugins)
            {
                wrapper.plugin.Initialize(this);
            }

            initialized = true;
        }

        [AutoDoc("Check if an expression resolves to true.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        string checkValue = \"HP > 0\";<br/>        Debug.Log(\"Condition: \" + source.IsConditionTrue(checkValue));<br/>    }<br/><br/>}")]
        [AutoDocParameter("Condition to validate")]
        [AutoDocParameter("Source used when `other:` is requested")]
        public virtual bool IsConditionTrue(string condition, StatSource other = null)
        {
            return GDTKStatsManager.IsConditionTrue(condition, statSource, other);
        }

        [AutoDoc("Export all data to json", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        string json = source.JSONExport();<br/>    }<br/><br/>}")]
        public virtual string JSONExport()
        {
            jsonStatsList result = new jsonStatsList();

            result.attributes = m_attributeIds.ToList();
            result.conditions = m_conditionIds.ToList();
            foreach (GDTKStat val in m_stats) result.stats.Add(val);

            return SimpleJson.ToJSON(result);
        }

        public string JSONExport(List<int> statIndexes, List<int> conditionIndexes, List<int> attributeIndexes)
        {
            jsonStatsList result = new jsonStatsList();

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

            return SimpleJson.ToJSON(result);
        }

        [AutoDoc("Import json data", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        string json = source.JSONExport();<br/>        source.JSONImport(json, true);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Data to import")]
        [AutoDocParameter("Clear all existing data before importing")]
        public virtual void JSONImport(string json, bool clearExisting)
        {
            JSONImport(SimpleJson.FromJSON<jsonStatsList>(json), clearExisting);
        }

        public void JSONImport(jsonStatsList source, bool clearExisting)
        {
            if (clearExisting) m_stats.Clear();

            m_attributeIds = source.attributes.ToList();
            m_conditionIds = source.conditions.ToList();
            foreach (GDTKStat val in source.stats) m_stats.Add(val);
        }

        public void JSONImportSmart(jsonStatsList source)
        {
            foreach (string id in source.attributes) if (!m_attributeIds.Contains(id)) m_attributeIds.Add(id);
            foreach (string id in source.conditions) if (!m_conditionIds.Contains(id)) m_conditionIds.Add(id);

            int index;
            foreach (GDTKStat val in source.stats)
            {
                index = UninitializedStatIndex(val);
                if (index < 0)
                {
                    m_stats.Add(val);
                }
                else
                {
                    m_stats[index] = val;
                }
            }
        }

        [AutoDoc("Raises a Stat Event by id", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        string id = source.database.events[0].info.id;<br/>        source.RaiseEvent(id);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of the event to raise")]
        public virtual void RaiseEvent(string eventId)
        {
            GDTKEvent se = database.GetEvent(eventId);
            if (se == null) return;

            foreach (GDTKStatModifier mod in se.statModifiers)
            {
                AddStatModifier(mod, null);
            }

            if (se.raiseTokenHeartbeat)
            {
                onTokenHeartbeat?.Invoke(se.tokens);
            }

            onEventTriggered?.Invoke(se);
        }

        [AutoDoc("Raises a Token Heartbeat", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        source.RaiseTokenHeartbeat();<br/>    }<br/><br/>}")]
        [AutoDocParameter("Count to associate with heartbeat")]
        public virtual void RaiseTokenHeartbeat(float tokenCount = 1)
        {
            source.RaiseTokenHeartbeat(tokenCount);
            onTokenHeartbeat?.Invoke(tokenCount);
        }

        [AutoDoc("Manually add an IUniquelyIdentifiable source", "using NullSave.GDTK;<br/>using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour, IUniquelyIdentifiable<br/>{<br/>    public string instanceId<br/>    {<br/>        get { return \"exampleInstance\"; }<br/>    }<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        source.RegisterSource(this);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Source to add")]
        public virtual void RegisterSource(IUniquelyIdentifiable source)
        {
            this.source.RegisterSource(source);
        }

        [AutoDoc("Remove an Attribute by id", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        string id = source.database.attributes[0].info.id;<br/>        source.RemoveAttribute(id);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of Attribute to remove")]
        public virtual void RemoveAttribute(string id)
        {
            foreach (GDTKAttribute attribute in m_attributes.ToArray())
            {
                if (attribute.info.id == id)
                {
                    m_attributes.Remove(attribute);
                    RaiseAttributeChange(id);
                    onAttributeRemoved?.Invoke(attribute);
                }
            }
        }

        [AutoDoc("Remove all Attributes from a specified source", "using NullSave.GDTK;<br/>using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        IUniquelyIdentifiable uid = source.registeredSources[0];<br/>        source.RemoveAttributesFromSource(uid);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Source associated with request")]
        public virtual void RemoveAttributesFromSource(IUniquelyIdentifiable source)
        {
            foreach (GDTKAttribute attribute in m_attributes.ToArray())
            {
                if (attribute.sourceId == source?.instanceId)
                {
                    m_attributes.Remove(attribute);
                    RaiseAttributeChange(attribute.info.id);
                    onAttributeRemoved?.Invoke(attribute);
                }
            }
        }

        [AutoDoc("Remove a IUniquelyIdentifiable source", "using NullSave.GDTK;<br/>using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour, IUniquelyIdentifiable<br/>{<br/>    public string instanceId<br/>    {<br/>        get { return \"exampleInstance\"; }<br/>    }<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        source.RemoveSource(this);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Source to be removed")]
        public virtual void RemoveSource(IUniquelyIdentifiable source)
        {
            this.source.RemoveSource(source);
        }

        [AutoDoc("Remove a IUniquelyIdentifiable source by Id", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        source.RemoveSource(\"exampleInstance\");<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of the source to remove")]
        public virtual void RemoveSource(string uniqueId)
        {
            source.RemoveSource(uniqueId);
        }

        [AutoDoc("Remove a Stat Modifier", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        // Create a modifier that applies 1 time immediately<br/>        // Modifier changes the value of a Stat with the Id<br/>        // of 'HP' by adding 2<br/>        GDTKStatModifier modifier = new GDTKStatModifier();<br/>        modifier.target = ModifierTarget.Value;<br/>        modifier.applies = ModifierApplication.Immediately;<br/>        modifier.affectsStatId = \"HP\";<br/>        modifier.changeType = ModifierChangeType.Add;<br/>        modifier.value.valueExpression = \"2\";<br/><br/>        source.AddStatModifier(modifier);<br/><br/>        source.RemoveStatModifier(modifier);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Stat Modifier to remove")]
        public virtual bool RemoveStatModifier(GDTKStatModifier modifier)
        {
            stats.TryGetValue(modifier.affectsStatId, out GDTKStat stat);

            if (stat == null)
            {
                StringExtensions.LogError(name, "RemoveStatModifier", "Stat cannot be found named " + modifier.affectsStatId);
            }
            else
            {
                stat.RemoveModifier(modifier);
                return true;
            }

            return false;
        }

        [AutoDoc("Remove all Stat Modifiers from a specified source", "using NullSave.GDTK;<br/>using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        IUniquelyIdentifiable uid = source.registeredSources[0];<br/>        source.RemoveStatModifiersFromSource(uid);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Source associated with request")]
        public virtual void RemoveStatModifiersFromSource(IUniquelyIdentifiable source)
        {
            foreach (var entry in statSource.stats)
            {
                entry.Value.RemoveModifierFromSource(source);
            }
        }

        [AutoDoc("Remove a Status Condition by Id", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        string id = source.database.statusConditions[0].info.id;<br/>        source.RemoveStatusCondition(id);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of the Status Condition to remove")]
        public virtual void RemoveStatusCondition(string conditionId)
        {
            foreach (GDTKStatusCondition condition in m_statusConditions.ToArray())
            {
                if (condition.info.id == conditionId)
                {
                    RemoveAttributesFromSource(condition);
                    m_statusConditions.Remove(condition);
                    RemoveSource(condition);
                    onStatusConditionRemoved?.Invoke(condition);
                }
            }
        }

        [AutoDoc("Remove all Status Conditions from a specified source", "using NullSave.GDTK;<br/>using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        IUniquelyIdentifiable uid = source.registeredSources[0];<br/>        source.RemoveStatusConditionsFromSource(uid);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Source associated with request")]
        public virtual void RemoveStatusConditionsFromSource(IUniquelyIdentifiable source)
        {
            foreach (GDTKStatusCondition condition in m_statusConditions.ToArray())
            {
                if (condition.sourceId == source?.instanceId)
                {
                    RemoveAttributesFromSource(condition);
                    m_statusConditions.Remove(condition);
                    RemoveSource(condition);
                    onStatusConditionRemoved?.Invoke(condition);
                }
            }
        }

        [AutoDoc("Shutodwn the system", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        source.Shutdown(true);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Clear all Status, Conditions, Source, and subscriptions")]
        public virtual void Shutdown(bool fullShutdown)
        {
            if (!initialized) return;

            statSource.Shutdown();

            for (int i = 0; i < m_statusConditions.Count; i++)
            {
                m_statusConditions[i].Shutdown();
            }
            m_statusConditions.Clear();

            m_attributes.Clear();

            foreach (UniversalPluginWrapper<StatsPlugin> wrapper in plugins)
            {
                wrapper.plugin.Shutdown();
            }

            if (fullShutdown)
            {
                foreach (var d in onEventTriggered.GetInvocationList())
                {
                    onEventTriggered -= (EventTrigger)d;
                }

                foreach (var d in onAttributeAdded.GetInvocationList())
                {
                    onAttributeAdded -= (AttributeEvent)d;
                }

                foreach (var d in onAttributeRemoved.GetInvocationList())
                {
                    onAttributeRemoved -= (AttributeEvent)d;
                }

                foreach (var d in onStatusConditionAdded.GetInvocationList())
                {
                    onStatusConditionAdded -= (StatusConditionEvent)d;
                }

                foreach (var d in onStatusConditionRemoved.GetInvocationList())
                {
                    onStatusConditionRemoved -= (StatusConditionEvent)d;
                }

                foreach (var d in onStatsReloaded.GetInvocationList())
                {
                    onStatsReloaded -= (SimpleEvent)d;
                }

                foreach (var d in onHeartbeat.GetInvocationList())
                {
                    onHeartbeat -= (HeartbeatEvent)d;
                }

                foreach (var d in onTokenHeartbeat.GetInvocationList())
                {
                    onTokenHeartbeat -= (HeartbeatEvent)d;
                }
            }

            initialized = false;
        }

        [AutoDoc("Subscribe to Attribute changes", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        string id = source.database.attributes[0].info.id;<br/>        source.SubscribeToAttributeChange(AttributeChange, id);<br/>    }<br/><br/>    private void AttributeChange()<br/>    {<br/>        Debug.Log(\"Attribute has been added or removed.\");<br/>    }<br/><br/>}")]
        [AutoDocParameter("Handler to invoke on change")]
        [AutoDocParameter("Id of the attribute to subscribe to")]
        public virtual void SubscribeToAttributeChange(SimpleEvent handler, string attributeId)
        {
            List<SimpleEvent> triggers;

            if (!attribSubs.ContainsKey(attributeId))
            {
                triggers = new List<SimpleEvent>();
                triggers.Add(handler);
                attribSubs.Add(attributeId, triggers);
                return;
            }

            triggers = attribSubs[attributeId];
            if (!triggers.Contains(handler))
            {
                triggers.Add(handler);
            }
        }

        [AutoDoc("Subscribe to Stat Events", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        source.SubscribeToEventTriggers(StatEventTriggered);<br/>    }<br/><br/>    private void StatEventTriggered(GDTKEvent statEvent)<br/>    {<br/>        Debug.Log(statEvent.info.id + \" raised\");<br/>    }<br/><br/>}")]
        [AutoDocParameter("Handler to invoke when Stat Events occur")]
        public virtual void SubscribeToEventTriggers(EventTrigger handler)
        {
            onEventTriggered += handler;
        }

        [AutoDoc("Subscribe to Heartbeats", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        source.SubscribeToHeartbeat(Heartbeat);<br/>    }<br/><br/>    public void Unsubscribe(BasicStats source)<br/>    {<br/>        source.UnsubscribeFromHeartbeat(Heartbeat);<br/>    }<br/><br/>    public void Heartbeat(float time)<br/>    {<br/>        Debug.Log(\"Heatbeat: \" + time);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Handler to invoke each Heartbeat")]
        public virtual void SubscribeToHeartbeat(HeartbeatEvent handler)
        {
            statSource.SubscribeToHeartbeat(handler);
        }

        [AutoDoc("Subscribe to Token Heartbeats", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        source.SubscribeToTokenHeartbeat(Heartbeat);<br/>    }<br/><br/>    public void Unsubscribe(BasicStats source)<br/>    {<br/>        source.SubscribeToTokenHeartbeat(Heartbeat);<br/>    }<br/><br/>    public void Heartbeat(float time)<br/>    {<br/>        Debug.Log(\"TokenHeatbeat: \" + time);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Handler to invoke each TokenHeartbeat")]
        public virtual void SubscribeToTokenHeartbeat(HeartbeatEvent handler)
        {
            statSource.SubscribeToTokenHeartbeat(handler);
        }

        [AutoDoc("Remove subscription from Attribute changes", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        string id = source.database.attributes[0].info.id;<br/>        source.UnsubscribeFromAttributeChange(AttributeChange, id);<br/>    }<br/><br/>    private void AttributeChange()<br/>    {<br/>        Debug.Log(\"Attribute has been added or removed.\");<br/>    }<br/><br/>}")]
        [AutoDocParameter("Handler to remove")]
        [AutoDocParameter("Id of attribute")]
        public virtual void UnsubscribeFromAttributeChange(SimpleEvent handler, string attributeId)
        {
            List<SimpleEvent> triggers;

            if (!attribSubs.ContainsKey(attributeId))
            {
                return;
            }

            triggers = attribSubs[attributeId];
            triggers.Remove(handler);
            if (triggers.Count == 0)
            {
                attribSubs.Remove(attributeId);
            }
        }

        [AutoDoc("Remove subscription from Stat Events", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        source.SubscribeToEventTriggers(StatEventTriggered);<br/>    }<br/><br/>    public void Unsubscribe(BasicStats source)<br/>    {<br/>        source.UnsubscribeFromEventTriggers(StatEventTriggered);<br/>    }<br/><br/>    private void StatEventTriggered(GDTKEvent statEvent)<br/>    {<br/>        Debug.Log(statEvent.info.id + \" raised\");<br/>    }<br/><br/>}")]
        [AutoDocParameter("Handler to remove")]
        public virtual void UnsubscribeFromEventTriggers(EventTrigger handler)
        {
            onEventTriggered -= handler;
        }

        [AutoDoc("Remove subscription from Heartbeats", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        source.SubscribeToHeartbeat(Heartbeat);<br/>    }<br/><br/>    public void Unsubscribe(BasicStats source)<br/>    {<br/>        source.UnsubscribeFromHeartbeat(Heartbeat);<br/>    }<br/><br/>    public void Heartbeat(float time)<br/>    {<br/>        Debug.Log(\"Heatbeat: \" + time);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Handler to remove")]
        public virtual void UnsubscribeFromHeartbeat(HeartbeatEvent handler)
        {
            statSource.UnsubscribeFromHeartbeat(handler);
        }

        [AutoDoc("Remove subscription from TokenHeartbeats", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(BasicStats source)<br/>    {<br/>        source.SubscribeToTokenHeartbeat(Heartbeat);<br/>    }<br/><br/>    public void Unsubscribe(BasicStats source)<br/>    {<br/>        source.SubscribeToTokenHeartbeat(Heartbeat);<br/>    }<br/><br/>    public void Heartbeat(float time)<br/>    {<br/>        Debug.Log(\"TokenHeatbeat: \" + time);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Handler to remove")]
        public virtual void UnsubscribeFromTokenHeartbeat(HeartbeatEvent handler)
        {
            statSource.UnsubscribeFromTokenHeartbeat(handler);
        }

        #endregion

        #region Broadcast Methods

        [AutoDocSuppress]
        public virtual void BroadcastReceived(object sender, string channel, string message, object[] args) { }

        [AutoDocSuppress]
        public virtual void PublicBroadcastReceived(object sender, string message)
        {
            StatsDebugManager.BasicStatsRequest(this, gameObject, message);
        }

        #endregion

        #region Private Methods

        internal virtual Data.BasicStatsData GenerateData()
        {
            Data.BasicStatsData result = new Data.BasicStatsData
            {
                stats = new List<GDTKStat>(),
                statusConditions = m_statusConditions.ToList(),
                attributes = m_attributes.ToList(),
                plugins = plugins.ToList()
            };

            foreach(var entry in statSource.stats)
            {
                result.stats.Add(entry.Value);
            }

            return result;
        }

        internal virtual void LoadData(Data.BasicStatsData data)
        {
            busy = true;
            initialized = true;

            Shutdown(false);
            statSource.LoadData(data);

            foreach(GDTKStatusCondition condition in data.statusConditions)
            {
                condition.FinalizeLoading(this);
                m_statusConditions.Add(condition);
            }

            foreach(GDTKAttribute attribute in data.attributes)
            {
                m_attributes.Add(attribute);
                onAttributeAdded?.Invoke(attribute);
            }

            plugins = data.plugins.ToList();

            busy = false;
        }

        private void RaiseAttributeChange(string attributeId)
        {
            if (!attribSubs.ContainsKey(attributeId)) return;

            foreach (SimpleEvent target in attribSubs[attributeId])
            {
                target?.Invoke();
            }
        }

        private void Source_HasAttribute(object[] args, ref object result)
        {
            result = HasAttribute((string)args[0]);
        }

        private void Source_SubscribeToAttributeChange(object[] args, ref object result)
        {
            SubscribeToAttributeChange((SimpleEvent)args[0], (string)args[1]);
            result = null;
        }

        private int UninitializedStatIndex(GDTKStat stat)
        {
            for (int i = 0; i < m_stats.Count; i++)
            {
                if (m_stats[i].info.id == stat.info.id)
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion

    }
}
#endif
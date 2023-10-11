#if GDTK
using NullSave.GDTK.JSON;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [ExecuteInEditMode]
    [DefaultExecutionOrder(-599)]
    [AutoDoc("The Stats Database component provides a list of all objects that can be used by various Stat Sources. This way they can be reused and quickly updated by AssetBundles or Json updates.")]
    public class StatsDatabase : MonoBehaviour, ISerializationCallbackReceiver
    {

        #region Fields

        [Tooltip("Sets where to load the data from")] public DataSource dataSource;
        [Tooltip("Name of the AssetBundle to load from")] public string bundleName;
        [Tooltip("Name of the Asset in the AssetBundle to load")] public string assetName;
        [Tooltip("Relative path to asset in Resources")] public string path;
        [Tooltip("Load data on awake")] public bool loadOnAwake;


        [System.NonSerialized] public List<ActionSequenceList> m_actionSequences;
        [SerializeField] [Tooltip("List of available Action Sequences")] private List<jsonActionSequenceList> jsonActionSequences;
        [Tooltip("List of available Attributes")] public List<GDTKAttribute> attributes;
        [Tooltip("List of available Backgrounds")] public List<GDTKBackground> backgrounds;
        [Tooltip("List of available Classes")] public List<GDTKClass> classes;
        [Tooltip("List of available Status Effects")] public List<GDTKStatusEffect> effects;
        [Tooltip("List of available Stat Events")] public List<GDTKEvent> events;
        [Tooltip("LIst of available Languages")] public List<GDTKLanguage> languages;
        [Tooltip("List of available Perks")] public List<GDTKPerk> perks;
        [Tooltip("List of available Races")] public List<GDTKRace> races;
        [Tooltip("List of available Status Conditions")] public List<GDTKStatusCondition> statusConditions;
        [Tooltip("List of available Level Rewards")] public List<GDTKLevelReward> levelRewards;
        [Tooltip("List of available Custom Rewards")] public List<GDTKLevelReward> customRewards;

        private bool needsRefresh;

        #endregion

        #region Properties

        [AutoDoc("Returns a list of all available Action Sequences", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod()<br/>    {<br/>        int count = StatsDatabase.instance.actionSequences.Count;<br/>        Debug.Log(\"Available Action Sequences: \" + count);<br/>    }<br/><br/>}")]
        public List<ActionSequenceList> actionSequences
        {
            get
            {
                if (m_actionSequences == null || needsRefresh)
                {
                    m_actionSequences = new List<ActionSequenceList>();
                    foreach (jsonActionSequenceList sequenceList in jsonActionSequences)
                    {
                        m_actionSequences.Add(ActionSequenceList.FromJSON(sequenceList));
                    }

                    needsRefresh = false;
                }

                return m_actionSequences;
            }
            private set
            {
                needsRefresh = false;

                m_actionSequences = value;
                jsonActionSequences.Clear();
                foreach (ActionSequenceList val in value)
                {
                    jsonActionSequences.Add(val.ToJSON());
                }
            }
        }

        [AutoDoc("Returns a reference to the current instance", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod()<br/>    {<br/>        int count = StatsDatabase.instance.actionSequences.Count;<br/>        Debug.Log(\"Available Action Sequences: \" + count);<br/>    }<br/><br/>}")]
        public static StatsDatabase instance { get; private set; }

        [AutoDoc("True if the database has been loaded", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod()<br/>    {<br/>        if(!StatsDatabase.instance.isLoaded)<br/>        {<br/>            StatsDatabase.instance.Load();<br/>        }<br/>    }<br/><br/>}")]
        public bool isLoaded { get; private set; }

        #endregion

        #region Unity Methods

        [AutoDocSuppress]
        public void Awake()
        {
            if (instance == null)
            {
                instance = this;
                ToolRegistry.RegisterComponent(this);

                if (Application.isPlaying)
                {
                    transform.SetParent(null);
                    DontDestroyOnLoad(gameObject);
                }

                if (dataSource == DataSource.Local)
                {
                    isLoaded = true;
                }
                else if (loadOnAwake)
                {
                    Load();
                }
            }
            else if (instance != this && Application.isPlaying)
            {
                StringExtensions.Log(name, "StatsDatabase", "An instance already exists, removing extra instance");
                InterfaceManager.ObjectManagement.DestroyObject(gameObject);
            }
        }

        private void OnDestroy()
        {
            ToolRegistry.RemoveComponent(this);
        }

        private void Reset()
        {
            loadOnAwake = true;
            m_actionSequences = new List<ActionSequenceList>();
            jsonActionSequences = new List<jsonActionSequenceList>();
            attributes = new List<GDTKAttribute>();
            backgrounds = new List<GDTKBackground>();
            classes = new List<GDTKClass>();
            effects = new List<GDTKStatusEffect>();
            events = new List<GDTKEvent>();
            languages = new List<GDTKLanguage>();
            perks = new List<GDTKPerk>();
            races = new List<GDTKRace>();
            statusConditions = new List<GDTKStatusCondition>();
            levelRewards = new List<GDTKLevelReward>();
        }

        #endregion

        #region Public Methods

        [AutoDoc("Get an Action Sequence by Id", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod()<br/>    {<br/>        Debug.Log(StatsDatabase.instance.GetActionSequence(\"exampleId\"));<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of object to retrieve")]
        public ActionSequenceList GetActionSequence(string id)
        {
            foreach (ActionSequenceList list in actionSequences)
            {
                if (list.id == id)
                {
                    return list;
                }
            }

            return null;
        }

        [AutoDoc("Get an Attribute by Id", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod()<br/>    {<br/>        Debug.Log(StatsDatabase.instance.GetAttribute(\"exampleId\"));<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of object to retrieve")]
        public GDTKAttribute GetAttribute(string id)
        {
            foreach (GDTKAttribute value in attributes)
            {
                if (value.info.id == id)
                {
                    return value;
                }
            }

            return null;
        }

        [AutoDoc("Get a Background by Id", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod()<br/>    {<br/>        Debug.Log(StatsDatabase.instance.GetBackground(\"exampleId\"));<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of object to retrieve")]
        public GDTKBackground GetBackground(string id)
        {
            foreach (GDTKBackground value in backgrounds)
            {
                if (value.info.id == id)
                {
                    return value;
                }
            }

            return null;
        }

        [AutoDoc("Get a Class by Id", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod()<br/>    {<br/>        Debug.Log(StatsDatabase.instance.GetClass(\"exampleId\"));<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of object to retrieve")]
        public GDTKClass GetClass(string id)
        {
            foreach (GDTKClass value in classes)
            {
                if (value.info.id == id)
                {
                    return value;
                }
            }

            return null;
        }

        [AutoDoc("Get a Status Effect by Id", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod()<br/>    {<br/>        Debug.Log(StatsDatabase.instance.GetEffect(\"exampleId\"));<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of object to retrieve")]
        public GDTKStatusEffect GetEffect(string id)
        {
            foreach (GDTKStatusEffect effect in effects)
            {
                if (effect.info.id == id)
                {
                    return effect;
                }
            }

            return null;
        }

        [AutoDoc("Get a Stat Event by Id", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod()<br/>    {<br/>        Debug.Log(StatsDatabase.instance.GetEvent(\"exampleId\"));<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of object to retrieve")]
        public GDTKEvent GetEvent(string id)
        {
            foreach (GDTKEvent value in events)
            {
                if (value.info.id == id)
                {
                    return value;
                }
            }

            return null;
        }

        [AutoDoc("Get a Language by Id", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod()<br/>    {<br/>        Debug.Log(StatsDatabase.instance.GetLanguage(\"exampleId\"));<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of object to retrieve")]
        public GDTKLanguage GetLanguage(string id)
        {
            foreach (GDTKLanguage language in languages)
            {
                if (language.info.id == id)
                {
                    return language;
                }
            }

            return null;
        }

        [AutoDoc("Get a Perk by Id", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod()<br/>    {<br/>        Debug.Log(StatsDatabase.instance.GetPerk(\"exampleId\"));<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of object to retrieve")]
        public GDTKPerk GetPerk(string id)
        {
            foreach (GDTKPerk value in perks)
            {
                if (value.info.id == id)
                {
                    return value;
                }
            }

            return null;
        }

        [AutoDoc("Get a Race by Id", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod()<br/>    {<br/>        Debug.Log(StatsDatabase.instance.GetRace(\"exampleId\"));<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of object to retrieve")]
        public GDTKRace GetRace(string id)
        {
            foreach (GDTKRace value in races)
            {
                if (value.info.id == id)
                {
                    GDTKRace race = value.Clone();

                    if (!string.IsNullOrEmpty(value.parentId) && value.parentId != race.info.id)
                    {
                        GDTKRace parent = GetRace(value.parentId);
                        if (parent != null)
                        {
                            foreach (GDTKTrait trait in parent.traits)
                            {
                                race.traits.Add(trait.Clone());
                            }
                        }
                    }

                    return race;
                }
            }

            return null;
        }

        [AutoDoc("Export all data to json", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(StatsDatabase source)<br/>    {<br/>        string json = source.JSONExport();<br/>    }<br/><br/>}")]
        public string JSONExport()
        {
            jsonStatsDatabase json = new jsonStatsDatabase();

            json.dataSource = dataSource;
            json.bundleName = bundleName;
            json.assetName = assetName;
            json.path = path;
            json.loadOnAwake = loadOnAwake;

            json.actionSequences = actionSequences.ToList();
            json.attributes = attributes.ToList();
            json.backgrounds = backgrounds.ToList();
            json.classes = classes.ToList();
            json.effects = effects.ToList();
            json.events = events.ToList();
            json.perks = perks.ToList();
            json.races = races.ToList();
            json.statusConditions = statusConditions.ToList();
            json.levelRewards = levelRewards.ToList();
            json.customRewards = customRewards.ToList();

            return SimpleJson.ToJSON(json);
        }

        [AutoDoc("Import json data", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(StatsDatabase source)<br/>    {<br/>        string json = source.JSONExport();<br/>        source.JSONImport(json, true);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Data to import")]
        [AutoDocParameter("Clear all existing data before importing")]
        public virtual void JSONImport(string json, bool clearExisting)
        {
            if (clearExisting)
            {
                m_actionSequences = null;
                jsonActionSequences.Clear();
                attributes.Clear();
                backgrounds.Clear();
                classes.Clear();
                effects.Clear();
                events.Clear();
                languages.Clear();
                perks.Clear();
                races.Clear();
                statusConditions.Clear();
                levelRewards.Clear();
            }

            jsonStatsDatabase source = SimpleJson.FromJSON<jsonStatsDatabase>(json);

            dataSource = source.dataSource;
            bundleName = source.bundleName;
            assetName = source.assetName;
            path = source.path;
            loadOnAwake = source.loadOnAwake;

            actionSequences = source.actionSequences.ToList();
            attributes = source.attributes.ToList();
            backgrounds = source.backgrounds.ToList();
            classes = source.classes.ToList();
            effects = source.effects.ToList();
            events = source.events.ToList();
            perks = source.perks.ToList();
            races = source.races.ToList();
            statusConditions = source.statusConditions.ToList();
            levelRewards = source.levelRewards.ToList();
            customRewards = source.customRewards.ToList();
        }

        [AutoDoc("Load data from source", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod()<br/>    {<br/>        if(!StatsDatabase.instance.isLoaded)<br/>        {<br/>            StatsDatabase.instance.Load();<br/>        }<br/>    }<br/><br/>}")]
        public void Load()
        {
            switch (dataSource)
            {
                case DataSource.AssetBundle:
                    InternalLoad(GetBundle().LoadAsset<TextAsset>(assetName).text);
                    break;
                case DataSource.PersistentData:
                    InternalLoad(File.ReadAllText(Path.Combine(Application.persistentDataPath, path)));
                    break;
                case DataSource.Resources:
                    InternalLoad(Resources.Load<TextAsset>(path).text);
                    break;
            }
        }

        #endregion

        #region Private Methods

        private AssetBundle GetBundle()
        {
            foreach (AssetBundle bundle in AssetBundle.GetAllLoadedAssetBundles())
            {
                if (bundle.name == bundleName) return bundle;
            }

            return AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, path));
        }

        private void InternalLoad(string json)
        {



            isLoaded = true;
        }

        #endregion

        #region Serialization

        [AutoDocSuppress]
        public void OnAfterDeserialize()
        {
            needsRefresh = true;
        }

        [AutoDocSuppress]
        public void OnBeforeSerialize()
        {
            if (needsRefresh) return;
            jsonActionSequences = new List<jsonActionSequenceList>();
            foreach (ActionSequenceList sequenceList in actionSequences)
            {
                try
                {
                    // A plug-in can be deleted or fail compile
                    // So we need to catch for null returns
                    jsonActionSequences.Add(sequenceList.ToJSON());
                }
                catch { }
            }
        }

        #endregion

    }
}
#endif
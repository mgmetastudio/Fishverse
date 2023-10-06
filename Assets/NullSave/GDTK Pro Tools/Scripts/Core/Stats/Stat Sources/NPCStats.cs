#if GDTK
using NullSave.GDTK.JSON;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [AutoDoc("This component inherits from StatsAndEffects and languages & respawn ability to an object. It provides access to Stats, Attributes, Status Conditions, Stat Events, Status Effects, Languages, Respawn and Plugins.")]
    [DefaultExecutionOrder(-510)]
    public class NPCStats : StatsAndEffects
    {

        #region Fields

        private List<GDTKLanguage> m_languages;

        [Tooltip("Condition that must be true to trigger respawn")] public string respawnCondition;
        [Tooltip("Add position when saving stats")] public bool savePosition;
        [Tooltip("Add rotation when saving stats")] public bool saveRotation;
        [Tooltip("Stat Modifiers to apply on a respawn")] public List<GDTKStatModifier> respawnModifiers;
        [Tooltip("Filename to load on respawn, if blank no load is triggered")] public string saveFilename;

        [Tooltip("Event raised when object respawns")] public SimpleEvent onRespawn;

        private List<SimpleEvent> respawnSubs;

        #endregion

        #region Properties

        [AutoDoc("Returns a read-only list of languages known to object", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(NPCStats source)<br/>    {<br/>        string id = source.database.languages[0].info.id;<br/>        source.RemoveLanguage(id);<br/>    }<br/><br/>}")]
        public IReadOnlyList<GDTKLanguage> languages
        {
            get { return m_languages; }
        }

        #endregion

        #region Unity Methods

        [AutoDocSuppress]
        public override void Awake()
        {
            m_languages = new List<GDTKLanguage>();
            base.Awake();
        }

        #endregion

        #region Public Methods

        [AutoDoc("Added a language by id", "using NullSave.GDTK;<br/>using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour, IUniquelyIdentifiable<br/>{<br/><br/>    // Implment the IUniquelyIdentifiable interface to call RemoveXBySource<br/>    public string instanceId<br/>    {<br/>        get { return \"exampleInstance\"; }<br/>    }<br/><br/>    public void ExampleMethod(NPCStats source)<br/>    {<br/>        source.AddLanguage(source.database.languages[0].info.id, this);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of language to add")]
        [AutoDocParameter("Object (if any) that added the language")]
        public virtual GDTKLanguage AddLanguage(string id, IUniquelyIdentifiable source)
        {
            RegisterSource(source);

            foreach (GDTKLanguage value in database.languages)
            {
                if (value.info.id == id)
                {
                    GDTKLanguage clone = value.Clone();
                    clone.sourceId = source?.instanceId;
                    m_languages.Add(clone);
                    return clone;
                }
            }

            return null;
        }

        [AutoDocSuppress]
        public override int DataLoad(Stream stream)
        {
            Data.NPCStatsData data = new Data.NPCStatsData(stream);
            LoadData(data);
            return data.version;
        }

        public override void DataLoadJSON(string json)
        {
            Data.NPCStatsData data = SimpleJson.FromJSON<Data.NPCStatsData>(json);
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
            Data.NPCStatsData data = (Data.NPCStatsData)GenerateData();
            return SimpleJson.ToJSON(data, removeNulls, readable);
        }

        public override void DataSaveJSON(string filename)
        {
            using (FileStream fs = new FileStream(Path.Combine(Application.persistentDataPath, filename), FileMode.Create, FileAccess.Write))
            {
                fs.WriteString(DataSaveJSON());
            }
        }

        [AutoDocSuppress]
        public override void Initialize(bool reset)
        {
            base.Initialize(reset);

            respawnSubs = GDTKStatsManager.AutoSubscribe(respawnCondition, TriggerRespawn, source);
        }

        public override string JSONExport()
        {
            jsonNPCList result = new jsonNPCList();

            foreach (string val in startingAttributeIds) result.attributes.Add(val);
            foreach (string val in startingConditionIds) result.conditions.Add(val);
            foreach (var val in stats) result.stats.Add(val.Value);

            result.respawnCondition = respawnCondition;
            result.saveFilename = saveFilename;
            result.savePosition = savePosition;
            result.saveRotation = saveRotation;
            foreach (GDTKStatModifier val in respawnModifiers) result.respawnModifiers.Add(val);

            return SimpleJson.ToJSON(result);
        }

        public string JSONExport(List<int> statIndexes, List<int> conditionIndexes, List<int> attributeIndexes, bool includeRespawn)
        {
            jsonNPCList result = new jsonNPCList();

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

            return SimpleJson.ToJSON(result);
        }

        public override void JSONImport(string json, bool clearExisting)
        {
            JSONImport(SimpleJson.FromJSON<jsonNPCList>(json), clearExisting);
        }

        public void JSONImport(jsonNPCList source, bool clearExisting)
        {
            JSONImport((jsonStatsList)source, clearExisting);

            if (clearExisting) respawnModifiers.Clear();

            respawnCondition = source.respawnCondition;
            foreach (GDTKStatModifier val in source.respawnModifiers) respawnModifiers.Add(val);
            saveFilename = source.saveFilename;
            savePosition = source.savePosition;
            saveRotation = source.saveRotation;
        }

        public void JSONImportSmart(jsonNPCList source)
        {
            JSONImportSmart((jsonStatsList)source);

            if (source.respawnData)
            {
                respawnCondition = source.respawnCondition;
                foreach (GDTKStatModifier val in source.respawnModifiers) respawnModifiers.Add(val);
                saveFilename = source.saveFilename;
                savePosition = source.savePosition;
                saveRotation = source.saveRotation;
            }
        }

        [AutoDoc("Removes a language by id", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(NPCStats source)<br/>    {<br/>        string id = source.database.languages[0].info.id;<br/>        source.RemoveLanguage(id);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of language to remove")]
        public virtual bool RemoveLanguage(string id)
        {
            bool found = false;

            foreach (GDTKLanguage language in m_languages.ToArray())
            {
                if (language.info.id == id)
                {
                    m_languages.Remove(language);
                    found = true;
                }
            }

            return found;
        }

        [AutoDoc("Remove all languages from a specified source", "using NullSave.GDTK;<br/>using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(NPCStats source)<br/>    {<br/>        IUniquelyIdentifiable uid = source.registeredSources[0];<br/>        source.RemoveLanguagesFromSource(uid);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Source associated with request")]
        public virtual void RemoveLanguagesFromSource(IUniquelyIdentifiable source)
        {
            foreach (GDTKLanguage language in m_languages.ToArray())
            {
                if (language.sourceId == source?.instanceId)
                {
                    m_languages.Remove(language);
                }
            }
        }

        [AutoDocSuppress]
        public override void Shutdown(bool fullShutdown)
        {
            base.Shutdown(fullShutdown);

            if (fullShutdown)
            {
                onRespawn = null;
            }

            if (respawnSubs != null)
            {
                for (int i = 0; i < respawnSubs.Count; i++)
                {
                    respawnSubs[i] -= TriggerRespawn;
                }
            }

            m_languages.Clear();
        }

        #endregion

        #region Broadcast Methods

        [AutoDocSuppress]
        public override void PublicBroadcastReceived(object sender, string message)
        {
            StatsDebugManager.NPCStatsRequest(this, gameObject, message);
        }

        #endregion

        #region Private Methods

        internal override Data.BasicStatsData GenerateData()
        {
            Data.StatsAndEffectsData baseData = (Data.StatsAndEffectsData)base.GenerateData();

            return new Data.NPCStatsData
            {
                stats = baseData.stats,
                statusConditions = baseData.statusConditions,
                attributes = baseData.attributes,
                plugins = baseData.plugins,
                activeEffects = baseData.activeEffects,
                forbiddenIds = baseData.forbiddenIds,
                languages = m_languages.ToList(),
                respawnCondition = respawnCondition,
                savePosition = savePosition,
                position = transform.position,
                saveRotation = saveRotation,
                rotation = transform.rotation,
            };
        }

        internal override void LoadData(Data.BasicStatsData data)
        {
            base.LoadData(data);

            Data.NPCStatsData sdata = (Data.NPCStatsData)data;

            CharacterController cc = GetComponentInChildren<CharacterController>();
            if (cc != null) cc.enabled = false;

            respawnCondition = sdata.respawnCondition;

            if (sdata.savePosition) transform.position = sdata.position;
            if (sdata.saveRotation) transform.rotation = sdata.rotation;

            if (cc != null) cc.enabled = true;

            foreach (GDTKStatModifier mod in respawnModifiers)
            {
                AddStatModifier(mod, null);
            }

            respawnSubs = GDTKStatsManager.AutoSubscribe(respawnCondition, TriggerRespawn, source);
        }

        private void TriggerRespawn()
        {
            if (!GDTKStatsManager.IsConditionTrue(respawnCondition, source)) return;

            onRespawn?.Invoke();

            if (!string.IsNullOrEmpty(saveFilename))
            {
                DataLoad(saveFilename);
            }
            else
            {
                foreach (GDTKStatModifier mod in respawnModifiers)
                {
                    AddStatModifier(mod, null);
                }
            }
        }

        #endregion

    }
}
#endif
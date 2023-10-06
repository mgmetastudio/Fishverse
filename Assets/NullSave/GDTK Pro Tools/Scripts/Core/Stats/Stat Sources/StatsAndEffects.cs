#if GDTK
using NullSave.GDTK.JSON;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [AutoDoc("This component inherits from BasicStats and adds stats and effects to an object. It provides access to Stats, Attributes, Status Conditions, Stat Events, Status Effects, and Plugins.")]
    public class StatsAndEffects : BasicStats
    {

        #region Fields

        private List<GDTKStatusEffect> m_activeEffects;
        private List<string> forbiddenIds;

        [Tooltip("Event raised when a Status Effect is added")] public StatusEffectEvent onEffectAdded;
        [Tooltip("Event raised when a Status Effect is removed")] public StatusEffectEvent onEffectRemoved;

        #endregion

        #region Properties

        [AutoDoc("Returns a read-only list of Status Effects currently active on this object.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(StatsAndEffects source)<br/>    {<br/>        foreach(GDTKStatusEffect effect in source.activeEffects)<br/>        {<br/>            source.RemoveEffect(effect);<br/>        }<br/>    }<br/><br/>}")]
        public IReadOnlyList<GDTKStatusEffect> activeEffects
        {
            get
            {
                return m_activeEffects;
            }
        }

        #endregion

        #region Unity Methods

        public override void Awake()
        {
            m_activeEffects = new List<GDTKStatusEffect>();
            forbiddenIds = new List<string>();
            base.Awake();
        }

        #endregion

        #region Public Methods

        [AutoDoc("Adds a Status Effect to the object by id.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(StatsAndEffects source)<br/>    {<br/>        string id = source.database.effects[0].info.id;<br/>        source.AddEffect(id);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of the Status Effect to add")]
        public virtual bool AddEffect(string statusEffectId)
        {
            GDTKStatusEffect statusEffect = database.GetEffect(statusEffectId);
            if (statusEffect != null)
            {
                return AddEffect(statusEffect);
            }
            return false;
        }

        [AutoDoc("Adds a Status Effect to the object.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(StatsAndEffects source)<br/>    {<br/>        GDTKStatusEffect effect = source.database.effects[0];<br/>        source.AddEffect(effect);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Status Effect to Remove")]
        public virtual bool AddEffect(GDTKStatusEffect statusEffect)
        {
            if (!GDTKStatsManager.StatMeetsConditions(statusEffect, m_activeEffects))
            {
                // Check for existing copy that may need lifetime reset
                foreach (GDTKStatusEffect effect in m_activeEffects)
                {
                    if (effect.info.id == statusEffect.info.id)
                    {
                        effect.ResetLifespans();
                    }
                }

                return false;
            }

            // Check if effect is being prevented
            if (forbiddenIds.Contains(statusEffect.info.id))
            {
                return false;
            }

            // Clone effect
            statusEffect = statusEffect.Clone();
            statusEffect.affecting = this;
            statusEffect.Initialize(this);

            bool addToActive = statusEffect.livingModifiers.Count > 0;

            // Check for lifetime
            if (statusEffect.expires != EffectExpiry.Automatically)
            {
                addToActive = true;
            }

            // Check for cancelled effects
            foreach (string id in statusEffect.cancelEffectIds)
            {
                RemoveEffect(id, true);
                if (addToActive)
                {
                    forbiddenIds.Add(id);
                }
            }

            if (addToActive)
            {
                // Check for attributes
                if (statusEffect.attributeIds.Count > 0)
                {
                    foreach (string id in statusEffect.attributeIds)
                    {
                        AddAttribute(id, statusEffect);
                    }
                }

                m_activeEffects.Add(statusEffect);
                onEffectAdded?.Invoke(statusEffect);
            }

            return true;
        }

        [AutoDocSuppress]
        public override int DataLoad(Stream stream)
        {
            Data.StatsAndEffectsData data = new Data.StatsAndEffectsData(stream);
            LoadData(data);
            return data.version;
        }

        public override void DataLoadJSON(string json)
        {
            Data.StatsAndEffectsData data = SimpleJson.FromJSON<Data.StatsAndEffectsData>(json);
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
            Data.StatsAndEffectsData data = (Data.StatsAndEffectsData)GenerateData();
            return SimpleJson.ToJSON(data, removeNulls, readable);
        }

        public override void DataSaveJSON(string filename)
        {
            using (FileStream fs = new FileStream(Path.Combine(Application.persistentDataPath, filename), FileMode.Create, FileAccess.Write))
            {
                fs.WriteString(DataSaveJSON());
            }
        }

        public GDTKStatModifier GetModifier(string instanceId)
        {
            foreach(var entry in stats)
            {
                foreach(GDTKStatModifier modifier in entry.Value.activeModifiers)
                {
                    if(modifier.instanceId == instanceId)
                    {
                        return modifier;
                    }
                }
            }

            return null;
        }

        [AutoDoc("Removes an effect from the object by id.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(StatsAndEffects source)<br/>    {<br/>        foreach(GDTKStatusEffect effect in source.activeEffects)<br/>        {<br/>            source.RemoveEffect(effect);<br/>        }<br/>    }<br/><br/>}")]
        [AutoDocParameter("Id of Status Effect to remove.")]
        [AutoDocParameter("If true all instance of effects with the supplied id are removed, otherwise only the 1st instance is removed.")]
        public virtual bool RemoveEffect(string statusEffectId, bool allInstances = false)
        {
            if (allInstances)
            {
                bool result = false;
                List<GDTKStatusEffect> removal = new List<GDTKStatusEffect>();

                foreach (GDTKStatusEffect effect in activeEffects)
                {
                    if (effect.info.id == statusEffectId)
                    {
                        removal.Add(effect);
                        result = true;
                    }
                }

                foreach (GDTKStatusEffect effect in removal)
                {
                    RemoveEffect(effect);
                }

                return result;
            }

            foreach (GDTKStatusEffect effect in activeEffects)
            {
                if (effect.info.id == statusEffectId)
                {
                    return RemoveEffect(effect);
                }
            }

            return false;
        }

        [AutoDoc("Removes a specific instance of an Status Effect from the object.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(StatsAndEffects source)<br/>    {<br/>        string id = source.database.effects[0].info.id;<br/>        source.RemoveEffect(id, true);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Status Effect to remove.")]
        public virtual bool RemoveEffect(GDTKStatusEffect statusEffect)
        {
            if (!m_activeEffects.Contains(statusEffect)) return false;
            RemoveEffectModifiers(statusEffect);
            m_activeEffects.Remove(statusEffect);

            // Shutdown
            statusEffect.Shutdown();

            // Remove forbidden effects
            foreach (string id in statusEffect.cancelEffectIds)
            {
                forbiddenIds.Remove(id);
            }

            // Check for attributes
            if (statusEffect.attributeIds.Count > 0 && this is NPCStats)
            {
                RemoveAttributesFromSource(statusEffect);
            }

            RemoveSource(statusEffect);
            statusEffect.onRemoved?.Invoke();
            onEffectRemoved?.Invoke(statusEffect);

            return true;
        }

        [AutoDocSuppress]
        public override void Shutdown(bool fullShutdown)
        {
            base.Shutdown(fullShutdown);
            
            foreach(GDTKStatusEffect effect in m_activeEffects.ToArray())
            {
                effect.Shutdown();
                onEffectRemoved?.Invoke(effect);
            }
            m_activeEffects.Clear();

            forbiddenIds.Clear();
        }

        [AutoDoc("Subscribes to Status Effect events.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(StatsAndEffects source)<br/>    {<br/>        source.SubscribeToEffectEvents(EffectAdded, EffectRemoved);<br/>    }<br/><br/>    public void Unsubscribe(StatsAndEffects source)<br/>    {<br/>        source.UnsubscribeFromEffectEvents(EffectAdded, EffectRemoved);<br/>    }<br/><br/>    private void EffectAdded(GDTKStatusEffect effect)<br/>    {<br/>        Debug.Log(effect.info.id + \" added\");<br/>    }<br/><br/>    private void EffectRemoved(GDTKStatusEffect effect)<br/>    {<br/>        Debug.Log(effect.info.id + \" removed\");<br/>    }<br/><br/>}")]
        [AutoDocParameter("Status Effect Addded handler.")]
        [AutoDocParameter("Status Effect Removed handler.")]
        public virtual void SubscribeToEffectEvents(StatusEffectEvent addedHandler, StatusEffectEvent removedHandler)
        {
            if (addedHandler != null) onEffectAdded += addedHandler;
            if (removedHandler != null) onEffectRemoved += removedHandler;
        }

        [AutoDoc("Unsubscribes from Status Effect events.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(StatsAndEffects source)<br/>    {<br/>        source.SubscribeToEffectEvents(EffectAdded, EffectRemoved);<br/>    }<br/><br/>    public void Unsubscribe(StatsAndEffects source)<br/>    {<br/>        source.UnsubscribeFromEffectEvents(EffectAdded, EffectRemoved);<br/>    }<br/><br/>    private void EffectAdded(GDTKStatusEffect effect)<br/>    {<br/>        Debug.Log(effect.info.id + \" added\");<br/>    }<br/><br/>    private void EffectRemoved(GDTKStatusEffect effect)<br/>    {<br/>        Debug.Log(effect.info.id + \" removed\");<br/>    }<br/><br/>}")]
        [AutoDocParameter("Status Effect Addded handler.")]
        [AutoDocParameter("Status Effect Removed handler.")]
        public virtual void UnsubscribeFromEffectEvents(StatusEffectEvent addedHandler, StatusEffectEvent removedHandler)
        {
            if (addedHandler != null) onEffectAdded -= addedHandler;
            if (removedHandler != null) onEffectRemoved -= removedHandler;
        }

        #endregion

        #region Broadcast Methods

        [AutoDocSuppress]
        public override void PublicBroadcastReceived(object sender, string message)
        {
            StatsDebugManager.StatsAndEffectsRequest(this, gameObject, message);
        }

        #endregion

        #region Private Methods

        internal override Data.BasicStatsData GenerateData()
        {
            Data.BasicStatsData baseData = base.GenerateData();

            return new Data.StatsAndEffectsData
            {
                stats = baseData.stats,
                statusConditions = baseData.statusConditions,
                attributes = baseData.attributes,
                plugins = baseData.plugins,
                activeEffects = m_activeEffects,
                forbiddenIds = forbiddenIds,
            };
        }

        internal override void LoadData(Data.BasicStatsData data)
        {
            base.LoadData(data);

            Data.StatsAndEffectsData sdata = (Data.StatsAndEffectsData)data;
            m_activeEffects = sdata.activeEffects.ToList();
            forbiddenIds = sdata.forbiddenIds;

            foreach (GDTKStatusEffect effect in m_activeEffects)
            {
                effect.FinalizeLoading(this);
                onEffectAdded?.Invoke(effect);
            }
        }

        private void RemoveEffectModifiers(GDTKStatusEffect effect)
        {
            foreach (GDTKStatModifier modifier in effect.modifiers)
            {
                stats.TryGetValue(modifier.affectsStatId, out GDTKStat stat);

                if (stat != null)
                {
                    stat.RemoveModifier(modifier);
                }
            }
        }

        #endregion

    }
}
#endif
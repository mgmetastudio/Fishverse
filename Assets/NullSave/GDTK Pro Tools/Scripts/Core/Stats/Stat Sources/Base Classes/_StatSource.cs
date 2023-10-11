#if GDTK
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [Serializable]
    public class StatSource
    {

        #region Constants

        public const int STATS_FILE_VERSION = 2;

        #endregion

        #region Fields

        [Tooltip("Event raised when Stats are reloaded")] public SimpleEvent onStatsReloaded;

        [Tooltip("Event raised every Update")] public HeartbeatEvent onHeartbeat;
        [Tooltip("Event raised when a Token Heartbeat is invoked")] public HeartbeatEvent onTokenHeartbeat;

        private Dictionary<string, IUniquelyIdentifiable> identifiableSources;
        private Dictionary<string, GDTKStat> initializedStats;
        private Dictionary<string, StatSourceAction> actions;
        [SerializeField] [JsonSerialize] private List<GDTKStat> m_stats;
        private string m_instanceId;

        #endregion

        #region Properties

        public bool initialized { get; private set; }

        public string instanceId
        {
            get
            {
                if(m_instanceId is null)
                {
                    m_instanceId = Guid.NewGuid().ToString();
                }

                return m_instanceId;
            }
        }

        public IReadOnlyList<string> statIds
        {
            get {
                return initializedStats.Select(_ => _.Key).ToList();
            }
        }

        public IReadOnlyDictionary<string, GDTKStat> stats
        {
            get
            {
                return initializedStats;
            }
        }

        public IReadOnlyList<IUniquelyIdentifiable> registeredSources
        {
            get
            {
                return identifiableSources.Select(x => x.Value).ToList();
            }
        }

        #endregion

        #region Constructors

        public StatSource()
        {
            actions = new Dictionary<string, StatSourceAction>();
            initializedStats = new Dictionary<string, GDTKStat>();
            m_stats = new List<GDTKStat>();
            identifiableSources = new Dictionary<string, IUniquelyIdentifiable>();
        }

        #endregion

        #region Public Methods

        public virtual bool AddStatModifier(GDTKStatModifier modifier, IUniquelyIdentifiable source = null, StatSource other = null)
        {
            Dictionary<string, StatSource> sources = new Dictionary<string, StatSource>();
            sources.Add("", this);
            if(other != null)
            {
                sources.Add("other:", other);
            }
            GlobalStats global = ToolRegistry.GetComponent<GlobalStats>();
            if(global != null)
            {
                sources.Add("global:", global.source);
            }

            return AddStatModifer(modifier, source, sources);
        }

        public virtual bool AddStatModifer(GDTKStatModifier modifier, IUniquelyIdentifiable source, Dictionary<string, StatSource> sources)
        {
            // Apply modifiers
            modifier.sourceId = source?.instanceId;
            RegisterSource(source);

            string target = modifier.affectsStatId;
            bool isLocal = target.IndexOf(':') < 0;
            foreach (var entry in sources)
            {
                if ((isLocal && entry.Key == "") || (entry.Key != "" && target.StartsWith(entry.Key)))
                {
                    target = target.Substring(entry.Key.Length);
                    entry.Value.stats.TryGetValue(target, out GDTKStat stat);
                    if (stat == null)
                    {
                        StringExtensions.LogError("StatSource", "AddStatModifer", "Stat cannot be found named " + target);
                        return false;
                    }

                    if (GDTKStatsManager.IsConditionTrue(modifier.requirements, sources))
                    {
                        stat.AddModifier(modifier, sources);
                        return true;
                    }

                    return false;

                }
            }

            return false;
        }

        public virtual void ClearSources()
        {
            if (initialized)
            {
                StringExtensions.LogError("StatSource", "ClearSources", "Cannot modify stats while initialized, call Shutdown() first");
                return;
            }

            identifiableSources.Clear();
        }

        public virtual void ClearStats()
        {
            if (initialized)
            {
                StringExtensions.LogError("StatSource", "ClearStats", "Cannot modify stats while initialized, call Shutdown() first");
                return;
            }

            InternalClearStats();
        }

        public virtual IUniquelyIdentifiable GetSource(string uniqueId)
        {
            identifiableSources.TryGetValue(uniqueId, out IUniquelyIdentifiable result);
            return result;
        }

        public virtual GDTKStat GetStat(string statId)
        {
            if (string.IsNullOrEmpty(statId)) return null;
            initializedStats.TryGetValue(statId, out GDTKStat result);
            return result;
        }

        public virtual float GetValue(string formula, StatSource other = null)
        {
            return (float)GDTKStatsManager.GetValue(formula, this, other);
        }

        public virtual float GetValue(string formula, Dictionary<string, StatSource> sources)
        {
            return (float)GDTKStatsManager.GetValue(formula, sources);
        }

        public void Initialize(bool reset)
        {
            if (reset)
            {
                initialized = false;
                initializedStats = new Dictionary<string, GDTKStat>();
            }
            else if (initialized)
            {
                return;
            }

            // Create entries
            foreach (GDTKStat stat in m_stats)
            {
                if (initializedStats.ContainsKey(stat.info.id))
                {
                    StringExtensions.LogError("StatSource", "Initialize", "Id " + stat.info.id + " is used more than once");
                }
                else
                {
                    initializedStats.Add(stat.info.id, stat);
                }
            }

            foreach (var entry in initializedStats)
            {
                entry.Value.Initialize(this);
            }
        }

        public void Initialize(List<GDTKStat> overrideStats)
        {
            if (initialized == true) return;

            m_stats = overrideStats.ToList();
            Initialize(false);
    }

        public virtual bool IsConditionTrue(string condition, StatSource other = null)
        {
            return GDTKStatsManager.IsConditionTrue(condition, this, other);
        }

        public void LoadData(Data.BasicStatsData data)
        {
            initialized = true;
            Shutdown();

            // Add to lists
            foreach (GDTKStat stat in data.stats)
            {
                if (stat.info.id != null)
                {
                    m_stats.Add(stat);
                    initializedStats.Add(stat.info.id, stat);
                }
            }

            // Finalize Stat Loading
            foreach (var entry in initializedStats)
            {
                entry.Value.FinalizeLoading(this);
            }
            onStatsReloaded?.Invoke();
        }

        public virtual void RaiseHeartbeat(float time)
        {
            onHeartbeat?.Invoke(time);
        }

        public virtual void RaiseTokenHeartbeat(float time)
        {
            onTokenHeartbeat?.Invoke(time);
        }

        public virtual void RegisterAction(string id, StatSourceAction action)
        {
            actions.Add(id, action);
        }

        public virtual void RegisterSource(IUniquelyIdentifiable source)
        {
            if (source == null) return;

            if (!identifiableSources.ContainsKey(source.instanceId))
            {
                identifiableSources.Add(source.instanceId, source);
            }
            else
            {
                // When data loading this is the fastest way to re-reference
                identifiableSources[source.instanceId] = source;
            }
        }

        public virtual void RemoveSource(IUniquelyIdentifiable source)
        {
            identifiableSources.Remove(source.instanceId);
        }

        public virtual void RemoveSource(string uniqueId)
        {
            identifiableSources.Remove(uniqueId);
        }

        public virtual object RunAction(string id, object[] args)
        {
            object result = null;

            if(actions.ContainsKey(id))
            {
                actions[id].Invoke(args, ref result);
            }

            return result;
        }

        public virtual void Shutdown()
        {
            if (!initialized) return;
            initialized = false;

            InternalClearStats();
            identifiableSources.Clear();
        }

        public virtual void SubscribeToHeartbeat(HeartbeatEvent handler)
        {
            onHeartbeat += handler;
        }

        public virtual void SubscribeToTokenHeartbeat(HeartbeatEvent handler)
        {
            onTokenHeartbeat += handler;
        }

        public virtual void UnsubscribeFromHeartbeat(HeartbeatEvent handler)
        {
            onHeartbeat -= handler;
        }

        public virtual void UnsubscribeFromTokenHeartbeat(HeartbeatEvent handler)
        {
            onTokenHeartbeat -= handler;
        }

        #endregion

        #region Private Methods
        private void InternalClearStats()
        {
            foreach (GDTKStat stat in m_stats)
            {
                stat.Shutdown();
            }

            m_stats.Clear();
            initializedStats = new Dictionary<string, GDTKStat>();
        }

        #endregion

    }
}
#endif
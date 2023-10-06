#if GDTK
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [Serializable]
    public class GDTKStatusEffect : IUniquelyIdentifiable
    {

        #region Fields

        [Tooltip("Information about this effect")] public BasicInfo info;
        [Tooltip("Modifiers to apply with this effect")] public List<GDTKStatModifier> modifiers;
        [Tooltip("Maximum number of times this effect can be active at once on a single target")] public int maxStack;
        [Tooltip("Effects that are cancelled, removed, prevented while this effect is active")] public List<string> cancelEffectIds;

        [Tooltip("Determines when the effect (and its modifiers) expire")] public EffectExpiry expires;
        [Tooltip("Seconds/Tokens until expiration")] public float expiryTime;
        [Tooltip("Attributes to add while effect is active")] public List<string> attributeIds;
        [Tooltip("Item to spawn")] public GDTKSpawnInfo spawnInfo;

        private string m_instanceId;

        [NonSerialized] private BasicStats owner;
        [NonSerialized][JsonSerializeAs("livingModifiers")] private List<GDTKStatModifier> m_livingModifiers;

        [JsonDoNotSerialize] public SimpleEvent onLifeChanged, onModifiersChanged, onRemoved;

#if UNITY_EDITOR
        [SerializeField] private bool z_expanded;
#endif

        #endregion

        #region Properties

        [JsonDoNotSerialize]
        public StatsAndEffects affecting { get; set; }

        public bool initialized { get; private set; }

        public string instanceId
        {
            get
            {
                if (string.IsNullOrEmpty(m_instanceId))
                {
                    m_instanceId = Guid.NewGuid().ToString().Replace("-", "");
                }

                return m_instanceId;
            }
        }

        public float lifeRemaining { get; private set; }

        public IReadOnlyList<GDTKStatModifier> livingModifiers
        {
            get { return m_livingModifiers; }
        }

        #endregion

        #region Constructor

        public GDTKStatusEffect()
        {
            info = new BasicInfo();
            modifiers = new List<GDTKStatModifier>();
            maxStack = 1;
            cancelEffectIds = new List<string>();
            attributeIds = new List<string>();
            spawnInfo = new GDTKSpawnInfo();
        }

        #endregion

        #region Public Methods

        public GDTKStatusEffect Clone()
        {
            GDTKStatusEffect result = new GDTKStatusEffect();

            result.info = info.Clone();
            foreach (GDTKStatModifier mod in modifiers) result.modifiers.Add(mod.Clone());
            result.maxStack = maxStack;
            result.cancelEffectIds = cancelEffectIds.ToList();
            result.expires = expires;
            result.expiryTime = expiryTime;
            result.attributeIds = attributeIds;
            result.spawnInfo = spawnInfo.Clone();

            return result;
        }

        public void DataLoad(Stream stream, int version)
        {
            if (version > BasicStats.STATS_FILE_VERSION)
            {
                throw new NotSupportedException("Invalid file version");
            }

            Shutdown();
            LoadData(new Data.StatusEffectData(stream, version));
        }

        public void DataSave(Stream stream, int version)
        {
            if (version > BasicStats.STATS_FILE_VERSION)
            {
                throw new NotSupportedException("Invalid file version");
            }

            GenerateData().Write(stream, version);
        }

        public void Initialize(BasicStats source)
        {
            if (initialized) return;

            owner = source;
            owner.RegisterSource(this);
            m_livingModifiers = new List<GDTKStatModifier>();
            lifeRemaining = expiryTime;

            switch (expires)
            {
                case EffectExpiry.AfterSeconds:
                    owner.SubscribeToHeartbeat(UpdateExpiry);
                    break;
                case EffectExpiry.AfterTokens:
                    owner.SubscribeToTokenHeartbeat(UpdateExpiry);
                    break;
            }

            // Apply modifiers
            foreach (GDTKStatModifier modifier in modifiers)
            {
                modifier.sourceId = instanceId;
                if(owner.AddStatModifier(modifier, this))
                {
                    if (modifier.applies != ModifierApplication.Immediately)
                    {
                        modifier.onDeactivated += RemoveExpiredModifier;
                        modifier.onLifeChanged += UpdateModLife;
                        m_livingModifiers.Add(modifier);
                    }
                }
            }

            spawnInfo.Spawn(owner.transform);
            initialized = true;
        }

        public void ResetLifespans()
        {
            lifeRemaining = expiryTime;
            foreach (GDTKStatModifier mod in m_livingModifiers)
            {
                mod.ResetLifespan();
            }
            onLifeChanged?.Invoke();
        }

        public void Shutdown()
        {
            if (owner != null)
            {
                owner.UnsubscribeFromHeartbeat(UpdateExpiry);
                owner.UnsubscribeFromTokenHeartbeat(UpdateExpiry);
            }
            onRemoved?.Invoke();

            if (!initialized) return;
            initialized = false;

            foreach (GDTKStatModifier mod in modifiers)
            {
                mod.Shutdown();
            }

            onLifeChanged = null;
            onModifiersChanged = null;
            onRemoved = null;

            spawnInfo.DestroySpawn();

            owner = null;
        }

        #endregion

        #region Private Methods

        protected internal void FinalizeLoading(BasicStats host)
        {
            if (initialized) return;

            owner = host;
            owner.RegisterSource(this);

            if (m_livingModifiers == null) m_livingModifiers = new List<GDTKStatModifier>();

            StatsAndEffects sae = (StatsAndEffects)host;
            for(int i=0; i < m_livingModifiers.Count; i++)
            {
                m_livingModifiers[i] = sae.GetModifier(m_livingModifiers[i].instanceId);
                m_livingModifiers[i].onDeactivated += RemoveExpiredModifier;
                m_livingModifiers[i].onLifeChanged += UpdateModLife;

            }

            switch (expires)
            {
                case EffectExpiry.AfterSeconds:
                    owner.SubscribeToHeartbeat(UpdateExpiry);
                    break;
                case EffectExpiry.AfterTokens:
                    owner.SubscribeToTokenHeartbeat(UpdateExpiry);
                    break;
            }

            initialized = true;
        }

        private Data.StatusEffectData GenerateData()
        {
            return new Data.StatusEffectData
            {
                info = info,
                livingModifiers = m_livingModifiers,
                modifiers = modifiers,
                maxStack = maxStack,
                cancelEffectIds = cancelEffectIds,
                expires = expires,
                expiryTime = expiryTime,
                lifeRemaining = lifeRemaining,
                attributeIds = attributeIds,
                spawnInfo = spawnInfo,
                instanceId = instanceId,
            };
        }

        private void LoadData(Data.StatusEffectData data)
        {
            info = data.info;
            m_livingModifiers = data.livingModifiers.ToList();
            modifiers = data.modifiers.ToList();
            maxStack = data.maxStack;
            cancelEffectIds = data.cancelEffectIds;
            expires = data.expires;
            expiryTime = data.expiryTime;
            lifeRemaining = data.lifeRemaining;
            attributeIds = data.attributeIds;
            spawnInfo = data.spawnInfo;
            m_instanceId = data.instanceId;

            foreach(GDTKStatModifier mod in m_livingModifiers)
            {
                mod.onDeactivated += RemoveExpiredModifier;
            }
        }

        private void RemoveExpiredModifier(GDTKStatModifier modifier)
        {
            m_livingModifiers.Remove(modifier);
            if (affecting != null)
            {
                affecting.RemoveEffect(this);
            }
            onModifiersChanged?.Invoke();
            modifier.onLifeChanged -= UpdateModLife;

            if (expires == EffectExpiry.Automatically && m_livingModifiers.Count == 0)
            {
                ShutdownOrRemove();
            }
        }

        private void ShutdownOrRemove()
        {
            if (owner is StatsAndEffects effects)
            {
                effects.RemoveEffect(this);
            }
            else
            {
                Shutdown();
            }
            onRemoved?.Invoke();
        }

        private void UpdateExpiry(float time)
        {
            lifeRemaining -= time;
            onLifeChanged?.Invoke();
            if (lifeRemaining <= 0)
            {
                ShutdownOrRemove();
            }
        }

        private void UpdateModLife()
        {
            onModifiersChanged?.Invoke();
        }

        #endregion

    }
}
#endif
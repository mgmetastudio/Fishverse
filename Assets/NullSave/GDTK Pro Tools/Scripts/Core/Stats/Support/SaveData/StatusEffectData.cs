#if GDTK
using System.Collections.Generic;
using System.IO;

namespace NullSave.GDTK.Stats.Data
{
    public class StatusEffectData
    {

        #region Fields

        public BasicInfo info;
        public List<GDTKStatModifier> livingModifiers;
        public List<GDTKStatModifier> modifiers;
        public int maxStack;
        public List<string> cancelEffectIds;
        public EffectExpiry expires;
        public float expiryTime;
        public float lifeRemaining;
        public List<string> attributeIds;
        public GDTKSpawnInfo spawnInfo;
        public string instanceId;

        #endregion

        #region Constructors

        public StatusEffectData() { }

        public StatusEffectData(Stream stream, int version)
        {
            int count;
            info = new BasicInfo();
            livingModifiers = new List<GDTKStatModifier>();
            modifiers = new List<GDTKStatModifier>();
            cancelEffectIds = new List<string>();
            attributeIds = new List<string>();
            spawnInfo = new GDTKSpawnInfo();

            instanceId = stream.ReadStringPacket();
            info.DataLoad(stream, 1);

            count = stream.ReadInt();
            List<string> addToLiving = new List<string>();
            for (int i = 0; i < count; i++)
            {
                addToLiving.Add(stream.ReadStringPacket());
            }

            count = stream.ReadInt();
            for (int i = 0; i < count; i++)
            {
                GDTKStatModifier mod = new GDTKStatModifier();
                mod.DataLoad(stream, version);
                modifiers.Add(mod);
                if (addToLiving.Contains(mod.instanceId))
                {
                    livingModifiers.Add(mod);
                }
            }

            maxStack = stream.ReadInt();

            cancelEffectIds = new List<string>();
            count = stream.ReadInt();
            for (int i = 0; i < count; i++)
            {
                cancelEffectIds.Add(stream.ReadStringPacket());
            }

            expires = (EffectExpiry)stream.ReadInt();
            expiryTime = stream.ReadFloat();
            lifeRemaining = stream.ReadFloat();

            attributeIds = new List<string>();
            count = stream.ReadInt();
            for (int i = 0; i < count; i++)
            {
                attributeIds.Add(stream.ReadStringPacket());
            }

            spawnInfo.DataLoad(stream);
        }

        #endregion

        #region Public Methods

        public void Write(Stream stream, int version)
        {
            stream.WriteStringPacket(instanceId);
            info.DataSave(stream, 1);

            stream.WriteInt(modifiers.Count);
            foreach (GDTKStatModifier mod in livingModifiers)
            {
                stream.WriteStringPacket(mod.instanceId);
            }

            stream.WriteInt(modifiers.Count);
            foreach (GDTKStatModifier mod in modifiers)
            {
                mod.DataSave(stream, version);
            }

            stream.WriteInt(maxStack);

            stream.WriteInt(cancelEffectIds.Count);
            foreach (string id in cancelEffectIds)
            {
                stream.WriteStringPacket(id);
            }

            stream.WriteInt((int)expires);
            stream.WriteFloat(expiryTime);
            stream.WriteFloat(lifeRemaining);

            stream.WriteInt(attributeIds.Count);
            foreach (string id in attributeIds)
            {
                stream.WriteStringPacket(id);
            }

            spawnInfo.DataSave(stream);
        }

        #endregion

    }
}
#endif
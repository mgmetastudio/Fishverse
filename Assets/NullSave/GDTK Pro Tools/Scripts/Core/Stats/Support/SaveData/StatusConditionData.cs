#if GDTK
using System.Collections.Generic;
using System.IO;

namespace NullSave.GDTK.Stats.Data
{
    public class StatusConditionData
    {

        #region Fields

        public string instanceId;
        public BasicInfo info;
        public bool active;
        public ConditionStartMode activateWhen;
        public string startCondition;
        public string startEvent;
        public ConditionEndMode deactivateWhen;
        public string endCondition;
        public string endEvent;
        public float endTime;
        public List<GDTKStatModifier> statModifiers;
        public List<string> attributeIds;
        public GDTKSpawnInfo spawnInfo;
        public string sourceId;
        public float timeRemaining;

        #endregion

        #region Constructors

        public StatusConditionData() { }

        public StatusConditionData(Stream stream, int version)
        {
            int count;
            
            info = new BasicInfo();
            statModifiers = new List<GDTKStatModifier>();
            attributeIds = new List<string>();
            spawnInfo = new GDTKSpawnInfo();

            if(version == 1) stream.ReadStringPacket();

            info.DataLoad(stream, 1);

            active = stream.ReadBool();

            activateWhen = (ConditionStartMode)stream.ReadInt();
            startCondition = stream.ReadStringPacket();
            startEvent = stream.ReadStringPacket();

            deactivateWhen = (ConditionEndMode)stream.ReadInt();
            endCondition = stream.ReadStringPacket();
            endEvent = stream.ReadStringPacket();

            endTime = stream.ReadFloat();

            statModifiers = new List<GDTKStatModifier>();
            count = stream.ReadInt();
            for (int i = 0; i < count; i++)
            {
                GDTKStatModifier mod = new GDTKStatModifier();
                mod.DataLoad(stream, version);
                statModifiers.Add(mod);
            }

            attributeIds = new List<string>();
            count = stream.ReadInt();
            for (int i = 0; i < count; i++)
            {
                attributeIds.Add(stream.ReadStringPacket());
            }

            spawnInfo.DataLoad(stream);

            instanceId = stream.ReadStringPacket();
            sourceId = stream.ReadStringPacket();
            timeRemaining = stream.ReadFloat();

        }

        #endregion

        #region Public Methods

        public void Write(Stream stream, int version)
        {
            if(version == 1) stream.WriteStringPacket(instanceId);

            info.DataSave(stream, 1);

            stream.WriteBool(active);
            stream.WriteInt((int)activateWhen);
            stream.WriteStringPacket(startCondition);
            stream.WriteStringPacket(startEvent);

            stream.WriteInt((int)deactivateWhen);
            stream.WriteStringPacket(endCondition);
            stream.WriteStringPacket(endEvent);

            stream.WriteFloat(endTime);

            stream.WriteInt(statModifiers.Count);
            foreach (GDTKStatModifier mod in statModifiers)
            {
                mod.DataSave(stream, version);
            }

            stream.WriteInt(attributeIds.Count);
            foreach (string id in attributeIds)
            {
                stream.WriteStringPacket(id);
            }

            spawnInfo.DataSave(stream);

            stream.WriteStringPacket(instanceId);
            stream.WriteStringPacket(sourceId);
            stream.WriteFloat(timeRemaining);
        }

        #endregion

    }
}
#endif
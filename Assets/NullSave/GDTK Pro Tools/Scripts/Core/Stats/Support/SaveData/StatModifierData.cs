#if GDTK
using System.IO;

namespace NullSave.GDTK.Stats.Data
{
    public class StatModifierData
    {

        #region Fields

        public string instanceId;
        public string affectsStatId;
        public string requirements;
        public ModifierApplication applies;
        public float lifespan;
        public bool wholeIncrements;
        public ModifierTarget target;
        public ModifierChangeType changeType;
        public GDTKStatValue value;
        public double remainingLife;
        public float secondProgress;
        public string sourceId;
        public double appliedValue;
        public string originalSetValue;

        #endregion

        #region Constructors

        public StatModifierData() { }

        public StatModifierData(Stream stream, int version)
        {
            value = new GDTKStatValue();

            instanceId = stream.ReadStringPacket();
            affectsStatId = stream.ReadStringPacket();
            requirements = stream.ReadStringPacket();
            applies = (ModifierApplication)stream.ReadInt();
            lifespan = stream.ReadFloat();
            wholeIncrements = stream.ReadBool();
            target = (ModifierTarget)stream.ReadInt();
            changeType = (ModifierChangeType)stream.ReadInt();
            value.DataLoad(stream, affectsStatId, version);
            remainingLife = stream.ReadDouble();
            secondProgress = stream.ReadFloat();
            sourceId = stream.ReadStringPacket();
            appliedValue = stream.ReadDouble();
            originalSetValue = stream.ReadStringPacket();

        }

        #endregion

        #region Public Methods

        public void Write(Stream stream, int version)
        {
            stream.WriteStringPacket(instanceId);
            stream.WriteStringPacket(affectsStatId);
            stream.WriteStringPacket(requirements);
            stream.WriteInt((int)applies);
            stream.WriteFloat(lifespan);
            stream.WriteBool(wholeIncrements);
            stream.WriteInt((int)target);
            stream.WriteInt((int)changeType);
            value.DataSave(stream, version);
            stream.WriteDouble(remainingLife);
            stream.WriteFloat(secondProgress);
            stream.WriteStringPacket(sourceId);

            stream.WriteDouble(appliedValue);
            stream.WriteStringPacket(originalSetValue);
        }

        #endregion

    }
}
#endif
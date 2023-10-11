#if GDTK
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NullSave.GDTK.Stats.Data
{
    public class NPCStatsData : StatsAndEffectsData
    {

        #region Fields

        public List<GDTKLanguage> languages;
        public string respawnCondition;
        public bool savePosition;
        public Vector3 position;
        public bool saveRotation;
        public Quaternion rotation;

        #endregion

        #region Constructors

        public NPCStatsData() { }

        public NPCStatsData(Stream stream) : base(stream)
        {
            int count;
            languages = new List<GDTKLanguage>();

            count = stream.ReadInt();
            for (int i = 0; i < count; i++)
            {
                stream.ReadStringPacket();
                stream.ReadStringPacket();
            }

            respawnCondition = stream.ReadStringPacket();
            savePosition = stream.ReadBool();
            if (savePosition)
            {
                position = stream.ReadVector3();
            }
            saveRotation = stream.ReadBool();
            if (saveRotation)
            {
                rotation = stream.ReadQuaternion();
            }
        }

        #endregion

        #region Public Methods

        public override void Write(Stream stream, int version)
        {
            base.Write(stream, version);

            stream.WriteInt(languages.Count);
            foreach (GDTKLanguage language in languages)
            {
                stream.WriteStringPacket(language.info.id);
                stream.WriteStringPacket(language.sourceId);
            }

            stream.WriteStringPacket(respawnCondition);
            stream.WriteBool(savePosition);
            if (savePosition)
            {
                stream.WriteVector3(position);
            }
            stream.WriteBool(saveRotation);
            if (saveRotation)
            {
                stream.WriteQuaternion(rotation);
            }

        }

        #endregion

    }
}
#endif
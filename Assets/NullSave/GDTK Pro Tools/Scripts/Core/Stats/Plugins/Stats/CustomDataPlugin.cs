#if GDTK
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    public class CustomDataPlugin : StatsPlugin
    {

        #region Fields

        [HideInInspector] public SimpleEvent onDataChanged;

        private Dictionary<string, object> data;

        #endregion

        #region Properties

        public override string title { get { return "Custom Data Plugin"; } }

        public override string description { get { return "Adds the ability to get/set custom data associated with stats."; } }

        #endregion

        #region Constructor

        public CustomDataPlugin()
        {
            data = new Dictionary<string, object>();
        }

        #endregion

        #region Public Methods

        public override void DataLoad(Stream stream)
        {
            data.Clear();

            int count = stream.ReadInt();
            for (int i = 0; i < count; i++)
            {
                data.Add(stream.ReadStringPacket(), ReadObjectData(stream));
            }

            onDataChanged?.Invoke();
        }

        public override void DataSave(Stream stream)
        {
            stream.WriteInt(data.Count);
            foreach (var dataPoint in data)
            {
                stream.WriteStringPacket(dataPoint.Key);
                WriteObjectData(stream, dataPoint.Value);
            }
        }

        /// <summary>
        /// Gets custom data associated with stat & key
        /// </summary>
        /// <typeparam name="T">Type of data to return</typeparam>
        /// <param name="statId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetCustomData<T>(string statId, string key)
        {
            string id = statId + "#" + key;

            if (data.ContainsKey(id))
            {
                return (T)data[id];
            }

            return default;
        }

        /// <summary>
        /// Sets custom data associated with stat & key
        /// </summary>
        /// <typeparam name="T">Type of data to set</typeparam>
        /// <param name="statId"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetCustomData(string statId, string key, object value)
        {
            string id = statId + "#" + key;

            if (data.ContainsKey(id))
            {
                data[id] = value;
            }
            else
            {
                data.Add(id, value);
            }

            onDataChanged?.Invoke();
        }

        #endregion

        #region Private Methods

        private object ReadObjectData(Stream stream)
        {
            string type = stream.ReadStringPacket();
            switch (type)
            {
                case "bool":
                    return stream.ReadBool();
                case "double":
                    return stream.ReadDouble();
                case "int":
                    return stream.ReadInt();
                case "float":
                    return stream.ReadFloat();
                case "long":
                    return stream.ReadLong();
                case "string":
                    return stream.ReadStringPacket();
                default:
                    return SimpleJson.FromJSON(stream.ReadStringPacket(), Type.GetType(type));
            }
        }

        private void WriteObjectData(Stream stream, object value)
        {
            if (value is bool @bool)
            {
                stream.WriteStringPacket("bool");
                stream.WriteBool(@bool);
            }
            else if (value is double @double)
            {
                stream.WriteStringPacket("double");
                stream.WriteDouble(@double);
            }
            else if (value is int @int)
            {
                stream.WriteStringPacket("int");
                stream.WriteInt(@int);
            }
            else if (value is float @float)
            {
                stream.WriteStringPacket("float");
                stream.WriteFloat(@float);
            }
            else if (value is long @long)
            {
                stream.WriteStringPacket("long");
                stream.WriteLong(@long);
            }
            else if (value is string @string)
            {
                stream.WriteStringPacket("string");
                stream.WriteStringPacket(@string);
            }
            else
            {
                stream.WriteStringPacket(value.GetType().AssemblyQualifiedName);
                stream.WriteStringPacket(SimpleJson.ToJSON(value));
            }
        }

        #endregion

        #region Private Classes

        [Serializable]
        private class jsonObjectData
        {

            #region Fields

            public string valueKey;
            public Type valueType;
            public object value;

            #endregion

        }

        #endregion

    }
}
#endif
using Newtonsoft.Json;
using UnityEngine;

namespace LibEngine.Auth
{
    public interface ISaveable<T>
    {
        T Current { get; set; }

        void Save();
    }

    public class PlayerPrefsSaveableScheme<T> : ISaveable<T> where T : new()
    {
        private readonly string key;
        private bool autoSave;
        private T current;

        public PlayerPrefsSaveableScheme(string key, bool autoSave = true)
        {
            this.key = key;
            this.autoSave = autoSave;
            Load();
        }

        public T Current
        {
            get => current;
            set
            {
                current = value;
                if (autoSave)
                {
                    Save();
                }
            }
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(current);
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }

        private void Load()
        {
            var json = PlayerPrefs.GetString(key);
            if (!string.IsNullOrEmpty(json))
            {
                current = JsonConvert.DeserializeObject<T>(json);
            }
            else
            {
                current = new T();
            }
        }
    }
}

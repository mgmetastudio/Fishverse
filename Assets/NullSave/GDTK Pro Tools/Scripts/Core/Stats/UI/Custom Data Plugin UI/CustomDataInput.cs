#if GDTK
using TMPro;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    public class CustomDataInput : MonoBehaviour
    {

        #region Enumerations

        public enum DataType
        {
            String,
            Int,
            Long,
            Float,
            Boolean,
        }

        #endregion

        #region Fields

        [Tooltip("Determine if source is a Direct Reference or in the Tool Registry")] public StatSourceReference source;
        [Tooltip("Stat Source")] [SerializeField] private BasicStats m_stats;
        [Tooltip("Key used to find source")] public string key;

        [Tooltip("Input field to use as source for data")] public TMP_InputField inputField;
        [Tooltip("Id of stat to which data should be associated")] public string statId;
        [Tooltip("Key used to save associated data")] public string dataKey;
        [Tooltip("Type to which data should be cast")] public DataType dataType;

        private CustomDataPlugin plugin;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            if (statId == null) return;

            if (source == StatSourceReference.FindInRegistry)
            {
                m_stats = ToolRegistry.GetComponent<BasicStats>(key);
            }

            if (inputField == null)
            {
                StringExtensions.LogError(name, "CustomDataInput", "No input source supplied.");
                return;
            }

            if (m_stats == null)
            {
                StringExtensions.LogError(name, "CustomDataInput", "No stat source supplied/found.");
                return;
            }

            plugin = m_stats.GetPlugin<CustomDataPlugin>();
            if (plugin == null)
            {
                StringExtensions.LogError(name, "CustomDataInput", "No CustomDataPlugin found source.");
                return;
            }

            inputField.text = plugin.GetCustomData<string>(statId, dataKey);
        }

        #endregion

        #region Public Methods

        public void SetData()
        {
            switch (dataType)
            {
                case DataType.Boolean:
                    plugin.SetCustomData(statId, dataKey, inputField.text.ToBool());
                    break;
                case DataType.Float:
                    plugin.SetCustomData(statId, dataKey, float.Parse(inputField.text));
                    break;
                case DataType.Int:
                    plugin.SetCustomData(statId, dataKey, int.Parse(inputField.text));
                    break;
                case DataType.Long:
                    plugin.SetCustomData(statId, dataKey, long.Parse(inputField.text));
                    break;
                default:
                    plugin.SetCustomData(statId, dataKey, inputField.text);
                    break;
            }
        }

        #endregion

    }
}
#endif
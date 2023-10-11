#if GDTK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    public class CustomDataDisplay : MonoBehaviour
    {

        #region Fields

        [Tooltip("Determine if source is a Direct Reference or in the Tool Registry")] public StatSourceReference source;
        [Tooltip("Stat Source")] [SerializeField] private BasicStats m_stats;
        [Tooltip("Key used to find source")] public string key;

        [Tooltip("Label used for displaying data")] public Label label;
        [Tooltip("Format used when displaying data")] public string format = "{0}";
        [Tooltip("Id of stat to which data should be associated")] public string statId;
        [Tooltip("Key used to save associated data")] public string dataKey;

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

            if (m_stats == null)
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

            plugin.onDataChanged += UpdateData;
            UpdateData();
        }

        #endregion

        #region Private Methods

        private void UpdateData()
        {
            object val = plugin.GetCustomData<object>(statId, dataKey);
            if (val == null)
            {
                label.text = "";
            }
            else
            {
                label.text = format
                    .Replace("{0}", val.ToString())
                    .Replace("{1}", val.GetType().ToString())
                    ;
            }
        }

        #endregion

    }
}
#endif
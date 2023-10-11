#if GDTK
using NullSave.GDTK.JSON;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [Serializable]
    public class GDTKLevelReward : ISerializationCallbackReceiver
    {

        #region Fields

        [Tooltip("Information about this Status Condition")] public BasicInfo info;
        [Tooltip("Condition that must be true to get this reward")] public string requirements;
        [Tooltip("Modifiers to apply while this condition is active")] public List<GDTKStatModifier> statModifiers;

#if UNITY_EDITOR
        [SerializeField] private bool z_expanded;
        [SerializeField] private bool z_info_expanded;
#endif

        private List<UniversalPluginWrapper<AddOnPlugin>> m_addOnPlugins;
        [SerializeField] [JsonSerializeAs("addOnPlugins")] private List<jsonUniversalPluginWrapper> jsonAddOnPlugins;
        [SerializeField] private bool needsRefresh;

        [JsonSerializeAs("addOnPluginChoices")] public List<AddOnPluginChoice> pluginChoices;

        #endregion

        #region Properties

        public List<UniversalPluginWrapper<AddOnPlugin>> addOnPlugins
        {
            get
            {
                if (needsRefresh)
                {
                    m_addOnPlugins = new List<UniversalPluginWrapper<AddOnPlugin>>();
                    foreach (jsonUniversalPluginWrapper aow in jsonAddOnPlugins)
                    {
                        m_addOnPlugins.Add(new UniversalPluginWrapper<AddOnPlugin>(aow.id, aow.serializationNamespace, aow.serializationType, aow.serializationData));
                    }

                    needsRefresh = false;

                }

                return m_addOnPlugins;
            }
        }

#if UNITY_EDITOR

        public bool editorExpanded
        {
            get { return z_expanded; }
            set { z_expanded = value; }
        }

        public bool editorInfoExpanded
        {
            get { return z_info_expanded; }
            set { z_info_expanded = value; }
        }

#endif

        #endregion

        #region Constructor

        public GDTKLevelReward()
        {
            info = new BasicInfo();
            statModifiers = new List<GDTKStatModifier>();
            jsonAddOnPlugins = new List<jsonUniversalPluginWrapper>();
            m_addOnPlugins = new List<UniversalPluginWrapper<AddOnPlugin>>();
            pluginChoices = new List<AddOnPluginChoice>();
        }

        #endregion

        #region Public Methods

        public GDTKLevelReward Clone()
        {
            GDTKLevelReward result = new GDTKLevelReward();

            result.requirements = requirements;
            result.info = info.Clone();
            foreach (GDTKStatModifier mod in statModifiers) result.statModifiers.Add(mod.Clone());
            foreach (UniversalPluginWrapper<AddOnPlugin> plugin in addOnPlugins) result.addOnPlugins.Add(plugin.Clone());
            foreach (AddOnPluginChoice choice in pluginChoices) result.pluginChoices.Add(choice.Clone());

            return result;
        }

        #endregion

        #region Serialization

        [JsonAfterDeserialization]
        public void OnAfterDeserialize()
        {
            needsRefresh = true;
        }

        [JsonBeforeSerialization]
        public void OnBeforeSerialize()
        {
            if (needsRefresh) return;
            jsonAddOnPlugins = new List<jsonUniversalPluginWrapper>();
            foreach (UniversalPluginWrapper<AddOnPlugin> addOn in addOnPlugins)
            {
                jsonAddOnPlugins.Add(addOn.ToJSON());
            }
        }

        #endregion

    }
}
#endif
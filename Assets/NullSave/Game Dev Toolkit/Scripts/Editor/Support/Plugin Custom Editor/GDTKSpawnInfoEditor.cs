using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK
{
    [CustomUniversalPluginEditor(typeof(GDTKSpawnInfo))]
    public class GDTKSpawnInfoEditor : UniversalPluginEditor
    {

        #region Public Methods

        public override void OnInspectorGUI()
        {
            GameObject go = (GameObject)PropertyObjectValue("gameObject");
            PropertyField("gameObject");
            GameObject checkGo = (GameObject)PropertyObjectValue("gameObject");
            if (go != checkGo)
            if (go != checkGo)
            {
                if (checkGo == null)
                {
                    PropertyIntValue("source", (int)SpawnSource.Resources);
                    PropertyStringValue("bundleName", null);
                    PropertyStringValue("path", null);
                    PropertyStringValue("assetName", null);
                    PropertyBoolValue("z_spawnError", false);
                }
                else
                {
                    string assetPath = AssetDatabase.GetAssetPath(checkGo);
                    string bundlePath = string.IsNullOrEmpty(assetPath) ? string.Empty : AssetDatabase.GetImplicitAssetBundleName(assetPath);
                    if (!string.IsNullOrEmpty(bundlePath))
                    {
                        PropertyIntValue("source", (int)SpawnSource.AssetBundle);
                        PropertyStringValue("bundleName", bundlePath);
                        PropertyStringValue("path", assetPath);
                        PropertyStringValue("assetName", checkGo.name);
                        PropertyBoolValue("z_spawnError", false);
                    }
                    else if (!string.IsNullOrEmpty(assetPath))
                    {
                        if (assetPath.IndexOf("resources", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            PropertyIntValue("source", (int)SpawnSource.Resources);
                            PropertyStringValue("bundleName", null);
                            PropertyStringValue("path", Path.Combine(Path.GetDirectoryName(assetPath.Substring(assetPath.LastIndexOf("resources", StringComparison.OrdinalIgnoreCase) + 10)), checkGo.name));
                            PropertyStringValue("assetName", null);
                            PropertyBoolValue("z_spawnError", false);
                        }
                        else
                        {
                            PropertyBoolValue("z_spawnError", true);
                        }
                    }
                    else if (checkGo == null)
                    {
                        PropertyIntValue("source", (int)SpawnSource.Resources);
                        PropertyStringValue("bundleName", null);
                        PropertyStringValue("path", null);
                        PropertyStringValue("assetName", null);
                        PropertyBoolValue("z_spawnError", false);
                    }
                    else
                    {
                        PropertyBoolValue("z_spawnError", true);
                    }
                }
            }

            if (PropertyBoolValue("z_spawnError"))
            {
                GUILayout.Label("Must be in Resources or an Asset Bundle", GDTKEditor.Styles.ErrorTextStyle);
            }

            PropertyField("parent");
            PropertyField("offset");

        }

        #endregion


    }
}
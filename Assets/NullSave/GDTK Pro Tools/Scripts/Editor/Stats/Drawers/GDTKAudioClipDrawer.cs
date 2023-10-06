#if GDTK
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [CustomPropertyDrawer(typeof(GDTKAudioClip))]
    public class GDTKAudioClipDrawer : GDTKPropertyDrawer
    {

        #region Unity Methods

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.FindPropertyRelative("z_resourceError").boolValue)
            {
                return (EditorGUIUtility.singleLineHeight + 2) * 2;
            }

            return EditorGUIUtility.singleLineHeight + 2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (fieldInfo.GetCustomAttributes(typeof(TooltipAttribute), true).FirstOrDefault() is TooltipAttribute tt) label.tooltip = tt.tooltip;

            Rect rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;


            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.BeginChangeCheck();
            SimpleProperty(rect, property, "m_audioClip", label.text, label.tooltip);
            rect.y += EditorGUIUtility.singleLineHeight + 2;
            if (EditorGUI.EndChangeCheck())
            {
                AudioClip audioClip = (AudioClip)property.FindPropertyRelative("m_audioClip").objectReferenceValue;
                if (audioClip == null)
                {
                    SimpleInt(property, "source", (int)ImageSource.None);
                    SimpleString(property, "bundleName", null);
                    SimpleString(property, "path", null);
                    SimpleString(property, "assetName", null);
                    SimpleValue(property, "z_resourceError", false);
                }
                else
                {
                    string assetPath = AssetDatabase.GetAssetPath(audioClip);
                    string bundlePath = AssetDatabase.GetImplicitAssetBundleName(assetPath);
                    if (!string.IsNullOrEmpty(bundlePath))
                    {
                        SimpleInt(property, "source", (int)ImageSource.AssetBundle);
                        SimpleString(property, "bundleName", bundlePath);
                        SimpleString(property, "assetName", audioClip.name);
                        SimpleString(property, "path", assetPath);
                        SimpleValue(property, "z_resourceError", false);
                    }
                    else if (!string.IsNullOrEmpty(assetPath))
                    {
                        if (assetPath.IndexOf("resources", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            SimpleInt(property, "source", (int)ImageSource.Resources);
                            SimpleString(property, "path", Path.Combine(Path.GetDirectoryName(assetPath.Substring(assetPath.LastIndexOf("resources", StringComparison.OrdinalIgnoreCase) + 10)), audioClip.name));
                            SimpleString(property, "bundleName", null);
                            SimpleString(property, "assetName", null);
                            SimpleValue(property, "z_resourceError", false);
                        }
                        else
                        {
                            SimpleValue(property, "z_resourceError", true);
                        }
                    }
                    else if (audioClip == null)
                    {
                        SimpleInt(property, "source", (int)ImageSource.None);
                        SimpleString(property, "path", null);
                        SimpleString(property, "bundleName", null);
                        SimpleString(property, "assetName", null);
                        SimpleValue(property, "z_resourceError", false);
                    }
                    else
                    {
                        SimpleValue(property, "z_resourceError", true);
                    }
                }
            }

            if (SimpleValue<bool>(property, "z_resourceError"))
            {
                EditorGUI.LabelField(rect, "Must be in Resources or an Asset Bundle", GDTKEditor.Styles.ErrorTextStyle);
                rect.y += EditorGUIUtility.singleLineHeight + 2;
            }

            EditorGUI.EndProperty();
        }


        #endregion

    }
}
#endif
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK
{
    [CustomPropertyDrawer(typeof(ImageInfo), true)]
    public class ImageInfoDrawer : GDTKPropertyDrawer
    {

        #region Unity Methods

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int lines = 1;
            if (property.FindPropertyRelative("z_imageError").boolValue) lines++;
            return (EditorGUIUtility.singleLineHeight + 2) * lines;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (fieldInfo.GetCustomAttributes(typeof(TooltipAttribute), true).FirstOrDefault() is TooltipAttribute tt) label.tooltip = tt.tooltip;

            Rect rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.BeginProperty(position, label, property);


            EditorGUI.BeginChangeCheck();
            SimpleProperty(rect, property, "sprite");
            rect.y += EditorGUIUtility.singleLineHeight + 2;
            if (EditorGUI.EndChangeCheck())
            {
                Sprite sprite = (Sprite)property.FindPropertyRelative("sprite").objectReferenceValue;
                if (sprite == null)
                {
                    SimpleInt(property, "source", (int)ImageSource.None);
                    SimpleString(property, "bundleName", null);
                    SimpleString(property, "path", null);
                    SimpleString(property, "assetName", null);
                    SimpleBool(property, "z_imageError", false);
                }
                else
                {
                    string assetPath = AssetDatabase.GetAssetPath(sprite);
                    string bundlePath = string.IsNullOrEmpty(assetPath) ? string.Empty : AssetDatabase.GetImplicitAssetBundleName(assetPath);
                    if (!string.IsNullOrEmpty(bundlePath))
                    {
                        SimpleInt(property, "source", (int)ImageSource.AssetBundle);
                        SimpleString(property, "bundleName", bundlePath);
                        SimpleString(property, "assetName", sprite.name);
                        SimpleString(property, "path", assetPath);
                        SimpleBool(property, "z_imageError", false);
                    }
                    else if (!string.IsNullOrEmpty(assetPath))
                    {
                        if (assetPath.IndexOf("resources", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            SimpleInt(property, "source", (int)ImageSource.Resources);
                            SimpleString(property, "path", Path.Combine(Path.GetDirectoryName(assetPath.Substring(assetPath.LastIndexOf("resources", StringComparison.OrdinalIgnoreCase) + 10)), sprite.name));
                            SimpleString(property, "bundleName", null);
                            SimpleString(property, "assetName", null);
                            SimpleBool(property, "z_imageError", false);
                        }
                        else
                        {
                            SimpleBool(property, "z_imageError", true);
                        }
                    }
                    else if (sprite == null)
                    {
                        SimpleInt(property, "source", (int)ImageSource.None);
                        SimpleString(property, "path", null);
                        SimpleString(property, "bundleName", null);
                        SimpleString(property, "assetName", null);
                        SimpleBool(property, "z_imageError", false);
                    }
                    else
                    {
                        SimpleBool(property, "z_imageError", true);
                    }
                }
            }

            if (SimpleBool(property, "z_imageError"))
            {
                EditorGUI.LabelField(rect, "Must be in Resources or an Asset Bundle", GDTKEditor.Styles.ErrorTextStyle);
                rect.y += EditorGUIUtility.singleLineHeight + 2;
            }

            EditorGUI.EndProperty();
        }


        #endregion

    }
}
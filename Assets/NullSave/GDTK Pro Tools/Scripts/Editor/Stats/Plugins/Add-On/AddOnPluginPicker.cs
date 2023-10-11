﻿#if GDTK
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    public class AddOnPluginPicker : PopupWindowContent
    {

        #region Fields

        public SerializedProperty targetList;
        public List<UniversalPluginWrapper<AddOnPlugin>> target;


        public SerializedProperty targetProperty;

        private List<UniversalPluginInfo<AddOnPlugin>> plugins;
        private List<UniversalPluginInfo<AddOnPlugin>> filteredPlugins;
        private int selIndex;
        private long lastDownIndex;
        private long lastDownTicks;
        private string searchValue;
        private Vector2 scrollInfo;

        #endregion

        #region Unity Methods

        public override void OnGUI(Rect rect)
        {
            if (plugins == null)
            {

                editorWindow.minSize = editorWindow.maxSize = new Vector2(Mathf.Clamp(rect.width, 300, 900), 202);
                plugins = UniversalPluginManager<AddOnPlugin>.Plugins;
                filteredPlugins = new List<UniversalPluginInfo<AddOnPlugin>>();
                filteredPlugins.AddRange(plugins);
            }

            GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"), GUILayout.Height(20));
            string newSearch = GUILayout.TextField(searchValue, GDTKEditor.GetSearchbarStyle());
            if (GUILayout.Button(string.Empty, GDTKEditor.GetSearchbarCancelStyle()))
            {
                newSearch = string.Empty;
                GUI.FocusControl(null);
            }
            GUILayout.EndHorizontal();

            if (newSearch != searchValue)
            {
                selIndex = 0;
                filteredPlugins.Clear();
                filteredPlugins.AddRange(plugins.Where(_ => _.plugin.title.ToLower().Contains(newSearch.ToLower())));
                searchValue = newSearch;
            }

            int newIndex = selIndex;
            scrollInfo = GUILayout.BeginScrollView(scrollInfo, GUILayout.ExpandHeight(true));
            for (int i = 0; i < filteredPlugins.Count; i++)
            {
                GUILayout.BeginVertical(i == selIndex ? GDTKEditor.Styles.ListItemHover : GDTKEditor.Styles.ListItem);
                GUILayout.BeginHorizontal();

                GUILayout.Label(filteredPlugins[i].plugin.title, i == selIndex ? GDTKEditor.Styles.ListItemTextHover : GDTKEditor.Styles.ListItemText);

                GUILayout.EndHorizontal();
                GUILayout.EndVertical();

                if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {
                        if (lastDownIndex == i && TimeSpan.FromTicks(DateTime.Now.Ticks - lastDownTicks) <= TimeSpan.FromSeconds(0.25f))
                        {
                            if (target != null)
                            {
                                if (targetList != null)
                                {
                                    targetList.serializedObject.Update();
                                }
                                target.Add(new UniversalPluginWrapper<AddOnPlugin>((AddOnPlugin)Activator.CreateInstance(filteredPlugins[i].plugin.GetType())));
                                if (targetList != null)
                                {
                                    targetList.serializedObject.ApplyModifiedProperties();
                                    EditorUtility.SetDirty(targetList.serializedObject.targetObject);
                                }
                            }
                            editorWindow.Close();
                        }
                        else
                        {
                            newIndex = i;
                            editorWindow.Repaint();
                        }

                        lastDownIndex = i;
                        lastDownTicks = DateTime.Now.Ticks;
                    }
                }
            }
            GUILayout.EndScrollView();

            GUILayout.BeginVertical(GDTKEditor.Styles.DetailsPanel, GUILayout.Height(56), GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            if (filteredPlugins.Count > selIndex)
            {
                GUILayout.Label(filteredPlugins[selIndex].plugin.title, GDTKEditor.Styles.SubHeaderStyle);
                GUILayout.Space(-4);
                GUILayout.Label(filteredPlugins[selIndex].plugin.description, GDTKEditor.Styles.DescriptionStyle, GUILayout.Height(30));
            }
            else
            {
                GUILayout.Label("No Results", GDTKEditor.Styles.SubHeaderStyle);
                GUILayout.Space(-4);
                GUILayout.Label("There are no plugins that fit the system and/or search requirements.", GDTKEditor.Styles.DescriptionStyle, GUILayout.Height(30));
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();


            selIndex = newIndex;
        }

        #endregion

    }
}
#endif
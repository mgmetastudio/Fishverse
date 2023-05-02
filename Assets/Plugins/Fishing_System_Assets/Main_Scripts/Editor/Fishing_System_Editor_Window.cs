using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Fishing_System_Editor_Window : EditorWindow
{
    string Version = "1.0";

    bool InputsFoldout = false;
    bool Axe_1Foldout = false;
    bool Axe_2Foldout = false;
    bool PlayerprefsFoldout = false;

    [MenuItem("Fishing System/Main and Setup Window")]
    public static void Init()
    {
        const int width = 800;
        const int height = 1000;

        var x = (Screen.currentResolution.width - width) / 2;
        var y = (Screen.currentResolution.height - height) / 2;

        GetWindow<Fishing_System_Editor_Window>().position = new Rect(x, y, width, height);
    }

    void OnGUI()
    {
        //Debug.Log("Window width: " + position.width);
        //Debug.Log("Window height: " + position.height);
        GUILayout.Label("Fishing System", EditorStyles.boldLabel);
        GUILayout.Label("Version: " + Version, EditorStyles.boldLabel);

        GUILayout.Label("For the fishing system to work properly, you need to add these tags, layers & input axes:", EditorStyles.boldLabel);
        GUILayout.Label("Tags:", EditorStyles.label);
        EditorGUILayout.TextField("Pause_Menu, Currency_System, Trigger_Colliders, Backpack_System, Fishing_Area, Fish_Swim_Area, Fishing_Action_System, ", EditorStyles.textField);
        EditorGUILayout.TextField("Fishing_Float, Float_Move_Ground, Fish, Buy_Items_Canvas, Catched_Marlin, Fishing_Rod_Line_Point, Fish_Destination, Level_System, ", EditorStyles.textField);
        EditorGUILayout.TextField("Fishing_Aim_Trigger", EditorStyles.textField);
        GUILayout.Label("All listed tags should have been added automatically after opening the scene: ''Fishing_Test_Scene''.", EditorStyles.boldLabel);
        EditorGUILayout.TextField("Assets/Fishing_System_Assets/Scenes/Fishing_Test_Scene.unity", EditorStyles.textArea);
        /*if (GUILayout.Button("Add all tags automatically"))
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            //SerializedProperty layersProp = tagManager.FindProperty("layers");

            string Tags = "";
            //string Layers = "Overlay";

            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
            n.stringValue = Tags;

            /*SerializedProperty sp = layersProp.GetArrayElementAtIndex(29);
            if (sp != null) sp.stringValue = Layers;*/

        //tagManager.ApplyModifiedProperties();
        //}
        GUILayout.Label("Layer:", EditorStyles.label);
        EditorGUILayout.TextField("Overlay");
        GUILayout.Label("(Add the layer 'Overlay' to 'User Layer 29' manually.)", EditorStyles.boldLabel);
        if (GUILayout.Button("Show All Tags & Layers"))
        {
            EditorApplication.ExecuteMenuItem("Edit/Project Settings/Tags and Layers");
        }

        InputsFoldout = EditorGUILayout.Foldout(InputsFoldout, "Input Axes:");
        if (InputsFoldout)
        {
            GUILayout.Label("(You have to add the two input axes manually.)", EditorStyles.boldLabel);
            if (GUILayout.Button("Open Input Manager"))
            {
                EditorApplication.ExecuteMenuItem("Edit/Project Settings/Input");
            }
            Axe_1Foldout = EditorGUILayout.Foldout(Axe_1Foldout, "Axe 1:");
            if (Axe_1Foldout)
            {
                GUILayout.Label("Name:");
                EditorGUILayout.TextField("Action");
                GUILayout.Label("Positive Button:");
                EditorGUILayout.TextField("e");
                GUILayout.Label("Gravity:");
                EditorGUILayout.TextField("1000");
                GUILayout.Label("Dead:");
                EditorGUILayout.TextField("0.001");
                GUILayout.Label("Sensitivity:");
                EditorGUILayout.TextField("1000");
                GUILayout.Label("Type:");
                EditorGUILayout.TextField("Key or Mouse Button");
                GUILayout.Label("Axis:");
                EditorGUILayout.TextField("X axis");
                GUILayout.Label("Joy Num:");
                EditorGUILayout.TextField("Get Motion from all Joysticks");
            }
            Axe_2Foldout = EditorGUILayout.Foldout(Axe_2Foldout, "Axe 2:");
            if (Axe_2Foldout)
            {
                GUILayout.Label("Name:");
                EditorGUILayout.TextField("ActionF");
                GUILayout.Label("Positive Button:");
                EditorGUILayout.TextField("f");
                GUILayout.Label("Gravity:");
                EditorGUILayout.TextField("1000");
                GUILayout.Label("Dead:");
                EditorGUILayout.TextField("0.001");
                GUILayout.Label("Sensitivity:");
                EditorGUILayout.TextField("1000");
                GUILayout.Label("Type:");
                EditorGUILayout.TextField("Key or Mouse Button");
                GUILayout.Label("Axis:");
                EditorGUILayout.TextField("X axis");
                GUILayout.Label("Joy Num:");
                EditorGUILayout.TextField("Get Motion from all Joysticks");
            }
        }

        PlayerprefsFoldout = EditorGUILayout.Foldout(PlayerprefsFoldout, "PlayerPrefs");
        if (PlayerprefsFoldout)
        {
            if (GUILayout.Button("Delete All Saved PlayerPrefs"))
            {
                PlayerPrefs.DeleteAll();
            }
        }

        GUILayout.Label("Contact:", EditorStyles.label);
        GUILayout.Label("If you need help or have any questions please contact me via this e-mail:", EditorStyles.boldLabel);
        EditorGUILayout.TextField("Contact@RedicionStudio.com");
    }
}

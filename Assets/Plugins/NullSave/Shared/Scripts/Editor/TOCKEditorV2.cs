using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK
{
    public class TOCKEditorV2 : Editor
    {

        #region Constants

        private const string DRAG_DROP = "  Drag/Drop Here  ";

        #endregion

        #region Variables

        private GUISkin skin;
        private readonly Color proColor = new Color(0.9f, 0.9f, 0.9f, 1);
        private readonly Color freeColor = new Color(0.1f, 0.1f, 0.1f, 1);
        private Texture2D viewIcon, expandedIcon, collapsedIcon;
        private Dictionary<string, Texture2D> icons;
        private GUIStyle boxSub, boxGreen, boxRed, boxWhite, boxBlue, subItemBox, errorText;
        private string lastDir;

        private GUIStyle btnLeft, btnLeftPressed;
        private GUIStyle btnMid, btnMidPressed;
        private GUIStyle btnRight, btnRightPressed;

        internal readonly string[] bones = new string[] {"Hips", "LeftUpperLeg", "RightUpperLeg", "LeftLowerLeg", "RightLowerLeg", "LeftFoot", "RightFoot", "Spine", "Chest", "Neck", "Head",
                    "LeftShoulder", "RightShoulder", "LeftUpperArm", "RightUpperArm", "LeftLowerArm", "RightLowerArm", "LeftHand", "RightHand", "LeftToes", "RightToes", "LeftEye", "RightEye",
                    "Jaw", "LeftThumbProximal", "LeftThumbIntermediate", "LeftThumbDistal", "LeftIndexProximal", "LeftIndexIntermediate", "LeftIndexDistal", "LeftMiddleProximal",
                    "LeftMiddleIntermediate", "LeftMiddleDistal", "LeftRingProximal", "LeftRingIntermediate", "LeftRingDistal", "LeftLittleProximal", "LeftLittleIntermediate",
                    "LeftLittleDistal", "RightThumbProximal", "RightThumbIntermediate", "RightThumbDistal", "RightIndexProximal", "RightIndexIntermediate", "RightIndexDistal",
                    "RightMiddleProximal", "RightMiddleIntermediate", "RightMiddleDistal", "RightRingProximal", "RightRingIntermediate", "RightRingDistal", "RightLittleProximal",
                    "RightLittleIntermediate", "RightLittleDistal", "UpperChest", "LastBone" };

        #endregion

        #region Properties

        internal GUIStyle BoxBlue
        {
            get
            {
                if (boxBlue == null || boxBlue.normal.background == null)
                {
                    boxBlue = new GUIStyle(GUI.skin.box);
                    boxBlue.normal.textColor = Color.white;
                    boxBlue.normal.background = MakeTex(19, 19, new Color(0.227f, 0.4f, 0.722f));
                }

                return boxBlue;
            }
        }

        internal GUIStyle BoxGreen
        {
            get
            {
                if (boxGreen == null || boxGreen.normal.background == null)
                {
                    boxGreen = new GUIStyle(GUI.skin.box);
                    boxGreen.normal.textColor = Color.white;
                    boxGreen.normal.background = MakeTex(19, 19, new Color(0.427f, 0.722f, 0.4f));
                }

                return boxGreen;
            }
        }

        internal GUIStyle BoxRed
        {
            get
            {
                if (boxRed == null || boxRed.normal.background == null)
                {
                    boxRed = new GUIStyle(GUI.skin.box);
                    boxRed.normal.textColor = Color.white;
                    boxRed.normal.background = MakeTex(19, 19, new Color(0.72f, 0.4f, 0.4f));
                }

                return boxRed;
            }
        }

        internal GUIStyle BoxWhite
        {
            get
            {
                if (boxWhite == null || boxWhite.normal.background == null)
                {
                    boxWhite = new GUIStyle(GUI.skin.box);
                    boxWhite.normal.textColor = Color.white;
                    boxWhite.normal.background = MakeTex(19, 19, EditorGUIUtility.isProSkin ? Color.black : Color.white);
                }

                return boxWhite;
            }
        }

        internal GUIStyle BoxSub
        {
            get
            {
                if (boxSub == null || boxSub.normal.background == null)
                {
                    boxSub = new GUIStyle(GUI.skin.box);
                    boxSub.normal.textColor = Color.white;
                    boxSub.padding = new RectOffset(0, 0, 6, 0);
                    Color col = new Color(0.85f, 0.85f, 0.85f);
                    Color[] pix = new Color[19 * 19];

                    for (int i = 0; i < pix.Length; i++)
                    {
                        pix[i] = col;
                    }

                    Texture2D result = new Texture2D(19, 19);
                    result.SetPixels(pix);
                    result.Apply();

                    boxSub.normal.background = result;
                }

                return boxSub;
            }
        }

        internal GUIStyle ButtonLeft
        {
            get
            {
                if (btnLeft == null || btnLeft.normal.background == null)
                {
                    btnLeft = new GUIStyle("ButtonLeft");
                    btnLeft.alignment = TextAnchor.MiddleLeft;
                }

                return btnLeft;
            }
        }

        internal GUIStyle ButtonLeftPressed
        {
            get
            {
                if (btnLeftPressed == null || btnLeftPressed.normal.background == null)
                {
                    btnLeftPressed = new GUIStyle("ButtonLeft");
                    btnLeftPressed.normal = btnLeftPressed.active;
                    btnLeftPressed.alignment = TextAnchor.MiddleLeft;
                }

                return btnLeftPressed;
            }
        }

        internal GUIStyle ButtonMid
        {
            get
            {
                if (btnMid == null || btnMid.normal.background == null)
                {
                    btnMid = new GUIStyle("ButtonMid");
                    btnMid.alignment = TextAnchor.MiddleLeft;
                }

                return btnMid;
            }
        }

        internal GUIStyle ButtonMidPressed
        {
            get
            {
                if (btnMidPressed == null || btnMidPressed.normal.background == null)
                {
                    btnMidPressed = new GUIStyle("ButtonMid");
                    btnMidPressed.normal = btnMidPressed.active;
                    btnMidPressed.alignment = TextAnchor.MiddleLeft;
                }

                return btnMidPressed;
            }
        }

        internal GUIStyle ButtonRight
        {
            get
            {
                if (btnRight == null || btnRight.normal.background == null)
                {
                    btnRight = new GUIStyle("ButtonRight");
                    btnRight.alignment = TextAnchor.MiddleLeft;
                }

                return btnRight;
            }
        }

        internal GUIStyle ButtonRightPressed
        {
            get
            {
                if (btnRightPressed == null || btnRightPressed.normal.background == null)
                {
                    btnRightPressed = new GUIStyle("ButtonRight");
                    btnRightPressed.normal = btnRightPressed.active;
                    btnRightPressed.alignment = TextAnchor.MiddleLeft;
                }

                return btnRightPressed;
            }
        }

        internal Color EditorColor
        {
            get
            {
                if (EditorGUIUtility.isProSkin) return proColor;
                return freeColor;
            }
        }

        internal GUIStyle ErrorTextStyle
        {
            get
            {
                if (errorText == null || errorText.normal.background == null)
                {
                    errorText = new GUIStyle(GUI.skin.label);
                    errorText.normal.textColor = new Color(0.518f, 0.145f, 0.145f);
                    errorText.wordWrap = true;
                }

                return errorText;
            }
        }

        private Texture2D Icon { get; set; }

        internal GUISkin Skin
        {
            get
            {
                if (skin == null)
                {
                    if (EditorGUIUtility.isProSkin)
                    {
                        skin = Resources.Load("Skins/TOCK_SkinPro") as GUISkin;
                    }
                    else
                    {
                        skin = Resources.Load("Skins/TOCK_Skin") as GUISkin;
                    }
                }

                return skin;
            }
        }

        internal int View { get; set; }

        private Texture2D ViewIcon
        {
            get
            {
                if (viewIcon == null)
                {
                    viewIcon = (Texture2D)Resources.Load("Icons/view", typeof(Texture2D));
                }

                return viewIcon;
            }
        }

        internal Texture2D ExpandedIcon
        {
            get
            {
                if (expandedIcon == null)
                {
                    expandedIcon = (Texture2D)Resources.Load("Skins/tock_expanded");
                }
                return expandedIcon;
            }
        }

        internal Texture2D CollapsedIcon
        {
            get
            {
                if (collapsedIcon == null)
                {
                    collapsedIcon = (Texture2D)Resources.Load("Skins/tock_collapsed");
                }
                return collapsedIcon;
            }
        }

        internal GUIStyle SubSectionBox
        {
            get
            {
                if (subItemBox == null || subItemBox.normal.background == null)
                {
                    subItemBox = new GUIStyle(GUI.skin.box);
                    subItemBox.normal.background = MakeTex(19, 19, !EditorGUIUtility.isProSkin ? new Color(0, 0, 0, 0.1f) : new Color(1, 1, 1, 0.1f));
                    subItemBox.margin = new RectOffset(4, 4, 4, 4);
                }

                return subItemBox;
            }
        }

        #endregion

        #region Internal Methods v2.1

        internal ScriptableObject CreateNew(string objectTypeName, Type objectType)
        {
            if (string.IsNullOrWhiteSpace(lastDir))
            {
                lastDir = Application.dataPath;
            }
            string path = EditorUtility.SaveFilePanelInProject("Create New " + objectTypeName, "New " +
 objectTypeName, "asset", "Select a location to create the new " + objectTypeName + ".", lastDir);


            if (path.Length != 0)
            {
                lastDir = Path.GetDirectoryName(path) + "/";
                ScriptableObject addItem = ScriptableObject.CreateInstance(objectType);
                addItem.name = Path.GetFileNameWithoutExtension(path);
                AssetDatabase.CreateAsset(addItem, path);
                AssetDatabase.Refresh();

                return addItem;
            }

            return null;
        }

        internal static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }
            return assets;
        }

        internal void FoldoutNavigation(bool showUp, out bool moveUp, bool showDown, out bool moveDown, out bool delAtEnd)
        {
            Rect clickRect;

            if (showUp)
            {
                GUILayout.Label(GetIcon("Up", "Skins/up-circle"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
                clickRect = GUILayoutUtility.GetLastRect();
                if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
                {
                    moveUp = true;
                }
                else
                {
                    moveUp = false;
                }
            }
            else
            {
                GUILayout.Label(GetIcon("UpDis", "Skins/up-circle-dis"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
                moveUp = false;
            }

            if (showDown)
            {
                GUILayout.Label(GetIcon("Down", "Skins/down-circle"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
                clickRect = GUILayoutUtility.GetLastRect();
                if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
                {
                    moveDown = true;
                }
                else
                {
                    moveDown = false;
                }
            }
            else
            {
                GUILayout.Label(GetIcon("DownDis", "Skins/down-circle-dis"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
                moveDown = false;
            }

            GUILayout.Label(GetIcon("TrashWhite", "Skins/trash-white"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
            clickRect = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                delAtEnd = true;
            }
            else
            {
                delAtEnd = false;
            }
        }

        internal void FoldoutNavigation2(bool showUp, out bool moveUp, bool showDown, out bool moveDown, out bool delAtEnd)
        {
            Rect clickRect;
            Color c = GUI.contentColor;
            GUI.contentColor = EditorColor;

            if (showUp)
            {
                GUILayout.Label(new GUIContent(GetIcon("Up", "Skins/up-circle"), "Move Up"), ButtonMid, GUILayout.Height(21), GUILayout.Width(21));
                clickRect = GUILayoutUtility.GetLastRect();
                if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
                {
                    moveUp = true;
                }
                else
                {
                    moveUp = false;
                }
            }
            else
            {
                moveUp = false;
            }

            if (showDown)
            {
                GUILayout.Label(new GUIContent(GetIcon("Down", "Skins/down-circle"), "Move Down"), ButtonMid, GUILayout.Height(21), GUILayout.Width(21));
                clickRect = GUILayoutUtility.GetLastRect();
                if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
                {
                    moveDown = true;
                }
                else
                {
                    moveDown = false;
                }
            }
            else
            {
                moveDown = false;
            }

            GUILayout.Label(new GUIContent(GetIcon("TrashWhite", "Skins/trash-white"), "Delete"), ButtonRight, GUILayout.Height(21), GUILayout.Width(21));
            clickRect = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                delAtEnd = true;
            }
            else
            {
                delAtEnd = false;
            }

            GUI.contentColor = c;
        }

        internal void FoldoutTrashOnly(out bool delAtEnd, bool blackIcon = false)
        {
            Rect clickRect;

            GUILayout.Label(blackIcon ? GetIcon("TrashBlack", "Skins/trash-black") : GetIcon("TrashWhite", "Skins/trash-white"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
            clickRect = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                delAtEnd = true;
            }
            else
            {
                delAtEnd = false;
            }
        }

        internal Texture2D GetIcon(string name, string path)
        {
            if (icons == null) icons = new Dictionary<string, Texture2D>();

            if(icons.ContainsKey(name))
            {
                return icons[name];
            }

            icons.Add(name, (Texture2D)Resources.Load(path, typeof(Texture2D)));
            return icons[name];
        }

        internal bool HandleDragDrop(SerializedProperty list, Type acceptedType)
        {
            bool wasAdded = false;
            var dragAreaGroup = GUILayoutUtility.GetLastRect();
            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dragAreaGroup.Contains(Event.current.mousePosition))
                        break;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (var dragged in DragAndDrop.objectReferences)
                        {
                            if (dragged.GetType() == acceptedType || dragged.GetType().BaseType == acceptedType
                                || dragged.GetType().GetNestedTypes().Contains(acceptedType)
                                || (dragged is GameObject && ((GameObject)dragged).GetComponentInChildren(acceptedType) != null))
                            {
                                list.arraySize++;
                                list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = dragged;
                                wasAdded = true;
                            }
                        }
                    }
                    serializedObject.ApplyModifiedProperties();
                    Event.current.Use();
                    break;
            }

            return wasAdded;
        }

        internal void MainContainerBeginSlim()
        {
            serializedObject.Update();
            GUILayout.BeginVertical();
        }

        internal bool SectionToggle(int displayFlags, int flag, string title, Texture2D icon = null)
        {
            bool hasFlag = (displayFlags & flag) == flag;
            bool result = SectionGroup(title, icon, hasFlag);

            if (result != hasFlag)
            {
                displayFlags = result ? displayFlags | flag : displayFlags & ~flag;
                serializedObject.FindProperty("z_display_flags").intValue = (int)displayFlags;
            }

            return hasFlag;
        }

        internal bool SectionToggle(int displayFlags, int flag, string listName, Type acceptedType, string title, Texture2D icon = null)
        {
            bool hasFlag = (displayFlags & flag) == flag;
            bool result = SectionGroup(title, icon, hasFlag, listName, acceptedType);

            if (result != hasFlag)
            {
                displayFlags = result ? displayFlags | flag : displayFlags & ~flag;
                serializedObject.FindProperty("z_display_flags").intValue = (int)displayFlags;
            }

            return hasFlag;
        }

        internal bool SectionToggleWithButton(int displayFlags, int flag, string buttonText, out bool buttonPressed, string title, Texture2D icon = null)
        {
            bool hasFlag = (displayFlags & flag) == flag;
            bool result = SectionGroupWithButton(title, icon, hasFlag, buttonText, out buttonPressed);

            if (result != hasFlag)
            {
                displayFlags = result ? displayFlags | flag : displayFlags & ~flag;
                serializedObject.FindProperty("z_display_flags").intValue = (int)displayFlags;
            }

            return hasFlag;
        }

        internal bool SectionDropToggle(int displayFlags, int flag, string title, Texture2D icon = null, string listName = null, Type acceptedType = null)
        {
            bool hasFlag = (displayFlags & flag) == flag;
            bool result = SectionGroup(title, icon, hasFlag, listName, acceptedType);

            if (result != hasFlag)
            {
                displayFlags = result ? displayFlags | flag : displayFlags & ~flag;
                serializedObject.FindProperty("z_display_flags").intValue = (int)displayFlags;
            }

            return hasFlag;
        }

        internal bool SectionDropToggleWithButton(int displayFlags, int flag, string buttonText, out bool buttonPressed, string title, Texture2D icon = null, string listName = null, Type acceptedType = null)
        {
            bool hasFlag = (displayFlags & flag) == flag;
            bool result = SectionGroupWithButton(buttonText, out buttonPressed, title, icon, hasFlag, listName, acceptedType);

            if (result != hasFlag)
            {
                displayFlags = result ? displayFlags | flag : displayFlags & ~flag;
                serializedObject.FindProperty("z_display_flags").intValue = (int)displayFlags;
            }

            return hasFlag;
        }

        internal bool SectionGroup(string title, Texture2D icon, bool expand, string listName = null, Type acceptedType = null)
        {
            bool resValue = expand;
            SerializedProperty list = serializedObject.FindProperty(listName);
            bool displayList = list != null && acceptedType != null;

            // Top spacing
            GUILayout.Space(8);

            // Container start
            GUILayout.BeginHorizontal();

            // Expand collapse icon
            GUILayout.BeginVertical();
            Color res = GUI.color;
            if (displayList)
            {
                GUILayout.Space(7);
            }
            else
            {
                GUILayout.Space(5);
            }
            Texture2D texture = resValue ? ExpandedIcon : CollapsedIcon;
            GUI.color = EditorColor;
            GUILayout.Label(texture, GUILayout.Width(12));
            GUILayout.EndVertical();

            // Icon
            if (icon != null)
            {
                GUILayout.BeginVertical();
                if (displayList)
                {
                    GUILayout.Space(4);

                }
                GUILayout.Label(icon, GUILayout.Width(18), GUILayout.Height(18));
                GUILayout.EndVertical();
            }
            GUI.color = res;

            // Title
            GUILayout.BeginVertical();
            if (icon != null)
            {
                if (displayList)
                {
                    GUILayout.Space(4);
                }
                else
                {
                    GUILayout.Space(2);
                }
            }

            GUILayout.Label(title, Skin.GetStyle("SectionHeader"));
            GUILayout.EndVertical();

            // Drag and drop
            if (displayList)
            {
                GUILayout.BeginVertical();
                GUILayout.Space(4);
                GUI.color = EditorColor;
                GUILayout.Label(GetIcon("Drop", "Icons/tock-drop"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
                GUI.color = res;
                GUILayout.EndVertical();
            }

            GUILayout.FlexibleSpace();

            // Container End
            GUILayout.EndHorizontal();

            if(displayList)
            {
                if (ProcessDragDrop(list, acceptedType)) resValue = true;
            }

            // Toggle
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseUp)
            {
                resValue = !resValue;
                Repaint();
            }

            GUILayout.Space(4);

            return resValue;
        }

        internal bool SectionGroupWithButton(string title, Texture2D icon, bool expand, string buttonText, out bool buttonPressed)
        {
            bool resValue = expand;

            // Top spacing
            GUILayout.Space(8);

            // Container start
            GUILayout.BeginHorizontal();

            // Expand collapse icon
            GUILayout.BeginVertical();
            Color res = GUI.color;
            GUILayout.Space(5);
            Texture2D texture = resValue ? ExpandedIcon : CollapsedIcon;
            GUI.color = EditorColor;
            GUILayout.Label(texture, GUILayout.Width(12));
            GUILayout.EndVertical();

            // Icon
            if (icon != null)
            {
                GUILayout.BeginVertical();
                GUILayout.Label(icon, GUILayout.Width(18), GUILayout.Height(18));
                GUILayout.EndVertical();
            }
            GUI.color = res;

            // Title
            GUILayout.BeginVertical();
            if (icon != null)
            {
                GUILayout.Space(2);
            }

            GUILayout.Label(title, Skin.GetStyle("SectionHeader"));
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            // Button
            buttonPressed = false;
            if (GUILayout.Button(buttonText))
            {
                buttonPressed = true;
            }

            // Container End
            GUILayout.EndHorizontal();

            // Toggle
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseUp)
            {
                resValue = !resValue;
                Repaint();
            }

            GUILayout.Space(4);

            return resValue;
        }

        internal bool SectionGroupWithButton(string buttonText, out bool buttonPressed, string title, Texture2D icon, bool expand, string listName = null, Type acceptedType = null)
        {
            bool resValue = expand;
            SerializedProperty list = serializedObject.FindProperty(listName);
            bool displayList = list != null && acceptedType != null;

            // Top spacing
            GUILayout.Space(8);

            // Container start
            GUILayout.BeginHorizontal();

            // Expand collapse icon
            GUILayout.BeginVertical();
            Color res = GUI.color;
            if (displayList)
            {
                GUILayout.Space(7);
            }
            else
            {
                GUILayout.Space(5);
            }
            Texture2D texture = resValue ? ExpandedIcon : CollapsedIcon;
            GUI.color = EditorColor;
            GUILayout.Label(texture, GUILayout.Width(12));
            GUILayout.EndVertical();

            // Icon
            if (icon != null)
            {
                GUILayout.BeginVertical();
                if (displayList)
                {
                    GUILayout.Space(4);

                }
                GUILayout.Label(icon, GUILayout.Width(18), GUILayout.Height(18));
                GUILayout.EndVertical();
            }
            GUI.color = res;

            // Title
            GUILayout.BeginVertical();
            if (icon != null)
            {
                if (displayList)
                {
                    GUILayout.Space(4);
                }
                else
                {
                    GUILayout.Space(2);
                }
            }

            GUILayout.Label(title, Skin.GetStyle("SectionHeader"));
            GUILayout.EndVertical();

            // Drag and drop
            if (displayList)
            {
                GUILayout.BeginVertical();
                GUI.color = EditorColor;
                GUILayout.Space(4);
                GUILayout.Label(GetIcon("Drop", "Icons/tock-drop"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
                GUI.color = res;
                GUILayout.EndVertical();
            }


            GUILayout.FlexibleSpace();

            buttonPressed = GUILayout.Button(buttonText);

            // Container End
            GUILayout.EndHorizontal();

            // Drag and drop
            if (displayList)
            {
                if (ProcessDragDrop(list, acceptedType)) resValue = true;
            }


            // Toggle
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseUp)
            {
                resValue = !resValue;
                Repaint();
            }
        
            GUILayout.Space(4);

            return resValue;
        }

        internal void SectionHeader(string title, string listName = null, Type acceptedType = null)
        {
            SerializedProperty list = serializedObject.FindProperty(listName);
            bool displayList = list != null && acceptedType != null;

            GUILayout.Space(displayList ? 8 : 12);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);

            if (!displayList)
            {
                GUILayout.Label(title, Skin.GetStyle("SectionHeader"));
            }
            else
            {
                GUILayout.BeginVertical();
                GUILayout.Space(5);
                GUILayout.Label(title, Skin.GetStyle("SectionHeader"));
                GUILayout.EndVertical();

                // Drag and drop
                if (displayList)
                {
                    Color res = GUI.color;
                    GUILayout.BeginVertical();
                    GUI.color = EditorColor;
                    GUILayout.Space(4);
                    GUILayout.Label(GetIcon("Drop", "Icons/tock-drop"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
                    GUI.color = res;
                    GUILayout.EndVertical();
                    GUILayout.FlexibleSpace();
                }

            }

            GUILayout.EndHorizontal();

            if(displayList)
            {
                ProcessDragDrop(list, acceptedType);
            }

            GUILayout.Space(4);
        }

        internal void SectionHeaderWithButton(string title, string buttonText, out bool buttonPressed, string listName = null, Type acceptedType = null)
        {
            SerializedProperty list = serializedObject.FindProperty(listName);
            bool displayList = list != null && acceptedType != null;

            GUILayout.Space(displayList ? 8 : 12);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);

            if (!displayList)
            {
                GUILayout.Label(title, Skin.GetStyle("SectionHeader"));
            }
            else
            {
                GUILayout.BeginVertical();
                GUILayout.Space(5);
                GUILayout.Label(title, Skin.GetStyle("SectionHeader"));
                GUILayout.EndVertical();

                // Drag and drop
                if (displayList)
                {
                    Color res = GUI.color;
                    GUILayout.BeginVertical();
                    GUI.color = EditorColor;
                    GUILayout.Space(4);
                    GUILayout.Label(GetIcon("Drop", "Icons/tock-drop"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
                    GUI.color = res;
                    GUILayout.EndVertical();
                    GUILayout.FlexibleSpace();
                }
            }

            GUILayout.FlexibleSpace();
            buttonPressed = GUILayout.Button(buttonText);

            GUILayout.EndHorizontal();

            if (displayList)
            {
                ProcessDragDrop(list, acceptedType);
            }

            GUILayout.Space(4);
        }

        internal bool SimpleBool(string propertyName)
        {
            return serializedObject.FindProperty(propertyName).boolValue;
        }

        internal void SimpleBool(string propertyName, bool value)
        {
            serializedObject.FindProperty(propertyName).boolValue = value;
        }

        internal bool SimpleBool(SerializedProperty property, string relativeName)
        {
            return property.FindPropertyRelative(relativeName).boolValue;
        }

        internal void SimpleBool(SerializedProperty property, string relativeName, bool value)
        {
            property.FindPropertyRelative(relativeName).boolValue = value;
        }

        internal int SimpleInt(string propertyName)
        {
            return serializedObject.FindProperty(propertyName).intValue;
        }

        internal void SimpleInt(string propertyName, int value)
        {
            serializedObject.FindProperty(propertyName).intValue = value;
        }

        internal int SimpleInt(SerializedProperty property, string relativeName)
        {
            return property.FindPropertyRelative(relativeName).intValue;
        }

        internal void SimpleInt(SerializedProperty property, string relativeName, int value)
        {
            property.FindPropertyRelative(relativeName).intValue = value;
        }

        internal void SubHeader(string title, string listName = null, Type acceptedType = null)
        {
            SerializedProperty list = serializedObject.FindProperty(listName);
            bool displayList = list != null && acceptedType != null;

            GUILayout.Space(displayList ? 8 : 12);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);

            if (!displayList)
            {
                GUILayout.Label(title, Skin.GetStyle("SubHeader"));
            }
            else
            {
                GUILayout.BeginVertical();
                GUILayout.Space(5);
                GUILayout.Label(title, Skin.GetStyle("SubHeader"));
                GUILayout.EndVertical();

                Color res = GUI.color;
                GUILayout.BeginVertical();
                GUI.color = EditorColor;
                GUILayout.Space(4);
                GUILayout.Label(GetIcon("Drop", "Icons/tock-drop"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
                GUI.color = res;
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();

                ProcessDragDrop(list, acceptedType);
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
        }

        internal void SubHeader(string title, string buttonText, out bool buttonPressed, string listName = null, Type acceptedType = null)
        {
            buttonPressed = false;
            SerializedProperty list = serializedObject.FindProperty(listName);
            bool displayList = list != null && acceptedType != null;

            GUILayout.Space(displayList ? 8 : 12);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);

            if (!displayList)
            {
                GUILayout.Label(title, Skin.GetStyle("SubHeader"));
            }
            else
            {
                GUILayout.BeginVertical();
                GUILayout.Space(5);
                GUILayout.Label(title, Skin.GetStyle("SubHeader"));
                GUILayout.EndVertical();

                Color res = GUI.color;
                GUILayout.BeginVertical();
                GUI.color = EditorColor;
                GUILayout.Space(4);
                GUILayout.Label(GetIcon("Drop", "Icons/tock-drop"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
                GUI.color = res;
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();

                buttonPressed = GUILayout.Button(buttonText);

                ProcessDragDrop(list, acceptedType);
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
        }

        #endregion

        #region Internal Methods v2

        internal void DragBox(string title)
        {
            GUILayout.BeginVertical();
            Color c = GUI.contentColor;
            GUI.contentColor = EditorColor;
            GUILayout.Box(title, "box", GUILayout.MinHeight(25), GUILayout.ExpandWidth(true));
            GUI.contentColor = c;
            GUILayout.EndVertical();
        }

        internal void DragBox(SerializedProperty list, Type acceptedType, string title = "  Drag/Drop Here  ")
        {
            GUILayout.BeginVertical();
            GUI.skin.box.alignment = TextAnchor.MiddleCenter;
            GUI.skin.box.normal.textColor = Color.white;

            DragBox(title);

            var dragAreaGroup = GUILayoutUtility.GetLastRect();
            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dragAreaGroup.Contains(Event.current.mousePosition))
                        break;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (var dragged in DragAndDrop.objectReferences)
                        {
                            if (dragged.GetType() == acceptedType || dragged.GetType().BaseType == acceptedType
                                || dragged.GetType().GetNestedTypes().Contains(acceptedType)
                                || (dragged is GameObject && ((GameObject)dragged).GetComponentInChildren(acceptedType) != null))
                            {
                                list.arraySize++;
                                list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = dragged;
                            }
                        }
                    }
                    serializedObject.ApplyModifiedProperties();
                    Event.current.Use();
                    break;
            }

            GUILayout.EndVertical();
        }

        internal void DragDropList(SerializedProperty list, Type acceptedType)
        {
            DragBox(list, acceptedType);

            EditorGUILayout.Separator();

            for (int i = 0; i < list.arraySize; i++)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("X", GUILayout.Width(24)))
                {
                    list.DeleteArrayElementAtIndex(i);
                }

                if (i < list.arraySize && i >= 0)
                {
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), new GUIContent(string.Empty, null, string.Empty));
                }

                GUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Add"))
            {
                list.arraySize++;
            }

            if (GUILayout.Button("Clear"))
            {
                list.arraySize = 0;
            }
            GUILayout.EndHorizontal();
        }

        internal void MainContainerBegin(string title, string image, bool useEditorColor = true)
        {
            serializedObject.Update();
            GUILayout.BeginVertical();
            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            Color c = GUI.color;

            if (Icon == null && !string.IsNullOrEmpty(image))
            {
                Icon = (Texture2D)Resources.Load(image, typeof(Texture2D));
            }

            if (Icon != null)
            {
                GUI.color = useEditorColor ? EditorColor : Color.white; ;
                GUILayout.Label((Texture2D)Resources.Load(image, typeof(Texture2D)), GUILayout.MaxHeight(28), GUILayout.MaxWidth(28));
            }

            GUI.color = EditorColor;
            GUILayout.BeginVertical();
            GUILayout.Space(6);
            GUILayout.Label(title, Skin.GetStyle("CogHeader"));
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUI.color = c;
        }

        internal void MainContainerEnd()
        {
            GUILayout.Space(8);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("©2019-2021 NULLSAVE", Skin.GetStyle("CogFooter"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }

        internal bool SectionGroup(SerializedObject serializedObject, string title, string collapseVar)
        {
            bool resValue = serializedObject.FindProperty(collapseVar).boolValue;

            GUILayout.Space(12);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);

            Texture2D texture = resValue ? ExpandedIcon : CollapsedIcon;

            Color res = GUI.color;
            if (EditorGUIUtility.isProSkin)
            {
                GUI.color = new Color(1, 1, 1, .6f);
            }
            else
            {
                GUI.color = new Color(0.2f, 0.2f, 0.2f, .8f);
            }
            GUILayout.Label(texture, GUILayout.Width(12));
            GUI.color = res;

            GUILayout.Label(title, Skin.GetStyle("SectionHeader"));

            GUILayout.EndHorizontal();

            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseUp)
            {
                serializedObject.FindProperty(collapseVar).boolValue = !resValue;
            }

            GUILayout.Space(4);

            return resValue;
        }

        internal bool SectionGroup(string title, bool expand)
        {
            bool resValue = expand;

            GUILayout.Space(12);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);

            Texture2D texture = resValue ? ExpandedIcon : CollapsedIcon;

            Color res = GUI.color;
            if (EditorGUIUtility.isProSkin)
            {
                GUI.color = new Color(1, 1, 1, .6f);
            }
            else
            {
                GUI.color = new Color(0.2f, 0.2f, 0.2f, .8f);
            }
            GUILayout.Label(texture, GUILayout.Width(12));
            GUI.color = res;

            GUILayout.Label(title, Skin.GetStyle("SectionHeader"));

            GUILayout.EndHorizontal();

            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseUp)
            {
                resValue = !resValue;
                Repaint();
            }

            GUILayout.Space(4);

            return resValue;
        }

        internal bool SectionGroup(string title, Texture2D icon, bool expand)
        {
            bool resValue = expand;

            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);

            Texture2D texture = resValue ? ExpandedIcon : CollapsedIcon;

            Color res = GUI.color;
            GUI.color = EditorColor;

            GUILayout.BeginVertical();
            GUILayout.Space(6);
            GUILayout.Label(texture, GUILayout.Width(12));
            GUILayout.EndVertical();
            GUILayout.Label(icon, GUILayout.Width(18), GUILayout.Height(18));

            GUI.color = res;

            GUILayout.BeginVertical();
            GUILayout.Space(2);
            GUILayout.Label(title, Skin.GetStyle("SectionHeader"));
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseUp)
            {
                resValue = !resValue;
                Repaint();
            }

            GUILayout.Space(4);

            return resValue;
        }

        internal void SimpleList(string listName, bool showAdd = true)
        {
            EditorGUILayout.Separator();
            SimpleList(serializedObject.FindProperty(listName), showAdd);
        }

        internal Vector2 SimpleList(string listName, Vector2 scrollPos, float maxHeight, int lineCount = 1)
        {
            EditorGUILayout.Separator();
            return SimpleList(serializedObject.FindProperty(listName), scrollPos, maxHeight, lineCount);
        }

        internal void SimpleList(string listName, Type acceptedType)
        {
            SimpleList(serializedObject.FindProperty(listName), acceptedType);
        }

        internal Vector2 SimpleList(string listName, Type acceptedType, Vector2 scrollPos, float maxHeight, int lineCount = 1)
        {
            return SimpleList(serializedObject.FindProperty(listName), acceptedType, scrollPos, maxHeight, lineCount);
        }

        internal void SimpleList(SerializedProperty list, Type acceptedType)
        {
            if (acceptedType != null)
            {
                DragBox(list, acceptedType);
                EditorGUILayout.Separator();
            }
            SimpleList(list);
        }

        internal Vector2 SimpleList(SerializedProperty list, Type acceptedType, Vector2 scrollPos, float maxHeight, int lineCount = 1)
        {
            DragBox(list, acceptedType);
            EditorGUILayout.Separator();
            return SimpleList(list, scrollPos, maxHeight, lineCount);
        }

        internal void SimpleMultiSelect(string propertyName)
        {
            EditorGUILayout.BeginHorizontal();
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            EditorGUILayout.LabelField(property.displayName, GUILayout.Width(EditorGUIUtility.labelWidth), GUILayout.ExpandWidth(false));
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            int newValue = EditorGUILayout.MaskField(property.intValue, property.enumDisplayNames);
            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = newValue;
            }
            EditorGUILayout.EndHorizontal();

        }

        internal void SimpleMultiSelect(string propertyName, string title)
        {
            EditorGUILayout.BeginHorizontal();
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            EditorGUILayout.LabelField(title, GUILayout.Width(EditorGUIUtility.labelWidth), GUILayout.ExpandWidth(false));
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            int newValue = EditorGUILayout.MaskField(property.intValue, property.enumDisplayNames);
            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = newValue;
            }
            EditorGUILayout.EndHorizontal();
        }

        internal void SimpleProperty(string propertyName)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(propertyName));
        }

        internal void SimpleProperty(string propertyName, string title)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(propertyName), new GUIContent(title, null, string.Empty));
        }

        internal void SimplePropertyRelative(SerializedProperty property, string relativeName)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative(relativeName));
        }

        internal void SimplePropertyRelative(SerializedProperty property, string relativeName, string title)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative(relativeName), new GUIContent(title, null, string.Empty));
        }

        internal string SimpleString(string propertyName)
        {
            return serializedObject.FindProperty(propertyName).stringValue;
        }

        internal void SimpleString(string propertyName, string value)
        {
            serializedObject.FindProperty(propertyName).stringValue = value;
        }

        internal void VerticalSpace(float pixels)
        {
            GUILayout.BeginVertical();
            GUILayout.Space(pixels);
            GUILayout.EndVertical();
        }

        internal void ViewSelect(string[] options)
        {
            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            GUILayout.Space(4);
            GUILayout.BeginVertical();
            GUILayout.Space(4);
            GUILayout.Label("View", Skin.GetStyle("SectionHeader"));
            GUILayout.EndVertical();
            GUILayout.Space(4);
            Color c = GUI.color;
            GUI.color = EditorColor;
            GUILayout.Label(ViewIcon, GUILayout.MaxHeight(21), GUILayout.MaxWidth(21));
            GUI.color = c;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            float oldHeight = EditorStyles.popup.fixedHeight;
            EditorStyles.popup.fixedHeight = 18;
            View = EditorGUILayout.Popup(View, options);
            EditorStyles.popup.fixedHeight = oldHeight;
            GUILayout.EndHorizontal();
        }

        #endregion

        #region Private Methods

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = col;
            }

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        private bool ProcessDragDrop(SerializedProperty list, Type acceptedType)
        {
            bool resValue = false;

            var dragAreaGroup = GUILayoutUtility.GetLastRect();
            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dragAreaGroup.Contains(Event.current.mousePosition))
                        break;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (var dragged in DragAndDrop.objectReferences)
                        {
                            if (dragged.GetType() == acceptedType || dragged.GetType().BaseType == acceptedType
                                || dragged.GetType().GetNestedTypes().Contains(acceptedType)
                                || (dragged is GameObject && ((GameObject)dragged).GetComponentInChildren(acceptedType) != null))
                            {
                                list.arraySize++;
                                list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = dragged;
                                resValue = true;
                            }
                        }
                    }
                    serializedObject.ApplyModifiedProperties();
                    Event.current.Use();
                    break;
            }

            return resValue;
        }

        private void SimpleList(SerializedProperty list, bool showAdd = true)
        {
            for (int i = 0; i < list.arraySize; i++)
            {
                if (i < list.arraySize && i >= 0)
                {
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), new GUIContent(string.Empty, null, string.Empty));
                }
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(8);
            GUILayout.BeginVertical();
            if (list.arraySize > 0)
            {
                GUILayout.Space(4);
                GUILayout.Label("Right-click item to remove", Skin.GetStyle("CogFooter"));
            }
            else
            {
                GUILayout.Label("{Empty}", Skin.GetStyle("CogFooter"));
            }
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            if (showAdd)
            {
                if (GUILayout.Button("Add"))
                {
                    list.arraySize++;
                }
            }
            if (GUILayout.Button("Clear")) { list.arraySize = 0; }
            GUILayout.EndHorizontal();
        }

        private Vector2 SimpleList(SerializedProperty list, Vector2 scrollPos, float maxHeight, int lineCount)
        {
            Vector2 result = Vector2.zero;
            float neededHeight = (EditorGUIUtility.singleLineHeight + 2) * lineCount * list.arraySize;
            
            if (neededHeight > maxHeight)
            {
                result = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(maxHeight));
            }

            if (list.arraySize > 0)
            {
                for (int i = 0; i < list.arraySize; i++)
                {
                    if (i < list.arraySize && i >= 0)
                    {
                        EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), new GUIContent(string.Empty, null, string.Empty));
                    }
                }
            }

            if (neededHeight > maxHeight)
            {
                GUILayout.EndScrollView();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(8);
            GUILayout.BeginVertical();
            if (list.arraySize > 0)
            {
                GUILayout.Label("Right-click item to remove", Skin.GetStyle("CogFooter"));
            }
            else
            {
                GUILayout.Label("{Empty}", Skin.GetStyle("CogFooter"));
            }
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add"))
            {
                list.arraySize++;
                result = new Vector2(0, (EditorGUIUtility.singleLineHeight + 2) * lineCount * list.arraySize);
            }
            if (GUILayout.Button("Clear")) { list.arraySize = 0; }
            GUILayout.EndHorizontal();

            return result;
        }

        #endregion

    }
}

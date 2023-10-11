#if GDTK
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.GDTK.Stats
{
    public class UIMenuItem
    {

        #region Menu Methods

        [MenuItem("GameObject/UI/GDTK: Stats/Background UI", false)]
        private static void AddBackgroundUI()
        {
            GameObject go = UITools.CreateAndParentUIControl("Background UI", 160, 30);

            TextMeshProUGUI tmpro = go.AddComponent<TextMeshProUGUI>();
            tmpro.text = "{title}";
            tmpro.alignment = TextAlignmentOptions.Left;
            tmpro.verticalAlignment = VerticalAlignmentOptions.Top;
            tmpro.fontSizeMin = 10;
            tmpro.enableAutoSizing = false;
            tmpro.color = Color.white;
            tmpro.fontSize = 18;

            Label label = go.AddComponent<Label>();
            label.text = "{title}";
            UnityEditorInternal.ComponentUtility.MoveComponentUp(label);

            BackgroundUI tmp = go.AddComponent<BackgroundUI>();
            tmp.labels = new List<TemplatedLabel>();
            tmp.labels.Add(new TemplatedLabel() { format = "{title}", target = label });
            UnityEditorInternal.ComponentUtility.MoveComponentUp(tmp);
            UnityEditorInternal.ComponentUtility.MoveComponentUp(tmp);

            Selection.activeObject = go;
        }


        [MenuItem("GameObject/UI/GDTK: Stats/Class UI", false)]
        private static void AddClassUI()
        {
            GameObject go = UITools.CreateAndParentUIControl("Class UI", 160, 30);

            TextMeshProUGUI tmpro = go.AddComponent<TextMeshProUGUI>();
            tmpro.text = "{title}";
            tmpro.alignment = TextAlignmentOptions.Left;
            tmpro.verticalAlignment = VerticalAlignmentOptions.Top;
            tmpro.fontSizeMin = 10;
            tmpro.enableAutoSizing = false;
            tmpro.color = Color.white;
            tmpro.fontSize = 18;

            Label label = go.AddComponent<Label>();
            label.text = "{title}";
            UnityEditorInternal.ComponentUtility.MoveComponentUp(label);

            ClassUI tmp = go.AddComponent<ClassUI>();
            tmp.labels = new List<TemplatedLabel>();
            tmp.labels.Add(new TemplatedLabel() { format = "{title}", target = label });
            UnityEditorInternal.ComponentUtility.MoveComponentUp(tmp);
            UnityEditorInternal.ComponentUtility.MoveComponentUp(tmp);

            Selection.activeObject = go;
        }

        [MenuItem("GameObject/UI/GDTK: Stats/Race UI", false)]
        private static void AddRaceLabel()
        {
            GameObject go = UITools.CreateAndParentUIControl("Race UI", 160, 30);

            TextMeshProUGUI tmpro = go.AddComponent<TextMeshProUGUI>();
            tmpro.text = "Label Text";
            tmpro.alignment = TextAlignmentOptions.Left;
            tmpro.verticalAlignment = VerticalAlignmentOptions.Top;
            tmpro.fontSizeMin = 10;
            tmpro.enableAutoSizing = false;
            tmpro.color = Color.white;
            tmpro.fontSize = 18;

            Label label = go.AddComponent<Label>();
            label.text = "Label Text";
            UnityEditorInternal.ComponentUtility.MoveComponentUp(label);

            RaceUI tmp = go.AddComponent<RaceUI>();
            tmp.labels = new List<TemplatedLabel>();
            tmp.labels.Add(new TemplatedLabel() { format = "{title}", target = label });
            UnityEditorInternal.ComponentUtility.MoveComponentUp(tmp);
            UnityEditorInternal.ComponentUtility.MoveComponentUp(tmp);

            Selection.activeObject = go;
        }

        [MenuItem("GameObject/UI/GDTK: Stats/Stat UI", false)]
        private static void AddStatLabel()
        {
            GameObject go = UITools.CreateAndParentUIControl("Stat UI", 160, 30);

            TextMeshProUGUI tmpro = go.AddComponent<TextMeshProUGUI>();
            tmpro.text = "Label Text";
            tmpro.alignment = TextAlignmentOptions.Left;
            tmpro.verticalAlignment = VerticalAlignmentOptions.Top;
            tmpro.fontSizeMin = 10;
            tmpro.enableAutoSizing = false;
            tmpro.color = Color.white;
            tmpro.fontSize = 18;

            Label label = go.AddComponent<Label>();
            label.text = "Label Text";
            UnityEditorInternal.ComponentUtility.MoveComponentUp(label);

            StatUI tmp = go.AddComponent<StatUI>();
            tmp.labels = new List<TemplatedLabel>();
            tmp.labels.Add(new TemplatedLabel() { format = "{value}", target = label });
            UnityEditorInternal.ComponentUtility.MoveComponentUp(tmp);
            UnityEditorInternal.ComponentUtility.MoveComponentUp(tmp);

            Selection.activeObject = go;
        }

        [MenuItem("GameObject/UI/GDTK: Stats/Stat Progressbar", false)]
        private static void AddStatProgressbar()
        {
            GameObject go = UITools.CreateAndParentUIControl("Stat Progressbar", 160, 10);

            GameObject goBkg = UITools.AddFullsizedChild("Background", go.transform);
            goBkg.AddComponent<CanvasRenderer>();
            Image imgBkg = goBkg.AddComponent<Image>();
            imgBkg.color = new Color(0, 0, 0, 0.8f);

            GameObject fill = UITools.AddFullsizedChild("Fill", goBkg.transform);
            Image imgFill = fill.AddComponent<Image>();
            imgFill.sprite = Resources.Load<Sprite>("flat-background");
            imgFill.color = Color.blue;
            RectTransform rtFill = fill.GetComponent<RectTransform>();
            rtFill.anchorMax = new Vector2(0, 1);
            rtFill.pivot = new Vector2(0, 1);

            Progressbar progressbar = go.AddComponent<Progressbar>();
            progressbar.targetGraphic = imgFill;

            StatProgressbar statSlider = go.AddComponent<StatProgressbar>();
            UnityEditorInternal.ComponentUtility.MoveComponentUp(statSlider);

            Selection.activeObject = go;
        }

        [MenuItem("GameObject/UI/GDTK: Stats/Stat Progressbar (with Icon)", false)]
        private static void AddStatProgressbarWithIcon()
        {
            Transform parent = Selection.activeGameObject != null ? Selection.activeGameObject.transform : null;
            var window = Resources.Load<GameObject>("Stats/Prefabs/Stat Progressbar (with Icon)");
            Selection.activeObject = GameObject.Instantiate(window, parent);
            Selection.activeObject.name = "Stat Progressbar";
        }

        [MenuItem("GameObject/UI/GDTK: Stats/Stat Slider", false)]
        private static void AddStatSlider()
        {
            GameObject go = UITools.CreateAndParentUIControl("Stat Slider", 160, 20);

            GameObject goBkg = UITools.AddAnchoredChild("Background", go.transform, 0, 0.25f, 1, 0.75f);
            goBkg.AddComponent<CanvasRenderer>();
            Image imgBkg = goBkg.AddComponent<Image>();
            imgBkg.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            imgBkg.type = Image.Type.Sliced;

            GameObject goFillArea = UITools.AddAnchoredChild("FillArea", go.transform, 0, 0.25f, 1, 0.75f, 5, 0, 15);
            GameObject goFill = UITools.AddLeftAlignedFullHeightChild("Fill", goFillArea.transform, 10);
            Image imgFill = goFill.AddComponent<Image>();
            imgFill.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            imgFill.type = Image.Type.Sliced;

            GameObject goHandleArea = UITools.AddFullsizedChild("Handle Area", go.transform, 10, 0, 10);
            GameObject goHandle = UITools.AddLeftAlignedFullHeightChild("Handle", goHandleArea.transform, 20);
            Image imgHandle = goHandle.AddComponent<Image>();
            imgHandle.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            imgHandle.type = Image.Type.Sliced;

            Slider slider = go.AddComponent<Slider>();
            slider.targetGraphic = imgHandle;
            slider.fillRect = goFill.GetComponent<RectTransform>();
            slider.handleRect = goHandle.GetComponent<RectTransform>();

            StatSlider statSlider = go.AddComponent<StatSlider>();
            UnityEditorInternal.ComponentUtility.MoveComponentUp(statSlider);

            Selection.activeObject = go;
        }


        [MenuItem("GameObject/UI/GDTK: Stats/Status Condition UI", false)]
        private static void NewStatusConditionUI()
        {
            Transform parent = Selection.activeGameObject != null ? Selection.activeGameObject.transform : null;
            var window = Resources.Load<GameObject>("Stats/Prefabs/Status Condition UI");
            Selection.activeObject = GameObject.Instantiate(window, parent);
            Selection.activeObject.name = "Status Condition UI";
        }


        //** LISTS **//


        [MenuItem("GameObject/UI/GDTK: Stats/Lists/Attribute List", false)]
        private static void NewAttributeList()
        {
            Transform parent = Selection.activeGameObject != null ? Selection.activeGameObject.transform : null;
            var window = Resources.Load<GameObject>("Stats/Prefabs/Attribute List");
            Selection.activeObject = GameObject.Instantiate(window, parent);
            Selection.activeObject.name = "Attribute List";
        }

        [MenuItem("GameObject/UI/GDTK: Stats/Lists/Class List", false)]
        private static void NewClassList()
        {
            Transform parent = Selection.activeGameObject != null ? Selection.activeGameObject.transform : null;
            var window = Resources.Load<GameObject>("Stats/Prefabs/Class List");
            Selection.activeObject = GameObject.Instantiate(window, parent);
            Selection.activeObject.name = "Class List";
        }

        [MenuItem("GameObject/UI/GDTK: Stats/Lists/Perk List", false)]
        private static void NewPerkList()
        {
            Transform parent = Selection.activeGameObject != null ? Selection.activeGameObject.transform : null;
            var window = Resources.Load<GameObject>("Stats/Prefabs/Perk List");
            Selection.activeObject = GameObject.Instantiate(window, parent);
            Selection.activeObject.name = "Perk List";
        }

        [MenuItem("GameObject/UI/GDTK: Stats/Lists/Stat List", false)]
        private static void NewStatList()
        {
            Transform parent = Selection.activeGameObject != null ? Selection.activeGameObject.transform : null;
            var window = Resources.Load<GameObject>("Stats/Prefabs/Stat List");
            Selection.activeObject = GameObject.Instantiate(window, parent);
            Selection.activeObject.name = "Stat List";
        }

        [MenuItem("GameObject/UI/GDTK: Stats/Lists/Status Condition List", false)]
        private static void NewStatusConditionList()
        {
            Transform parent = Selection.activeGameObject != null ? Selection.activeGameObject.transform : null;
            var window = Resources.Load<GameObject>("Stats/Prefabs/Status Condition List");
            Selection.activeObject = GameObject.Instantiate(window, parent);
            Selection.activeObject.name = "Status Condition List";
        }

        [MenuItem("GameObject/UI/GDTK: Stats/Lists/Status Effect List", false)]
        private static void NewStatusEffectList()
        {
            Transform parent = Selection.activeGameObject != null ? Selection.activeGameObject.transform : null;
            var window = Resources.Load<GameObject>("Stats/Prefabs/Status Effect List");
            Selection.activeObject = GameObject.Instantiate(window, parent);
            Selection.activeObject.name = "Status Effect List";
        }

        [MenuItem("GameObject/UI/GDTK: Stats/Lists/Trait List", false)]
        private static void NewTraitList()
        {
            Transform parent = Selection.activeGameObject != null ? Selection.activeGameObject.transform : null;
            var window = Resources.Load<GameObject>("Stats/Prefabs/Trait List");
            Selection.activeObject = GameObject.Instantiate(window, parent);
            Selection.activeObject.name = "Trait List";
        }

        //** WINDOWS **//

        [MenuItem("GameObject/UI/GDTK: Stats/Windows/Add-On Choice", false)]
        private static void NewAddOnWindow()
        {
            Transform parent = Selection.activeGameObject != null ? Selection.activeGameObject.transform : null;
            var window = Resources.Load<GameObject>("Stats/Prefabs/Add-On List Window");
            Selection.activeObject = GameObject.Instantiate(window, parent);
            Selection.activeObject.name = "Add-On List Window";
        }

        [MenuItem("GameObject/UI/GDTK: Stats/Windows/Select Background", false)]
        private static void NewBackgroundWindow()
        {
            Transform parent = Selection.activeGameObject != null ? Selection.activeGameObject.transform : null;
            var window = Resources.Load<GameObject>("Stats/Prefabs/Select Background Window");
            Selection.activeObject = GameObject.Instantiate(window, parent);
            Selection.activeObject.name = "Select Background Window";
        }

        [MenuItem("GameObject/UI/GDTK: Stats/Windows/Select Race", false)]
        private static void NewRaceWindow()
        {
            Transform parent = Selection.activeGameObject != null ? Selection.activeGameObject.transform : null;
            var window = Resources.Load<GameObject>("Stats/Prefabs/Select Race Window");
            Selection.activeObject = GameObject.Instantiate(window, parent);
            Selection.activeObject.name = "Select Race Window";
        }

        #endregion

    }
}
#endif
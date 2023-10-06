#if GDTK
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [CustomEditor(typeof(StatMapper))]
    public class StatMapperEditor : GDTKEditor
    {

        #region Fields

        private StatMapper myTarget;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            if (target is StatMapper mapper)
            {
                myTarget = mapper;
                foreach(GDTKStatMapperTarget st in myTarget.targets)
                {
                    st.UpdateOptions();
                }
            }
        }

        public override void OnInspectorGUI()
        {
            int removeAt = -1;

            MainContainerBegin();

            SectionHeader("Stats", GetIcon("icons/stats"));
            SimpleProperty("statSource");
            switch ((StatSourceReference)SimpleValue<int>("statSource"))
            {
                case StatSourceReference.DirectReference:
                    SimpleProperty("stats");
                    break;
                case StatSourceReference.FindInRegistry:
                    SimpleProperty("key");
                    break;
            }


            SerializedProperty list = serializedObject.FindProperty("targets");
            SectionHeader("Mappings", GetIcon("icons/navigation"));
            for(int i=0; i < myTarget.targets.Count; i++)
            {
                if(DrawMapping(myTarget.targets[i]))
                {
                    removeAt = i;
                }
            }

            if (GUILayout.Button(new GUIContent("Add Mapping", GetIcon("icons/add-small")), GUILayout.Height(24)))
            {
                GUI.FocusControl(null);
                myTarget.targets.Add(new GDTKStatMapperTarget());
                EditorUtility.SetDirty(myTarget);
            }

            if (removeAt > -1)
            {
                int preSize = list.arraySize;
                list.DeleteArrayElementAtIndex(removeAt);

                // This is an object so sometimes we need to delete twice to really clear
                if (list.arraySize == preSize)
                {
                    list.DeleteArrayElementAtIndex(removeAt);
                    GUI.FocusControl(null);
                }
            }

            MainContainerEnd();
        }

        #endregion

        #region Private Methods

        private bool DrawMapping(GDTKStatMapperTarget target)
        {
            bool wantsDelete = false;
            string title;

            if(target.target == null)
            {
                title = "No Target";
            }
            else
            {
                if(target.field == null)
                {
                    title = target.target.GetType().Name;
                }
                else
                {
                    title = target.target.GetType().Name + "." + target.fieldTarget;
                }
            }

            GUILayout.BeginHorizontal();

            DrawToggleBarTitleLeft(true, title);

            // Delete
            if (DrawBarIcon(Styles.ButtonRight, GetIcon("icons/trash")))
            {
                wantsDelete = true;
            }

            GUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();

            GUILayout.Space(-4);
            GUILayout.BeginVertical("box");

            target.target = (Component)EditorGUILayout.ObjectField("Target", target.target, typeof(Component), true);

            if(target.options == null)
            {
                target.UpdateOptions();
            }

            int sel = EditorGUILayout.Popup("Field", target.selOption, target.options);
            if(sel != target.selOption)
            {
                target.selOption = sel;
                target.fieldTarget = target.options[sel];
                target.field = target.target.GetType().GetField(target.fieldTarget);
            }

            if (target.isBool)
            {
                target.expression = EditorGUILayout.TextField("Expression", target.expression);
            }
            else
            {
                target.statName = EditorGUILayout.TextField("Stat Id", target.statName);
                target.biDirectional = EditorGUILayout.Toggle("Bi-Directional", target.biDirectional);
            }

            GUILayout.EndVertical();

            if(EditorGUI.EndChangeCheck())
            {
                target.UpdateOptions();
                EditorUtility.SetDirty(myTarget);
            }

            return wantsDelete;
        }

        #endregion

    }
}
#endif
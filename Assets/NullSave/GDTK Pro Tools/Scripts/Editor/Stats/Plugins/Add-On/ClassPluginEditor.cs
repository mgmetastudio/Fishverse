#if GDTK
using System.Collections.Generic;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [CustomUniversalPluginEditor(typeof(ClassPlugin))]
    public class ClassPluginEditor : UniversalPluginEditor
    {

        #region Fields

        StatsDatabase database;

        #endregion

        #region Public Methods

        public override void OnEnable()
        {
            database = Object.FindObjectOfType<StatsDatabase>();
        }

        public override void OnInspectorGUI()
        {
            AddOnMode mode = (AddOnMode)PropertyIntValue("mode");

            PropertyField("mode");

            switch (mode)
            {
                case AddOnMode.PickAny:
                    PropertyField("addCount");
                    PropertyField("excludeOwned");
                    break;
                case AddOnMode.AllInList:
                    if (database == null)
                    {
                        SimpleStringListProperty("optionIds", "List");
                    }
                    else
                    {
                        SimpleStringSearchListProperty("optionIds", "List", BuildList);
                    }
                    break;
                case AddOnMode.PickFromGroup:
                case AddOnMode.RandomFromGroup:
                    PropertyField("groupName");
                    PropertyField("addCount");
                    PropertyField("excludeOwned");
                    break;
                case AddOnMode.RandomFromList:
                case AddOnMode.PickFromList:
                    PropertyField("addCount");
                    if (database == null)
                    {
                        SimpleStringListProperty("optionIds", "List");
                    }
                    else
                    {
                        SimpleStringSearchListProperty("optionIds", "List", BuildList);
                    }
                    PropertyField("excludeOwned");
                    break;
            }
        }

        #endregion

        #region Private Methods

        private void BuildList(List<string> options)
        {
            foreach (GDTKClass attrib in database.classes)
            {
                options.Add(attrib.info.id);
            }
        }

        #endregion

    }
}
#endif
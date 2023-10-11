#if GDTK_Inventory2

using NullSave.GDTK.Inventory;
using System.Collections.Generic;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [CustomUniversalPluginEditor(typeof(AddItemPlugin))]
    public class AddItemPluginEditor : UniversalPluginEditor
    {

        #region Fields

        InventoryDatabase database;

        #endregion

        #region Public Methods

        public override void OnEnable()
        {
            database = Object.FindObjectOfType<InventoryDatabase>();
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
            foreach (GDTKItem item in database.items)
            {
                options.Add(item.info.id);
            }
        }

        #endregion

    }
}

#endif
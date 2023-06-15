using System.IO;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CreateAssetMenu(menuName = "TOCK/Inventory/Crafting Category", order = 0)]
    public class CraftingCategory : ScriptableObject
    {

        #region Variables

        public Sprite icon;
        public string displayName;
        [TextArea(2, 5)] public string description;
        public bool displayInList = true;
        public bool catUnlocked = true;

        #endregion

        #region Public Methods

        public void StateLoad(Stream stream, InventoryCog inventory)
        {
            // generic data
            displayInList = stream.ReadBool();
            catUnlocked = stream.ReadBool();
        }

        public void StateSave(Stream stream)
        {
            // Write generic data
            stream.WriteBool(displayInList);
            stream.WriteBool(catUnlocked);
        }

        #endregion

    }
}
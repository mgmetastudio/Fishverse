using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CreateAssetMenu(menuName = "TOCK/Inventory/Item Tag", order = 4)]
    public class InventoryItemUITag : ScriptableObject
    {

        #region Variables

        [Tooltip("Tag Icon")] public Sprite icon;
        [Tooltip("Color used to display icon")] public Color iconColor = Color.white;

        [Tooltip("Tag Text")] public string tagText = "Tag";
        [Tooltip("Color used to display text")] public Color textColor = Color.white;

        [Tooltip("Tag Category")] public string Category;
        [Tooltip("If checked this tag will not show up in the UI when on an item that is attached to another item")] public bool hideWhenAttached = false;

        #endregion

    }
}
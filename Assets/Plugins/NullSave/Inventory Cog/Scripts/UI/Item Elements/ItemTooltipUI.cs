using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class ItemTooltipUI : MonoBehaviour, IItemHost
    {

        #region Variables

        public TooltipLocation location;
        public Vector2 offset;

        public ItemUI itemUI;

        #endregion

        #region Properties

        public ItemUI Target { get; private set; }

        public InventoryCog Inventory { get; set; }

        public InventoryItem InventoryItem { get { return Target.Item; } set {; } }

        public LootItem LootItem { get { return null; } set {; } }

        #endregion

        #region Public Methods

        public void ShowTooltip(InventoryCog inventory, ItemUI item)
        {
            Target = item;
            Inventory = inventory;

            RectTransform rtTooltip = GetComponent<RectTransform>();
            RectTransform rtItem = item.gameObject.GetComponent<RectTransform>();

            Vector2 tooltipSize = GetElementSize(rtTooltip);
            Vector2 itemSize = GetElementSize(rtItem);

            switch(location)
            {
                case TooltipLocation.Left:
                    rtTooltip.position = new Vector3(rtItem.position.x - tooltipSize.x - itemSize.x + offset.x, rtItem.position.y - (tooltipSize.y / 2) + offset.y, rtItem.position.z);
                    break;
                case TooltipLocation.Right:
                    rtTooltip.position = new Vector3(rtItem.position.x + tooltipSize.x + itemSize.x + offset.x, rtItem.position.y - (tooltipSize.y / 2) + offset.y, rtItem.position.z);
                    break;
                case TooltipLocation.Top:
                    rtTooltip.position = new Vector3(rtItem.position.x - (tooltipSize.x / 2) + offset.x, rtItem.position.y - tooltipSize.y - itemSize.y + offset.y, rtItem.position.z);
                    break;
                case TooltipLocation.Bottom:
                    rtTooltip.position = new Vector3(rtItem.position.x - (tooltipSize.x / 2) + offset.x, rtItem.position.y + tooltipSize.y + itemSize.y + offset.y, rtItem.position.z);
                    break;
            }

            if(itemUI != null)
            {
                itemUI.valueModifier = item.valueModifier;
                itemUI.LoadItem(item.Inventory, item.Item);
            }

            UpdateChildren();
        }

        public void UpdateChildren()
        {
            ItemHostHelper.UpdateChildren(this, gameObject);
        }

        #endregion

        #region Private Methods

        private Vector2 GetElementSize(RectTransform rt)
        {
            if(rt.rect.width > 0)
            {
                return new Vector2(rt.rect.width, rt.rect.height);
            }
            return rt.sizeDelta;
        }

        #endregion

    }
}
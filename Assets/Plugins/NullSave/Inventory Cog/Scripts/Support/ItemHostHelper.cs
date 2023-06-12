using NullSave.TOCK.Stats;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class ItemHostHelper : MonoBehaviour
    {

        public static void UpdateChildren(IItemHost host, GameObject go)
        {
            if (host.Inventory == null) return;

            foreach(ItemText itemText in go.GetComponentsInChildren<ItemText>())
            {
                if (host.InventoryItem == null && host.LootItem == null)
                {
                    itemText.ClearText();
                }
                else
                {
                    itemText.UpdateText(host.Inventory, host.LootItem);
                    itemText.UpdateText(host.Inventory, host.InventoryItem);
                }
            }
        
            foreach (ItemContextActions ica in go.GetComponentsInChildren<ItemContextActions>())
            {
                ica.Inventory = host.Inventory;
                ica.UpdateContext(host.InventoryItem);
            }

            foreach (ItemContextButtons ica in go.GetComponentsInChildren<ItemContextButtons>())
            {
                ica.Inventory = host.Inventory;
                ica.UpdateContext(host.InventoryItem);
            }

            foreach (ItemUI itemUI in go.GetComponentsInChildren<ItemUI>())
            {
                if (host.InventoryItem != null)
                {
                    itemUI.LoadItem(host.Inventory, host.InventoryItem);
                }
                else if (host.LootItem != null)
                {
                    itemUI.LoadItem(host.Inventory, host.LootItem.item);
                }
            }

            foreach(StatUIList uiList in go.GetComponentsInChildren<StatUIList>())
            {
                uiList.statsCog = host.Inventory.StatsCog;
            }

#if INVENTORY_COG

            foreach (StatEffectListUI uiList in go.GetComponentsInChildren<StatEffectListUI>())
            {
                if (host.InventoryItem != null)
                {
                    uiList.LoadEffects(host.InventoryItem);
                }
                else if (host.LootItem != null)
                {
                    uiList.LoadEffects(host.LootItem.item);
                }
            }
#endif

            foreach (SkillSlotList uiList in go.GetComponentsInChildren<SkillSlotList>())
            {
                uiList.LoadSlots(host.Inventory);
            }

            foreach (AttachmentItemList uiList in go.GetComponentsInChildren<AttachmentItemList>())
            {
                uiList.Inventory = host.Inventory;
                if (host.InventoryItem != null)
                {
                    uiList.LoadAttachments(host.InventoryItem);
                }
                else if (host.LootItem != null)
                {
                    uiList.LoadAttachments(host.LootItem.item);
                }
            }

            foreach (ImageRarityColor ui in go.GetComponentsInChildren<ImageRarityColor>())
            {
                ui.Inventory = host.Inventory;
                if (host.InventoryItem != null)
                {
                    ui.SetRarity(host.InventoryItem.rarity);
                }
                else if (host.LootItem != null)
                {
                    ui.SetRarity(host.LootItem.item.rarity);
                }
            }

            foreach (RarityColorIndicator ui in go.GetComponentsInChildren<RarityColorIndicator>())
            {
                ui.ActiveTheme = host.Inventory.ActiveTheme;
                if (host.InventoryItem != null)
                {
                    ui.SetRarity(host.InventoryItem.rarity);
                }
                else if (host.LootItem != null)
                {
                    ui.SetRarity(host.LootItem.item.rarity);
                }
            }

            foreach (LoadoutListUI ui in go.GetComponentsInChildren<LoadoutListUI>())
            {
                ui.inventoryCog = host.Inventory;
            }

        }

    }
}
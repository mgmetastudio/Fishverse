using NullSave.TOCK.Stats;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class MenuHostHelper : MonoBehaviour
    {

        #region Public Methods

        public static void LoadComponents(IMenuHost host, GameObject go)
        {
            foreach (InventoryItemList itemList in go.GetComponentsInChildren<InventoryItemList>())
            {
                if (itemList.Inventory == null)
                {
                    itemList.Inventory = host.Inventory;
                    if (itemList.loadMode == ListLoadMode.OnEnable)
                    {
                        itemList.LoadItems();
                    }
                }

                itemList.ThemeHost = host.Inventory;
            }

            foreach (EquipPointUI itemList in go.GetComponentsInChildren<EquipPointUI>())
            {
                if (itemList.inventoryCog == null)
                {
                    itemList.inventoryCog = host.Inventory;
                    itemList.Refresh();
                }
            }

            foreach (RecipeList itemList in go.GetComponentsInChildren<RecipeList>())
            {
                if (itemList.Inventory == null)
                {
                    itemList.Inventory = host.Inventory;
                }
            }

            foreach (SlotItemUI itemList in go.GetComponentsInChildren<SlotItemUI>())
            {
                if (itemList.Inventory == null)
                {
                    itemList.Inventory = host.Inventory;
                }
                itemList.LoadSlot();
            }

            foreach (HotbarSlotUI itemList in go.GetComponentsInChildren<HotbarSlotUI>())
            {
                if (itemList.Inventory == null)
                {
                    itemList.Inventory = host.Inventory;
                }
            }

            foreach (SkillSlotUI itemList in go.GetComponentsInChildren<SkillSlotUI>())
            {
                if (itemList.inventorySource == null)
                {
                    itemList.inventorySource = host.Inventory;
                }
                itemList.Rebind();
            }

            foreach (SortUI ui in go.GetComponentsInChildren<SortUI>())
            {
                if (ui.Inventory == null)
                {
                    ui.Inventory = host.Inventory;
                }
            }

            foreach (CurrencyMonitor ui in go.GetComponentsInChildren<CurrencyMonitor>())
            {
                if (ui.inventoryCog == null)
                {
                    ui.inventoryCog = host.Inventory;
                }
            }

            foreach (Stats.StatUIList ui in go.GetComponentsInChildren<Stats.StatUIList>())
            {
                if (ui.statsCog == null)
                {
                    ui.statsCog = host.Inventory.StatsCog;
                }
            }

            foreach (Stats.SliderStat ui in go.GetComponentsInChildren<Stats.SliderStat>())
            {
                if (ui.statCog == null)
                {
                    ui.statCog = host.Inventory.StatsCog;
                    ui.Rebind();
                }
            }

            foreach (Stats.StatMonitorTMP ui in go.GetComponentsInChildren<Stats.StatMonitorTMP>())
            {
                if (ui.statsCog == null)
                {
                    ui.statsCog = host.Inventory.StatsCog;
                    ui.Rebind();
                }
            }

            foreach (Stats.StatImageValue ui in go.GetComponentsInChildren<Stats.StatImageValue>())
            {
                if (ui.statCog == null)
                {
                    ui.statCog = host.Inventory.StatsCog;
                    ui.Rebind();
                }
            }

            foreach (PlayerPreviewUI ui in go.GetComponentsInChildren<PlayerPreviewUI>())
            {
                ui.LoadPreview(host.Inventory);
            }

            foreach (CraftQueueList ui in go.GetComponentsInChildren<CraftQueueList>())
            {
                if(ui.Inventory == null)
                {
                    ui.Inventory = host.Inventory;
                    ui.LoadQueue();
                }
            }

            foreach(MerchantContextActions ui in go.GetComponentsInChildren<MerchantContextActions>())
            {
                ui.Inventory = host.Inventory;
            }

            foreach (SingleCheckoutUI ui in go.GetComponentsInChildren<SingleCheckoutUI>())
            {
                ui.playerInventory = host.Inventory;
            }

            foreach (CategoryList ui in go.GetComponentsInChildren<CategoryList>())
            {
                ui.Rebind();
            }

            foreach (BlindItemList ui in go.GetComponentsInChildren<BlindItemList>())
            {
                ui.ThemeHost = host.Inventory;
                ui.RefreshList();
            }

#if INVENTORY_COG

            foreach (EffectsList uiList in go.GetComponentsInChildren<EffectsList>())
            {
                uiList.statsCog = host.Inventory.StatsCog;
            }
#endif

            foreach (LoadoutListUI ui in go.GetComponentsInChildren<LoadoutListUI>())
            {
                ui.inventoryCog = host.Inventory;
            }
        }

        #endregion

    }
}
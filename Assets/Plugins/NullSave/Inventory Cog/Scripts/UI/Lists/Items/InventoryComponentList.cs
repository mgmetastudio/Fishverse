using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class InventoryComponentList : MonoBehaviour
    {

        #region Variables

        public ComponentListType listType;
        public InventoryItemList itemSource;
        public ComponentUI componentPrefab;
        public Transform componentContainer;
        public Sprite currencyImage;

        private List<ComponentUI> loadedComponents;

        #endregion

        #region Unity Methods

        private void OnDisable()
        {
            if (itemSource == null) return;
            itemSource.onSelectionChanged.RemoveListener(UpdateComponents);
        }

        private void OnEnable()
        {
            if (itemSource == null) return;
            itemSource.onSelectionChanged.AddListener(UpdateComponents);
            UpdateComponents(0);
        }

        #endregion

        #region Public Methods

        public void ClearComponents()
        {
            if (loadedComponents == null) return;
            foreach (ComponentUI component in loadedComponents)
            {
                Destroy(component.gameObject);
            }
            loadedComponents.Clear();
        }

        public void LoadItem(InventoryCog inventory, InventoryItem item)
        {
            ClearComponents();
            if (item == null) return;

            loadedComponents = new List<ComponentUI>();

            switch (listType)
            {
                case ComponentListType.Breakdown:
                    if (item.CanBreakdown)
                    {
                        foreach (ItemReference component in item.breakdownResult)
                        {
                            ComponentUI ui = Instantiate(componentPrefab, componentContainer);
                            ui.LoadComponent(component, inventory, false, 0, 0);
                            loadedComponents.Add(ui);
                        }
                    }
                    break;
                case ComponentListType.Repair:
                    if (item.CanRepair)
                    {
                        int maxCount = inventory.GetRepairMaxIncrements(item);
                        if(maxCount == 0)
                        {
                            maxCount = Mathf.RoundToInt((1 - item.condition) / item.repairIncrement);
                        }

                        if(item.incrementCost > 0)
                        {
                            ComponentUI ui = Instantiate(componentPrefab, componentContainer);
                            ui.LoadCurrency(inventory, true, item.incrementCost * maxCount, currencyImage);
                            loadedComponents.Add(ui);
                        }

                        foreach (ItemReference component in item.incrementComponents)
                        {
                            ComponentUI ui = Instantiate(componentPrefab, componentContainer);
                            ui.LoadComponent(component, inventory, true, 0, 0, maxCount);
                            loadedComponents.Add(ui);
                        }
                    }
                    break;
            }
        }

        #endregion

        #region Private Methods

        private void UpdateComponents(int index)
        {
            ClearComponents();
            if (itemSource.SelectedItem == null) return;
            LoadItem(itemSource.Inventory, itemSource.SelectedItem.Item);
        }

        #endregion

    }
}
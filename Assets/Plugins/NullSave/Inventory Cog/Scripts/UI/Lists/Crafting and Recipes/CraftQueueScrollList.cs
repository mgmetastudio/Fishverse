using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    [RequireComponent(typeof(ScrollRect))]
    public class CraftQueueScrollList : CraftQueueList
    {

        #region Variables

        // UI
        public CraftQueueUI itemPrefab;

        private List<CraftQueueUI> loadedItems;
        private ScrollRect scrollRect;
        private RectTransform viewPort, content;

        private List<string> lastCategoryFilter;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
            viewPort = scrollRect.viewport;
            content = viewPort.transform.GetChild(0).gameObject.GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            if (Inventory != null && loadMode == ListLoadMode.OnEnable)
            {
                LoadQueue();
            }
        }

        private void Start()
        {
            if (Inventory != null && loadMode == ListLoadMode.OnEnable)
            {
                LoadQueue();
            }
        }

        #endregion

        #region Public Methods

        public void ClearItems()
        {
            if (loadedItems != null)
            {
                foreach (CraftQueueUI ui in loadedItems)
                {
                    Destroy(ui.gameObject);
                }
            }
            loadedItems = new List<CraftQueueUI>();
        }

        public override void LoadQueue()
        {
            if (Inventory == null) return;

            ClearItems();
            Inventory.onCraftQueued.RemoveListener(AddItem);
            Inventory.onQueuedCraftComplete.RemoveListener(RemoveItem);
            Inventory.onCraftQueued.AddListener(AddItem);
            Inventory.onQueuedCraftComplete.AddListener(RemoveItem);

            foreach (CraftingQueueItem queueItem in Inventory.CraftingQueue)
            {
                CraftQueueUI ui = Instantiate(itemPrefab, content);
                ui.LoadQueue(queueItem);
                loadedItems.Add(ui);
            }
        }

        #endregion

        #region Private Methods

        private void AddItem(CraftingRecipe recipe, int count)
        {
            CraftQueueUI ui = Instantiate(itemPrefab, content);
            foreach(CraftingQueueItem cqi in Inventory.CraftingQueue)
            {
                if(cqi.recipe == recipe && cqi.count == count)
                {
                    ui.LoadQueue(cqi);
                    break;
                }
            }
            loadedItems.Add(ui);
        }

        private void RemoveItem(CraftingRecipe recipe, int count)
        {
            foreach (CraftQueueUI ui in loadedItems)
            {
                if(ui.item.recipe == recipe && ui.item.count == count)
                {
                    Destroy(ui.gameObject);
                    loadedItems.Remove(ui);
                    return;
                }
            }
        }

        #endregion

    }
}
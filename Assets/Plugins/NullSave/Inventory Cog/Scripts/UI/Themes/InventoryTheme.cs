using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CreateAssetMenu(menuName = "TOCK/Inventory/Theme", order = 6)]
    public class InventoryTheme : ScriptableObject
    {

        #region Variables

        public string displayName;
        public string description;
        public string spawnTag = "InventoryUI";

        public bool enableTriggers = true;
        public bool enableRaycast;
        public Vector3 raycastOffset;
        public float maxDistance = 5f;
        public bool enableUIClick = true;
        public RaycastSource raycastSource;
        public LayerMask raycastCulling;

        public PromptUI promptUI;
        public string itemText = "Loot";
        public string merchantText = "Shop";
        public string containerText = "Open";
        public string craftingText = "Craft";

        public GeneralUI generalUI;
        public InventoryMenuUI inventoryMenu;
        public ItemManagerMenuUI itemManagerMenu;
        public ItemContainerMenuUI itemContainerMenu;
        public CraftMenuUI craftingMenu;
        public CraftDetailMenuUI craftDetailMenu;
        public MerchantMenuUI merchantMenu;
        public TradeMenuUI tradeMenu;
        public ContainerMenuUI containerMenu;
        public AttachmentsUI attachments;
        public RenamePrompt renamePrompt;

        // Counts
        public CountSelectUI countSelection;
        public int minCount = 5;
        [TextArea] public string sellPrompt = "How many would you like to sell?";
        [TextArea] public string buyPrompt = "How many would you like to buy?";
        [TextArea] public string dropPrompt = "How many would you like to drop?";
        [TextArea] public string consumePrompt = "How many would you like to use?";
        [TextArea] public string breakdownPrompt = "How many would you like to breakdown?";
        [TextArea] public string craftPrompt = "How many would you like to craft?";
        [TextArea] public string genericPrompt = "Select Count";

        // Rarity
        public RarityLevel[] rarityLevels = new RarityLevel[] {
            new RarityLevel() { name = "Scrap", color = new Color(1, 1, 1) },
            new RarityLevel() { name = "Flimsy", color = new Color(0.6941177f, 0.6941177f, 0.6941177f) },
            new RarityLevel() { name = "Common", color = new Color(0.2117647f, 0.9568628f, 0.6666667f) },
            new RarityLevel() { name = "Uncommon", color = new Color(0.3607843f, 0.5960785f, 0.8431373f) },
            new RarityLevel() { name = "Unique", color = new Color(0.3019608f, 0.454902f, 0.3333333f) },
            new RarityLevel() { name = "Rare", color = new Color(0.7215686f, 0.9333334f, 0.01568628f) },
            new RarityLevel() { name = "Renowned", color = new Color(1, 0.8470589f, 0.003921569f) },
            new RarityLevel() { name = "Extraordinary", color = new Color(0.8078432f, 0.4117647f, 0.1921569f) },
            new RarityLevel() { name = "Legendary", color = new Color(0.8156863f, 0.172549f, 0.8901961f) },
            new RarityLevel() { name = "Epic", color = new Color(0.5254902f, 0.3176471f, 0.6784314f) },
            new RarityLevel() { name = "Mythic", color = new Color(0.9921569f, 0.2941177f, 0.2941177f) },
        };

        // Instances
        private InventoryCog host;
        private Canvas targetCanvas;
        private List<GameObject> promptTargets;

        // Wait target
        float remainder;
        GameObject target;

        private List<GameObject> hiddenItems;
        private GameObject lastRayHit;

        #endregion

        #region Properties

        public List<GameObject> ActiveMenus { get; private set; }

        public GameObject ActivePrompt { get; private set; }

        public GeneralUI ActiveUI { get; private set; }

        #endregion

        #region Public Methods

        public void ClearUI()
        {
            ICloseable closeable;
            while (ActiveMenus.Count > 0)
            {
                closeable = ActiveMenus[0].GetComponent<ICloseable>();
                if (closeable != null) closeable.onCloseCalled?.Invoke();
                Destroy(ActiveMenus[0]);
                ActiveMenus.RemoveAt(0);
                host.onMenuClose?.Invoke();
            }

            HidePrompts();
        }

        public void CloseMenu()
        {
            if (ActiveMenus == null || ActiveMenus.Count < 1) return;

            ICloseable closeable = ActiveMenus[0].GetComponent<ICloseable>();
            if (closeable != null) closeable.onCloseCalled?.Invoke();
            Destroy(ActiveMenus[0]);
            ActiveMenus.RemoveAt(0);
            host.onMenuClose?.Invoke();
        }

        public void CloseMenu(float destroyDelay)
        {
            if (ActiveMenus == null || ActiveMenus.Count < 1) return;

            ICloseable closeable = ActiveMenus[0].GetComponent<ICloseable>();
            if (closeable != null) closeable.onCloseCalled?.Invoke();
            if (target != null)
            {
                Destroy(target);
            }
            target = ActiveMenus[0];
            remainder = destroyDelay;
            ActiveMenus.RemoveAt(0);
            host.onMenuClose?.Invoke();
        }

        public void CloseMenu(GameObject target)
        {
            if (ActiveMenus == null || ActiveMenus.Count < 1 || target == null) return;

            ICloseable closeable = target.GetComponent<ICloseable>();
            if (closeable != null) closeable.onCloseCalled?.Invoke();
            ActiveMenus.Remove(target);
            Destroy(target);
            host.onMenuClose?.Invoke();
        }

        public void HidePrompts()
        {
            if (ActivePrompt != null)
            {
                Destroy(ActivePrompt.gameObject);
            }
        }

        /// <summary>
        /// Hides all UI elements, call ShowUI to restore
        /// </summary>
        public void HideUI()
        {
            foreach (GameObject go in ActiveMenus)
            {
                go.SetActive(false);
                hiddenItems.Add(go);
            }

            foreach (GameObject go in promptTargets)
            {
                go.SetActive(false);
                hiddenItems.Add(go);
            }

            if (ActiveUI != null)
            {
                ActiveUI.gameObject.SetActive(false);
                hiddenItems.Add(ActiveUI.gameObject);
            }
        }

        public void Initialize(InventoryCog owner, InventoryTheme previousTheme)
        {
            // Reset values
            targetCanvas = null;
            host = owner;
            ActiveMenus = new List<GameObject>();
            ActivePrompt = null;
            promptTargets = new List<GameObject>();
            hiddenItems = new List<GameObject>();

            // Find canvas in children
            foreach (Canvas canvas in owner.gameObject.GetComponentsInChildren<Canvas>())
            {
                if (canvas.gameObject.tag == spawnTag)
                {
                    targetCanvas = canvas;
                    break;
                }
            }

            // Find target canvas (in world)
            if (targetCanvas == null)
            {
                foreach (GameObject go in GameObject.FindGameObjectsWithTag(spawnTag))
                {
                    Canvas target = go.GetComponent<Canvas>();
                    if (target != null)
                    {
                        targetCanvas = target;
                        break;
                    }
                }
            }

            // Create General UI
            if (generalUI != null)
            {
                ActiveUI = Instantiate(generalUI, targetCanvas.transform);
                ActiveUI.Inventory = host;
                ActiveUI.LoadComponents();
            }

            if (previousTheme != null)
            {
                foreach (GameObject go in previousTheme.promptTargets)
                {
                    TriggerEnter(go);
                }
            }

            // Deal with previous theme
            if (previousTheme != null)
            {
                previousTheme.Shutdown();
            }

            // Check for issues
            if (targetCanvas == null)
            {
                Debug.LogError("Theme could not find a canvas with the tag '" + spawnTag + "'");
            }
        }

        public void OpenAttachments(InventoryItem item, System.Action onCloseCallback)
        {
            if (attachments != null)
            {
                // Ensure we don't already have this menu open
                foreach (GameObject obj in ActiveMenus)
                {
                    if (obj.GetComponentInChildren<AttachmentsUI>() != null) return;
                }

                AttachmentsUI newMenu = Instantiate(attachments, targetCanvas.transform);
                newMenu.Inventory = host;

                System.Action localClose = new System.Action(() =>
                {
                    CloseMenu(newMenu.gameObject);
                    onCloseCallback?.Invoke();
                });

                newMenu.LoadItem(host, item, localClose);

                ActiveMenus.Insert(0, newMenu.gameObject);
            }
            else
            {
                Debug.LogError("No attachments menu selected for theme.");
            }
        }

        public void OpenCountSelect(InventoryItem item, string prompt, System.Action<bool, int> onCloseCallback)
        {
            if (countSelection == null)
            {
                Debug.LogWarning("No count selection window select for theme; defaulting to 1");
                onCloseCallback?.Invoke(true, 1);
                return;
            }

            CountSelectUI newMenu = Instantiate(countSelection, targetCanvas.transform);

            System.Action<bool, int> localClose = new System.Action<bool, int>((bool confirm, int count) =>
            {
                CloseMenu(newMenu.gameObject);
                onCloseCallback?.Invoke(confirm, count);
            });

            newMenu.SelectCount(item.displayName, prompt, 0, host.GetItemTotalCount(item), 1, localClose);
            ActiveMenus.Insert(0, newMenu.gameObject);
        }

        public void OpenCountSelect(CraftingRecipe recipe, string prompt, System.Action<bool, int> onCloseCallback)
        {
            CountSelectUI newMenu = Instantiate(countSelection, targetCanvas.transform);

            System.Action<bool, int> localClose = new System.Action<bool, int>((bool confirm, int count) =>
            {
                CloseMenu(newMenu.gameObject);
                onCloseCallback?.Invoke(confirm, count);
            });

            newMenu.SelectCount(recipe.displayName, prompt, 0, host.GetCraftableCount(recipe), 1, localClose);
            ActiveMenus.Insert(0, newMenu.gameObject);
        }

        public void OpenCraftDetail(CraftingRecipe recipe, System.Action<bool, int> onCloseCallback)
        {
            if (craftDetailMenu != null)
            {
                // Ensure we don't already have this menu open
                foreach (GameObject obj in ActiveMenus)
                {
                    if (obj.GetComponentInChildren<CraftDetailMenuUI>() != null) return;
                }

                CraftDetailMenuUI newMenu = Instantiate(craftDetailMenu, targetCanvas.transform);

                System.Action<bool, int> localClose = new System.Action<bool, int>((bool confirm, int count) =>
                {
                    CloseMenu(newMenu.gameObject);
                    onCloseCallback?.Invoke(confirm, count);
                });

                newMenu.Inventory = host;
                newMenu.LoadCraft(recipe, localClose);

                ActiveMenus.Insert(0, newMenu.gameObject);
            }
            else
            {
                Debug.LogError("No craft detail menu selected for theme");
            }
        }

        public void OpenInventory()
        {
            if (ActiveMenus == null || ActiveMenus.Count != 0) return;

            InventoryMenuUI newMenu;
            if (inventoryMenu.overrideSpawn)
            {
                newMenu = Instantiate(inventoryMenu, GameObject.FindGameObjectWithTag(inventoryMenu.overrideTag).transform);
            }
            else
            {
                newMenu = Instantiate(inventoryMenu, targetCanvas.transform);
            }
            newMenu.Inventory = host;
            newMenu.LoadInventory(host);
            host.onMenuOpen?.Invoke();

            ActiveMenus.Insert(0, newMenu.gameObject);
        }

        public void OpenItemContainer(InventoryItem item, System.Action onCloseCallback)
        {
            if (itemContainerMenu != null)
            {
                // Ensure we don't already have this menu open
                foreach (GameObject obj in ActiveMenus)
                {
                    if (obj.GetComponentInChildren<ItemContainerMenuUI>() != null) return;
                }

                ItemContainerMenuUI newMenu = Instantiate(itemContainerMenu, targetCanvas.transform);
                newMenu.LoadItemContainer(host, item, onCloseCallback);
                ActiveMenus.Insert(0, newMenu.gameObject);
            }
            else
            {
                Debug.LogError("No item container menu selected for theme");
            }
        }

        public void OpenItemManager(InventoryItem item, System.Action onCloseCallback)
        {
            if (itemManagerMenu != null)
            {
                // Ensure we don't already have this menu open
                foreach (GameObject obj in ActiveMenus)
                {
                    if (obj.GetComponentInChildren<ItemManagerMenuUI>() != null) return;
                }

                ItemManagerMenuUI newMenu = Instantiate(itemManagerMenu, targetCanvas.transform);
                newMenu.Inventory = host;
                newMenu.InventoryItem = item;
                newMenu.onCloseCalled = onCloseCallback;
                newMenu.UpdateChildren();

                ActiveMenus.Insert(0, newMenu.gameObject);
            }
            else
            {
                Debug.LogError("No item manager menu selected for theme");
            }
        }

        public void OpenRename(InventoryItem item, System.Action onCloseCallback)
        {
            if (renamePrompt != null)
            {
                RenamePrompt newMenu = Instantiate(renamePrompt, targetCanvas.transform);
                newMenu.Inventory = host;
                newMenu.Item = item;
                newMenu.SetUI();

                System.Action localClose = new System.Action(() =>
                {
                    onCloseCallback?.Invoke();
                });

                newMenu.onCloseCalled = localClose;

                ActiveMenus.Insert(0, newMenu.gameObject);
            }
            else
            {
                Debug.LogError("No rename prompt selected for theme");
            }
        }

        public void OpenTradeManager(InventoryCog inventory, InventoryMerchant merchant, InventoryItem item, TradeMode tradeMode, System.Action<bool, int> onCloseCallback)
        {
            if (tradeMenu != null)
            {
                // Ensure we don't already have this menu open
                foreach (GameObject obj in ActiveMenus)
                {
                    if (obj.GetComponentInChildren<TradeMenuUI>() != null) return;
                }

                TradeMenuUI newMenu = Instantiate(tradeMenu, targetCanvas.transform);
                newMenu.Inventory = host;
                newMenu.InventoryItem = item;
                newMenu.Merchant = merchant;
                newMenu.TradeMode = tradeMode;

                System.Action<bool, int> localClose = new System.Action<bool, int>((bool confirm, int count) =>
                {
                    CloseMenu(newMenu.gameObject);
                    onCloseCallback?.Invoke(confirm, count);
                });

                if (tradeMode == TradeMode.Buy)
                {
                    newMenu.Trade(merchant.sellModifier, localClose);
                }
                else
                {
                    newMenu.Trade(merchant.buyModifier, localClose);
                }


                ActiveMenus.Insert(0, newMenu.gameObject);
            }
            else
            {
                Debug.LogError("No trade menu selected for theme.");
            }
        }

        /// <summary>
        /// Restores UI suppressed by HideUI
        /// </summary>
        public void ShowUI()
        {
            foreach (GameObject go in hiddenItems)
            {
                go.SetActive(true);
            }
            hiddenItems.Clear();
        }

        public void TriggerEnter(GameObject other)
        {
            if (LootItemEnter(other.GetComponent<LootItem>())) return;
            if (ContainerEnter(other.GetComponent<InventoryContainer>())) return;
            if (MerchantEnter(other.GetComponent<InventoryMerchant>())) return;
            if (CraftEnter(other.GetComponent<CraftingArea>())) return;
        }

        public void TriggerExit(GameObject other)
        {
            if (LootItemExit(other.GetComponentInChildren<LootItem>())) return;
            if (ContainerExit(other.GetComponentInChildren<InventoryContainer>())) return;
            if (MerchantExit(other.GetComponentInChildren<InventoryMerchant>())) return;
            if (CraftExit(other.GetComponentInChildren<CraftingArea>())) return;
        }

        public void UpdateUI()
        {
            if (enableRaycast)
            {
                UpdateRaycast();
            }

            if (ActiveMenus == null || ActiveMenus.Count != 0) return;

            if (inventoryMenu != null)
            {
                if ((inventoryMenu.openMode == NavigationType.ByButton && InventoryCog.GetButtonDown(inventoryMenu.openButton)) ||
                    (inventoryMenu.openMode == NavigationType.ByKey && InventoryCog.GetKeyDown(inventoryMenu.openKey)))
                {
                    OpenInventory();
                }
            }

            if (target != null)
            {
                remainder -= Time.deltaTime;
                if (remainder <= 0)
                {
                    Destroy(target);
                    target = null;
                }
            }
        }

        #endregion

        #region Private Methods

        private void CloseGeneralPrompt(GameObject go)
        {
            if (ActivePrompt == null) return;

            foreach (GameObject promptTarget in promptTargets)
            {
                if (promptTarget == go)
                {
                    PromptUI prompt = ActivePrompt.GetComponent<PromptUI>();
                    if (prompt.TargetObject != go) return;
                    prompt.onConfirmPrompt.RemoveAllListeners();
                    promptTargets.Remove(go);
                    Destroy(ActivePrompt);
                    ActivePrompt = null;

                    break;
                }
            }

            if (promptTargets.Count > 0)
            {
                TriggerEnter(promptTargets[0]);
            }
        }

        private void CloseLootPrompt()
        {
            if (ActivePrompt != null)
            {
                PromptUI prompt = ActivePrompt.GetComponent<PromptUI>();
                prompt.onConfirmPrompt.RemoveAllListeners();
                foreach (GameObject target in promptTargets)
                {
                    if (target.Equals(prompt.LootItem.gameObject))
                    {
                        promptTargets.Remove(target);
                        break;
                    }
                }

                Destroy(ActivePrompt);
                ActivePrompt = null;
            }

            if (promptTargets.Count > 0)
            {
                TriggerEnter(promptTargets[0]);
            }
        }

        private void ConfirmContainerOpen()
        {
            PromptUI prompt = ActivePrompt.GetComponent<PromptUI>();

            ContainerMenuUI newMenu = Instantiate(containerMenu, targetCanvas.transform);
            newMenu.Inventory = host;
            newMenu.Container = prompt.TargetObject.GetComponent<InventoryContainer>();
            newMenu.RefreshAll();

            ActiveMenus.Add(newMenu.gameObject);

            CloseGeneralPrompt(prompt.TargetObject);
        }

        private void ConfirmCraft()
        {
            PromptUI prompt = ActivePrompt.GetComponent<PromptUI>();

            CraftingArea craftingArea = prompt.TargetObject.GetComponent<CraftingArea>();
            CraftMenuUI newMenu = Instantiate(craftingMenu, targetCanvas.transform);
            newMenu.Inventory = host;
            if (craftingArea.showAllCategories)
            {
                newMenu.LoadCrafting(host);
            }
            else
            {
                newMenu.LoadCrafting(host, craftingArea.allowedCategories);
            }
            newMenu.LoadComponents();
            ActiveMenus.Add(newMenu.gameObject);

            CloseGeneralPrompt(prompt.TargetObject);
        }

        private void ConfirmShop()
        {
            PromptUI prompt = ActivePrompt.GetComponent<PromptUI>();

            if (merchantMenu != null)
            {
                MerchantMenuUI newMenu = Instantiate(merchantMenu, targetCanvas.transform);
                newMenu.playerInventory = host;
                newMenu.merchantInventory = prompt.TargetObject.GetComponent<InventoryMerchant>();
                newMenu.LoadComponents();
                newMenu.RefreshAllInventory();
                ActiveMenus.Add(newMenu.gameObject);
            }
            else
            {
                Debug.LogError("No merchant menu assigned to theme.");
            }

            CloseGeneralPrompt(prompt.TargetObject);
        }

        private bool ContainerEnter(InventoryContainer container)
        {
            if (container == null || !container.enabled) return false;

            if (!promptTargets.Contains(container.gameObject)) promptTargets.Add(container.gameObject);

            if (promptTargets.Count == 1)
            {
                if (ActivePrompt == null)
                {
                    PromptUI newPrompt = Instantiate(promptUI, targetCanvas.transform);
                    newPrompt.Inventory = host;
                    newPrompt.SetPrompt(container.displayName, containerText);
                    newPrompt.TargetObject = container.gameObject;
                    newPrompt.onConfirmPrompt.AddListener(ConfirmContainerOpen);
                    ActivePrompt = newPrompt.gameObject;
                }
            }

            return true;
        }

        private bool ContainerExit(InventoryContainer container)
        {
            if (container == null) return false;

            GameObject go = container.gameObject;

            if (promptTargets.Count > 0)
            {
                CloseGeneralPrompt(go);
                return true;
            }

            return false;
        }

        private bool CraftEnter(CraftingArea area)
        {
            if (craftingMenu == null || area == null || !area.enabled) return false;

            if (!promptTargets.Contains(area.gameObject)) promptTargets.Add(area.gameObject);

            if (promptTargets.Count == 1)
            {
                if (ActivePrompt == null)
                {
                    PromptUI newPrompt = Instantiate(promptUI, targetCanvas.transform);
                    newPrompt.Inventory = host;
                    newPrompt.SetPrompt(area.displayName, string.IsNullOrEmpty(area.overrideActionName) ? craftingText : area.overrideActionName);
                    newPrompt.TargetObject = area.gameObject;
                    newPrompt.onConfirmPrompt.AddListener(ConfirmCraft);
                    ActivePrompt = newPrompt.gameObject;
                }
            }

            return true;
        }

        private bool CraftExit(CraftingArea area)
        {
            if (craftingMenu == null || area == null) return false;

            GameObject go = area.gameObject;

            if (promptTargets.Count > 0)
            {
                CloseGeneralPrompt(go);
                return true;
            }

            return false;
        }

        private void LootItemConfirm()
        {
            PromptUI prompt = ActivePrompt.GetComponent<PromptUI>();
            host.AddToInventory(prompt.LootItem);
            Destroy(prompt.LootItem.gameObject);
            CloseLootPrompt();
        }

        private bool LootItemEnter(LootItem lootItem)
        {
            if (lootItem == null || !lootItem.enabled) return false;

            if (lootItem.autoPickup)
            {
                host.currency += lootItem.currency;
                lootItem.currency = 0;

                if (lootItem.item != null)
                {
                    if (lootItem.item.itemType == ItemType.Consumable && host.StatsCog != null && host.StatsCog.EvaluateCondition(lootItem.autoConsumeWhen))
                    {
                        host.StatsCog.AddInventoryEffects(lootItem.item, lootItem.count);
                    }
                    else
                    {
                        host.AddToInventory(lootItem);
                    }
                }
                else
                {
                    lootItem.onLoot?.Invoke();
                }

                Destroy(lootItem.gameObject);
                return false;
            }

            if (!promptTargets.Contains(lootItem.gameObject)) promptTargets.Add(lootItem.gameObject);

            if (ActivePrompt == null)
            {
                PromptUI newPrompt = Instantiate(promptUI, targetCanvas.transform);
                newPrompt.Inventory = host;
                newPrompt.SetPrompt(lootItem);
                newPrompt.onConfirmPrompt.AddListener(LootItemConfirm);
                newPrompt.TargetObject = lootItem.gameObject;
                ActivePrompt = newPrompt.gameObject;
            }

            return true;
        }

        private bool LootItemExit(LootItem lootItem)
        {
            if (lootItem == null) return false;

            GameObject go = lootItem.gameObject;

            if (promptTargets.Count > 0)
            {
                CloseLootPrompt();

                return true;
            }

            return false;
        }

        private bool MerchantEnter(InventoryMerchant merchant)
        {
            if (merchant == null || !merchant.enabled) return false;

            if (!promptTargets.Contains(merchant.gameObject)) promptTargets.Add(merchant.gameObject);

            if (promptTargets.Count == 1)
            {
                if (ActivePrompt == null)
                {
                    PromptUI newPrompt = Instantiate(promptUI, targetCanvas.transform);
                    newPrompt.Inventory = host;
                    newPrompt.SetPrompt(merchant.displayName, merchantText);
                    newPrompt.TargetObject = merchant.gameObject;
                    newPrompt.onConfirmPrompt.AddListener(ConfirmShop);
                    ActivePrompt = newPrompt.gameObject;
                }
            }

            return true;
        }

        private bool MerchantExit(InventoryMerchant merchant)
        {
            if (merchant == null) return false;

            GameObject go = merchant.gameObject;

            if (promptTargets.Count > 0)
            {
                CloseGeneralPrompt(go);
                return true;
            }

            return false;
        }

        internal void Shutdown()
        {
            foreach (GameObject menu in ActiveMenus)
            {
                Destroy(menu.gameObject);
            }

            if (ActivePrompt != null) Destroy(ActivePrompt.gameObject);
            if (ActiveUI != null) Destroy(ActiveUI.gameObject);
        }

        private void UpdateRaycast()
        {
            Vector3 sourcePoint, direction;

            if (raycastSource == RaycastSource.Character)
            {
                sourcePoint = host.gameObject.transform.position + raycastOffset;
                direction = host.gameObject.transform.forward;
            }
            else
            {
                sourcePoint = Camera.main.transform.position + raycastOffset;
                direction = Camera.main.transform.forward;
            }

            if (Physics.Raycast(sourcePoint, direction, out RaycastHit hit, maxDistance, raycastCulling))
            {
                if(hit.transform.gameObject != lastRayHit)
                {
                    if (lastRayHit != null)
                    {
                        TriggerExit(lastRayHit);
                        lastRayHit = null;
                    }

                    lastRayHit = hit.transform.gameObject;
                    TriggerEnter(lastRayHit);
                }
            }
            else if (lastRayHit != null)
            {
                TriggerExit(lastRayHit);
                lastRayHit = null;
            }
        }

        #endregion

    }
}
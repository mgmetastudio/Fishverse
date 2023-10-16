using LibEngine;
using NullSave.TOCK.Stats;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace NullSave.TOCK.Inventory
{
    [DefaultExecutionOrder(-100)]
    [HierarchyIcon("inventory_icon", "#ffffff")]
    public class InventoryCog : MonoBehaviour
    {
        [Inject]
        private InventoryRemote _inventoryRemote;

        #region Constants

        public const float FILE_VERSION = 1.9f;

        #endregion

        #region Variables

        [Tooltip("Apply Inventory DB related changes directly to the associated prefab")] public bool directDBPrefab = true;

#if PHOTON_UNITY_NETWORKING
        [Tooltip("Should we use PUN Instancing?")] public bool punInstance = false;
        [Tooltip("Rename GameObject to Pun Nickname?")] public bool useNickname = false;
#endif

        [Tooltip("Items to be placed into inventory at startup")] public List<ItemReference> startingItems;
        [Tooltip("AllExistingItems")] public List<InventoryItem> AllExisingPublicItems;

        [Tooltip("Available game currency")] public float currency;
        [Tooltip("Available game currency")] public float Fishcurrency;
        public Dictionary<string, InventoryItem> activeAmmo;
        // Sharing
        [Tooltip("Tag name used to share inventory between multiple sources")] public string shareTag;

        // Themes
        [Tooltip("Default theme to load at start")] public string defaultTheme = "Default";
        [Tooltip("Removes UI for this instance; helpful for NPCs")] public bool headless = false;

        // Pickup & Drop
        [Tooltip("If checked dropped objects will not be spawned by Inventory Cog, you must do so manually")] public bool spawnDropManually;
        [Tooltip("Offset from character to use when spawning dropped inventory")] public Vector3 dropOffset = new Vector3(0, 1.5f, 2);

        // Skills
        [Tooltip("List of strings used to define available Skill Slots")] public List<string> skillSlots;
        private Dictionary<string, string> assignedSkills;

        // Crafting
        [Tooltip("Item(s) to create when crafting fails")] public List<CraftingResult> failedResult;

        // Events
        public ItemCountChanged onItemDropped, onItemAdded, onItemRemoved, onSpawnDropRequested;
        public ItemChanged onItemEquipped, onItemStored, onItemUnequipped;
        public CraftingFailed onCraftingFailed, onCraftQueued, onQueuedCraftComplete;
        public UnityEvent onMenuOpen, onMenuClose;
        public SkillSlotChanged onSkillSlotChanged;
        public ThemeWindowEvent onThemeWindowOpened, onThemeWindowClosed;
        public ContainerDropped onContainerDropped;

        // Loadouts (old)
        [System.Obsolete] public InventoryLoadout[] loadouts = new InventoryLoadout[5];

        // Themes
        private InventoryTheme activeTheme;

        // Editor
        public int z_display_flags = 0;

        #endregion

        #region Properties

        public InventoryTheme ActiveTheme
        {
            get
            {
                if (headless) return null;
                return activeTheme;
            }
            set
            {
                if (headless || value == activeTheme) return;
                if (value != null) value.Initialize(this, activeTheme);
                activeTheme = value;
            }
        }

        public InventoryAttackManager[] AttackManagers { get; private set; }

        /// <summary>
        /// Get list of instantiated categories
        /// </summary>
        public List<Category> Categories { get; private set; }

        public List<CraftingCategory> CraftingCategories { get; private set; }
        public List<StockItemReference> ItemsStock { get; private set; }

        public List<CraftingQueueItem> CraftingQueue { get; private set; }

        public EquipPoint[] EquipPoints { get; private set; }

        public bool Initialized { get; private set; }

        public InventoryHotbar Hotbar { get; private set; }

        /// <summary>
        /// Get/Set menu open flag
        /// </summary>
        public bool IsMenuOpen
        {
            get
            {
                if (ActiveTheme == null) return false;
                return ActiveTheme.ActiveMenus.Count > 0;
            }
        }

        public bool IsPromptOpen
        {
            get
            {
                if (ActiveTheme == null) return false;
                return ActiveTheme.ActivePrompt != null;
            }
        }

        /// <summary>
        /// Get list of instantiated items
        /// </summary>
        public List<InventoryItem> Items { get; private set; }

        public InventoryLoadouts Loadouts { get; private set; }

#if PHOTON_UNITY_NETWORKING

        private Photon.Pun.PhotonView photonView;

        private Photon.Pun.PhotonView PhotonView
        {
            get
            {
                if (photonView == null)
                {
                    photonView = GetComponentInChildren<Photon.Pun.PhotonView>();

                    if(photonView == null)
                    {
                        Transform target = transform.gameObject.transform;
                        while(target.parent != null)
                        {
                            target = target.parent;
                            photonView = target.GetComponent<Photon.Pun.PhotonView>();
                            if(photonView != null)
                            {
                                break;
                            }
                        }
                    }

                }

                return photonView;
            }
        }

#endif

        /// <summary>
        /// Get list of instantiated recipes
        /// </summary>
        public List<CraftingRecipe> Recipes { get; private set; }

        /// <summary>
        /// Resort the inventory
        /// </summary>
        /// <param name="sortOrder"></param>
        public void Sort(InventorySortOrder sortOrder)
        {
            switch (sortOrder)
            {
                case InventorySortOrder.ConditionAsc:
                    Items = Items.OrderBy(_ => _.condition).ThenBy(_ => _.DisplayName).ToList();
                    break;
                case InventorySortOrder.ConditionDesc:
                    Items = Items.OrderByDescending(_ => _.condition).ThenBy(_ => _.DisplayName).ToList();
                    break;
                case InventorySortOrder.DisplayNameAsc:
                    Items = Items.OrderBy(_ => _.DisplayName).ToList();
                    break;
                case InventorySortOrder.DisplayNameDesc:
                    Items = Items.OrderByDescending(_ => _.DisplayName).ToList();
                    break;
                case InventorySortOrder.ItemCountAsc:
                    Items = Items.OrderBy(_ => _.CurrentCount).ThenBy(_ => _.DisplayName).ToList();
                    break;
                case InventorySortOrder.ItemCountDesc:
                    Items = Items.OrderByDescending(_ => _.CurrentCount).ThenBy(_ => _.DisplayName).ToList();
                    break;
                case InventorySortOrder.ItemTypeAsc:
                    Items = Items.OrderBy(_ => _.itemType).ThenBy(_ => _.DisplayName).ToList();
                    break;
                case InventorySortOrder.ItemTypeDesc:
                    Items = Items.OrderByDescending(_ => _.itemType).ThenBy(_ => _.DisplayName).ToList();
                    break;
                case InventorySortOrder.RarityAsc:
                    Items = Items.OrderBy(_ => _.rarity).ThenBy(_ => _.DisplayName).ToList();
                    break;
                case InventorySortOrder.RarityDesc:
                    Items = Items.OrderByDescending(_ => _.rarity).ThenBy(_ => _.DisplayName).ToList();
                    break;
                case InventorySortOrder.ValueAsc:
                    Items = Items.OrderBy(_ => _.value).ThenBy(_ => _.DisplayName).ToList();
                    break;
                case InventorySortOrder.ValueDesc:
                    Items = Items.OrderByDescending(_ => _.value).ThenBy(_ => _.DisplayName).ToList();
                    break;
                case InventorySortOrder.WeightAsc:
                    Items = Items.OrderBy(_ => _.weight).ThenBy(_ => _.DisplayName).ToList();
                    break;
                case InventorySortOrder.WeightDesc:
                    Items = Items.OrderByDescending(_ => _.weight).ThenBy(_ => _.DisplayName).ToList();
                    break;
            }

            foreach (Category category in Categories)
            {
                category.Sort(sortOrder);
            }
        }

        public StatsCog StatsCog { get; private set; }

        /// <summary>
        /// Get total weight of all held items
        /// </summary>
        public float TotalWeight { get; private set; }

        #endregion

        #region Unity Methods

        private void OnDisable()
        {
            // Unsubscribe from points
            foreach (EquipPoint point in EquipPoints)
            {
                point.onItemEquipped.RemoveListener(PointEquipped);
                point.onItemStored.RemoveListener(PointStored);
                point.onItemUnequipped.RemoveListener(PointUnequipped);
            }
        }

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            if (StatsCog == null)
            {
                StatsCog = GetComponentInChildren<StatsCog>();
            }
            Animator animator = GetComponentInChildren<Animator>();

            // Subscribe to points
            EquipPoints = GetComponentsInChildren<EquipPoint>();
            foreach (EquipPoint point in EquipPoints)
            {
                point.Inventory = this;
                point.Animator = animator;
                point.onItemEquipped.AddListener(PointEquipped);
                point.onItemStored.AddListener(PointStored);
                point.onItemUnequipped.AddListener(PointUnequipped);
                point.StatsCog = StatsCog;
            }

            AttackManagers = GetComponentsInChildren<InventoryAttackManager>();

            if (!Initialized)
            {
#if PHOTON_UNITY_NETWORKING

                if (Photon.Pun.PhotonNetwork.InRoom)
                {
                    if (PhotonView != null && !PhotonView.IsMine)
                    {
                        headless = true;
                    }

                    if (useNickname)
                    {
                        gameObject.name = Photon.Pun.PhotonNetwork.LocalPlayer.NickName;
                    }

                }
#endif

                // Add-ons
                Hotbar = GetComponentInChildren<InventoryHotbar>();
                Loadouts = GetComponentInChildren<InventoryLoadouts>();

                // Setup ammo
                activeAmmo = new Dictionary<string, InventoryItem>();

                // Setup categories
                Categories = new List<Category>();
                if (InventoryDB.Categories != null)
                {
                    foreach (Category category in InventoryDB.Categories)
                    {
                        if (category != null)
                        {
                            Category instance = Instantiate(category);
                            instance.name = category.name;
                            instance.StatsCog = StatsCog;
                            instance.Initialize();
                            Categories.Add(instance);
                            Debug.Log("categerie instantiate"+instance);
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("No categories loaded into Inventory DB");
                }

                // Setup Crafting Categoreis
                CraftingCategories = new List<CraftingCategory>();
                if (InventoryDB.CraftingCategories != null)
                {
                    foreach (CraftingCategory category in InventoryDB.CraftingCategories)
                    {
                        if (category != null)
                        {
                            CraftingCategory instance = Instantiate(category);
                            instance.name = category.name;
                            CraftingCategories.Add(instance);
                        }
                    }
                }

                // Setup items
                Items = new List<InventoryItem>();
                if (startingItems != null)
                {
                    foreach (ItemReference item in startingItems)
                    {
                        if (item != null)
                        {
                            AddToInventory(item.item, item.count);
                        }
                    }
                }

                try
                {
                    if (AllExisingPublicItems != null)
                    {
                        var privateOwning = _inventoryRemote.ItemsPrivateOwningInfo;

                        foreach (InventoryItem item in AllExisingPublicItems)
                        {
                            if (item != null)
                            {
                                var ownedItem = privateOwning.Where(x => x.Key.ItemKeyId.ToString() == item.customizationId);

                                foreach (var ownItem in ownedItem)
                                {
                                    var ownItemKey = ownItem.Key;
                                    AddToInventory(item, ownItemKey.Count);
                                }
                            }
                        }
                    }

                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Handled exception: " + ex.Message);
                }
                
                // Setup Recipes
                Recipes = new List<CraftingRecipe>();
                if (InventoryDB.Recipes != null)
                {
                    foreach (CraftingRecipe recipe in InventoryDB.Recipes)
                    {
                        if (recipe != null)
                        {
                            CraftingRecipe instance = Instantiate(recipe);
                            instance.name = recipe.name;
                            instance.Initialize(this);
                            Recipes.Add(instance);
                        }
                    }
                }

                // Setup crafting queue
                CraftingQueue = new List<CraftingQueueItem>();

                // Activate skills
                assignedSkills = new Dictionary<string, string>();

                // Activate themes
                if (!headless)
                {
                    if (InventoryDB.Themes != null && InventoryDB.Themes.Count > 0)
                    {
                        foreach (InventoryTheme theme in InventoryDB.Themes)
                        {
                            if (theme.name == defaultTheme)
                            {
                                ActiveTheme = theme;
                                break;
                            }
                        }

                        if (ActiveTheme == null)
                        {
                            ActiveTheme = InventoryDB.Themes[0];
                        }
                    }
                }

                Initialized = true;
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if (!other.enabled) return;
            TriggerEnter(other.gameObject);
        }

        public void OnTriggerStay(Collider other)
        {
            if (!other.enabled) return;
            TriggerEnter(other.gameObject);
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.enabled) return;
            TriggerEnter(other.gameObject);
        }

        public void OnTriggerExit(Collider other)
        {
            if (!other.enabled) return;
            TriggerExit(other.gameObject);
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            if (!other.enabled) return;
            TriggerExit(other.gameObject);
        }

        private void Update()
        {
            if (activeTheme != null)
            {
                activeTheme.UpdateUI();
            }

            UpdateCraftingQueue();
        }

        #endregion

        #region Public Methods

        public void AddToInventory(InventoryItem item)
        {
            AddToInventory(item, 1);
        }

        /// <summary>
        /// Add item to inventory
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <returns>Count to drop from full inventory</returns>
        public int AddToInventory(LootItem item, bool allowAutoEquip = true)
        {
            int looted = 0;

            currency += item.currency;
            Fishcurrency +=item.currency;
            item.currency = 0;
            if (item.item == null)
            {
                item.onLoot?.Invoke();
                return 0;
            }


            if (item.item.itemType != ItemType.Container)
            {
                looted = AddToInventory(item.GenerateValues(), item.count, allowAutoEquip);
                if (looted != item.count) item.onLoot?.Invoke();
            }
            else
            {
                int localLoot;
                InventoryItem reference = null;
                for (int i=0; i < item.count; i++)
                {
                    localLoot = AddToInventory(item.GenerateValues(), 1, out reference, true, true);
                    if (reference != null && item.ContainedItems != null)
                    {
                        foreach (ItemReference storedItem in item.ContainedItems)
                        {
                            reference.AddStoredItem(storedItem.item, storedItem.count);
                        }

                        item.onLoot?.Invoke();
                    }
                }
            }


            return looted;
        }

        public int AddToInventory(InventoryItem item, int count, out InventoryItem itemReference, bool allowAutoEquip, bool raiseEvents)
        {
            itemReference = null;
            if (item == null) return count;

            if (item.autoUse)
            {
                UseEffects(item, count);
                return 0;
            }

            int orgCount = count;
            int dropCount = 0;

            // Check container
            if (item.itemType == ItemType.Container && item.mustEmptyToHold && item.storedItems.Count > 0)
            {
                return count;
            }

            // Add to current stack if able
            if (item.canStack)
            {
                if (IsItemInInventory(item, out InventoryItem localItem))
                {
                    dropCount = AddToItemStack(localItem, count);
                    if (raiseEvents)
                    {
                        if (item.itemType == ItemType.Ammo) UpdateAmmoEvents(item);
                        onItemAdded?.Invoke(item, orgCount - dropCount);
                    }
                    return dropCount;
                }
            }

            int holdOver = 0;

            // Add new item
            itemReference = Instantiate(item);
            itemReference.Initialize(this);
            itemReference.name = item.name;
            itemReference.CurrentCount = count;
            itemReference.value = item.value;
            itemReference.rarity = item.rarity;
            itemReference.slotId = item.slotId;
            itemReference.storedIn = null;

            if (itemReference.useSlotId)
            {
                if (IsSlotIdUsed(itemReference.slotId))
                {
                    itemReference.slotId = GetFirstFreeSlotId();
                }
            }

            // Check stack
            if (itemReference.canStack && itemReference.CurrentCount > itemReference.countPerStack)
            {
                holdOver = itemReference.CurrentCount - itemReference.countPerStack;
                itemReference.CurrentCount = itemReference.countPerStack;
            }
            else if (itemReference.CurrentCount > 1 && !itemReference.canStack)
            {
                holdOver = orgCount - 1;
                itemReference.CurrentCount = 1;
            }

            // Do we have an empty slot?
            Category category = GetItemCategory(itemReference);
            if (CategoryHasFreeSlot(category) && ItemHasFreeStack(itemReference, category))
            {
                // Add to inventory
                Items.Add(itemReference);
                category.AddItem(itemReference);
                TotalWeight += itemReference.weight * itemReference.CurrentCount;

                // Auto equip
                if (allowAutoEquip)
                {
                    switch (itemReference.autoEquip)
                    {
                        case AutoEquipMode.Always:
                            EquipFirstOrEmpty(itemReference);
                            break;
                        case AutoEquipMode.IfSlotFree:
                            EquipFirstEmpty(itemReference);
                            break;
                    }
                }

                // Set active
                if (itemReference.itemType == ItemType.Ammo)
                {
                    if (GetSelectedAmmo(itemReference.ammoType) == null)
                    {
                        //SetSelectedAmmo(itemReference);
                    }
                }
       
                if (raiseEvents)
                {
                    if (item.itemType == ItemType.Ammo) UpdateAmmoEvents(item);
                    onItemAdded?.Invoke(item, orgCount - dropCount);
                }
            }
            else
            {
                return holdOver + itemReference.CurrentCount;
            }

            if (holdOver > 0)
            {
                dropCount = AddToInventory(itemReference, holdOver, allowAutoEquip, false);
                if (raiseEvents)
                {
                    if (item.itemType == ItemType.Ammo) UpdateAmmoEvents(item);
                    onItemAdded?.Invoke(item, orgCount - dropCount);
                }
                return dropCount;
            }

            if (raiseEvents)
            {
                if (item.itemType == ItemType.Ammo) UpdateAmmoEvents(item);
                onItemAdded?.Invoke(item, orgCount - holdOver);
            }
            return 0;
        }

        /// <summary>
        /// Add item to inventory
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <returns>Count to drop from full inventory</returns>
        public int AddToInventory(InventoryItem item, int count, bool allowAutoEquip = true, bool raiseEvents = true)
        {
            InventoryItem ignore;
            return AddToInventory(item, count, out ignore, allowAutoEquip, raiseEvents);
        }

        /// <summary>
        /// Check if any attachments that fit specified item are available in inventory
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool AttachmentsAvailableForItem(InventoryItem item)
        {
            switch (item.attachRequirement)
            {
                case AttachRequirement.InCategory:
                    List<string> catNames = new List<string>();
                    foreach (Category cat in item.attachCatsFilter)
                    {
                        catNames.Add(cat.name);
                    }
                    foreach (InventoryItem invItem in Items)
                    {
                        if (catNames.Contains(invItem.category.name) && CanAttachToItem(invItem, item))
                        {
                            return true;
                        }
                    }
                    break;
                case AttachRequirement.InItemList:
                    List<string> itemNames = new List<string>();
                    foreach (InventoryItem itm in item.attachItemsFilter)
                    {
                        itemNames.Add(itm.name);
                    }
                    foreach (InventoryItem invItem in Items)
                    {
                        if (itemNames.Contains(invItem.name) && CanAttachToItem(invItem, item))
                        {
                            return true;
                        }
                    }

                    break;
            }

            return false;
        }

        /// <summary>
        /// Breakdown an item and add parts to inventory
        /// </summary>
        /// <param name="item"></param>
        public void BreakdownItem(InventoryItem item)
        {
            if (!item.CanBreakdown || item.CurrentCount < 1) return;

            foreach (ItemReference component in item.breakdownResult)
            {
                AddToInventory(component.item, component.count);
            }

            item.CurrentCount -= 1;
            if (item.CurrentCount == 0) FinalizeRemove(item, GetItemCategory(item));
        }

        /// <summary>
        /// Breakdown item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        public void BreakdownItem(InventoryItem item, int count)
        {
            if (!item.CanBreakdown || count <= 0) return;

            while (count > 0)
            {
                InventoryItem target = GetItemFromInventory(item);
                if (item == null) return;

                if (target.CurrentCount >= count)
                {
                    foreach (ItemReference component in target.breakdownResult)
                    {
                        AddToInventory(component.item, component.count * count);
                    }
                    target.CurrentCount -= count;
                    if (target.CurrentCount == 0) FinalizeRemove(target, GetItemCategory(item));
                }
                else
                {
                    foreach (ItemReference component in target.breakdownResult)
                    {
                        AddToInventory(component.item, component.count * target.CurrentCount);
                    }
                    FinalizeRemove(target, GetItemCategory(item));
                }
            }
        }

        /// <summary>
        /// Check if an attachment can be added to a specified item
        /// </summary>
        /// <param name="attachment"></param>
        /// <param name="hostItem"></param>
        /// <returns></returns>
        public bool CanAttachToItem(InventoryItem attachment, InventoryItem hostItem)
        {
            if (attachment.itemType != ItemType.Attachment || attachment.IsAttached) return false;
            List<string> availSlotNames = new List<string>();
            foreach (AttachmentSlot slot in hostItem.Slots)
            {
                availSlotNames.Add(slot.AttachPoint.pointId);
            }
            foreach (string slotName in attachment.attachPoints)
            {
                if (availSlotNames.Contains(slotName)) return true;
            }
            return false;
        }

        /// <summary>
        /// Remove all items from inventory
        /// </summary>
        public void ClearInventory()
        {
            foreach (EquipPoint point in EquipPoints)
            {
                point.UnequipItem();
            }

            foreach (Category category in Categories)
            {
                category.Clear();
            }

            TotalWeight = 0;
            Items.Clear();
            CraftingQueue.Clear();
        }

        /// <summary>
        /// Close all active menus and prompts
        /// </summary>
        public void ClearUI()
        {
            ActiveTheme?.ClearUI();
        }

        /// <summary>
        /// Craft a recipe X times
        /// </summary>
        /// <param name="recipe"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<CraftingResult> Craft(CraftingRecipe recipe, int count, bool addToInventory = true, bool removeFromCounts = true)
        {
            if (!recipe.Unlocked)
            {
                return new List<CraftingResult>();
            }

            // Create list of items (return if not in stock)
            List<ItemReference> master = new List<ItemReference>();
            if (recipe.componentType == ComponentType.Standard)
            {
                foreach (ItemReference component in recipe.components)
                {
                    if (GetItemTotalCount(component.item) < component.count * count) return null;
                    ItemReference newComponent = new ItemReference
                    {
                        count = component.count * count,
                        item = component.item
                    };
                    master.Add(newComponent);
                }
            }
            else
            {
                foreach (AdvancedComponent component in recipe.advancedComponents)
                {
                    if (GetItemTotalCount(component.item, component.minCondition, component.minRarity) < component.count * count) return null;
                    ItemReference newComponent = new ItemReference
                    {
                        count = component.count * count,
                        item = component.item
                    };
                    newComponent.item.rarity = component.minRarity;
                    newComponent.item.condition = component.minCondition;
                    master.Add(newComponent);
                }
            }

            // Queue non-instant recipes
            if (recipe.craftTime != CraftingTime.Instant)
            {
                // Create entry
                CraftingQueueItem queueItem = new CraftingQueueItem
                {
                    recipe = recipe,
                    timeStarted = System.DateTime.Now,
                    count = count,
                    usedComponents = master
                };
                if (recipe.craftTime == CraftingTime.RealTime)
                {
                    queueItem.realWorldEnd = System.DateTime.Now.AddSeconds(recipe.craftSeconds);
                }
                else
                {
                    queueItem.secondsRemaining = recipe.craftSeconds;
                }
                CraftingQueue.Add(queueItem);

                // Remove counts
                if (removeFromCounts)
                {
                    foreach (ItemReference item in master)
                    {
                        if (recipe.componentType == ComponentType.Standard)
                        {
                            RemoveItem(item.item, item.count);
                        }
                        else
                        {
                            RemoveItem(item.item, item.count, item.item.condition, item.item.rarity);
                        }
                    }
                }

                // Raise queue event
                onCraftQueued?.Invoke(recipe, count);

                return new List<CraftingResult>();
            }

            // Get success/fail count
            int successCount = 0;
            int failCount = 0;
            float successChange = recipe.GetSuccessChance();
            if (successChange == 1)
            {
                successCount = count;
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    float craftRoll = Random.Range(0f, 1f);

                    if (craftRoll >= successChange)
                    {
                        failCount += 1;
                    }
                }
            }

            List<CraftingResult> results = new List<CraftingResult>();

            // Add success result to inventory
            if (successCount > 0)
            {
                recipe.SuccessCount += successCount;
                if (recipe.FirstCrafted == null) recipe.FirstCrafted = System.DateTime.Now;
                recipe.LastCrafted = System.DateTime.Now;

                foreach (CraftingResult item in recipe.result)
                {
                    InventoryItem result = Instantiate(item.item);
                    item.SetResults(master, result);
                    result.name = item.item.name;
                    result.InstanceId = System.Guid.NewGuid().ToString();
                    if (addToInventory)
                    {
                        int dropped = AddToInventory(result, item.count * successCount);
                        SpawnDrop(result, dropped);
                    }

                    CraftingResult craft = new CraftingResult
                    {
                        item = result,
                        count = result.CurrentCount
                    };
                    results.Add(craft);
                }
            }

            // Add fail result to inventory
            if (failCount > 0)
            {
                recipe.FailCount += failCount;

                foreach (CraftingResult item in recipe.failResult)
                {
                    InventoryItem result = Instantiate(item.item);
                    item.SetResults(master, result);
                    result.name = item.item.name;
                    result.InstanceId = System.Guid.NewGuid().ToString();
                    if (addToInventory)
                    {
                        int dropped = AddToInventory(result, item.count * successCount);
                        SpawnDrop(result, dropped);
                    }

                    CraftingResult craft = new CraftingResult
                    {
                        item = result,
                        count = result.CurrentCount
                    };
                    results.Add(craft);
                }

                onCraftingFailed?.Invoke(recipe, failCount);
            }

            // Remove components from inventory
            if (removeFromCounts)
            {
                foreach (ItemReference item in master)
                {
                    if (recipe.componentType == ComponentType.Standard)
                    {
                        RemoveItem(item.item, item.count);
                    }
                    else
                    {
                        RemoveItem(item.item, item.count, item.item.condition, item.item.rarity);
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Attempt to craft using a list of items
        /// </summary>
        /// <param name="items">References to items and counts</param>
        /// <param name="addToInventory">Add result(s) to inventory if true</param>
        /// <param name="removeFromCounts">Remove used items from inventory when true</param>
        /// <returns></returns>
        public List<CraftingResult> Craft(List<ItemReference> items, bool addToInventory = true, bool removeFromCounts = true)
        {
            // Create master list
            int i;
            bool found;
            List<ItemReference> master = new List<ItemReference>();
            foreach (ItemReference item in items)
            {
                found = false;
                for (i = 0; i < master.Count; i++)
                {
                    if (master[i].item == item.item)
                    {
                        master[i].count += item.count;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    master.Add(item);
                }
            }

            CraftingRecipe recipe = GetRecipe(master);
            if (recipe == null)
            {
                if (!recipe.Unlocked)
                {
                    return new List<CraftingResult>();
                }

                if (failedResult != null)
                {
                    if (addToInventory)
                    {
                        foreach (CraftingResult item in failedResult)
                        {
                            InventoryItem result = Instantiate(item.item);
                            result.name = item.item.name;
                            result.InstanceId = System.Guid.NewGuid().ToString();
                            result.CurrentCount = item.count;
                            item.SetResults(master, result);
                            AddToInventory(result, item.count);
                        }
                    }

                    if (removeFromCounts)
                    {
                        foreach (ItemReference item in items)
                        {
                            UseItem(item.item, item.count);
                        }
                    }

                    return failedResult;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return Craft(recipe, 1, addToInventory, removeFromCounts);
            }
        }

        /// <summary>
        /// Complete a queue item immediately
        /// </summary>
        /// <param name="recipe"></param>
        public void CompleteQueueRecipe(CraftingQueueItem recipe)
        {
            foreach (CraftingQueueItem item in CraftingQueue)
            {
                if (item == recipe)
                {
                    CraftingQueue.Remove(item);
                    CompleteCraft(item.recipe, item.count, item.usedComponents);
                    onQueuedCraftComplete?.Invoke(item.recipe, item.count);
                }
            }
        }

        /// <summary>
        /// Use up a consumable or ingredient item and apply mods
        /// Mods requires Stats Cog
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        public void ConsumeItem(InventoryItem item, int count)
        {
            switch (item.itemType)
            {
                case ItemType.Consumable:
                    while (count > 0)
                    {
                        if (item.CurrentCount > 0)
                        {
                            UseEffects(item, 1);
                            count -= 1;
                            item.CurrentCount -= 1;
                            TotalWeight -= item.weight;

                            if (item.CurrentCount == 0) FinalizeRemove(item, GetItemCategory(item));
                        }
                        else
                        {
                            FinalizeRemove(item, GetItemCategory(item));
                            InventoryItem newItem = GetItemFromInventory(item);
                            if (newItem != null)
                            {
                                ConsumeItem(newItem, count);
                            }
                            return;
                        }
                    }
                    break;
                default:
                    if (item.CurrentCount > count)
                    {
                        item.CurrentCount -= count;
                        TotalWeight -= item.weight * count;
                        UseEffects(item, count);

                    }
                    else if (item.CurrentCount == count)
                    {
                        FinalizeRemove(item, GetItemCategory(item));
                        TotalWeight -= item.weight * count;
                        UseEffects(item, count);
                    }
                    else
                    {
                        count -= item.CurrentCount;
                        FinalizeRemove(item, GetItemCategory(item));
                        TotalWeight -= item.weight * item.CurrentCount;
                        UseEffects(item, count);

                        InventoryItem newItem = GetItemFromInventory(item);
                        if (newItem != null)
                        {
                            ConsumeItem(newItem, count);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Drop an item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        public void DropItem(InventoryItem item, int count, bool spawnDrop = true)
        {
            if (count < 1) return;

            if (Items.Contains(item))
            {
                if (item.canDrop)
                {
                    if (item.canStack && item.CurrentCount > count)
                    {
                        item.CurrentCount -= count;
                        TotalWeight -= item.weight * count;
                        if (spawnDrop)
                        {
                            SpawnDrop(item, count);
                        }
                    }
                    else if (item.canStack)
                    {
                        if (item.EquipState != EquipState.NotEquipped)
                        {
                            item.CurrentEquipPoint.UnequipItem();
                        }
                        if (spawnDrop)
                        {
                            SpawnDrop(item, item.CurrentCount);
                        }
                        FinalizeRemove(item, GetItemCategory(item));
                        count -= item.CurrentCount;
                        TotalWeight -= item.weight * item.CurrentCount;
                        if (count > 0)
                        {
                            DropItem(GetItemFromInventory(item), count, spawnDrop);
                        }
                    }
                    else
                    {
                        if (item.EquipState != EquipState.NotEquipped)
                        {
                            item.CurrentEquipPoint.UnequipItem();
                        }
                        if (spawnDrop)
                        {
                            SpawnDrop(item, 1);
                        }
                        TotalWeight -= item.weight;
                        FinalizeRemove(item, GetItemCategory(item));
                        count -= 1;
                        if (count > 0)
                        {
                            DropItem(item, count);
                        }
                    }
                }

                onItemDropped?.Invoke(item, count);
                return;
            }
        }

        /// <summary>
        /// Equip an item
        /// </summary>
        /// <param name="item"></param>
        public void EquipItem(InventoryItem item)
        {
            if (item == null) return;
         
            if (item.itemType == ItemType.Ammo)
            {

                SetSelectedAmmo(item);

                return;
            }
      
            if (!item.CanEquip) return;

            InventoryItem targetItem = GetItemInstanceInInventory(item);
            if (targetItem == null) return;

            // Check if item is already equipped
            if (targetItem.CurrentEquipPoint != null)
            {
                return;
            }

            // Equip item
            EquipFirstOrEmpty(targetItem);
        }

        /// <summary>
        /// Equip item to a specific slot
        /// </summary>
        /// <param name="item"></param>
        /// <param name="equipPointId"></param>
        public void EquipItem(InventoryItem item, string equipPointId)
        {
            if (item.itemType == ItemType.Ammo)
            {
                SetSelectedAmmo(item);
                return;
            }

            if (!item.CanEquip) return;

            // Get item reference
            InventoryItem targetItem = GetItemInstanceInInventory(item);
            if (targetItem == null) return;

            // Check if equip point is allowed
            bool validTarget = false;
            foreach (string epi in item.equipPoints)
            {
                if (epi == equipPointId)
                {
                    validTarget = true;
                    break;
                }
            }
            if (!validTarget) return;

            // Get target equip point
            EquipPoint ep = GetEquipPoint(equipPointId);
            if (ep == null) return;

            // Check if item is already equipped
            if (targetItem.CurrentEquipPoint != null)
            {
                // Check if it's already on the requested point
                if (targetItem.CurrentEquipPoint.pointId == equipPointId)
                {
                    return;
                }

                // Unequip from current location
                targetItem.CurrentEquipPoint.UnequipItem();
            }

            // Assign item
            ep.EquipItem(targetItem);
        }

        /// <summary>
        /// Get the skill (InventoryItem) assigned to a slot
        /// </summary>
        /// <param name="skillSlot"></param>
        /// <returns></returns>
        public InventoryItem GetAssignedSkill(string skillSlot)
        {
            if (!assignedSkills.ContainsKey(skillSlot)) return null;
            return GetItemFromInventory(assignedSkills[skillSlot]);
        }

        /// <summary>
        /// Check if there are any attachments compatible with supplied item in the inventory
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool GetAnyAttachmentsInInventory(InventoryItem item)
        {
            if (item == null)
            {
                return false;
            }
            switch (item.attachRequirement)
            {
                case AttachRequirement.InCategory:
                    List<string> catNames = new List<string>();
                    foreach (Category cat in item.attachCatsFilter)
                    {
                        catNames.Add(cat.name);
                    }
                    foreach (InventoryItem invItem in Items)
                    {
                        if (catNames.Contains(invItem.category.name))
                        {
                            return true;
                        }
                    }
                    break;
                case AttachRequirement.InItemList:
                    List<string> itemNames = new List<string>();
                    foreach (InventoryItem itm in item.attachItemsFilter)
                    {
                        itemNames.Add(itm.name);
                    }
                    foreach (InventoryItem invItem in Items)
                    {
                        if (itemNames.Contains(invItem.name))
                        {
                            return true;
                        }
                    }

                    break;
            }

            return false;
        }

        /// <summary>
        /// Get a list of available attachments for item in inventory
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public List<InventoryItem> GetAvailableAttachments(InventoryItem item)
        {
            List<InventoryItem> result = new List<InventoryItem>();

            switch (item.attachRequirement)
            {
                case AttachRequirement.InCategory:
                    List<string> catNames = new List<string>();
                    foreach (Category cat in item.attachCatsFilter)
                    {
                        catNames.Add(cat.name);
                    }
                    foreach (InventoryItem invItem in Items)
                    {
                        if (catNames.Contains(invItem.category.name) && CanAttachToItem(invItem, item))
                        {
                            result.Add(invItem);
                        }
                    }
                    break;
                case AttachRequirement.InItemList:
                    List<string> itemNames = new List<string>();
                    foreach (InventoryItem itm in item.attachItemsFilter)
                    {
                        itemNames.Add(itm.name);
                    }
                    foreach (InventoryItem invItem in Items)
                    {
                        if (itemNames.Contains(invItem.name) && CanAttachToItem(invItem, item))
                        {
                            result.Add(invItem);
                        }
                    }

                    break;
            }

            return result;
        }

        public List<InventoryItem> GetAvailableAttachments(InventoryItem item, string slotId)
        {
            List<InventoryItem> result = new List<InventoryItem>();
            switch (item.attachRequirement)
            {
                case AttachRequirement.InCategory:
                    List<string> catNames = new List<string>();
                    foreach (Category cat in item.attachCatsFilter)
                    {
                        catNames.Add(cat.name);
                    }

                    foreach (InventoryItem invItem in Items)
                    {
                        if (catNames.Contains(invItem.category.name) && invItem.AttachesToPoint(slotId) && CanAttachToItem(invItem, item))
                        {
                            result.Add(invItem);
                        }
                    }
                    break;
                case AttachRequirement.InItemList:
                    List<string> itemNames = new List<string>();
                    foreach (InventoryItem itm in item.attachItemsFilter)
                    {
                        itemNames.Add(itm.name);
                    }
                    foreach (InventoryItem invItem in Items)
                    {
                        if (itemNames.Contains(invItem.name) && invItem.AttachesToPoint(slotId) && CanAttachToItem(invItem, item))
                        {
                            result.Add(invItem);
                        }
                    }

                    break;
            }

            return result;
        }

        public static float GetAxis(string inputName)
        {
#if GAME_COG
            if (GameCog.Input != null) return GameCog.Input.GetAxis(inputName);
#endif
            return Input.GetAxis(inputName);
        }

        /// <summary>
        /// Get list of breakable items
        /// </summary>
        /// <returns></returns>
        public List<InventoryItem> GetBreakableItems()
        {
            List<InventoryItem> breakable = new List<InventoryItem>();

            foreach (InventoryItem item in Items)
            {
                if (item.CanBreakdown)
                {
                    breakable.Add(item);
                }
            }

            return breakable;
        }

        /// <summary>
        /// Get list of breakable items in category
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public List<InventoryItem> GetBreakableItems(string categoryName)
        {
            List<InventoryItem> breakable = new List<InventoryItem>();

            foreach (InventoryItem item in Items)
            {
                if (item.CanBreakdown && item.category.name == categoryName)
                {
                    breakable.Add(item);
                }
            }

            return breakable;
        }

        /// <summary>
        /// Get list of breakable items in category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<InventoryItem> GetBreakableItems(Category category)
        {
            return GetBreakableItems(category.name);
        }

        /// <summary>
        /// Get list of breakable items in categories
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public List<InventoryItem> GetBreakableItems(List<string> categories)
        {
            List<InventoryItem> breakable = new List<InventoryItem>();

            foreach (InventoryItem item in Items)
            {
                if (item.CanBreakdown && categories.Contains(item.category.name))
                {
                    breakable.Add(item);
                }
            }

            return breakable;
        }

        /// <summary>
        /// Get list of breakable items in categories
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public List<InventoryItem> GetBreakableItems(List<Category> categories)
        {
            List<string> categoryList = new List<string>();
            foreach (Category category in categories)
            {
                categoryList.Add(category.name);
            }
            return GetBreakableItems(categoryList);
        }

        public static bool GetButtonDown(string buttonName)
        {
#if GAME_COG
            if (GameCog.Input != null) return GameCog.Input.GetButtonDown(buttonName);
#endif
            return Input.GetButtonDown(buttonName);
        }

        /// <summary>
        /// Get total number of times a recipe can be crafted with current inventory
        /// </summary>
        /// <param name="recipe"></param>
        /// <returns></returns>
        public int GetCraftableCount(CraftingRecipe recipe)
        {
            int craftable = int.MaxValue;
            float itemCount;
            int itemCraftable;
            int baseItemCount = 0;

            if (recipe.craftType == CraftingType.Upgrade)
            {
                // Check for original item
                foreach (InventoryItem item in Items)
                {
                    if (item.name == recipe.baseItem.upgradeItem.name &&
                        item.condition >= recipe.baseItem.minCondition && item.condition <= recipe.baseItem.maxCondition &&
                        item.rarity >= recipe.baseItem.minRarity && item.rarity <= recipe.baseItem.maxRarity)
                    {
                        baseItemCount += 1;
                    }
                }
                if (baseItemCount == 0) return 0;
            }

            if (recipe.componentType == ComponentType.Standard)
            {
                foreach (ItemReference component in recipe.components)
                {
                    itemCount = GetItemTotalCount(component.item);
                    if (itemCount == 0) return 0;
                    itemCraftable = Mathf.FloorToInt(itemCount / component.count);
                    if (itemCraftable == 0) return 0;
                    if (itemCraftable < craftable) craftable = itemCraftable;
                }
            }
            else
            {
                foreach (AdvancedComponent component in recipe.advancedComponents)
                {
                    itemCount = GetItemTotalCount(component.item, component.minCondition, component.minRarity);
                    if (itemCount == 0) return 0;
                    itemCraftable = Mathf.FloorToInt(itemCount / component.count);
                    if (itemCraftable == 0) return 0;
                    if (itemCraftable < craftable) craftable = itemCraftable;
                }
            }

            if (recipe.craftType == CraftingType.Upgrade && craftable > baseItemCount)
            {
                craftable = baseItemCount;
            }

            return craftable;
        }

        /// <summary>
        /// Get all recipes that can be crafted with current inventory items
        /// </summary>
        /// <returns></returns>
        public List<CraftingRecipe> GetCraftableRecipes()
        {
            List<CraftingRecipe> result = new List<CraftingRecipe>();

            foreach (CraftingRecipe recipe in Recipes)
            {
                if (recipe.Unlocked && GetRecipeCraftable(recipe))
                {
                    result.Add(recipe);
                }
            }

            return result;
        }

        /// <summary>
        /// Get all recipes in a category that can be crafted with current inventory items
        /// </summary>
        /// <returns></returns>
        public List<CraftingRecipe> GetCraftableRecipesByCategory(string categoryName)
        {
            List<CraftingRecipe> result = new List<CraftingRecipe>();

            foreach (CraftingRecipe recipe in Recipes)
            {
                if (recipe.craftingCategory.name == categoryName && recipe.Unlocked && GetRecipeCraftable(recipe))
                {
                    result.Add(recipe);
                }
            }

            return result;
        }

        /// <summary>
        /// Get all recipes in a category set that can be crafted with current inventory items
        /// </summary>
        /// <returns></returns>
        public List<CraftingRecipe> GetCraftableRecipesByCategory(List<string> categories)
        {
            List<CraftingRecipe> result = new List<CraftingRecipe>();

            foreach (CraftingRecipe recipe in Recipes)
            {
                if (categories.Contains(recipe.craftingCategory.name) && recipe.Unlocked && GetRecipeCraftable(recipe))
                {
                    result.Add(recipe);
                }
            }

            return result;
        }

        public CraftingCategory GetCraftingCategory(string categoryName)
        {
            foreach (CraftingCategory category in CraftingCategories)
            {
                if (category.name == categoryName)
                {
                    return category;
                }
            }

            return null;
        }

        /// <summary>
        /// Get instanced copy of a category by name
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public Category GetCategory(string categoryName)
        {
            foreach (Category category in Categories)
            {
                if (category.name == categoryName)
                {
                    return category;
                }
            }

            return null;
        }

        /// <summary>
        /// Get a list of displayed categories
        /// </summary>
        /// <param name="excludeLockedCategories"></param>
        /// <returns></returns>
        public List<Category> GetDisplayedCategories(bool excludeLockedCategories)
        {
            List<Category> categories = new List<Category>();
            foreach (Category category in Categories)
            {
                if (category.displayInList && (category.catUnlocked || !excludeLockedCategories))
                {
                    categories.Add(category);
                }
            }
            return categories;
        }

        /// <summary>
        /// Get equip point by name
        /// </summary>
        /// <param name="pointId"></param>
        /// <returns></returns>
        public EquipPoint GetEquipPoint(string pointId)
        {
            foreach (EquipPoint point in EquipPoints)
            {
                if (point.pointId == pointId)
                {
                    return point;
                }
            }

            return null;
        }

        /// <summary>
        /// Get the available count for active ammo by type
        /// </summary>
        /// <param name="ammoType"></param>
        /// <returns></returns>
        public int GetEquippedAmmoCount(string ammoType)
        {
            if (activeAmmo.ContainsKey(ammoType))
            {
                return GetItemTotalCount(activeAmmo[ammoType]);
            }
            return 0;
        }
        public int GetEquippedAmmoSpinningCount(string ammoType)
        {
            if (activeAmmo.ContainsKey(ammoType))
            {
                if(activeAmmo[ammoType].previewScale==2)
                {
                    return GetItemTotalCount(activeAmmo[ammoType]);
                }

            }
            return 0;
        }
        /// <summary>
        /// Get the first unused slot id (does not check if there is room for item)
        /// </summary>
        /// <returns></returns>
        public int GetFirstFreeSlotId()
        {
            List<int> usedId = new List<int>();
            foreach (InventoryItem item in Items)
            {
                if (item.useSlotId)
                {
                    usedId.Add(item.slotId);
                }
            }

            int i = 0;
            while (true)
            {
                if (usedId.Contains(i))
                {
                    i++;
                }
                else
                {
                    return i;
                }
            }
        }

        /// <summary>
        /// Get total count of all items grouped by InventoryItem regardless of stacking
        /// </summary>
        /// <returns></returns>
        public List<ItemReference> GetGroupedCounts()
        {
            Dictionary<string, int> tempResult = new Dictionary<string, int>();

            foreach (InventoryItem item in Items)
            {
                if (tempResult.ContainsKey(item.name))
                {
                    tempResult[item.name] += item.CurrentCount;
                }
                else
                {
                    tempResult.Add(item.name, item.CurrentCount);
                }
            }

            List<ItemReference> result = new List<ItemReference>();
            foreach (KeyValuePair<string, int> entry in tempResult)
            {
                ItemReference item = new ItemReference
                {
                    count = entry.Value,
                    item = GetItemByName(entry.Key)
                };
                result.Add(item);
            }
            return result;
        }

        /// <summary>
        /// Get total count of all items grouped by InventoryItem regardless of stacking
        /// </summary>
        /// <returns></returns>
        public List<ItemReference> GetGroupedCounts(string categoryName)
        {
            Dictionary<string, int> tempResult = new Dictionary<string, int>();

            foreach (InventoryItem item in Items)
            {
                if (item.category.name == categoryName)
                {
                    if (tempResult.ContainsKey(item.name))
                    {
                        tempResult[item.name] += item.CurrentCount;
                    }
                    else
                    {
                        tempResult.Add(item.name, item.CurrentCount);
                    }
                }
            }

            List<ItemReference> result = new List<ItemReference>();
            foreach (KeyValuePair<string, int> entry in tempResult)
            {
                ItemReference item = new ItemReference
                {
                    count = entry.Value,
                    item = GetItemFromInventory(entry.Key)
                };
                result.Add(item);
            }
            return result;
        }

        /// <summary>
        /// Get total count of all items grouped by InventoryItem regardless of stacking
        /// </summary>
        /// <returns></returns>
        public List<ItemReference> GetGroupedCounts(List<string> categoryNames)
        {
            Dictionary<string, int> tempResult = new Dictionary<string, int>();

            foreach (InventoryItem item in Items)
            {
                if (categoryNames.Contains(item.category.name))
                {
                    if (tempResult.ContainsKey(item.name))
                    {
                        tempResult[item.name] += item.CurrentCount;
                    }
                    else
                    {
                        tempResult.Add(item.name, item.CurrentCount);
                    }
                }
            }

            List<ItemReference> result = new List<ItemReference>();
            foreach (KeyValuePair<string, int> entry in tempResult)
            {
                ItemReference item = new ItemReference
                {
                    count = entry.Value,
                    item = GetItemByName(entry.Key)
                };
                result.Add(item);
            }
            return result;
        }

        /// <summary>
        /// Get item by assigned instance id
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public InventoryItem GetItemByInstanceId(string instanceId)
        {
            foreach (InventoryItem item in Items)
            {
                if (item.InstanceId == instanceId)
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieve an item from the list of all inventory items by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public InventoryItem GetItemByName(string name)
        {
            foreach (InventoryItem item in InventoryDB.AvailableItems)
            {
                if (item != null && item.name == name)
                {
                    return item;
                }
            }
            return null;
        }

        public InventoryItem GetItemInstanceInInventory(InventoryItem item)
        {
            if (!Items.Contains(item))
            {
                foreach (InventoryItem invItem in Items)
                {
                    if (invItem.name == item.name)
                    {
                        return invItem;
                    }
                }
            }
            else
            {
                return item;
            }

            return null;
        }

        /// <summary>
        /// Get a count of item from all stacks in inventory
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int GetItemTotalCount(InventoryItem item)
        {
            int count = 0;

            foreach (InventoryItem invItem in Items)
            {
                if (invItem.name == item.name)
                {
                    count += invItem.CurrentCount;
                }
            }

            return count;
        }
     

        /// <summary>
        /// Get a count of item from all stacks in inventory with minimum condition & rarity
        /// </summary>
        /// <param name="item"></param>
        /// <param name="minCondition"></param>
        /// <param name="minRarity"></param>
        /// <returns></returns>
        public int GetItemTotalCount(InventoryItem item, float minCondition, int minRarity)
        {
            int count = 0;

            foreach (InventoryItem invItem in Items)
            {
                if (invItem.name == item.name && invItem.condition >= minCondition && invItem.rarity >= minRarity)
                {
                    count += invItem.CurrentCount;
                }
            }

            return count;
        }

        /// <summary>
        /// Get first active instance of an item from the inventory
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public InventoryItem GetItemFromInventory(string itemName)
        {
            foreach (InventoryItem iitem in Items)
            {
                if (iitem.name == itemName && iitem.CurrentCount > 0)
                {
                    return iitem;
                }
            }
            return null;
        }

        public InventoryItem GetItemFromInventory(string itemName, float minCondition, int minRarity, int exactCount)
        {
            foreach (InventoryItem iitem in Items)
            {
                if (iitem.name == itemName && iitem.rarity >= minRarity && iitem.condition >= minCondition && iitem.CurrentCount == exactCount)
                {
                    return iitem;
                }
            }
            return null;
        }

        /// <summary>
        /// Get first active instance of an item from the inventory
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public InventoryItem GetItemFromInventory(InventoryItem item)
        {
            foreach (InventoryItem iitem in Items)
            {
                if (iitem.name == item.name && iitem.CurrentCount > 0)
                {
                    return iitem;
                }
            }
            return null;
        }

        /// <summary>
        /// Get a list of all items in a category
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public List<InventoryItem> GetItems(string categoryName)
        {
            List<InventoryItem> result = new List<InventoryItem>();

            foreach (InventoryItem item in Items)
            {
                if (item.category.name == categoryName)
                {
                    result.Add(item);
                }
            }

            return result;
        }

        /// <summary>
        /// Get a list of all items in a category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<InventoryItem> GetItems(Category category)
        {
            return GetItems(category.name);
        }

        /// <summary>
        /// Get a list of all items in a list of categories
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public List<InventoryItem> GetItems(List<string> categories)
        {
            List<InventoryItem> result = new List<InventoryItem>();

            foreach (InventoryItem item in Items)
            {
                if (categories.Contains(item.category.name))
                {
                    result.Add(item);
                }
            }

            return result;
        }

        /// <summary>
        /// Get a list of all items in a list of categories
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public List<InventoryItem> GetItems(List<Category> categories)
        {
            List<string> categoryNames = new List<string>();
            foreach (Category category in categories)
            {
                categoryNames.Add(category.name);
            }

            return GetItems(categoryNames);
        }

        /// <summary>
        ///  Get a list of all inventory items with specified tag value
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<InventoryItem> GetItemsWithTag(string tagName, string value)
        {
            List<InventoryItem> results = new List<InventoryItem>();

            foreach (InventoryItem item in Items)
            {
                if (item.GetCustomTag(tagName) == value)
                {
                    results.Add(item);
                }
            }

            return results;
        }

        /// <summary>
        ///  Get a list of all inventory items with specified tag value in category
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<InventoryItem> GetItemsWithTag(string tagName, string value, string category)
        {
            List<InventoryItem> results = new List<InventoryItem>();

            foreach (InventoryItem item in Items)
            {
                if (item.category.name == category && item.GetCustomTag(tagName) == value)
                {
                    results.Add(item);
                }
            }

            return results;
        }

        /// <summary>
        ///  Get a list of all inventory items with specified tag value in categories
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<InventoryItem> GetItemsWithTag(string tagName, string value, List<string> categories)
        {
            List<InventoryItem> results = new List<InventoryItem>();

            foreach (InventoryItem item in Items)
            {
                if (categories.Contains(item.category.name) && item.GetCustomTag(tagName) == value)
                {
                    results.Add(item);
                }
            }

            return results;
        }

        public static bool GetKeyDown(KeyCode key)
        {
#if GAME_COG
            if (GameCog.Input != null) return GameCog.Input.GetKeyDown(key);
#endif
            return Input.GetKeyDown(key);
        }

        /// <summary>
        /// Get the equip pointby id
        /// </summary>
        /// <param name="pointId"></param>
        /// <returns></returns>
        public EquipPoint GetPointById(string pointId)
        {
            // Attempt to find empty slot
            foreach (EquipPoint point in EquipPoints)
            {
                if (point.pointId == pointId)
                {
                    return point;
                }
            }

            return null;
        }

        /// <summary>
        /// Get the equip point that will be used if item is equipped
        /// </summary>
        /// <param name="item">First Empty or First Matching</param>
        /// <returns></returns>
        public EquipPoint GetPointToUse(InventoryItem item)
        {
            // Attempt to find empty slot
            foreach (EquipPoint point in EquipPoints)
            {
                if (item.equipPoints.Contains(point.pointId))
                {
                    if (!point.IsItemEquippedOrStored)
                    {
                        return point;
                    }
                }
            }

            // Find first matching slot
            foreach (EquipPoint point in EquipPoints)
            {
                if (item.equipPoints.Contains(point.pointId))
                {
                    return point;
                }
            }

            return null;
        }

        /// <summary>
        /// Get total count of queued recipe
        /// </summary>
        /// <param name="recipe"></param>
        /// <returns></returns>
        public int GetQueuedCount(CraftingRecipe recipe)
        {
            int count = 0;
            foreach (CraftingQueueItem item in CraftingQueue)
            {
                if (item.recipe.name == recipe.name)
                {
                    count += item.count;
                }
            }

            return count;
        }

        /// <summary>
        /// Get the progress of first instance of queued recipe
        /// </summary>
        /// <param name="recipe"></param>
        /// <returns></returns>
        public float GetQueuedFirstProgress(CraftingRecipe recipe)
        {
            foreach (CraftingQueueItem item in CraftingQueue)
            {
                if (item != null && item.recipe != null)
                {
                    if (item.recipe.name == recipe.name)
                    {
                        switch (item.recipe.craftTime)
                        {
                            case CraftingTime.Instant:
                                return 0;
                            case CraftingTime.GameTime:
                                return 1 - (item.secondsRemaining / item.recipe.craftSeconds);
                            case CraftingTime.RealTime:
                                return (System.DateTime.Now.Ticks - item.timeStarted.Ticks) / (float)(item.realWorldEnd.Ticks - item.timeStarted.Ticks);
                        }
                    }
                }
            }

            return -1;
        }

        public float GetQueuedProgress(CraftingQueueItem item)
        {
            switch (item.recipe.craftTime)
            {
                case CraftingTime.Instant:
                    return 0;
                case CraftingTime.GameTime:
                    return 1 - (item.secondsRemaining / item.recipe.craftSeconds);
                case CraftingTime.RealTime:
                    return (System.DateTime.Now.Ticks - item.timeStarted.Ticks) / (float)(item.realWorldEnd.Ticks - item.timeStarted.Ticks);
            }

            return -1;
        }

        /// <summary>
        /// Find a reciepe based on ingredients
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public CraftingRecipe GetRecipe(List<ItemReference> items)
        {
            bool matches;

            foreach (CraftingRecipe recipe in Recipes)
            {
                if (recipe.components.Count == items.Count)
                {
                    matches = true;
                    foreach (ItemReference component in recipe.components)
                    {
                        if (!ListContainsComponent(component, items))
                        {
                            matches = false;
                            break;
                        }
                    }
                    if (matches)
                    {
                        return recipe;
                    }
                }
            }

            return null;
        }

        public CraftingRecipe GetRecipe(string recipeName)
        {
            foreach (CraftingRecipe recipe in Recipes)
            {
                if (recipe.name == recipeName)
                {
                    return recipe;
                }
            }

            return null;
        }

        /// <summary>
        /// Get a list of all recipes by category name
        /// </summary>
        /// <param name="craftingCategory"></param>
        /// <returns></returns>
        public List<CraftingRecipe> GetRecipesByCategory(string craftingCategory)
        {
            List<CraftingRecipe> result = new List<CraftingRecipe>();

            foreach (CraftingRecipe recipe in Recipes)
            {
                if (recipe.craftingCategory.name == craftingCategory)
                {
                    result.Add(recipe);
                }
            }

            return result;
        }

        /// <summary>
        /// Get a list of all recipes by category name
        /// </summary>
        /// <param name="craftingCategory"></param>
        /// <returns></returns>
        public List<CraftingRecipe> GetRecipesByCategory(List<string> craftingCategories)
        {
            List<CraftingRecipe> result = new List<CraftingRecipe>();

            foreach (CraftingRecipe recipe in Recipes)
            {
                if (craftingCategories.Contains(recipe.craftingCategory.name))
                {
                    result.Add(recipe);
                }
            }

            return result;
        }

        /// <summary>
        /// Check if inventory contains items needed to craft recipe
        /// </summary>
        /// <returns></returns>
        public bool GetRecipeCraftable(CraftingRecipe recipe)
        {
            if (recipe == null) return false;

            if (recipe.componentType == ComponentType.Standard)
            {
                foreach (ItemReference item in recipe.components)
                {
                    if (GetItemTotalCount(item.item) < item.count) return false;
                }
            }
            else
            {
                foreach (AdvancedComponent item in recipe.advancedComponents)
                {
                    if (GetItemTotalCount(item.item, item.minCondition, item.minRarity) < item.count) return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check if we have the components required for repair
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool GetRepairable(InventoryItem item)
        {
            if (item == null || !item.CanRepair || item.condition == 1) return false;

            foreach (ItemReference component in item.incrementComponents)
            {
                if (GetItemTotalCount(component.item) < component.count) return false;
            }

            return true;
        }

        /// <summary>
        /// Get a list of all items we have components to repair
        /// </summary>
        /// <returns></returns>
        public List<InventoryItem> GetRepairableItems()
        {
            List<InventoryItem> repairableItems = new List<InventoryItem>();

            foreach (InventoryItem item in Items)
            {
                if (GetRepairable(item))
                {
                    repairableItems.Add(item);
                }
            }

            return repairableItems;
        }

        /// <summary>
        /// Get a list of all items we have components to repair in a category
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public List<InventoryItem> GetRepairableItems(string categoryName)
        {
            List<InventoryItem> repairableItems = new List<InventoryItem>();

            foreach (InventoryItem item in Items)
            {
                if (item.category.name == categoryName && GetRepairable(item))
                {
                    repairableItems.Add(item);
                }
            }

            return repairableItems;
        }

        /// <summary>
        /// Get a list of all items we have components to repair in a category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<InventoryItem> GetRepairableItems(Category category)
        {
            return GetRepairableItems(category.name);
        }

        /// <summary>
        /// Get a list of all items we have components to repair in a list of categories
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public List<InventoryItem> GetRepairableItems(List<string> categories)
        {
            List<InventoryItem> repairableItems = new List<InventoryItem>();

            foreach (InventoryItem item in Items)
            {
                if (categories.Contains(item.category.name) && GetRepairable(item))
                {
                    repairableItems.Add(item);
                }
            }

            return repairableItems;
        }

        /// <summary>
        /// Get a list of all items we have components to repair in a list of categories
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public List<InventoryItem> GetRepairableItems(List<Category> categories)
        {
            List<string> categoryNames = new List<string>();
            foreach (Category category in categories)
            {
                categoryNames.Add(category.name);
            }

            return GetRepairableItems(categoryNames);
        }

        /// <summary>
        /// Get the maximum number of increments we can repair w/ current inventory
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int GetRepairMaxIncrements(InventoryItem item)
        {
            if (item == null || !item.CanRepair || item.condition == 1) return 0;

            int maxRepairCount = Mathf.CeilToInt((1 - item.condition) / item.repairIncrement);
            int availCount = maxRepairCount;
            int itemCount;
            int itemAvail;

            // Calculate how many increments we can repair
            foreach (ItemReference component in item.incrementComponents)
            {
                itemCount = GetItemTotalCount(component.item);
                if (itemCount == 0) return 0;

                itemAvail = Mathf.FloorToInt(itemCount / (float)component.count);
                if (itemAvail == 0) return 0;
                if (itemAvail < availCount)
                {
                    availCount = itemAvail;
                }
            }

            if (item.incrementCost > 0)
            {
                if (currency == 0) return 0;
                return Mathf.Min((int)(currency / item.incrementCost), availCount);
                if (Fishcurrency == 0) return 0;
                return Mathf.Min((int)(Fishcurrency / item.incrementCost), availCount);
            }

            return availCount;
        }

        /// <summary>
        /// Get a list of all items needing repair
        /// </summary>
        /// <returns></returns>
        public List<InventoryItem> GetRepairNeededItems()
        {
            List<InventoryItem> repairableItems = new List<InventoryItem>();

            foreach (InventoryItem item in Items)
            {
                if (item.CanRepair && item.condition < 1)
                {
                    repairableItems.Add(item);
                }
            }

            return repairableItems;
        }

        /// <summary>
        /// Get a list of all items needing repair in a category
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public List<InventoryItem> GetRepairNeededItems(string categoryName)
        {
            List<InventoryItem> repairableItems = new List<InventoryItem>();

            foreach (InventoryItem item in Items)
            {
                if (item.category.name == categoryName && item.CanRepair && item.condition < 1)
                {
                    repairableItems.Add(item);
                }
            }

            return repairableItems;
        }

        /// <summary>
        /// Get a list of all items needing repair in a category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<InventoryItem> GetRepairNeededItems(Category category)
        {
            return GetRepairNeededItems(category.name);
        }

        /// <summary>
        /// Get a list of all items needing repair in a list of categories
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public List<InventoryItem> GetRepairNeededItems(List<string> categories)
        {
            List<InventoryItem> repairableItems = new List<InventoryItem>();

            foreach (InventoryItem item in Items)
            {
                if (categories.Contains(item.category.name))
                {
                    if (item.CanRepair && item.condition < 1)
                    {
                        repairableItems.Add(item);
                    }
                }
            }

            return repairableItems;
        }

        /// <summary>
        /// Get a list of all items needing repair in a list of categories
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public List<InventoryItem> GetRepairNeededItems(List<Category> categories)
        {
            List<string> categoryNames = new List<string>();
            foreach (Category category in categories)
            {
                categoryNames.Add(category.name);
            }

            return GetRepairNeededItems(categoryNames);
        }

        /// <summary>
        /// Get a list of all unlocked recipes by category name
        /// </summary>
        /// <param name="craftingCategory"></param>
        /// <returns></returns>
        public List<CraftingRecipe> GetUnlockedRecipesByCategory(string craftingCategory)
        {
            List<CraftingRecipe> result = new List<CraftingRecipe>();

            foreach (CraftingRecipe recipe in Recipes)
            {
                if (recipe.Unlocked && recipe.craftingCategory.name == craftingCategory)
                {
                    result.Add(recipe);
                }
            }

            return result;
        }

        /// <summary>
        /// Get a list of all unlocked recipes by category name
        /// </summary>
        /// <param name="craftingCategory"></param>
        /// <returns></returns>
        public List<CraftingRecipe> GetUnlockedRecipesByCategory(List<string> craftingCategories)
        {
            List<CraftingRecipe> result = new List<CraftingRecipe>();

            foreach (CraftingRecipe recipe in Recipes)
            {
                if (recipe.Unlocked && craftingCategories.Contains(recipe.craftingCategory.name))
                {
                    result.Add(recipe);
                }
            }

            return result;
        }

        /// <summary>
        /// Get the currently active ammo by type
        /// </summary>
        /// <param name="ammoType"></param>
        /// <returns></returns>
        public InventoryItem GetSelectedAmmo(string ammoType)
        {
            if (activeAmmo.ContainsKey(ammoType))
            {
                return activeAmmo[ammoType];
            }
            return null;
        }

        /// <summary>
        /// Load inventory state from stream
        /// </summary>
        /// <param name="stream"></param>
        public void InventoryStateLoad(Stream stream)
        {
            if (stream.Length == 0) return;

            float version = stream.ReadFloat();
            if (version < 1 || version > FILE_VERSION)
            {
                Debug.LogError("Unknown file version");
                return;
            }

            TotalWeight = stream.ReadFloat();
            if (version >= 1.2f)
            {
                currency = stream.ReadFloat();
                Fishcurrency= stream.ReadFloat();
            }

            Items.Clear();
            string itemName, value;
            int count = stream.ReadInt();
            for (int i = 0; i < count; i++)
            {
                itemName = stream.ReadStringPacket();
                InventoryItem baseItem = GetItemByName(itemName);
                if (baseItem == null)
                {
                    Debug.Log("Cannot find:" + baseItem);
                }
                InventoryItem item = Instantiate(GetItemByName(itemName));
                item.name = itemName;
                item.Initialize(this);
                item.StateLoad(stream, this);
                Items.Add(item);
            }

            if (version >= 1.4f)
            {
                count = stream.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    LoadCategoryState(stream.ReadStringPacket(), stream, version);
                }
            }
            else
            {
                foreach (Category category in Categories)
                {
                    category.StateLoad(stream, this, version);
                }
            }

            // Crafting Categories
            if (version >= 1.6f)
            {
                CraftingCategories.Clear();

                count = stream.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    CraftingCategory newCC = ScriptableObject.CreateInstance<CraftingCategory>();
                    newCC.name = stream.ReadStringPacket();
                    newCC.StateLoad(stream, this);
                    CraftingCategories.Add(newCC);
                }
            }

            // Read EquipPoint data
            foreach (EquipPoint point in EquipPoints)
            {
                point.StateLoad(stream, this);
            }

            if (version >= 1.1f)
            {
                CraftingQueue.Clear();
                count = stream.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    CraftingQueueItem item = new CraftingQueueItem();
                    item.LoadState(stream);
                    CraftingQueue.Add(item);
                }
            }

            if (version >= 1.3f)
            {
                foreach (CraftingRecipe recipe in Recipes)
                {
                    recipe.StateLoad(stream, version);
                }
            }

            if (version >= 1.5f && version < 1.9f)
            {
                // Loadouts
                for (int i = 0; i < 5; i++)
                {
                    loadouts[i].Load(stream);
                }

                // Assigned Skills
                assignedSkills.Clear();
                count = stream.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    itemName = stream.ReadStringPacket();
                    value = stream.ReadStringPacket();
                    assignedSkills.Add(itemName, value);
                    onSkillSlotChanged?.Invoke(itemName, GetItemFromInventory(value));
                }
            }

            if (version >= 1.8f)
            {
                if (Hotbar != null)
                {
                    Hotbar.Load(stream, this);
                }
            }
        }

        /// <summary>
        /// Load inventory state from file
        /// </summary>
        /// <param name="filename"></param>
        public void InventoryStateLoad(string filename)
        {
            if (File.Exists(filename))
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    InventoryStateLoad(fs);
                }
            }
            else
            {
                Debug.LogWarning(name + ".InventoryCog.InventoryStateLoad file does not exist: " + filename);
            }
        }

        /// <summary>
        /// Save current inventory state to a stream
        /// </summary>
        /// <param name="stream"></param>
        public void InventoryStateSave(Stream stream)
        {
            // Write version number
            stream.WriteFloat(FILE_VERSION);

            stream.WriteFloat(TotalWeight);
            stream.WriteFloat(currency);
            stream.WriteFloat(Fishcurrency);

            // Write item data
            stream.WriteInt(Items.Count);
            foreach (InventoryItem item in Items)
            {
                stream.WriteStringPacket(item.name);
                item.StateSave(stream);
            }

            // Write category data
            stream.WriteInt(Categories.Count);
            foreach (Category category in Categories)
            {
                stream.WriteStringPacket(category.name);
                category.StateSave(stream, FILE_VERSION);
            }

            // Write Crafting Category Data
            stream.WriteInt(CraftingCategories.Count);
            foreach (CraftingCategory category in CraftingCategories)
            {
                stream.WriteStringPacket(category.name);
                category.StateSave(stream);
            }

            // Write EquipPoint data
            foreach (EquipPoint point in EquipPoints)
            {
                point.StateSave(stream);
            }

            // Write CraftingQueue data
            stream.WriteInt(CraftingQueue.Count);
            foreach (CraftingQueueItem item in CraftingQueue)
            {
                item.SaveState(stream);
            }

            // Write recipe data
            foreach (CraftingRecipe recipe in Recipes)
            {
                recipe.StateSave(stream, FILE_VERSION);
            }

            //// Write loadouts
            /// Removed starting file v1.9
            //for (int i = 0; i < 5; i++)
            //{
            //    loadouts[i].Save(stream);
            //}

            // Write skill slots
            stream.WriteInt(assignedSkills.Count);
            foreach (var item in assignedSkills)
            {
                stream.WriteStringPacket(item.Key);
                stream.WriteStringPacket(item.Value);
            }

            // Write hotbar
            if (Hotbar != null)
            {
                stream.WriteInt(Hotbar.hotbarSize);
                for (int i = 0; i < Hotbar.hotbarSize; i++)
                {
                    Hotbar.HotbarSlots[i].StateSave(stream);
                }
            }
        }

        /// <summary>
        /// Save current inventory state to a file
        /// </summary>
        /// <param name="filename"></param>
        public void InventoryStateSave(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write))
            {
                InventoryStateSave(fs);
            }
        }

        /// <summary>
        /// Save inventory to specified loadout
        /// </summary>
        /// <param name="loadoutIndex"></param>
        /// <param name="equippedOnly"></param>
        public void InventoryToLoadout(int loadoutIndex, bool equippedOnly)
        {
            InventoryLoadout loadout = new InventoryLoadout();
            loadout.PopulateFromInventory(this, equippedOnly);
        }

        /// <summary>
        /// Check if an item is using the requested slot id
        /// </summary>
        /// <param name="slotId"></param>
        /// <returns></returns>
        public bool IsSlotIdUsed(int slotId)
        {
            foreach (InventoryItem item in Items)
            {
                if (item.useSlotId && item.slotId == slotId)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if we have a recipe and components to upgrade an item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool ItemCanUpgrade(InventoryItem item)
        {
            foreach (CraftingRecipe recipe in Recipes)
            {
                if (recipe.craftType == CraftingType.Upgrade && recipe.baseItem.upgradeItem.name == item.name)
                {
                    if (GetRecipeCraftable(recipe)) return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Update inventory to reflect loadout
        /// </summary>
        /// <param name="loadoutIndex"></param>
        /// <param name="requireInInventory"></param>
        /// <param name="clearExistingInventory"></param>
        public void LoadoutToInventory(int loadoutIndex, bool requireInInventory, bool clearExistingInventory)
        {
            if (clearExistingInventory)
            {
                ClearInventory();
            }

            InventoryItem item = null;
            foreach (InventoryLoadoutItem loadoutItem in loadouts[loadoutIndex].loadoutItems)
            {
                if (requireInInventory)
                {
                    item = GetItemByInstanceId(loadoutItem.instanceId);
                }
                else
                {
                    item = GetItemByName(item.name);
                }

                if (item != null)
                {
                    if (!requireInInventory)
                    {
                        AddToInventory(item, loadoutItem.count);
                    }
                }
            }
        }

        /// <summary>
        /// Close inventory menu
        /// </summary>
        public void MenuClose()
        {
#if GAME_COG
            if (GameCog.IsModalVisible) return;
#endif
            if (ActiveTheme != null)
            {
                ActiveTheme.CloseMenu();
            }
        }

        /// <summary>
        /// Open inventory menu
        /// </summary>
        public void MenuOpen()
        {
#if GAME_COG
            if (GameCog.IsModalVisible) return;
#endif
            if (IsMenuOpen) return;

            if (ActiveTheme != null)
            {
                ActiveTheme.OpenInventory();
            }
        }

        /// <summary>
        /// Remove an item from inventory
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        public void RemoveItem(InventoryItem item, int count)
        {
            RemoveItem(item, count, 0, 0);
        }

        /// <summary>
        /// Remove an item from inventory
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        /// <param name="minCondition"></param>
        /// <param name="minRarity"></param>
        public void RemoveItem(InventoryItem item, int count, float minCondition, int minRarity)
        {
            if (count < 1) return;

            foreach (InventoryItem invItem in Items)
            {
                if (invItem.name == item.name && invItem.rarity >= minRarity && invItem.condition >= minCondition)
                {
                    if (invItem.canStack && invItem.CurrentCount > count)
                    {
                        invItem.CurrentCount -= count;
                        TotalWeight -= invItem.weight * count;
                        onItemRemoved?.Invoke(item, count);
                        if (item.itemType == ItemType.Ammo) UpdateAmmoEvents(item);
                    }
                    else if (invItem.canStack)
                    {
                        if (invItem.EquipState != EquipState.NotEquipped)
                        {
                            invItem.CurrentEquipPoint.UnequipItem();
                        }
                        FinalizeRemove(invItem, GetItemCategory(invItem));
                        count -= invItem.CurrentCount;
                        TotalWeight -= invItem.weight * invItem.CurrentCount;
                        if (item.itemType == ItemType.Ammo) UpdateAmmoEvents(item);

                        if (count > 0)
                        {
                            RemoveItem(GetItemFromInventory(invItem), count, minCondition, minRarity);
                        }
                    }
                    else
                    {
                        if (invItem.EquipState != EquipState.NotEquipped)
                        {
                            invItem.CurrentEquipPoint.UnequipItem();
                        }
                        TotalWeight -= invItem.weight;
                        FinalizeRemove(invItem, GetItemCategory(invItem));
                        count -= 1;
                        if (item.itemType == ItemType.Ammo) UpdateAmmoEvents(item);
                        if (count > 0)
                        {
                            RemoveItem(GetItemFromInventory(invItem), count, minCondition, minRarity);
                        }
                    }

                    return;

                }
            }
        }

        /// <summary>
        /// Repair item with components
        /// </summary>
        /// <param name="item"></param>
        public void RepairItem(InventoryItem item)
        {
            if (item == null || !item.CanRepair || item.condition == 1) return;

            // Consume repair components
            int availCount = GetRepairMaxIncrements(item);
            foreach (ItemReference component in item.incrementComponents)
            {
                UseItem(component.item, component.count * availCount);
            }

            // Remove currency
            currency -= item.incrementCost * availCount;
            Fishcurrency -= item.incrementCost * availCount;

            // Apply repair
            item.condition = Mathf.Clamp(item.condition + item.repairIncrement * availCount, 0, 1);
        }

        /// <summary>
        /// Set the active ammo for a type
        /// </summary>
        /// <param name="item"></param>
        public void SetSelectedAmmo(InventoryItem item)
        {
            if (activeAmmo.ContainsKey(item.ammoType))
            {
                if (GetEquippedAmmoCount("Bait") != 0 && item.previewScale == 2 && item.ammoType == "Float")
                {
                    activeAmmo.Remove("Bait");
                }
                if (item.ammoType == "Bait" && GetEquippedAmmoSpinningCount("Float") != 0)
                {
                    return;
                }

                activeAmmo[item.ammoType] = item;
            }
            else
            {
                if (GetEquippedAmmoCount("Bait") != 0 && item.previewScale == 2 && item.ammoType == "Float")
                {
                    activeAmmo.Remove("Bait");
                }
                if (item.ammoType == "Bait" && GetEquippedAmmoSpinningCount("Float") != 0)
                {
                    return;
                }
                activeAmmo.Add(item.ammoType, item);
            }
            

            onItemEquipped?.Invoke(item);

            UpdateAmmoEvents(item);
        }


        public void SkillAssign(InventoryItem item, string skillSlot)
        {
            if (item == null || item.itemType != ItemType.Skill)
            {
                Debug.LogError(name + ".InvnetoryCog.SkillAssign supplied item is not a skill");
                return;
            }

            if (!skillSlots.Contains(skillSlot))
            {
                Debug.LogError(name + ".InvnetoryCog.SkillAssign '" + skillSlot + "' is not a Skill Slot");
                return;
            }

            // Check for item already assigned
            foreach (var skill in assignedSkills)
            {
                if (skill.Value == item.name)
                {
                    if (skill.Key == skillSlot) return;
                    onSkillSlotChanged?.Invoke(skill.Key, null);
                    assignedSkills.Remove(skill.Key);
                    break;
                }
            }

            // Check for existing key w/ different value
            if (assignedSkills.ContainsKey(skillSlot))
            {
                GetItemFromInventory(assignedSkills[skillSlot]).AssignedSkillSlot = null;
                assignedSkills.Remove(skillSlot);
                assignedSkills.Add(skillSlot, item.name);
                item.AssignedSkillSlot = skillSlot;
                onSkillSlotChanged?.Invoke(skillSlot, item);
                return;
            }

            // New entry
            assignedSkills.Add(skillSlot, item.name);
            item.AssignedSkillSlot = skillSlot;
            onSkillSlotChanged?.Invoke(skillSlot, item);
        }

        public void SkillToggle(InventoryItem item, string skillSlot)
        {
            // Check for existing
            if (assignedSkills.ContainsKey(skillSlot))
            {
                if (assignedSkills[skillSlot] == item.name)
                {
                    assignedSkills.Remove(skillSlot);
                    item.AssignedSkillSlot = null;
                    onSkillSlotChanged?.Invoke(skillSlot, null);
                    return;
                }
            }

            SkillAssign(item, skillSlot);
        }

        /// <summary>
        /// Move equipment to storage point
        /// </summary>
        /// <param name="pointId">Equip point id</param>
        public void StorePointById(string pointId)
        {
            foreach (EquipPoint point in EquipPoints)
            {
                if (point.pointId == pointId)
                {
                    point.StoreItem();
                    return;
                }
            }
        }

        /// <summary>
        /// Unequip an item (if currently equipped)
        /// </summary>
        /// <param name="item"></param>
        public void UnequipItem(InventoryItem item)
        {
            foreach (EquipPoint point in EquipPoints)
            {
                if (point.IsItemEquipped && point.Item == item)
                {
                    point.UnequipItem();
                    return;
                }

                if (point.IsItemStored && point.storePoint.Item == item)
                {
                    point.UnequipItem();
                    return;
                }
            }
        }

        public void UnequipBaitItem()
        {
            foreach (EquipPoint point in EquipPoints)
            {
                if (point.IsItemEquipped && point.Item.equipPoints[0] == "Bait")
                {
                    point.UnequipItem();
                    return;
                }

                if (point.IsItemStored && point.storePoint.Item.equipPoints[0] == "Bait")
                {
                    point.UnequipItem();
                    return;
                }
            }
        }

        /// <summary>
        /// Move an item from storage to equip point
        /// </summary>
        /// <param name="pointId">Equip point id</param>
        public void UnstorePointById(string pointId)
        {
            foreach (EquipPoint point in EquipPoints)
            {
                if (point.pointId == pointId)
                {
                    point.UnstoreItem();
                    return;
                }
            }
        }

        /// <summary>
        /// Remove count from inventory. Unequip and remove as needed
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        /// <param name="removeOnZero"></param>
        public void UseItem(InventoryItem item, int count, bool removeOnZero = true)
        {
            if (item.CurrentCount > count)
            {
                item.CurrentCount -= count;
                UseEffects(item, count);
                TotalWeight -= item.weight * count;
            }
            else if (item.CurrentCount == count)
            {
                TotalWeight -= item.weight * item.CurrentCount;
                item.CurrentCount = 0;
                UseEffects(item, count);
                if (removeOnZero) FinalizeRemove(item, GetItemCategory(item));
            }
            else
            {
                UseEffects(item, item.CurrentCount);
                count -= item.CurrentCount;
                TotalWeight -= item.weight * item.CurrentCount;
                if (removeOnZero) FinalizeRemove(item, GetItemCategory(item));

                InventoryItem newItem = GetItemFromInventory(item);
                if (newItem != null)
                {
                    UseItem(newItem, count, removeOnZero);
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Add count to item stack
        /// </summary>
        /// <param name="item"></param>
        /// <param name="countToAdd"></param>
        /// <returns></returns>
        private int AddToItemStack(InventoryItem item, int countToAdd)
        {
            int usedStacks = 0;
            int added;
            int i;

            // Append whatever possible to existing stacks
            for (i = 0; i < Items.Count; i++)
            {
                if (Items[i].name == item.name && Items[i].condition == item.condition && Items[i].rarity == item.rarity && !item.IsAttached)
                {
                    usedStacks += 1;
                    if (Items[i].CurrentCount < item.countPerStack)
                    {
                        added = Mathf.Min(countToAdd, item.countPerStack - Items[i].CurrentCount);
                        Items[i].CurrentCount += added;

                        countToAdd -= added;
                        if (countToAdd == 0)
                        {
                            return 0;
                        }
                    }
                }
            }

            // Create new stacks (where possible)
            while (countToAdd > 0)
            {
                // Return if out of space
                if (item.hasMaxStacks && usedStacks >= item.maxStacks)
                {
                    return countToAdd;
                }

                usedStacks += 1;
                InventoryItem newItem = Instantiate(item);
                newItem.name = item.name;
                newItem.Initialize(this);
                added = Mathf.Min(countToAdd, item.countPerStack);
                newItem.CurrentCount = added;
                countToAdd -= added;

                if (newItem.useSlotId)
                {
                    if (IsSlotIdUsed(newItem.slotId))
                    {
                        newItem.slotId = GetFirstFreeSlotId();
                    }
                }

                Items.Add(newItem);
                GetItemCategory(item).AddItem(newItem);
                TotalWeight += newItem.weight + newItem.CurrentCount;
            }

            // Set remainder
            return 0;
        }

        /// <summary>
        /// Check if we have a free slot in our target category
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        private bool CategoryHasFreeSlot(Category category)
        {
            if (category == null) return false;
            if (!category.hasMaxSlots) return true;
            if (category.hasLockingSlots) return category.UsedSlots < category.UnlockedSlots;
            return category.UsedSlots < category.MaximumSlots;
        }

        private void CompleteCraft(CraftingRecipe recipe, int count, List<ItemReference> master)
        {
            // Get success/fail count
            int successCount = 0;
            int failCount = 0;
            float successChance = recipe.GetSuccessChance();
            if (successChance == 1)
            {
                successCount = count;
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    float craftRoll = Random.Range(0f, 1f);

                    if (craftRoll >= successChance)
                    {
                        failCount += 1;
                    }
                }
            }

            // Add success result to inventory
            List<CraftingResult> results = new List<CraftingResult>();
            if (recipe.craftType == CraftingType.Create)
            {
                if (successCount > 0)
                {
                    recipe.SuccessCount += successCount;
                    if (recipe.FirstCrafted == null) recipe.FirstCrafted = System.DateTime.Now;
                    recipe.LastCrafted = System.DateTime.Now;

                    foreach (CraftingResult item in recipe.result)
                    {
                        InventoryItem result = Instantiate(item.item);
                        item.SetResults(master, result);
                        result.name = item.item.name;
                        result.InstanceId = System.Guid.NewGuid().ToString();
                        AddToInventory(result, item.count * successCount);

                        CraftingResult craft = new CraftingResult
                        {
                            item = result,
                            count = result.CurrentCount
                        };
                        results.Add(craft);
                    }
                }

                // Add fail result to inventory
                if (failCount > 0)
                {
                    recipe.FailCount += failCount;

                    foreach (CraftingResult item in recipe.failResult)
                    {
                        InventoryItem result = Instantiate(item.item);
                        item.SetResults(master, result);
                        result.name = item.item.name;
                        result.InstanceId = System.Guid.NewGuid().ToString();
                        AddToInventory(result, item.count * failCount);

                        CraftingResult craft = new CraftingResult
                        {
                            item = result,
                            count = result.CurrentCount
                        };
                        results.Add(craft);
                    }

                    onCraftingFailed?.Invoke(recipe, failCount);
                }
            }
            else
            {
                if (successCount > 0)
                {
                    InventoryItem item = Instantiate(recipe.upgradeSuccess.upgradeResult);
                    item.name = recipe.upgradeSuccess.upgradeResult.name;
                    item.CurrentCount = successCount;
                    if (recipe.upgradeSuccess.rarity == UpgradeResult.AddAmount) item.rarity = master[0].item.rarity + recipe.upgradeSuccess.rarityChange;
                    if (recipe.upgradeSuccess.condition == UpgradeResult.AddAmount) item.condition = master[0].item.condition + recipe.upgradeSuccess.conditionChange;
                    AddToInventory(item, successCount);
                }
            }
        }

        /// <summary>
        /// Equip only to the first EMPTY slot
        /// </summary>
        /// <param name="item"></param>
        private bool EquipFirstEmpty(InventoryItem item)
        {
            foreach (EquipPoint point in EquipPoints)
            {
                if (item.equipPoints.Contains(point.pointId))
                {
                    if (!point.IsItemEquippedOrStored)
                    {
                        if (item.equipLocation == AutoEquipLocation.AlwaysStore && point.storePoint != null)
                        {
                            point.StoreItem(item);
                        }
                        else
                        {
                            point.EquipItem(item);
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Equip to first empty slot, if none equip to first slot
        /// </summary>
        /// <param name="item"></param>
        private void EquipFirstOrEmpty(InventoryItem item)
        {
            // Equip empty if possible
            if (EquipFirstEmpty(item)) return;

            // Swap w/ first item
            foreach (EquipPoint point in EquipPoints)
            {
                if (item.equipPoints.Contains(point.pointId))
                {
                    bool wasStored = point.IsItemStored;
                    //point.UnequipItem();

                    // Assign
                    if (wasStored && item.equipLocation == AutoEquipLocation.MirrorCurrent || (point.storePoint != null && item.equipLocation == AutoEquipLocation.AlwaysStore))
                    {
                        // Store
                        point.StoreItem(item);
                        return;
                    }
                    else
                    {
                        // Equip
                        point.EquipItem(item);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Finalize removal of item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="category"></param>
        private void FinalizeRemove(InventoryItem item, Category category)
        {
            if(item.itemType == ItemType.Container)
            {
                foreach(InventoryItem itm in item.StoredItems)
                {
                    FinalizeRemove(itm, itm.category);
                }
            }

            if (item.CanEquip && item.EquipState != EquipState.NotEquipped) UnequipItem(item);
            category.RemoveItem(item);
            Items.Remove(item);
            onItemRemoved?.Invoke(item, item.CurrentCount);
        }

        private Canvas GetOrCreateCanvas()
        {
            Canvas canvas = GameObject.FindObjectOfType<Canvas>();
            if (canvas != null) return canvas;

            GameObject go = new GameObject("InventoryCogCreatedCanvas");
            canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            go.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            go.AddComponent<UnityEngine.EventSystems.EventSystem>();
            go.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            return canvas;
        }

        /// <summary>
        /// Get active category reference for item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Category GetItemCategory(InventoryItem item)
        {
            if (item.category == null)
            {
                Debug.LogError(item.name + ": No category assigned to item.");
                return null;
            }

            foreach (Category category in Categories)
            {
                if (category.name == item.category.name)
                {
                    return category;
                }
            }

            return null;
        }

        /// <summary>
        /// Check if item is in active inventory
        /// </summary>
        /// <param name="item"></param>
        /// <param name="inventoryItem"></param>
        /// <returns></returns>
        private bool IsItemInInventory(InventoryItem item, out InventoryItem inventoryItem)
        {
            foreach (InventoryItem localItem in Items)
            {
                if (localItem.name == item.name && localItem.rarity == item.rarity &&
                    localItem.condition == item.condition && localItem.value == item.value)
                {
                    inventoryItem = localItem;
                    return true;
                }
            }

            inventoryItem = null;
            return false;
        }

        /// <summary>
        /// Check if we can create a new stack for item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        private bool ItemHasFreeStack(InventoryItem item, Category category)
        {
            if (!item.canStack || !item.hasMaxStacks) return true;
            return category.GetItemUsedStacks(item) < item.maxStacks;
        }

        /// <summary>
        /// Check if list has all required instances of an item
        /// </summary>
        /// <param name="component"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        private bool ListContainsComponent(ItemReference component, List<ItemReference> items)
        {
            foreach (ItemReference item in items)
            {
                if (item.item.name == component.item.name)
                {
                    return item.count == component.count;
                }
            }

            return false;
        }

        /// <summary>
        /// Loads a streamed category
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="stream"></param>
        private void LoadCategoryState(string categoryName, Stream stream, float version)
        {
            foreach (Category cat in Categories)
            {
                if (cat.name == categoryName)
                {
                    cat.StateLoad(stream, this, version);
                    return;
                }
            }

            Category newCat = Instantiate<Category>(Categories[0]);
            newCat.name = categoryName;
            Categories.Add(newCat);
            newCat.StateLoad(stream, this, version);
        }

        /// <summary>
        /// Raise equip event
        /// </summary>
        /// <param name="item"></param>
        private void PointEquipped(InventoryItem item)
        {
            onItemEquipped?.Invoke(item);
        }

        /// <summary>
        /// Raise store event
        /// </summary>
        /// <param name="item"></param>
        private void PointStored(InventoryItem item)
        {
            onItemStored?.Invoke(item);
        }

        /// <summary>
        /// Raise un-equip event
        /// </summary>
        /// <param name="item"></param>
        private void PointUnequipped(InventoryItem item)
        {
            onItemUnequipped?.Invoke(item);
        }

        internal void ReApplyAnimVars()
        {
            Animator anim = gameObject.GetComponentInChildren<Animator>();
            if (anim == null) return;

            foreach (EquipPoint ep in EquipPoints)
            {
                if (ep.IsItemEquippedOrStored)
                {
                    foreach (AnimatorMod mod in ep.Item.equipAnimatorMods)
                    {
                        mod.ApplyMod(anim);
                    }

                }
            }
        }

        /// <summary>
        /// Spawn drop object as needed
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        private void SpawnDrop(InventoryItem item, int count)
        {
            if (count < 1) return;

            if (spawnDropManually)
            {
                if (item.itemType != ItemType.Container)
                {
                    onSpawnDropRequested?.Invoke(item, count);
                }
                else
                {
                    List<ItemReference> contents = new List<ItemReference>();
                    foreach(InventoryItem itm in item.StoredItems)
                    {
                        contents.Add(new ItemReference { item = itm, count = itm.CurrentCount });
                    }

                    onContainerDropped?.Invoke(item, count, contents);
                }
            }
            else
            {
                if (item.dropObject == null) return;

                LootItem drop = Instantiate(item.dropObject);
                drop.condition1 = item.condition;
                drop.conditionGen = GenerationType.Constant;
                drop.rarity1 = item.rarity;
                drop.rarityGen = GenerationType.Constant;
                drop.value1 = item.value;
                drop.valueGen = GenerationType.Constant;
                drop.count = count;
                drop.transform.position = transform.position + (transform.forward + dropOffset);

                if(item.itemType == ItemType.Container)
                {
                    drop.ContainedItems = new List<ItemReference>();
                    foreach (InventoryItem itm in item.StoredItems)
                    {
                        drop.ContainedItems.Add(new ItemReference { item = itm, count = itm.CurrentCount });
                    }
                }
            }
        }

        private void TriggerEnter(GameObject other)
        {
            if (other.GetComponent<LootItemWithUI>() != null) return;

            if (!headless && ActiveTheme != null && ActiveTheme.enableTriggers)
            {
                ActiveTheme.TriggerEnter(other);
            }
        }

        private void TriggerExit(GameObject other)
        {
            if (!headless && ActiveTheme != null && ActiveTheme.enableTriggers)
            {
                ActiveTheme.TriggerExit(other);
            }
        }

        private void UpdateAmmoEvents(InventoryItem ammoItem)
        {
            foreach (EquipPoint ep in EquipPoints)
            {
                if (ep.Item != null && ep.Item.usesAmmo && ep.Item.ammoType == ammoItem.ammoType)
                {
                    ep.onItemAmmoChanged?.Invoke(ep.Item);
                }
            }
        }

        private void UpdateCraftingQueue()
        {
            if (CraftingQueue == null)
            {
                CraftingQueue = new List<CraftingQueueItem>();
                return;
            }

            List<CraftingQueueItem> removeList = new List<CraftingQueueItem>();

            foreach (CraftingQueueItem item in CraftingQueue)
            {
                if (item.recipe.craftTime == CraftingTime.GameTime)
                {
                    item.secondsRemaining = Mathf.Clamp(item.secondsRemaining - Time.deltaTime, 0, item.secondsRemaining);
                    if (item.secondsRemaining == 0)
                    {
                        removeList.Add(item);
                        CompleteCraft(item.recipe, item.count, item.usedComponents);
                    }
                }
                else if (System.DateTime.Now >= item.realWorldEnd)
                {
                    removeList.Add(item);
                    CompleteCraft(item.recipe, item.count, item.usedComponents);
                }
            }

            foreach (CraftingQueueItem item in removeList)
            {
                CraftingQueue.Remove(item);
                onQueuedCraftComplete?.Invoke(item.recipe, item.count);
            }
        }

        private void UseEffects(InventoryItem item, int count)
        {
            // Unlocking
            foreach (Category category in item.unlockCategories)
            {
                Category localCat = GetCategory(category.name);
                if (localCat != null) localCat.catUnlocked = true;
            }

            foreach (CraftingCategory category in item.unlockCraftingCategories)
            {
                CraftingCategory localCat = GetCraftingCategory(category.name);
                if (localCat != null) localCat.catUnlocked = true;
            }

            foreach (CraftingRecipe recipe in item.unlockRecipes)
            {
                CraftingRecipe localRecipe = GetRecipe(recipe.name);
                if (localRecipe != null) localRecipe.unlocked = true;
            }


            // Stats
            if (StatsCog != null)
            {
                StatsCog.AddInventoryEffects(item, count);
            }
        }

        #endregion

        #region PUN Methods
#if PHOTON_UNITY_NETWORKING

        private GameObject FindChild(GameObject source, string name)
        {
            return (from x in source.GetComponentsInChildren<Transform>(true)
                    where x.gameObject.name == name
                    select x.gameObject).First();
        }

        public void InvokeParentChild(int childViewId, string subItemName)
        {
            Photon.Pun.PhotonView view = GetComponentInChildren<Photon.Pun.PhotonView>();
            view.RPC("ParentChild", Photon.Pun.RpcTarget.All, view.ViewID, childViewId, subItemName);
        }

        public void InvokeRebindObject(string skinnedObjectName)
        {
            Photon.Pun.PhotonView view = GetComponentInChildren<Photon.Pun.PhotonView>();
            view.RPC("RebindObject", Photon.Pun.RpcTarget.All, view.ViewID, skinnedObjectName);
        }

        [Photon.Pun.PunRPC]
        public void ParentChild(int parentViewId, int childViewId, string subItemName)
        {
            GameObject self = Photon.Pun.PhotonView.Find(parentViewId).gameObject;
            GameObject target = Photon.Pun.PhotonView.Find(childViewId).gameObject;

            if (!string.IsNullOrWhiteSpace(subItemName))
            {
                self = FindChild(self, subItemName);
            }

            target.transform.SetParent(self.transform);
            target.transform.localPosition = Vector3.zero;
            target.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }


        [Photon.Pun.PunRPC]
        public void RebindObject(int parentViewId, string skinnedObjectName)
        {
            SkinnedEquipPoint sep = FindChild(Photon.Pun.PhotonView.Find(parentViewId).gameObject, skinnedObjectName).GetComponent<SkinnedEquipPoint>();
            sep.ObjectReference = sep.gameObject.GetComponentInChildren<Photon.Pun.PhotonView>().gameObject;
            sep.RebindObject();
        }

#endif
        #endregion

    }
}
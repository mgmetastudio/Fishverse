using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    [HierarchyIcon("item_icon", "#ffffff", false)]
    public class ItemUI : MonoBehaviour, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {

        #region Variables

        public Image itemImage;
        public TextMeshProUGUI displayName;
        public TextMeshProUGUI description;
        public TextMeshProUGUI subtext;
        public GameObject hideIfNoSubtext;
        public GameObject equippedIndicator;
        public GameObject equipableIndicator;
        public string countPrefix = "x";
        public TextMeshProUGUI count;
        public string countSuffix;
        public GameObject hideIfCountSub2;
        public GameObject selectedIndicator;
        public RarityColorIndicator rarityColorIndicator;
        public Slider conditionSlider;
        public Image lockedIndicator;
        public Image bkgItem, bkgNoItem;
        public TextMeshProUGUI weight;
        public TextMeshProUGUI value;
        public InventoryComponentList repairComponents;

        public TextMeshProUGUI healthMod;
        public GameObject hideIfHealthModZero;

        public TextMeshProUGUI damageMod;
        public GameObject hideIfDamageModZero;

        public Slider raritySlider;
        public bool hideIfConditionZero, hideIfConditionOne;
        public bool hideIfRarityZero;

        public ItemTagUI tagPrefab;
        public Transform tagContainer;

        public RecipeUI recipeUI;

        public ItemChanged onLoadedItem;
        public ItemUIClick onClick, onZeroCount, onPointerEnter, onPointerExit;

        public Transform slotContainer;
        public AttachmentSlotUI slotPrefab;

        public float valueModifier = 1;

        // Drag and Drop
        public bool enableDragDrop;
        private bool dragStarted;
        private GameObject moveGO;
        private bool loadedTags;

        #endregion

        #region Properties

        public BlindItemList BlindListParent { get; set; }

        public InventoryCog Inventory { get; set; }

        public InventoryContainer Container { get; set; }

        public InventoryItem Item { get; set; }

        public InventoryItemList ItemListParent { get; set; }

        #endregion

        #region Unity Methods

        public void OnDrag(PointerEventData eventData)
        {
            if (Item == null || !enableDragDrop || (ItemListParent != null && !ItemListParent.enableDragDrop) || (BlindListParent != null && !BlindListParent.enableDragDrop)) return;

            if (!dragStarted)
            {
                // Find canvas
                Canvas c = GetComponentInParent<Canvas>();

                moveGO = new GameObject("InventoryCog_DragItem");
                Image img = moveGO.AddComponent<Image>();
                img.sprite = Item.icon;
                img.raycastTarget = false;
                SlotItemUI slotItemUI = moveGO.AddComponent<SlotItemUI>();
                slotItemUI.Item = Item;
                slotItemUI.Inventory = Inventory;
                slotItemUI.Container = Container;
                moveGO.transform.SetParent(transform.parent);
                moveGO.transform.position = transform.localPosition;

                RectTransform rt = GetComponent<RectTransform>();
                if (rt.sizeDelta.x == 0)
                {
                    moveGO.GetComponent<RectTransform>().sizeDelta = new Vector2(rt.rect.width, rt.rect.height);
                }
                else
                {
                    moveGO.GetComponent<RectTransform>().sizeDelta = rt.sizeDelta;
                }
                moveGO.transform.SetParent(c.transform);

                dragStarted = true;
            }

            moveGO.transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            dragStarted = false;
            Destroy(moveGO);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (enableDragDrop)
            {
                CompleteDrop(eventData);
            }
        }

        private void Update()
        {
            if (Item == null) return;
            UpdateUI();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onPointerEnter?.Invoke(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onPointerExit?.Invoke(this);
        }

        #endregion

        #region Public Methods

        public virtual void Click()
        {
            if (Item != null)
            {
                onClick?.Invoke(this);
            }
        }

        public virtual void Equip()
        {
            if (Inventory != null)
            {
                Inventory.EquipItem(Item);
            }
        }

        public virtual void Unequip()
        {
            if (Inventory != null)
            {
                Inventory.UnequipItem(Item);
            }
        }

        public virtual void LoadItem(InventoryCog inventory, InventoryItem inventoryItem)
        {
            LoadItem(inventory, null, inventoryItem);
        }

        public virtual void LoadItem(InventoryCog inventory, InventoryContainer container, InventoryItem inventoryItem)
        {
            Inventory = inventory;
            Container = container;
            Item = inventoryItem;

            if (lockedIndicator)
            {
                lockedIndicator.gameObject.SetActive(false);
            }

            if (Item == null)
            {
                if (itemImage != null) itemImage.enabled = false;
                if (count != null) count.text = string.Empty;
                if (equippedIndicator != null) equippedIndicator.SetActive(false);
                if (equipableIndicator != null) equipableIndicator.SetActive(false);
                if (displayName != null) displayName.text = string.Empty;
                if (description != null) description.text = string.Empty;
                if (subtext != null) subtext.text = string.Empty;
                if (hideIfNoSubtext != null) hideIfNoSubtext.SetActive(false);
                if (hideIfDamageModZero != null) hideIfDamageModZero.SetActive(false);
                if (hideIfHealthModZero != null) hideIfHealthModZero.SetActive(false);
                if (rarityColorIndicator != null) rarityColorIndicator.gameObject.SetActive(false);
                if (conditionSlider != null)
                {
                    conditionSlider.value = conditionSlider.minValue;
                    if (hideIfConditionZero || hideIfConditionOne) conditionSlider.gameObject.SetActive(false);
                }
                if (raritySlider != null) raritySlider.value = raritySlider.minValue;
                if (hideIfCountSub2 != null) hideIfCountSub2.SetActive(false);
                if (hideIfRarityZero && raritySlider != null) raritySlider.gameObject.SetActive(false);
                if (recipeUI != null) recipeUI.gameObject.SetActive(false);
                if (bkgItem != null) bkgItem.gameObject.SetActive(bkgNoItem == null);
                if (bkgNoItem != null) bkgNoItem.gameObject.SetActive(true);
                if (weight != null) weight.text = string.Empty;
                if (value != null) value.text = string.Empty;
                if (repairComponents != null) repairComponents.ClearComponents();
                if (slotContainer != null) slotContainer.gameObject.SetActive(false);
                return;
            }

            if (slotContainer != null && slotPrefab != null)
            {
                foreach (AttachmentSlot slot in Item.attachSlots)
                {
                    AttachmentSlotUI slotItem = Instantiate(slotPrefab, slotContainer);
                    slotItem.LoadSlot(slot);
                }
            }
            UpdateUI();

            onLoadedItem?.Invoke(Item);
        }

        public virtual void LoadItemByReference(InventoryCog inventory, ItemReference reference)
        {
            if (reference == null || reference.item == null)
            {
                LoadItem(inventory, Container, null);
                return;
            }

            InventoryItem item = InventoryDB.GetItemByName(reference.item.name);
            item.CurrentCount = reference.count;
            LoadItem(inventory, Container, item);
        }

        public void LoadLockedSlot(InventoryCog inventory)
        {
            Inventory = inventory;
            Item = null;

            if (lockedIndicator)
            {
                lockedIndicator.gameObject.SetActive(true);
            }

            if (itemImage != null) itemImage.enabled = false;
            if (count != null) count.text = string.Empty;
            if (equippedIndicator != null) equippedIndicator.SetActive(false);
            if (displayName != null) displayName.text = string.Empty;
            if (subtext != null) subtext.text = string.Empty;
            if (hideIfNoSubtext != null) hideIfNoSubtext.SetActive(false);
            if (hideIfDamageModZero != null) hideIfDamageModZero.SetActive(false);
            if (hideIfHealthModZero != null) hideIfHealthModZero.SetActive(false);
            if (rarityColorIndicator != null) rarityColorIndicator.LoadItem(null);
            if (conditionSlider != null) conditionSlider.value = conditionSlider.minValue;
            if (raritySlider != null) raritySlider.value = raritySlider.minValue;
            if (hideIfCountSub2 != null) hideIfCountSub2.SetActive(false);
            if (hideIfConditionZero && conditionSlider != null) conditionSlider.gameObject.SetActive(false);
            if (hideIfRarityZero && raritySlider != null) raritySlider.gameObject.SetActive(false);
            if (recipeUI != null) recipeUI.gameObject.SetActive(false);
        }

        public void SetSelected(bool selected)
        {
            if (selectedIndicator != null) selectedIndicator.SetActive(selected);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Click();
        }

        #endregion

        #region Private Methods

        internal virtual void CompleteDrop(PointerEventData eventData)
        {
            SlotItemUI draggableItem = eventData.pointerDrag.gameObject.GetComponentInChildren<SlotItemUI>();
            if (draggableItem != null)
            {
                if (draggableItem.Item == null) return;

                InventoryItem item = draggableItem.Item;
                draggableItem.LoadItem(Inventory, Container, Item);
                LoadItem(Inventory, Container, item);
            }
            else if (ItemListParent != null)
            {
                ItemListParent.OnDrop(eventData);
            }
            else if (BlindListParent != null)
            {
                BlindListParent.OnDrop(eventData);
            }
        }

        private void UpdateUI()
        {
            if (bkgItem != null) bkgItem.gameObject.SetActive(Item != null);
            if (bkgNoItem != null) bkgNoItem.gameObject.SetActive(Item == null);

            if (itemImage != null)
            {
                itemImage.sprite = Item.icon;
                itemImage.enabled = itemImage.sprite != null;
            }

            if (count != null)
            {
                count.text = countPrefix + Item.CurrentCount + countSuffix;
            }
            if (hideIfCountSub2 != null) hideIfCountSub2.SetActive(Item.CurrentCount > 1);
        
            if (conditionSlider != null)
            {
                conditionSlider.minValue = 0;
                conditionSlider.maxValue = 1;
                conditionSlider.value = Item.condition;
                if (hideIfConditionZero)
                {
                    conditionSlider.gameObject.SetActive(Item.condition > 0);
                }
                if (hideIfConditionOne)
                {
                    conditionSlider.gameObject.SetActive(Item.condition != 1);
                }
            }

            if (raritySlider != null)
            {
                raritySlider.minValue = 0;
                raritySlider.maxValue = 10;
                raritySlider.value = Item.rarity;
                if (hideIfRarityZero)
                {
                    raritySlider.gameObject.SetActive(Item.rarity > 0);
                }
            }

            if (Item.itemType == ItemType.Ammo)
            {
                if (equippedIndicator != null) equippedIndicator.SetActive(Inventory != null && Inventory.GetSelectedAmmo(Item.ammoType) == Item);
            }
            else if (Item.CanEquip)
            {
                if (equippedIndicator != null) equippedIndicator.SetActive(Item.EquipState != EquipState.NotEquipped);

            }
            else if (Item.itemType == ItemType.Skill)
            {
                if (equippedIndicator != null) equippedIndicator.SetActive(!string.IsNullOrEmpty(Item.AssignedSkillSlot));
            }
            else
            {
                if (equippedIndicator != null) equippedIndicator.SetActive(false);
            }
            if (equipableIndicator != null) equipableIndicator.SetActive(Item.CanEquip && (equippedIndicator == null || !equippedIndicator.activeSelf));

            if (displayName != null)
            {
                displayName.text = Item.DisplayName;
            }

            if (description != null)
            {
                description.text = Item.DisplayName;
            }

            if (subtext != null)
            {
                subtext.text = Item.subtext;
            }
            if (hideIfNoSubtext != null)
            {
                hideIfNoSubtext.SetActive(Item.subtext != null && Item.subtext != string.Empty);
            }

            if (!loadedTags && tagPrefab != null)
            {
                foreach (InventoryItemUITag tag in Item.uiTags)
                {
                    Instantiate(tagPrefab, tagContainer).LoadTag(Inventory, tag);
                }
                loadedTags = true;
            }

            if (rarityColorIndicator != null) rarityColorIndicator.LoadItem(Item);
            if (weight != null) weight.text = Item.weight.ToString();
            if (value != null) value.text = (Item.value * valueModifier).ToString();
            if (repairComponents != null)
            {
                repairComponents.listType = ComponentListType.Repair;
                repairComponents.LoadItem(Inventory, Item);
            }

        }

        #endregion

    }
}
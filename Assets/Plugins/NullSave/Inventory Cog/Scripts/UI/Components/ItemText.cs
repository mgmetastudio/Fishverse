using System.Text;
using TMPro;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ItemText : MonoBehaviour
    {

        #region Variables

        public ItemTextTarget targetProperty;
        public string format = "{0}";
        public bool useColor;

        #endregion

        #region Public Methods

        public virtual void ClearText()
        {
            GetComponent<TextMeshProUGUI>().text = string.Empty;
        }

        public virtual void UpdateText(InventoryCog inventory, InventoryItem item)
        {
            if (item == null) return;

            TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
            ItemTextAppend[] appends = GetComponentsInChildren<ItemTextAppend>();

            if (appends == null || appends.Length == 0)
            {
                text.text = format.Replace("{0}", GetText(inventory, item, targetProperty));
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(format.Replace("{0}", GetText(inventory, item, targetProperty)));

                foreach (ItemTextAppend append in appends)
                {
                    if (MeetsMax(item, append) && MeetsMin(item, append))
                    {
                        sb.Append(append.format.Replace("{0}", GetText(inventory, item, append.targetProperty)));
                    }
                }

                text.text = sb.ToString();
            }
        }

        public virtual void UpdateText(InventoryCog inventory, LootItem item)
        {
            if (item == null) return;

            TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
            ItemTextAppend[] appends = GetComponentsInChildren<ItemTextAppend>();

            if (appends == null || appends.Length == 0)
            {
                text.text = format.Replace("{0}", GetText(inventory, item, targetProperty));
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(format.Replace("{0}", GetText(inventory, item, targetProperty)));

                foreach (ItemTextAppend append in appends)
                {
                    if (MeetsMax(item, append) && MeetsMin(item, append))
                    {
                        sb.Append(append.format.Replace("{0}", GetText(inventory, item, append.targetProperty)));
                    }
                }

                text.text = sb.ToString();
            }
        }

        #endregion

        #region Private Methods

        private string GetText(InventoryCog inventory, InventoryItem item, ItemTextTarget target)
        {
            if (item == null) return string.Empty;

            switch (target)
            {
                case ItemTextTarget.AmmoType:
                    return item.ammoType;
                case ItemTextTarget.BreakdownExpression:
                    return item.breakdownExpression;
                case ItemTextTarget.CategoryDescription:
                    return item.category.description;
                case ItemTextTarget.CategoryName:
                    return item.category.displayName;
                case ItemTextTarget.Condition:
                    return item.condition.ToString();
                case ItemTextTarget.Count:
                    return item.CurrentCount.ToString();
                case ItemTextTarget.CountPerStack:
                    return item.countPerStack.ToString();
                case ItemTextTarget.CustomTagList:
                    StringBuilder sb = new StringBuilder();
                    foreach (StringValue tag in item.customTags)
                    {
                        if (sb.Length > 0) sb.Append(", ");
                        sb.Append(tag.Value);
                    }
                    return sb.ToString();
                case ItemTextTarget.Description:
                    return item.description;
                case ItemTextTarget.DisplayName:
                    return item.DisplayName;
                case ItemTextTarget.EquipExpression:
                    return item.equipExpression;
                case ItemTextTarget.ItemTypeName:
                    switch (item.itemType)
                    {
                        case ItemType.Ammo:
                            return "Ammo";
                        case ItemType.Armor:
                            return "Armor";
                        case ItemType.Attachment:
                            return "Attachment";
                        case ItemType.Component:
                            return "Component";
                        case ItemType.Consumable:
                            return "Consumable";
                        case ItemType.Container:
                            return "Container";
                        case ItemType.Ingredient:
                            return "Ingredient";
                        case ItemType.Journal:
                            return "Journal";
                        case ItemType.QuestItem:
                            return "QuestItem";
                        case ItemType.Shield:
                            return "Shield";
                        case ItemType.Skill:
                            return "Skill";
                        case ItemType.Weapon:
                            return "Weapon";
                        default:
                            return string.Empty;
                    }
                case ItemTextTarget.Rarity:
                    return item.rarity.ToString();
                case ItemTextTarget.RarityName:
                    RarityLevel rarity = inventory.ActiveTheme.rarityLevels[item.rarity];
                    if (useColor)
                    {
                        return "<color=#" + ColorUtility.ToHtmlStringRGB(rarity.color) + ">" + rarity.name + "</color>";
                    }
                    else
                    {
                        return rarity.name;
                    }
                case ItemTextTarget.RepairExpression:
                    return item.repairExpression;
                case ItemTextTarget.RepairIncrement:
                    return item.repairIncrement.ToString();
                case ItemTextTarget.RepairIncrementCost:
                    return item.CostOfIncrementRepairComponents.ToString();
                case ItemTextTarget.SubText:
                    return item.subtext;
                case ItemTextTarget.Value:
                    return item.value.ToString();
                case ItemTextTarget.Weight:
                    return item.weight.ToString();
                default:
                    return string.Empty;
            }
        }

        private string GetText(InventoryCog inventory, LootItem item, ItemTextTarget target)
        {
            if (item == null) return string.Empty;

            switch (target)
            {
                case ItemTextTarget.ActionName:
                    return string.IsNullOrEmpty(item.overrideAction) ? inventory.ActiveTheme.itemText : item.overrideAction;
                case ItemTextTarget.DisplayName:
                    return string.IsNullOrEmpty(item.overrideName) ? item.item.DisplayName : item.overrideName;
                case ItemTextTarget.Count:
                    return item.count.ToString();
                case ItemTextTarget.Rarity:
                    return item.item.rarity.ToString();
                case ItemTextTarget.RarityName:
                    RarityLevel rarity = inventory.ActiveTheme.rarityLevels[item.item.rarity];
                    if (useColor)
                    {
                        return "<color=#" + ColorUtility.ToHtmlStringRGB(rarity.color) + ">" + rarity.name + "</color>";
                    }
                    else
                    {
                        return rarity.name;
                    }
                case ItemTextTarget.Value:
                    return item.currency.ToString();
            }

            return string.Empty;
        }

        private bool MeetsMax(InventoryItem item, ItemTextAppend target)
        {
            if (!target.hasMaxValue) return true;

            switch (target.targetProperty)
            {
                case ItemTextTarget.Condition:
                    return item.condition <= target.maxValue;
                case ItemTextTarget.Count:
                    return item.CurrentCount <= target.maxValue;
                case ItemTextTarget.CountPerStack:
                    return item.countPerStack <= target.maxValue;
                case ItemTextTarget.Rarity:
                    return item.rarity <= target.maxValue;
                case ItemTextTarget.RepairIncrement:
                    return item.repairIncrement <= target.maxValue;
                case ItemTextTarget.RepairIncrementCost:
                    return item.CostOfIncrementRepairComponents <= target.maxValue;
                case ItemTextTarget.Value:
                    return item.value <= target.maxValue;
                case ItemTextTarget.Weight:
                    return item.weight <= target.maxValue;
                default:
                    return true;
            }
        }

        private bool MeetsMax(LootItem item, ItemTextAppend target)
        {
            if (!target.hasMaxValue) return true;

            switch (target.targetProperty)
            {
                case ItemTextTarget.Count:
                    return item.count <= target.maxValue;
                case ItemTextTarget.CountPerStack:
                    return item.item.countPerStack <= target.maxValue;
                case ItemTextTarget.Rarity:
                    return item.item.rarity <= target.maxValue;
                case ItemTextTarget.RepairIncrement:
                    return item.item.repairIncrement <= target.maxValue;
                case ItemTextTarget.RepairIncrementCost:
                    return item.item.CostOfIncrementRepairComponents <= target.maxValue;
                case ItemTextTarget.Value:
                    return item.currency <= target.maxValue;
                case ItemTextTarget.Weight:
                    return item.item.weight <= target.maxValue;
                default:
                    return true;
            }
        }

        private bool MeetsMin(InventoryItem item, ItemTextAppend target)
        {
            if (!target.hasMinValue) return true;

            switch(target.targetProperty)
            {
                case ItemTextTarget.Condition:
                    return item.condition >= target.minValue;
                case ItemTextTarget.Count:
                    return item.CurrentCount >= target.minValue;
                case ItemTextTarget.CountPerStack:
                    return item.countPerStack >= target.minValue;
                case ItemTextTarget.Rarity:
                    return item.rarity >= target.minValue;
                case ItemTextTarget.RepairIncrement:
                    return item.repairIncrement >= target.minValue;
                case ItemTextTarget.RepairIncrementCost:
                    return item.CostOfIncrementRepairComponents >= target.minValue;
                case ItemTextTarget.Value:
                    return item.value >= target.minValue;
                case ItemTextTarget.Weight:
                    return item.weight >= target.minValue;
                default:
                    return true;
            }
        }

        private bool MeetsMin(LootItem item, ItemTextAppend target)
        {
            if (!target.hasMinValue) return true;

            switch (target.targetProperty)
            {
                case ItemTextTarget.Count:
                    return item.count >= target.minValue;
                case ItemTextTarget.CountPerStack:
                    return item.item.countPerStack >= target.minValue;
                case ItemTextTarget.Rarity:
                    return item.item.rarity >= target.minValue;
                case ItemTextTarget.RepairIncrement:
                    return item.item.repairIncrement >= target.minValue;
                case ItemTextTarget.RepairIncrementCost:
                    return item.item.CostOfIncrementRepairComponents >= target.minValue;
                case ItemTextTarget.Value:
                    return item.currency >= target.minValue;
                case ItemTextTarget.Weight:
                    return item.item.weight >= target.minValue;
                default:
                    return true;
            }
        }

        #endregion

    }
}
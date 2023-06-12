using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Inventory
{
    public class PromptUI : MonoBehaviour, IItemHost
    {

        #region Variables

        public TextMeshProUGUI promptText;
        public string textFormat = "Pickup {0}";

        public TextMeshProUGUI additionalText;
        public string additionalFormat = "{0}";

        public UnityEvent onConfirmPrompt;

        #endregion

        #region Properties

        public InventoryCog Inventory { get; set; }

        public InventoryItem InventoryItem { get; set; }

        public LootItem LootItem { get; set; }

        public GameObject TargetObject { get; set; }

        #endregion

        #region Public Methods

        public void ConfirmPrompt()
        {
            onConfirmPrompt?.Invoke();
        }

        public void SetPrompt(string itemName)
        {
            if (promptText != null)
            {
                promptText.text = textFormat.Replace("{0}", itemName);
            }

            if (additionalText != null)
            {
                additionalText.text = additionalFormat.Replace("{0}", itemName);
            }
        }

        public void SetPrompt(string itemName, string actionName)
        {
            if (promptText != null)
            {
                promptText.text = textFormat.Replace("{0}", itemName).Replace("{1}", actionName);
            }

            if (additionalText != null)
            {
                additionalText.text = additionalFormat.Replace("{0}", itemName).Replace("{1}", actionName);
            }
        }

        public void SetPrompt(LootItem lootItem)
        {
            LootItem = lootItem;

            string itemName = string.IsNullOrEmpty(LootItem.overrideName) ? lootItem.item == null ? "Currency" : lootItem.item.displayName : lootItem.overrideName;
            string actionName = string.IsNullOrEmpty(LootItem.overrideAction) ? Inventory.ActiveTheme.itemText : lootItem.overrideAction;

            if (promptText != null)
            {
                promptText.text = textFormat.Replace("{0}", itemName).Replace("{1}", actionName);
            }

            if (additionalText != null)
            {
                additionalText.text = additionalFormat.Replace("{0}", itemName).Replace("{1}", actionName);
            }

            UpdateChildren();
        }

        public void UpdateChildren()
        {
            ItemHostHelper.UpdateChildren(this, gameObject);
        }

        #endregion

    }
}
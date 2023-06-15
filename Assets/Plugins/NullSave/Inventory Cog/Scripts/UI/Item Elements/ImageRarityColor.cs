using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    [RequireComponent(typeof(Image))]
    public class ImageRarityColor : MonoBehaviour
    {

        #region Properties

        public InventoryCog Inventory { get; set; }

        #endregion

        #region Public Methods

        public void LoadItem(InventoryItem item)
        {
            Image image = GetComponent<Image>();
            image.enabled = !(item == null);

            if (item != null)
            {
                image.color = Inventory.ActiveTheme.rarityLevels[item.rarity].color;
            }
        }

        public void SetRarity(int rarity)
        {
            Image image = GetComponent<Image>();
            image.enabled = rarity > 0;
            image.color = Inventory.ActiveTheme.rarityLevels[rarity].color;
        }

        #endregion

    }
}
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    [RequireComponent(typeof(Image))]
    public class RarityColorIndicator : MonoBehaviour
    {

        #region Variables

        public Color[] rarityColors = new Color[] { Color.white, Color.black, Color.blue, Color.green, Color.yellow,
                Color.red, Color.magenta, Color.magenta, Color.magenta, Color.magenta, Color.magenta };

        private Image image;

        #endregion

        #region Properties

        public InventoryTheme ActiveTheme { get; set; }

        #endregion

        #region Public Methods

        public void LoadItem(InventoryItem item)
        {
            image = GetComponent<Image>();
            image.enabled = !(item == null);

            if (item != null)
            {
                if (ActiveTheme != null)
                {
                    image.color = ActiveTheme.rarityLevels[item.rarity].color;
                }
                else
                {
                    image.color = rarityColors[item.rarity];
                }
            }
        }

        public void SetRarity(int rarity)
        {
            image = GetComponent<Image>();
            image.enabled = rarity >= 0;
            if (ActiveTheme != null)
            {
                image.color = ActiveTheme.rarityLevels[rarity].color;
            }
            else
            {
                image.color = rarityColors[rarity];
            }
        }

        #endregion

    }
}
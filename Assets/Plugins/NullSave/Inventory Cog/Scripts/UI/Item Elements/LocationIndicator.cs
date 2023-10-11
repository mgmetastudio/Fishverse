using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    [RequireComponent(typeof(Image))]
    public class LocationIndicator : MonoBehaviour
    {

        #region Variables

        public Color[] rarityColors ;
        private Image image;

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        public void LoadItem(InventoryItem item)
        {
            image = GetComponent<Image>();
            image.enabled = !(item == null);

            if (item != null && item.customTags != null && item.customTags.Count > 0)
            {
                foreach (var tag in item.customTags)
                {
                    string tagValue = tag.Value;

                    if (tagValue == "Yellow")
                    {
                        image.color = rarityColors[0];
                        break;
                    }
                    else if (tagValue == "Blue")
                    {
                        image.color = rarityColors[1];
                        break;
                    }
                    else if (tagValue == "Orange")
                    {
                        image.color = rarityColors[2];
                        break;
                    }
                    else if (tagValue == "Purple")
                    {
                        image.color = rarityColors[3];
                        break;
                    }
                }

            }

        }

        #endregion

    }
}
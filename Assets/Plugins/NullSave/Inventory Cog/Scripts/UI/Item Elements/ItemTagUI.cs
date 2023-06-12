using NullSave.TOCK.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{

    public class ItemTagUI : MonoBehaviour
    {

        #region Variables

        public Image icon;
        public TextMeshProUGUI displayText;
        public bool applyTextColor = true;
        public bool applyImageColor = true;

        public bool autoSizeToText = true;
        public Padding textPadding;

        #endregion

        #region Properties

        public StatsCog StatsCog { get; set; }

        #endregion

        #region Unity Methods

        private void Start()
        {
            if (displayText != null && autoSizeToText)
            {
                AutoSize();
            }
        }

        #endregion

        #region Public Methods

        public void LoadTag(InventoryCog inventory, InventoryItemUITag tag)
        {
            StatsCog = inventory.gameObject.GetComponent<StatsCog>();

            if (icon != null)
            {
                icon.sprite = tag.icon;
                icon.enabled = tag.icon != null;
                if (applyImageColor)
                {
                    icon.color = tag.iconColor;
                }
            }

            if (displayText != null)
            {
                if (StatsCog != null && tag.tagText.Contains("{{"))
                {
                    string display = tag.tagText;
                    int startFrom = 0;
                    int endAt;
                    while(display.IndexOf("{{", startFrom) >= 0)
                    {
                        endAt = display.IndexOf("}}", startFrom);
                        if(endAt < 0)
                        {
                            startFrom += 2;
                        }
                        else
                        {
                            display = display.Substring(0, display.IndexOf("{{", startFrom)) + StatsCog.GetExpressionValue(display.Substring(startFrom + 3, endAt - startFrom - 3)) + display.Substring(endAt + 2);
                        }
                    }

                    displayText.text = display;
                }
                else
                {
                    displayText.text = tag.tagText;
                }

                if (applyTextColor)
                {
                    displayText.color = tag.textColor;
                }
            }
        }

        #endregion

        #region Private Methods

        private void AutoSize()
        {
            RectTransform rt = GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(1024, 768);
            displayText.ForceMeshUpdate();
            Vector2 newSize = displayText.textBounds.size;

            GetComponent<RectTransform>().sizeDelta = new Vector2(newSize.x + textPadding.left + textPadding.right, 
                newSize.y + textPadding.top + textPadding.bottom);
            displayText.ForceMeshUpdate();
        }

        #endregion

    }
}
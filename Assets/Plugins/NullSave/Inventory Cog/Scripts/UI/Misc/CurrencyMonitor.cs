using System.Collections;
using TMPro;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class CurrencyMonitor : MonoBehaviour
    {

        #region Variables

        public InventoryCog inventoryCog;
        public TextMeshProUGUI currencyText;
        public string shareTag;

        private float lastCurrency;

        #endregion

        #region Unity Methods

        private void Start()
        {
            UpdateUI();

            if(!string.IsNullOrEmpty(shareTag))
            {
                StartCoroutine(Bind());
            }
        }

        private void Update()
        {
            if (inventoryCog == null) return;
            if(inventoryCog.currency != lastCurrency)
            {
                UpdateUI();
            }
        }

        #endregion

        #region Private Methods

        private IEnumerator Bind()
        {
            while (inventoryCog == null || inventoryCog.shareTag != shareTag)
            {
                foreach (InventoryCog ic in FindObjectsOfType<InventoryCog>())
                {
                    if (ic.shareTag == shareTag)
                    {
                        inventoryCog = ic;
                        UpdateUI();
                        break;
                    }
                }

                yield return new WaitForSeconds(0.1f);
            }
        }

        private void UpdateUI()
        {
            if (inventoryCog == null) return;
            lastCurrency = inventoryCog.currency;
            currencyText.text = lastCurrency.ToString();
        }

        #endregion

    }
}
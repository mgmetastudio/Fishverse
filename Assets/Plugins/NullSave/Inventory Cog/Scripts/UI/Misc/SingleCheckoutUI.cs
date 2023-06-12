﻿using TMPro;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class SingleCheckoutUI : MonoBehaviour
    {

        #region Enumerations

        public enum CheckoutType
        {
            Auto = 0,
            Buy = 1,
            Sell = 2,
        }

        #endregion

        #region Variables

        public InventoryCog playerInventory;
        public TextMeshProUGUI startingCurrency, lineAmount, result;
        public string unsellableText = "N/A";

        public float valueModifier = 1;

        #endregion

        #region Public Methods

        public void LoadUI(InventoryItemList source)
        {
            if (playerInventory == null) return;

            switch (source.listSource)
            {
                case ListSource.InventoryCog:
                    startingCurrency.text = playerInventory.currency.ToString();
                    if(source.SelectedItem == null || source.SelectedItem.Item == null)
                    {
                        startingCurrency.text = " ";
                        lineAmount.text = " ";
                        result.text = " ";
                    }
                    else if (source.SelectedItem.Item.canSell)
                    {
                        lineAmount.text = "+" + (source.SelectedItem.Item.value * valueModifier);
                        result.text = (playerInventory.currency + (source.SelectedItem.Item.value * valueModifier)).ToString();
                    }
                    else
                    {
                        lineAmount.text = "+" + unsellableText;
                        result.text = playerInventory.currency.ToString();
                    }
                    break;
                case ListSource.InventoryMerchant:
                    startingCurrency.text = playerInventory.currency.ToString();
                    lineAmount.text = "-" + (source.SelectedItem.Item.value * valueModifier);
                    result.text = (playerInventory.currency - (source.SelectedItem.Item.value * valueModifier)).ToString();
                    break;
                default:
                    Debug.Log("Checkout UI only supported on Inventory Cog or Inventory Merchant sources.");
                    break;
            }

        }

        #endregion

    }
}
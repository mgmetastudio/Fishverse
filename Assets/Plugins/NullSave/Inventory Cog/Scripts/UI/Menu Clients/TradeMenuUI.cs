using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    public class TradeMenuUI : MonoBehaviour, IItemHost, IMenuHost
    {

        #region Variables

        public TextMeshProUGUI minCount, maxCount, curCount, totalCost, tradeType;
        public string buyText = "Buy";
        public string sellText = "Sell";
        public Slider countSlider;

        private Action<bool, int> callback;
        public Color fundsAvailable = Color.white;
        public Color insufficientFunds = Color.red;

        public UnityEvent onHasFunds, onDoesNotHaveFunds, onHideCount, onCannotSell;

        private Action<bool, int> closeCallback;
        private float valueMultiplier = 1;

        #endregion

        #region Properties

        public InventoryCog Inventory { get; set; }

        public InventoryItem InventoryItem { get; set; }

        public LootItem LootItem { get; set; }

        public InventoryMerchant Merchant { get; set; }

        public TradeMode TradeMode { get; set; }

        #endregion

        #region Unity Methods

        private void Start()
        {
            Inventory.onThemeWindowOpened?.Invoke(gameObject);
        }

        #endregion

        #region Public Methods

        public void CancelTrade()
        {
            Inventory.onThemeWindowClosed?.Invoke(gameObject);
            closeCallback?.Invoke(false, 0);
        }

        public void ConfirmTrade()
        {
            Inventory.onThemeWindowClosed?.Invoke(gameObject);
            if (countSlider != null && countSlider.gameObject.activeSelf)
            {
                closeCallback?.Invoke(true, (int)countSlider.value);
            }
            else
            {
                closeCallback?.Invoke(true, 1);
            }
        }

        public void Trade(float valueModifier, Action<bool, int> onCloseCallback)
        {
            closeCallback = onCloseCallback;

            if(TradeMode == TradeMode.Buy)
            {
                BuyMode(valueModifier);
            }
            else
            {
                SellMode(valueModifier);
            }

            UpdateChildren();
        }

        public void UpdateChildren()
        {
            ItemHostHelper.UpdateChildren(this, gameObject);
        }

        #endregion

        #region Private Methods

        private void BuyMode(float valueModifier)
        {
            valueMultiplier = valueModifier;

            if (tradeType != null) tradeType.text = buyText;

            int maxValue = Merchant.AvailableStock(InventoryItem);
            if (countSlider != null)
            {
                if (maxValue >= Inventory.ActiveTheme.minCount)
                {
                    countSlider.minValue = 1;
                    countSlider.maxValue = maxValue;
                    countSlider.value = 1;
                    countSlider.onValueChanged.RemoveListener(UpdateCount);
                    countSlider.onValueChanged.AddListener(UpdateCount);
                }
                else
                {
                    countSlider.gameObject.SetActive(false);
                    onHideCount?.Invoke();
                }
            }
            if (minCount != null) minCount.text = "1";
            if (maxCount != null) maxCount.text = maxValue.ToString();

            UpdateCount(1);
        }

        private void SellMode(float valueModifier)
        {
            valueMultiplier = valueModifier;

            if (tradeType != null) tradeType.text = sellText;

            int maxValue = Inventory.GetItemTotalCount(InventoryItem);
            if (countSlider != null)
            {
                if (maxValue >= Inventory.ActiveTheme.minCount)
                {
                    countSlider.minValue = 1;
                    countSlider.maxValue = maxValue;
                    countSlider.value = 1;
                    countSlider.onValueChanged.RemoveListener(UpdateCount);
                    countSlider.onValueChanged.AddListener(UpdateCount);
                }
                else
                {
                    countSlider.gameObject.SetActive(false);
                    onHideCount?.Invoke();
                }
            }
            if (minCount != null) minCount.text = "1";
            if (maxCount != null) maxCount.text = maxValue.ToString();

            UpdateCount(1);
        }

        public void LoadComponents()
        {
            MenuHostHelper.LoadComponents(this, gameObject);
        }

        private void UpdateCount(float count)
        {
            if (curCount != null)
            {
                curCount.text = ((int)count).ToString();
            }

            float total = (InventoryItem.value * valueMultiplier) * (int)count;
            bool hasFunds = true;
            if (TradeMode == TradeMode.Buy)
            {
                if (Inventory.currency < total) hasFunds = false;
            }
            else
            {
                if (Merchant.currency < total) hasFunds = false;
            }

            if (totalCost != null)
            {
                totalCost.text = total.ToString();
                totalCost.color = hasFunds ? fundsAvailable : insufficientFunds;
            }

            if(hasFunds)
            {
                onHasFunds?.Invoke();
            }
            else
            {
                onDoesNotHaveFunds?.Invoke();
            }

            if(TradeMode ==  TradeMode.Sell && !InventoryItem.canSell)
            {
                onCannotSell?.Invoke();
            }
        }

        #endregion

        #region Menu Save/Load Methods

        public void Load(string filename)
        {
            Inventory.InventoryStateLoad(filename);
        }

        public void Load(System.IO.Stream stream)
        {
            Inventory.InventoryStateLoad(stream);
        }

        public void Save(string filename)
        {
            Inventory.InventoryStateSave(filename);
        }

        public void Save(System.IO.Stream stream)
        {
            Inventory.InventoryStateSave(stream);
        }

        #endregion

    }
}
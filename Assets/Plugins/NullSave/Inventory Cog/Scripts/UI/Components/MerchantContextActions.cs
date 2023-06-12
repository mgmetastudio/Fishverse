using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Inventory
{
    [DefaultExecutionOrder(2)]
    public class MerchantContextActions : MonoBehaviour
    {

        #region Variables

        public MerchantMenuUI merchantMenu;
        public bool startWithNoSel = true;
        public UnityEvent onCanSell, onCannotSell, onCanBuy, onCannotBuy;

        #endregion

        #region Properties

        public InventoryCog Inventory { get; set; }

        #endregion

        #region Unity Methods

        private void Start()
        {
            if(startWithNoSel)
            {
                onCannotBuy?.Invoke();
                onCannotSell?.Invoke();
                if (merchantMenu.playerList != null) merchantMenu.playerList.SelectedIndex = -1;
                if (merchantMenu.merchantList != null) merchantMenu.merchantList.SelectedIndex = -1;
            }
        }

        #endregion

        #region Public Methods

        public void SetBuyItem(InventoryItem item)
        {
            onCannotSell?.Invoke();
            if (item != null && merchantMenu.CanBuyItem(item))
            {
                onCanBuy?.Invoke();
            }
            else
            {
                onCannotBuy?.Invoke();
            }
        }

        public void SetBuyItem(InventoryItemList listSource)
        {
            SetBuyItem(listSource.SelectedItem.Item);
        }

        public void SetSellItem(InventoryItem item)
        {
            onCannotBuy?.Invoke();
            if (item != null && merchantMenu.CanSellItem(item))
            {
                onCanSell?.Invoke();
            }
            else
            {
                onCannotSell?.Invoke();
            }
        }

        public void SetSellItem(InventoryItemList listSource)
        {
            SetSellItem(listSource.SelectedItem.Item);
        }

        #endregion

    }
}
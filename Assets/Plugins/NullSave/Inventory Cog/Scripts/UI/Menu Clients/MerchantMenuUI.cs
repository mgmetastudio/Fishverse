using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Inventory
{
    public class MerchantMenuUI : MonoBehaviour, IMenuHost
    {

        #region Variables

        // Closing
        public NavigationType closeMode = NavigationType.ByButton;
        public string closeButton = "Cancel";
        public KeyCode closeKey = KeyCode.Escape;

        // Load type
        public ListLoadMode loadMode = ListLoadMode.OnEnable;

        public InventoryCog playerInventory;
        public InventoryItemList playerList;

        public InventoryMerchant merchantInventory;
        public InventoryItemList merchantList;

        public TextMeshProUGUI playerCurrency;
        public string playerFormat = "{0}";

        public TextMeshProUGUI merchantName, merchantDesc;
        public TextMeshProUGUI merchantCurrency;
        public string merchantFormat = "{0}";

        // Events
        public UnityEvent onOpen, onClose;

        #endregion

        #region Properties

        public InventoryCog Inventory
        {
            get
            {
                return playerInventory;
            }
            set
            {
                playerInventory = value;
            }
        }

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            if (loadMode == ListLoadMode.OnEnable)
            {
                RefreshPlayerInventory();
                RefreshMerchantInventory();
            }

            LoadComponents();
            onOpen?.Invoke();
            if (merchantInventory != null) merchantInventory.InTransaction = true;
        }

        private void Start()
        {
            if (loadMode == ListLoadMode.OnEnable)
            {
                RefreshPlayerInventory();
                RefreshMerchantInventory();
            }

            onOpen?.Invoke();
            Inventory.onThemeWindowOpened?.Invoke(gameObject);
        }

        private void Update()
        {
            switch (closeMode)
            {
                case NavigationType.ByButton:
                    if (InventoryCog.GetButtonDown(closeButton))
                    {
                        CloseMenu();
                    }
                    break;
                case NavigationType.ByKey:
                    if (InventoryCog.GetKeyDown(closeKey))
                    {
                        CloseMenu();
                    }
                    break;
            }
        }

        #endregion

        #region Public Methods

        public void CloseMenu()
        {
            Inventory.ActiveTheme.CloseMenu(gameObject);
            onClose?.Invoke();
            if (merchantInventory != null) merchantInventory.InTransaction = false;

            playerInventory.ActiveTheme.CloseMenu(gameObject);
        }

        public bool CanBuyItem(InventoryItem item)
        {
            if (item == null) return false;
            float costEach = item.value * merchantInventory.buyModifier;
            int maxCount = Mathf.Min(merchantInventory.AvailableStock(item), Mathf.FloorToInt(playerInventory.currency / costEach));
            return maxCount > 0;
        }

        public bool CanSellItem(InventoryItem item)
        {
            if (item == null || !item.canSell) return false;
            float costEach = item.value * merchantInventory.sellModifier;
            return merchantInventory.currency >= costEach;
        }

        public void BuySelectedItem()
        {
            if (merchantList == null || merchantList.SelectedItem == null) return;

            void callback(bool success)
            {
                if (success)
                {
                    RefreshMerchantInventory();
                    RefreshPlayerInventory();
                }
            }
            merchantInventory.SellToPlayer(merchantList.SelectedItem.Item, playerInventory, callback);
        }

        public void LoadComponents()
        {
            MenuHostHelper.LoadComponents(this, gameObject);
        }

        public void OpenTradeBuy()
        {
            void callback(bool confirm, int count)
            {
                if (confirm)
                {
                    merchantInventory.SellToPlayer(merchantList.SelectedItem.Item, count, playerInventory);
                    RefreshAllInventory();
                }
            }
            playerInventory.ActiveTheme.OpenTradeManager(playerInventory, merchantInventory, merchantList.SelectedItem.Item, TradeMode.Buy, callback);
        }

        public void OpenTradeSell()
        {
            void callback(bool confirm, int count)
            {
                if (confirm)
                {
                    merchantInventory.BuyFromPlayer(playerList.SelectedItem.Item, count, playerInventory);
                    RefreshAllInventory();
                }
            }
            playerInventory.ActiveTheme.OpenTradeManager(playerInventory, merchantInventory, playerList.SelectedItem.Item, TradeMode.Sell, callback);
        }

        public void RefreshAllInventory()
        {
            RefreshMerchantInventory();
            RefreshPlayerInventory();
        }

        public void RefreshMerchantInventory()
        {
            if (merchantList != null)
            {
                if (merchantInventory != null)
                {
                    merchantList.Merchant = merchantInventory;
                    int i = merchantList.SelectedIndex;
                    merchantList.LoadItems();
                    merchantList.SelectedIndex = i;
                    merchantList.valueModifier = merchantInventory.sellModifier;
                }
                else
                {
                    Debug.LogWarning(name + ".MerchantMenuUI no Merchant Inventory supplied");
                }

                if (merchantInventory != null)
                {
                    if (merchantName != null)
                    {
                        merchantName.text = merchantInventory.displayName;
                    }

                    if (merchantDesc != null)
                    {
                        merchantDesc.text = merchantInventory.description;
                    }

                    if (merchantCurrency != null)
                    {
                        merchantCurrency.text = merchantFormat.Replace("{0}", merchantInventory.currency.ToString());
                    }
                }
            }
        }

        public void RefreshPlayerInventory()
        {
            if (playerList != null)
            {
                if (playerInventory != null)
                {
                    playerList.Inventory = playerInventory;
                    int i = playerList.SelectedIndex;
                    playerList.ReloadLast();
                    playerList.SelectedIndex = i;
                    playerList.valueModifier = merchantInventory.buyModifier;
                }
                else
                {
                    Debug.LogWarning(name + ".MerchantMenuUI no InventoryCog supplied");
                }
            }

            if (playerCurrency != null && playerInventory != null)
            {
                playerCurrency.text = playerFormat.Replace("{0}", playerInventory.currency.ToString());
            }

        }

        public void SellSelectedItem()
        {
            if (playerList == null || playerList.SelectedItem == null || !playerList.SelectedItem.Item.canSell) return;

            void callback(bool success)
            {
                if (success)
                {
                    RefreshMerchantInventory();
                    RefreshPlayerInventory();
                }
            }
            merchantInventory.BuyFromPlayer(playerList.SelectedItem.Item, playerInventory, callback);
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
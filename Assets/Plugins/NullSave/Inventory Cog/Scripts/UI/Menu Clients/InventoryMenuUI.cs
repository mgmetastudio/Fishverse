using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Inventory
{
    [HierarchyIcon("tock-menu","#ffffff", false)]
    public class InventoryMenuUI : MonoBehaviour, ICloseable, IMenuHost
    {

        #region Variables

        public bool overrideSpawn = false;
        public string overrideTag;

        public NavigationType openMode;
        public string openButton = "Cancel";
        public KeyCode openKey = KeyCode.Escape;

        public NavigationType closeMode;
        public string closeButton = "Cancel";
        public KeyCode closeKey = KeyCode.Escape;

        public UnityEvent onOpen;

        #endregion

        #region Properties

        public InventoryCog Inventory { get; set; }

        public System.Action onCloseCalled { get; set; }

        #endregion

        #region Unity Methods

        private void Start()
        {
            Inventory.onThemeWindowOpened?.Invoke(gameObject);
            onOpen?.Invoke();
        }

        public void Update()
        {
            if (Inventory == null || Inventory.IsPromptOpen) return;
            if (Inventory.ActiveTheme.ActiveMenus.Count == 0 || Inventory.ActiveTheme.ActiveMenus[0] != gameObject) return;

            if ((closeMode == NavigationType.ByButton && InventoryCog.GetButtonDown(closeButton)) ||
                (closeMode == NavigationType.ByKey && InventoryCog.GetKeyDown(closeKey)))
            {
                Close();
            }
        }

        #endregion

        #region Public Methods

        public void Close()
        {
            Inventory.onThemeWindowClosed?.Invoke(gameObject);
            Inventory.ActiveTheme.CloseMenu();
        }

        public void LoadComponents()
        {
            MenuHostHelper.LoadComponents(this, gameObject);
        }

        public void LoadInventory(InventoryCog inventory)
        {
            Inventory = inventory;
            if (Inventory == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    Inventory = player.GetComponentInChildren<InventoryCog>();
                }
            }

            LoadComponents();
        }

        public void RefreshChildren()
        {
            LoadInventory(Inventory);
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
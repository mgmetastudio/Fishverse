using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Inventory
{
    public class GeneralUI : MonoBehaviour, IMenuHost
    {

        #region Variables

        public UnityEvent onInventoryOpened, onInventoryClosed;

        #endregion

        #region Properties

        public InventoryCog Inventory { get; set; }

        #endregion

        #region Unity Methods

        private void Start()
        {
            Inventory.onMenuOpen.AddListener(() => onInventoryOpened?.Invoke());
            Inventory.onMenuClose.AddListener(() => onInventoryClosed?.Invoke());
        }

        #endregion

        #region Public Methods

        public void OpenInventory()
        {
            Inventory.ActiveTheme.OpenInventory();
        }

        public void LoadComponents()
        {
            MenuHostHelper.LoadComponents(this, gameObject);
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
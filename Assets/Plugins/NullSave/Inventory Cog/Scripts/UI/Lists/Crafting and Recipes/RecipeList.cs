using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Inventory
{
    public class RecipeList : MonoBehaviour
    {

        #region Variables

        // Load type
        public ListLoadMode loadMode = ListLoadMode.OnEnable;

        public InventoryCog inventoryCog;
        public bool hideSelectionWhenLocked = true;

        // Filtering
        public List<string> categories;

        public bool usePaging = false;
        public int startPage = 0;

        // Navigation
        public bool allowAutoWrap = false;
        public bool allowSelectByClick = false;
        public NavigationType navigationMode;
        public bool autoRepeat = true;
        public float repeatDelay = 0.5f;
        public bool lockInput = false;

        public NavigationTypeEx selectionMode;
        public string buttonSubmit = "Submit";
        public KeyCode keySubmit = KeyCode.Return;

        // Events
        public UnityEvent onInputLocked, onInputUnlocked, onBindingUpdated;
        public SelectedIndexChanged onSelectionChanged;
        public RecipeListSubmit onItemSubmit;

        // Editor
        public int z_display_flags = 4095;

        #endregion

        #region Properties

        public InventoryCog Inventory { get { return inventoryCog; } set { inventoryCog = value; onBindingUpdated?.Invoke(); } }

        public bool LockInput
        {
            get { return lockInput; }
            set
            {
                if (lockInput == value) return;
                lockInput = value;
                if (lockInput)
                {
                    onInputLocked?.Invoke();
                }
                else
                {
                    onInputUnlocked?.Invoke();
                }
                LockStateChanged();
            }
        }

        public virtual int SelectedIndex { get { throw new System.NotImplementedException(); } set { throw new System.NotImplementedException(); } }

        public virtual RecipeUI SelectedItem { get { throw new System.NotImplementedException(); } set { throw new System.NotImplementedException(); } }

        #endregion

        #region Public Methods

        public virtual void LoadFromCategoyList(CraftingCategoryList source) { throw new System.NotImplementedException(); }

        public virtual void LoadRecipes() { throw new System.NotImplementedException(); }

        public virtual void LoadRecipes(List<string> categories) { throw new System.NotImplementedException(); }

        public virtual void LoadRecipes(List<CraftingCategory> categories) { throw new System.NotImplementedException(); }

        public virtual void ReloadLast() { throw new System.NotImplementedException(); }

        public void ShowCraftDetailMenu()
        {
            if (Inventory.ActiveTheme == null) return;

            System.Action<bool, int> onClose = new System.Action<bool, int>((bool confirm, int count) =>
            {
                if(confirm)
                {
                    ReloadLast();
                }
            });

            Inventory.ActiveTheme.OpenCraftDetail(SelectedItem.Recipe, onClose);
        }

        public virtual void SelectItem(RecipeUI item)
        {
            SelectedItem = item;
        }

        public virtual void SubmitItem(RecipeUI item)
        {
            onItemSubmit?.Invoke(this, item);
        }

        #endregion

        #region Private Methods

        internal virtual void LockStateChanged() { }

        #endregion

    }
}
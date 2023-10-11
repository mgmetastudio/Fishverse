#if GDTK_Inventory2

using NullSave.GDTK.Inventory;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;


namespace NullSave.GDTK.Stats
{
    public class AddItemPlugin : AddOnPlugin
    {

        #region Fields

        [Tooltip("Mode of addition")] public AddOnMode mode;
        [Tooltip("Number of items to pick")] public int addCount;
        [Tooltip("Group name in item type to pick from")] public string groupName;
        [Tooltip("Options to add or pick from")] public List<string> optionIds;
        [Tooltip("Exclude items already present")] public bool excludeOwned;

        private InventoryDatabase database;
        private Inventory.Inventory inventory;
        private PlayerCharacterStats host;

        #endregion

        #region Constructor

        public AddItemPlugin()
        {
            addCount = 1;
            excludeOwned = true;
        }

        #endregion

        #region Properties

        public override string title { get { return "Add Item(s)"; } }

        public override string titlebarText { get { return title; } }

        public override string description { get { return "Add Item to target."; } }

        public override bool needsUI
        {
            get
            {
                return mode switch
                {
                    AddOnMode.AllInList => false,
                    _ => availableOptions.Count > addCount,
                };
            }
        }

        public override int selectCount { get { return addCount; } }

        #endregion

        #region Public Methods

        public override void Apply(PlayerCharacterStats target, GlobalStats global)
        {
            inventory = target.gameObject.GetComponentInChildren<Inventory.Inventory>();

            Thread thApply = new Thread(InternalApplyWait);
            thApply.Start();
        }

        public override void Initialize(PlayerCharacterStats target)
        {
            host = target;
            availableOptions = GetPickList(target);
            selectedOptions = database.items.Where(x => m_selectedIds.Contains(x.optionInfo.id)).ToList<ISelectableOption>();
        }

        #endregion

        #region Private Methods

        private List<ISelectableOption> GetPickList(PlayerCharacterStats target)
        {
            if(database == null)
            {
                database = ToolRegistry.GetComponent<InventoryDatabase>();
            }

            List<string> usedId = target.attributes.Select(x => x.optionInfo.id).ToList();

            switch (mode)
            {
                case AddOnMode.AllInList:
                    if (excludeOwned)
                    {
                        return database.items.Where(x => optionIds.Contains(x.info.id) && !usedId.Contains(x.info.id)).ToList<ISelectableOption>();
                    }
                    return database.items.Where(x => !optionIds.Contains(x.info.id) && usedId.Contains(x.info.id)).ToList<ISelectableOption>();
                case AddOnMode.PickFromGroup:
                    if (excludeOwned)
                    {
                        return database.items.Where(x => x.optionInfo.groupName == groupName && !usedId.Contains(x.info.id)).ToList<ISelectableOption>();
                    }
                    return database.items.Where(x => x.optionInfo.groupName == groupName).ToList<ISelectableOption>();
                case AddOnMode.RandomFromGroup:
                    if (excludeOwned)
                    {
                        return GetRandomList(database.items.Where(x => x.optionInfo.groupName == groupName && !x.info.hidden && !usedId.Contains(x.info.id)).ToList());
                    }
                    return GetRandomList(database.items.Where(x => x.optionInfo.groupName == groupName && !x.info.hidden).ToList());
                case AddOnMode.PickFromList:
                    if (excludeOwned)
                    {
                        return database.items.Where(x => optionIds.Contains(x.info.id) && !usedId.Contains(x.info.id)).ToList<ISelectableOption>();
                    }
                    return database.items.Where(x => optionIds.Contains(x.info.id)).ToList<ISelectableOption>();
                case AddOnMode.RandomFromList:
                    if (excludeOwned)
                    {
                        return GetRandomList(database.items.Where(x => x.optionInfo.groupName == groupName && !x.info.hidden && !usedId.Contains(x.info.id)).ToList());
                    }
                    return GetRandomList(database.items.Where(x => x.optionInfo.groupName == groupName && !x.info.hidden).ToList());
                default:    //Pick Any
                    if (excludeOwned)
                    {
                        return database.items.Where(x => !x.info.hidden && !usedId.Contains(x.info.id)).ToList<ISelectableOption>();
                    }
                    return database.items.Where(x => !x.info.hidden).ToList<ISelectableOption>();
            }
        }

        private List<ISelectableOption> GetRandomList(List<GDTKItem> source)
        {
            List<ISelectableOption> options = new List<ISelectableOption>();
            int remaining = addCount;
            int index;

            while (remaining > 0 && source.Count > 0)
            {
                index = Random.Range(0, source.Count);
                options.Add(source[index]);
                source.RemoveAt(index);
            }
            return options;
        }

        private void InternalApplyWait()
        {
            while(!inventory.initialized)
            {
                Thread.Sleep(10);
            }

            host.onHeartbeat += InternalApplyFinal;
        }

        private void InternalApplyFinal(float ticks)
        {
            host.onHeartbeat -= InternalApplyFinal;

            if (needsUI)
            {
                selectedOptions = database.items.Where(x => m_selectedIds.Contains(x.optionInfo.id)).ToList<ISelectableOption>();
                foreach (ISelectableOption option in selectedOptions)
                {
                    inventory.AddItem(option.optionInfo.id, 1, null);
                }
            }
            else
            {
                foreach (ISelectableOption option in availableOptions)
                {
                    inventory.AddItem(option.optionInfo.id, 1, null);
                }
            }
        }

        #endregion

    }
}

#endif
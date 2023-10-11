#if GDTK
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    public class PerkPlugin : AddOnPlugin
    {

        #region Fields

        [Tooltip("Mode of addition")] public AddOnMode mode;
        [Tooltip("Number of items to pick")] public int addCount;
        [Tooltip("Group name in item type to pick from")] public string groupName;
        [Tooltip("Options to add or pick from")] public List<string> optionIds;
        [Tooltip("Exclude items already present")] public bool excludeOwned;

        #endregion

        #region Constructor

        public PerkPlugin()
        {
            addCount = 1;
            excludeOwned = true;
        }

        #endregion

        #region Properties

        public override string title { get { return "Add Perk(s)"; } }

        public override string titlebarText { get { return title; } }

        public override string description { get { return "Add Perks to target."; } }

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
            if (needsUI)
            {
                selectedOptions = target.database.perks.Where(x => m_selectedIds.Contains(x.optionInfo.id)).ToList<ISelectableOption>();
                foreach (ISelectableOption option in selectedOptions)
                {
                    target.AddPerk(option.optionInfo.id, this);
                }
            }
            else
            {
                foreach (ISelectableOption option in availableOptions)
                {
                    target.AddPerk(option.optionInfo.id, this);
                }
            }
        }

        public override void Initialize(PlayerCharacterStats target)
        {
            availableOptions = GetPickList(target);
            selectedOptions = target.database.perks.Where(x => m_selectedIds.Contains(x.optionInfo.id)).ToList<ISelectableOption>();
        }

        #endregion

        #region Private Methods

        private List<ISelectableOption> GetPickList(PlayerCharacterStats target)
        {
            List<string> usedId = target.perks.Select(x => x.optionInfo.id).ToList();

            switch (mode)
            {
                case AddOnMode.AllInList:
                    if (excludeOwned)
                    {
                        return target.database.perks.Where(x => x.IsUnlocked(target) && optionIds.Contains(x.info.id) && !usedId.Contains(x.info.id)).ToList<ISelectableOption>();
                    }
                    return target.database.perks.Where(x => x.IsUnlocked(target) && !optionIds.Contains(x.info.id) && usedId.Contains(x.info.id)).ToList<ISelectableOption>();
                case AddOnMode.PickFromGroup:
                    if (excludeOwned)
                    {
                        return target.database.perks.Where(x => x.optionInfo.groupName == groupName && !usedId.Contains(x.info.id)).ToList<ISelectableOption>();
                    }
                    return target.database.perks.Where(x => x.optionInfo.groupName == groupName).ToList<ISelectableOption>();
                case AddOnMode.RandomFromGroup:
                    if (excludeOwned)
                    {
                        return GetRandomList(target.database.perks.Where(x => x.IsUnlocked(target) && x.optionInfo.groupName == groupName && !x.info.hidden && !usedId.Contains(x.info.id)).ToList());
                    }
                    return GetRandomList(target.database.perks.Where(x => x.IsUnlocked(target) && x.optionInfo.groupName == groupName && !x.info.hidden).ToList());
                case AddOnMode.PickFromList:
                    if (excludeOwned)
                    {
                        return target.database.perks.Where(x => optionIds.Contains(x.info.id) && !usedId.Contains(x.info.id)).ToList<ISelectableOption>();
                    }
                    return target.database.perks.Where(x => optionIds.Contains(x.info.id)).ToList<ISelectableOption>();
                case AddOnMode.RandomFromList:
                    if (excludeOwned)
                    {
                        return GetRandomList(target.database.perks.Where(x => x.IsUnlocked(target) && x.optionInfo.groupName == groupName && !x.info.hidden && !usedId.Contains(x.info.id)).ToList());
                    }
                    return GetRandomList(target.database.perks.Where(x => x.IsUnlocked(target) && x.optionInfo.groupName == groupName && !x.info.hidden).ToList());
                default:    //Pick Any
                    if (excludeOwned)
                    {
                        return target.database.perks.Where(x => !x.info.hidden && x.IsUnlocked(target) && !usedId.Contains(x.info.id)).ToList<ISelectableOption>();
                    }
                    return target.database.perks.Where(x => x.IsUnlocked(target) && !x.info.hidden).ToList<ISelectableOption>();
            }
        }

        private List<ISelectableOption> GetRandomList(List<GDTKPerk> source)
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

        #endregion

    }
}
#endif
#if GDTK
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    public class ClassPlugin : AddOnPlugin
    {

        #region Fields

        [Tooltip("Mode of addition")] public AddOnMode mode;
        [Tooltip("Number of items to pick")] public int addCount;
        [Tooltip("Group name in item type to pick from")] public string groupName;
        [Tooltip("Options to add or pick from")] public List<string> optionIds;
        [Tooltip("Exclude items already present")] public bool excludeOwned;

        #endregion

        #region Constructor

        public ClassPlugin()
        {
            addCount = 1;
            excludeOwned = true;
        }

        #endregion

        #region Properties

        public override string title { get { return "Add Class(s)"; } }

        public override string titlebarText { get { return title; } }

        public override string description { get { return "Add classes to target."; } }

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
                selectedOptions = target.database.classes.Where(x => m_selectedIds.Contains(x.optionInfo.id)).ToList<ISelectableOption>();
                foreach (ISelectableOption option in selectedOptions)
                {
                    target.AddClass(option.optionInfo.id, this);
                }
            }
            else
            {
                foreach (ISelectableOption option in availableOptions)
                {
                    target.AddClass(option.optionInfo.id, this);
                }
            }
        }

        public override void Initialize(PlayerCharacterStats target)
        {
            availableOptions = GetPickList(target);
            selectedOptions = target.database.classes.Where(x => m_selectedIds.Contains(x.optionInfo.id)).ToList<ISelectableOption>();
        }

        #endregion

        #region Private Methods

        private List<ISelectableOption> GetPickList(PlayerCharacterStats target)
        {
            List<string> usedId = target.classes.Select(x => x.optionInfo.id).ToList();

            switch (mode)
            {
                case AddOnMode.AllInList:
                    if (excludeOwned)
                    {
                        return target.database.classes.Where(x => x.IsUnlocked(target) && optionIds.Contains(x.info.id) && !usedId.Contains(x.info.id) && x.playable).ToList<ISelectableOption>();
                    }
                    return target.database.classes.Where(x => x.IsUnlocked(target) && !optionIds.Contains(x.info.id) && usedId.Contains(x.info.id) && x.playable).ToList<ISelectableOption>();
                case AddOnMode.PickFromGroup:
                    if (excludeOwned)
                    {
                        return target.database.classes.Where(x => x.optionInfo.groupName == groupName && !usedId.Contains(x.info.id)).ToList<ISelectableOption>();
                    }
                    return target.database.classes.Where(x => x.optionInfo.groupName == groupName).ToList<ISelectableOption>();
                case AddOnMode.RandomFromGroup:
                    if (excludeOwned)
                    {
                        return GetRandomList(target.database.classes.Where(x => x.IsUnlocked(target) && x.optionInfo.groupName == groupName && !x.info.hidden && x.playable && !usedId.Contains(x.info.id)).ToList());
                    }
                    return GetRandomList(target.database.classes.Where(x => x.IsUnlocked(target) && x.optionInfo.groupName == groupName && !x.info.hidden && x.playable).ToList());
                case AddOnMode.PickFromList:
                    if (excludeOwned)
                    {
                        return target.database.classes.Where(x => optionIds.Contains(x.info.id) && !usedId.Contains(x.info.id)).ToList<ISelectableOption>();
                    }
                    return target.database.classes.Where(x => optionIds.Contains(x.info.id)).ToList<ISelectableOption>();
                case AddOnMode.RandomFromList:
                    if (excludeOwned)
                    {
                        return GetRandomList(target.database.classes.Where(x => x.IsUnlocked(target) && x.optionInfo.groupName == groupName && !x.info.hidden && x.playable && !usedId.Contains(x.info.id)).ToList());
                    }
                    return GetRandomList(target.database.classes.Where(x => x.IsUnlocked(target) && x.optionInfo.groupName == groupName && !x.info.hidden && x.playable).ToList());
                default:    //Pick Any
                    if (excludeOwned)
                    {
                        return target.database.classes.Where(x => x.IsUnlocked(target) && !x.info.hidden && x.playable && !usedId.Contains(x.info.id)).ToList<ISelectableOption>();
                    }
                    return target.database.classes.Where(x => x.IsUnlocked(target) && !x.info.hidden && x.playable).ToList<ISelectableOption>();
            }
        }

        private List<ISelectableOption> GetRandomList(List<GDTKClass> source)
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
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//namespace NullSave.GDTK.Stats
//{
//    public class LanguagePlugin : AddOnPlugin
//    {

//        #region Fields

//        [Tooltip("Mode of addition")] public AddOnMode mode;
//        [Tooltip("Number of items to pick")] public int addCount;
//        [Tooltip("Group name in item type to pick from")] public string groupName;
//        [Tooltip("Options to add or pick from")] public List<string> optionIds;
//        [Tooltip("Exclude items already present")] public bool excludeOwned;

//        #endregion

//        #region Constructor

//        public LanguagePlugin()
//        {
//            addCount = 1;
//            excludeOwned = true;
//        }

//        #endregion

//        #region Properties

//        public override string title { get { return "Add Language(s)"; } }

//        public override string titlebarText { get { return title; } }

//        public override string description { get { return "Add Languages to target."; } }

//        public override bool needsUI
//        {
//            get
//            {
//                switch (mode)
//                {
//                    case AddOnMode.AllInList:
//                        return false;
//                    default:
//                        return availableOptions.Count > addCount;
//                }
//            }
//        }

//        public override int selectCount { get { return addCount; } }

//        #endregion

//        #region Public Methods

//        public override void Apply(PlayerCharacterStats target, GlobalStats global)
//        {
//            if (needsUI)
//            {
//                selectedOptions = target.database.languages.Where(x => m_selectedIds.Contains(x.optionInfo.id)).ToList<ISelectableOption>();
//                foreach (ISelectableOption option in selectedOptions)
//                {
//                    target.AddLanguage(option.optionInfo.id, this);
//                }
//            }
//            else
//            {
//                foreach (ISelectableOption option in availableOptions)
//                {
//                    target.AddLanguage(option.optionInfo.id, this);
//                }
//            }
//        }

//        public override void Initialize(PlayerCharacterStats target)
//        {
//            availableOptions = GetPickList(target);
//            selectedOptions = target.database.languages.Where(x => m_selectedIds.Contains(x.optionInfo.id)).ToList<ISelectableOption>();
//        }

//        #endregion

//        #region Private Methods

//        private List<ISelectableOption> GetPickList(PlayerCharacterStats target)
//        {
//            List<string> usedId = target.languages.Select(x => x.optionInfo.id).ToList();

//            switch (mode)
//            {
//                case AddOnMode.AllInList:
//                    if (excludeOwned)
//                    {
//                        return target.database.languages.Where(x => optionIds.Contains(x.info.id) && !usedId.Contains(x.info.id)).ToList<ISelectableOption>();
//                    }
//                    return target.database.languages.Where(x => !optionIds.Contains(x.info.id) && usedId.Contains(x.info.id)).ToList<ISelectableOption>();
//                case AddOnMode.PickFromGroup:
//                    if (excludeOwned)
//                    {
//                        return target.database.languages.Where(x => x.optionInfo.groupName == groupName && !usedId.Contains(x.info.id)).ToList<ISelectableOption>();
//                    }
//                    return target.database.languages.Where(x => x.optionInfo.groupName == groupName).ToList<ISelectableOption>();
//                case AddOnMode.RandomFromGroup:
//                    if (excludeOwned)
//                    {
//                        return GetRandomList(target.database.languages.Where(x => x.optionInfo.groupName == groupName && !x.info.hidden && !usedId.Contains(x.info.id)).ToList());
//                    }
//                    return GetRandomList(target.database.languages.Where(x => x.optionInfo.groupName == groupName && !x.info.hidden).ToList());
//                case AddOnMode.PickFromList:
//                    if (excludeOwned)
//                    {
//                        return target.database.languages.Where(x => optionIds.Contains(x.info.id) && !usedId.Contains(x.info.id)).ToList<ISelectableOption>();
//                    }
//                    return target.database.languages.Where(x => optionIds.Contains(x.info.id)).ToList<ISelectableOption>();
//                case AddOnMode.RandomFromList:
//                    if (excludeOwned)
//                    {
//                        return GetRandomList(target.database.languages.Where(x => x.optionInfo.groupName == groupName && !x.info.hidden && !usedId.Contains(x.info.id)).ToList());
//                    }
//                    return GetRandomList(target.database.languages.Where(x => x.optionInfo.groupName == groupName && !x.info.hidden).ToList());
//                default:    //Pick Any
//                    if (excludeOwned)
//                    {
//                        return target.database.languages.Where(x => !x.info.hidden && !usedId.Contains(x.info.id)).ToList<ISelectableOption>();
//                    }
//                    return target.database.languages.Where(x => !x.info.hidden).ToList<ISelectableOption>();
//            }
//        }

//        private List<ISelectableOption> GetRandomList(List<GDTKLanguage> source)
//        {
//            List<ISelectableOption> options = new List<ISelectableOption>();
//            int remaining = addCount;
//            int index;

//            while (remaining > 0 && source.Count > 0)
//            {
//                index = Random.Range(0, source.Count);
//                options.Add(source[index]);
//                source.RemoveAt(index);
//            }
//            return options;
//        }

//        #endregion

//    }
//}
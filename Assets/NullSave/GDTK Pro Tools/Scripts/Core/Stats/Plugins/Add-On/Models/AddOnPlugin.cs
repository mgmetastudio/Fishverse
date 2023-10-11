#if GDTK
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [Serializable]
    public class AddOnPlugin : UniversalPlugin, IUniquelyIdentifiable
    {

        #region Fields

        [HideInInspector] [SerializeField] [JsonSerialize] protected List<string> m_selectedIds;

        [HideInInspector] public IUniquelyIdentifiable source;

        [SerializeField] [JsonSerialize] protected string m_instanceId;

        #endregion

        #region Constructor

        public AddOnPlugin()
        {
            m_selectedIds = new List<string>();
        }

        #endregion

        #region Properties

        public virtual List<ISelectableOption> availableOptions { get; set; }

        public virtual bool hasUnused { get { return selectCount > m_selectedIds.Count; } }

        public string instanceId
        {
            get
            {
                if (string.IsNullOrEmpty(m_instanceId))
                {
                    m_instanceId = Guid.NewGuid().ToString();
                }

                return m_instanceId;
            }
        }

        public virtual bool needsUI { get; }

        public virtual int selectCount { get; }

        public IReadOnlyList<string> selectedIds { get { return m_selectedIds; } }

        public virtual List<ISelectableOption> selectedOptions { get; protected set; }

        #endregion

        #region Public Methods

        public virtual void AddSelection(ISelectableOption option)
        {
            if (m_selectedIds.Contains(option.optionInfo.id)) return;
            m_selectedIds.Add(option.optionInfo.id);
        }

        public virtual void Apply(PlayerCharacterStats target, GlobalStats global) { throw new NotImplementedException(); }

        public virtual void Initialize(PlayerCharacterStats target) { throw new NotImplementedException(); }

        public virtual bool OptionSelected(ISelectableOption option)
        {
            return m_selectedIds.Contains(option.optionInfo.id);
        }

        public virtual void RemoveSelection(ISelectableOption option)
        {
            m_selectedIds.Remove(option.optionInfo.id);
        }

        #endregion

    }
}
#endif
#if GDTK
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [Serializable]
    public class GDTKRace : IUniquelyIdentifiable
    {

        #region Fields

        [Tooltip("Info about this Race")] public BasicInfo info;
        [Tooltip("Parent Race Id, used for sub-Raceing")] public string parentId;
        [Tooltip("Traits to apply with this Race")] public List<GDTKTrait> traits;
        [Tooltip("Sets if Race is playable")] public bool playable;

        private string m_instanceId;

        #endregion

        #region Properties

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

        #endregion

        #region Constructor

        public GDTKRace()
        {
            info = new BasicInfo();
            traits = new List<GDTKTrait>();
            playable = true;
        }

        #endregion

        #region Public Methods

        public GDTKRace Clone()
        {
            GDTKRace result = new GDTKRace();

            result.info = info.Clone();
            result.parentId = parentId;
            result.playable = playable;
            foreach (GDTKTrait trait in traits) result.traits.Add(trait.Clone());

            return result;
        }

        #endregion

        #region Private Methods

        protected internal void FinalizeLoading(PlayerCharacterStats host)
        {
            foreach (GDTKTrait trait in traits)
            {
                trait.FinalizeLoading(host);
            }
        }

        protected internal Data.RaceData GenerateData()
        {
            return new Data.RaceData
            {
                id = info.id,
                instanceId = m_instanceId,
                traits = traits.ToList(),
            };
        }

        protected internal void LoadData(Data.RaceData data)
        {
            m_instanceId = data.instanceId;
            traits = data.traits.ToList();
        }

        #endregion

    }
}
#endif
#if GDTK
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [Serializable]
    public class GDTKBackground : IUniquelyIdentifiable
    {

        #region Fields

        [Tooltip("Info about this object")] public BasicInfo info;
        [Tooltip("Traits added with this background")] public List<GDTKTrait> traits;

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

        public GDTKBackground()
        {
            info = new BasicInfo();
            traits = new List<GDTKTrait>();
        }

        #endregion

        #region Public Methods

        public GDTKBackground Clone()
        {
            GDTKBackground result = new GDTKBackground();

            result.info = info.Clone();
            foreach (GDTKTrait trait in traits) result.traits.Add(trait.Clone());

            return result;
        }

        #endregion

        #region Private Methods

        protected internal void FinalizeLoading(PlayerCharacterStats host)
        {
            foreach(GDTKTrait trait in traits)
            {
                trait.FinalizeLoading(host);
            }
        }

        protected internal Data.BackgroundData GenerateData()
        {
            return new Data.BackgroundData
            {
                id = info.id,
                instanceId = m_instanceId,
                traits = traits.ToList(),
            };
        }

        protected internal void LoadData(Data.BackgroundData data)
        {
            m_instanceId = data.instanceId;
            traits = data.traits.ToList();
        }

        #endregion

    }
}
#endif
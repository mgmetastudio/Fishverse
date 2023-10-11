#if GDTK
using System.Collections.Generic;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [AutoDoc("This component allows you to automatically map stats to fields on another component")]
    public class StatMapper : MonoBehaviour
    {

        #region Fields

        [Tooltip("Source of stats")] public StatSourceReference statSource;
        [Tooltip("Stat Source")] public BasicStats stats;
        [Tooltip("Key used to find source")] public string key;

        public List<GDTKStatMapperTarget> targets;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (statSource == StatSourceReference.FindInRegistry)
            {
                stats = ToolRegistry.GetComponent<BasicStats>(key);
            }

            foreach(GDTKStatMapperTarget target in targets)
            {
                target.Initialize(stats);
            }
        }

        private void Reset()
        {
            targets = new List<GDTKStatMapperTarget>();
            stats = GetComponent<BasicStats>();
        }

        private void Update()
        {
            foreach(GDTKStatMapperTarget target in targets)
            {
                if(target.biDirectional)
                {
                    target.Update();
                }
            }
        }

        #endregion

    }
}
#endif
using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    [CreateAssetMenu(menuName = "TOCK/Stats Cog/Trait", order = 4)]
    public class Trait : ScriptableObject
    {

        #region Variables

        public string category = "Default";
        public string displayName;
        public string description;
        public Sprite sprite;
        public bool displayInList = true;

        public List<StatModifier> modifiers;
        public List<ValuePair> attributes;

        public List<StatExtension> extensions;

        public int z_display_flags = 4095;

        #endregion

    }
}
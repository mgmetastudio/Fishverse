#if GDTK
using UnityEditor;

namespace NullSave.GDTK.Stats
{
    [CustomEditor(typeof(TraitListUI))]
    public class TraitListUIEditor : GDTKEditor
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin();

            SectionHeader("Behavior");
            SimpleProperty("prefab");
            SimpleProperty("content");
            SimpleProperty("loadFromStats");
            if(SimpleValue<bool>("loadFromStats"))
            {
                SimpleProperty("source");
                switch ((StatSourceReference)SimpleValue<int>("source"))
                {
                    case StatSourceReference.DirectReference:
                        SimpleProperty("m_stats", "Reference");
                        break;
                    case StatSourceReference.FindInRegistry:
                        SimpleProperty("key");
                        break;
                }
            }

            SectionHeader("Events");
            SimpleProperty("onListUpdated");

            MainContainerEnd();
        }

        #endregion

    }
}
#endif
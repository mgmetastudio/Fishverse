using UnityEditor;

namespace NullSave.TOCK.Stats
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(EffectsList))]
    public class EffectsListEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBeginSlim();

            SectionHeader("Behaviour");
            SimpleProperty("statsCog");
            SimpleProperty("listItem", "Item Prefab");
            SimpleProperty("target", "Prefab Container");

            SectionHeader("Only Display Categories");
            SimpleList("filterCats");

            MainContainerEnd();
        }

        #endregion

    }
}
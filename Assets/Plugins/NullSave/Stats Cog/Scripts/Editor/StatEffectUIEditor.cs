using UnityEditor;

namespace NullSave.TOCK.Stats
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(StatEffectUI))]
    public class StatEffectUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Stat Effect UI", "Icons/statscog");

            SectionHeader("UI");
            SimpleProperty("icon");
            SimpleProperty("displayName");
            SimpleProperty("description");
            SimpleProperty("category");
            SimpleProperty("remainingTime");

            MainContainerEnd();
        }

        #endregion

    }
}
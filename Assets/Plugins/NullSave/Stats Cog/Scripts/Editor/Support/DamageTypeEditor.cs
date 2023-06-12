using UnityEditor;

namespace NullSave.TOCK
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DamageType))]
    public class DamageTypeEditor : TOCKEditorV2
    {

        #region Unity Events

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Damage Type", "icons/damage");

            DrawInspector();

            MainContainerEnd();
        }

        #endregion

        #region Internal Methods

        internal void DrawInspector()
        {
            SimpleProperty("displayName");
            SimpleProperty("icon");
        }

        #endregion

    }
}
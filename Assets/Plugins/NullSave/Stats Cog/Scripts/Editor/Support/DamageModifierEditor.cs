using UnityEditor;

namespace NullSave.TOCK
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DamageModifier))]
    public class DamageModifierEditor : TOCKEditorV2
    {

        #region Unity Events

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Damage Modifier", "icons/damage");

            DrawInspector();

            MainContainerEnd();
        }

        #endregion

        #region Internal Methods

        internal void DrawInspector()
        {
            SimpleProperty("damageType");
            SimpleProperty("modifierType");
            SimpleProperty("value");
            if ((DamageModType)SimpleInt("modifierType") == DamageModType.Weakness)
            {
                SimpleProperty("maximum");
            }
        }

        #endregion

    }
}
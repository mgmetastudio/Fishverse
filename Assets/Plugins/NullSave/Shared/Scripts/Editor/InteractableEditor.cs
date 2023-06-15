using UnityEditor;

namespace NullSave.TOCK
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(Interactable))]
    public class InteractableEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBeginSlim();

            SectionHeader("Behaviour");

#if LOCALIZATION_COG
            SimpleProperty("localize");
            if(SimpleBool("localize"))
            {
                SimpleProperty("prefix");
                SimpleProperty("entryId");
            }
            else
            {
                SimpleProperty("displayText");
             }
#else
            SimpleProperty("displayText");
#endif

            SimpleProperty("directional");
            if (SimpleBool("directional"))
            {
                SimpleProperty("directionTolerance");

                SectionHeader("Events");
                SimpleProperty("onInteractFront");
                SimpleProperty("onInteractBack");
                SimpleProperty("onInteractLeft");
                SimpleProperty("onInteractRight");
            }
            else
            {
                SectionHeader("Events");
                SimpleProperty("onInteract");
            }

            MainContainerEnd();
        }

        #endregion

    }
}
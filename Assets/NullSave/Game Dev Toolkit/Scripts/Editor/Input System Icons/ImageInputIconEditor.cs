#if ENABLE_INPUT_SYSTEM
using UnityEditor;

namespace NullSave.GDTK
{
    [CustomEditor(typeof(ImageInputIcon))]
    public class ImageInputIconEditor : GDTKEditor
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin();
            SimpleProperty("inputActions");
            SimpleProperty("actionName");
            MainContainerEnd();
        }

        #endregion

    }
}
#endif
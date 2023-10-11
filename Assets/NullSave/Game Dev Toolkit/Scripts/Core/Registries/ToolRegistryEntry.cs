using UnityEngine.SceneManagement;

namespace NullSave.GDTK
{

    internal class ToolRegistryEntry
    {

        #region Fields

        public string Key;
        public object Object;
        public bool Persist;
        public Scene Scene;

        #endregion

    }

}
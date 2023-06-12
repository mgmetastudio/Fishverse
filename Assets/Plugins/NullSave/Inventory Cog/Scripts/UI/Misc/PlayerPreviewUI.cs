using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class PlayerPreviewUI : MonoBehaviour
    {

        #region Variables

        public GameObject cameraSpawn;

        private GameObject activeSpawn;

        #endregion

        #region Unity Methods

        private void OnDestroy()
        {
            if (activeSpawn != null) Destroy(activeSpawn);
        }

        #endregion

        #region Public Methods

        public void LoadPreview(InventoryCog inventory)
        {
            if (activeSpawn != null) Destroy(activeSpawn);
            activeSpawn = Instantiate(cameraSpawn, inventory.gameObject.transform);
        }

        #endregion

    }
}
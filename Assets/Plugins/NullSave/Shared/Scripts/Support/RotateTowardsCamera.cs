using UnityEngine;

namespace NullSave.TOCK
{
    public class RotateTowardsCamera : MonoBehaviour
    {

        #region Unity Methods

        private void Update()
        {
            transform.LookAt(Camera.main.transform);
        }

        #endregion

    }
}
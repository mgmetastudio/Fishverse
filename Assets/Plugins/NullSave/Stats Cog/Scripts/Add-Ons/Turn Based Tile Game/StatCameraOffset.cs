using UnityEngine;

namespace NullSave.TOCK.Stats
{
    public class StatCameraOffset : MonoBehaviour
    {

        #region Variables

        public Transform lookAt;
        public Vector3 offset;

        #endregion

        #region Unity Methods

        private void Update()
        {
            transform.position = lookAt.position + offset;
            transform.LookAt(lookAt);
        }

        #endregion

    }
}
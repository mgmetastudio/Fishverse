using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK
{
    [HierarchyIcon("tock-key-trigger", false)]
    public class TriggerByKey : MonoBehaviour
    {

        #region Variables

        public KeyCode key = KeyCode.KeypadEnter;
        public UnityEvent onKey, onKeyDown, onKeyUp;

        #endregion

        #region Unity Methods

        private void Update()
        {
#if GAME_COG
            if (GameCog.Input != null)
            {
                if (GameCog.Input.GetKey(key))
                {
                    onKey?.Invoke();
                }
                if (GameCog.Input.GetKeyDown(key))
                {
                    onKeyDown?.Invoke();
                }
                if (GameCog.Input.GetKeyUp(key))
                {
                    onKeyUp.Invoke();
                }
                return;
            }
#endif
            if (Input.GetKey(key))
            {
                onKey?.Invoke();
            }
            if (Input.GetKeyDown(key))
            {
                onKeyDown?.Invoke();
            }
            if (Input.GetKeyUp(key))
            {
                onKeyUp.Invoke();
            }
        }

        #endregion

        #region Public Methods

        public void RaiseKey()
        {
            onKey?.Invoke();
        }

        public void RaiseKeyDown()
        {
            onKeyDown?.Invoke();
        }

        public void RaiseKeyUp()
        {
            onKeyUp.Invoke();
        }

        #endregion

    }
}
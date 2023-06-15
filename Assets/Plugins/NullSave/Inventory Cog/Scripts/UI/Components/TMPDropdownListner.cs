using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class TMPDropdownListner : MonoBehaviour
    {

        #region Variables

        public bool fireOnStart = false;
        public bool persistSelection = false;
        public string keyName = "PersistDropdown";

        public List<IndexListener> listeners;

        private TMP_Dropdown dropdown;

        #endregion

        #region Unity Methods

        private void Start()
        {
            dropdown = GetComponent<TMP_Dropdown>();
            dropdown.onValueChanged.AddListener(FireEvent);

            if (persistSelection)
            {
                int setIndex = PlayerPrefs.GetInt(keyName, 0);
                if(dropdown.value == setIndex)
                {
                    FireEvent(dropdown.value);
                }
                else
                {
                    dropdown.value = setIndex;
                }
            }
            else if (fireOnStart) FireEvent(dropdown.value);
        }

        #endregion

        #region Private Methods

        private void FireEvent(int index)
        {
            foreach(IndexListener listener in listeners)
            {
                if(listener.index == index)
                {
                    listener.onIndexSelected?.Invoke();
                }
            }

            if(persistSelection)
            {
                PlayerPrefs.SetInt(keyName, index);
            }
        }

        #endregion

    }
}
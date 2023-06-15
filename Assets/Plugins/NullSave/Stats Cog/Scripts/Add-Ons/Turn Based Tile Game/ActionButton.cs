using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Stats
{
    public class ActionButton : MonoBehaviour
    {

        #region Variables

        public TextMeshProUGUI actionName;
        public ActionButtonClicked onClick;
        public UnityEvent onEnable, onDisable;

        private string action;

        #endregion

        #region Properties

        public string ActionName
        {
            get { return action; }
            set
            {
                action = value;
                if (actionName != null) actionName.text = value;
            }
        }

        public StatAttack Action { get; set; }

        #endregion

        #region Public Methods

        public void Click()
        {
            onClick?.Invoke(this);
        }

        #endregion

    }
}
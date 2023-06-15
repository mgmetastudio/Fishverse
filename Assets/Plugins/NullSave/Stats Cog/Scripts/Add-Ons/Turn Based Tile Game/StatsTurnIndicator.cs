using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Stats
{
    public class StatsTurnIndicator : MonoBehaviour
    {

        #region Variables

        public TextMeshProUGUI playerName;
        public UnityEvent onTurnStart, onTurnEnd;

        #endregion

        #region Properties

        public StatsTurnGameCharacter Character { get; private set; }

        #endregion

        #region Public Methods

        public void LoadCharacter(StatsTurnGameCharacter character)
        {
            Character = character;
            if(playerName != null) playerName.text = Character.displayName;
        }

        #endregion

    }
}
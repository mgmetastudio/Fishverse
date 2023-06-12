using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    public class StatsTurnIndicatorList : MonoBehaviour
    {

        #region Variables

        public StatsTurnIndicator indicatorPrefab;

        private List<StatsTurnIndicator> loaded = new List<StatsTurnIndicator>();

        #endregion

        #region Public Methods

        public void Clear()
        {
            foreach (StatsTurnIndicator indicator in loaded)
            {
                Destroy(indicator.gameObject);
            }
            loaded.Clear();
        }

        public void LoadList(List<StatsTurnGameCharacter> characters, int curTurn)
        {
            for (int i = 0; i < characters.Count; i++)
            {
                StatsTurnIndicator prefab = Instantiate(indicatorPrefab, transform);
                prefab.LoadCharacter(characters[i]);
                if (i == curTurn)
                {
                    prefab.onTurnStart?.Invoke();
                }
                else
                {
                    prefab.onTurnEnd?.Invoke();
                }
                loaded.Add(prefab);
            }
        }

        public void SetTurn(int curTurn)
        {
            for (int i = 0; i < loaded.Count; i++)
            {
                if (i == curTurn)
                {
                    loaded[i].onTurnStart?.Invoke();
                }
                else
                {
                    loaded[i].onTurnEnd?.Invoke();
                }
            }
        }

        #endregion

    }
}
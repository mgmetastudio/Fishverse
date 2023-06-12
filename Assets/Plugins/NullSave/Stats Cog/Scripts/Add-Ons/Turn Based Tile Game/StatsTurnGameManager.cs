using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Stats
{
    public class StatsTurnGameManager : MonoBehaviour
    {

        #region Variables

        public ActionButtonList actionButtonList;
        public StatsTurnIndicatorList turnList;

        public List<string> turnStartCommands;

        public UnityEvent onTurnChanged;

        public List<StatsTurnGameCharacter> players = new List<StatsTurnGameCharacter>();
        public List<StatsTurnGameCharacter> npcs = new List<StatsTurnGameCharacter>();
        public List<StatsTurnGameCharacter> combatOrder = new List<StatsTurnGameCharacter>();
        public bool inCombat;

        public bool refreshRequested;
        public int curTurn;

        #endregion

        #region Properties

        public StatsTurnGameCharacter ActivePlayer
        {
            get
            {
                if (curTurn < 0) return null;
                if(inCombat)
                {
                    if (curTurn >= combatOrder.Count) return null;
                    return combatOrder[curTurn];
                }

                if (curTurn >= players.Count) return null;
                return players[curTurn];
            }
        }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if(actionButtonList != null)
            {
                actionButtonList.Manager = this;
            }
        }

        #endregion

        #region Public Methods

        public void AddCharacter(StatsTurnGameCharacter character)
        {
            if(character.isNPC)
            {
                if (!npcs.Contains(character))
                {
                    npcs.Add(character);
                }
            }
            else
            {
                if (!players.Contains(character))
                {
                    character.CurrentOrder = character.PlayOrder;
                    players.Add(character);
                    players = players.OrderBy(x => x.CurrentOrder).ToList();
                    
                }
            }

            if (!refreshRequested)
            {
                refreshRequested = true;
                StartCoroutine(RefreshData());
            }

            StartTurn();
        }

        public void EndTurn()
        {
            // Update enemy vision
            foreach(StatsTurnGameCharacter npc in npcs)
            {
                //!!
            }

            curTurn += 1;
            if (inCombat)
            {
                if (curTurn >= combatOrder.Count) curTurn = 0;
            }
            else
            {
                if (curTurn >= players.Count) curTurn = 0;
            }

            StartTurn();
        }

        #endregion

        #region Private Methods

        private IEnumerator RefreshData()
        {
            yield return new WaitForEndOfFrame();

            if(turnList != null)
            {
                turnList.LoadList(inCombat ? combatOrder : players, curTurn);
            }

            StartTurn();

            refreshRequested = false;
        }

        private void StartTurn()
        {
            if (ActivePlayer == null) return;

            foreach (string command in turnStartCommands)
            {
                ActivePlayer.StatSource.SendCommand(command);
            }

            onTurnChanged?.Invoke();
        }

        #endregion

    }
}
using System;
using System.Collections.Generic;

namespace LibsEngine.States.UI.Components
{
    [System.Serializable]
    /// <summary>
    /// Дополняет BaseStatesChange<Tstate, Ttypes>, добавляя поведения при смене стэйтов
    /// на выполнения каких-то Actions
    /// </summary>
    public class BaseStatesChangeCustomActions<Tstate, Ttypes> : BaseStatesChange<Tstate, Ttypes>
    {
        /// <summary>
        /// Делегаты-экшены которые будут выполняться при смене типов для них
        /// </summary>
        private List<Action<Ttypes>> _activatedStates = new List<Action<Ttypes>>(); //?переделать на массив
                                                                      //в будуем добавить для быстрого поведения вью через движок с UnityEvents

        /// <summary>
        /// Засэтапить делегаты-действия при сменах типов стэйтов
        /// </summary>
        public void SetupActions(List<Action<Ttypes>> activatedStates)
        {
            _activatedStates = activatedStates;
        }

        public override void SetState(Tstate setState, bool isForcibly = false)
        {
            if(!isForcibly)
            {
                if (lastState.Equals(setState))
                    return;
            }

            base.SetState(setState, isForcibly);
        }

        protected override void StateChangeImplement(Ttypes typeSet)
        {
            int index = GetIndexFromState(typeSet);
            try
            {
                var action = _activatedStates[index];
                action?.Invoke(typeSet);

            }
            catch (Exception)
            {

            }
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace LibsEngine.States
{
    /// <summary>
    /// Репозиторий-декораор для реализации ISetState<Tstate> внутри с коллекцией ISetState<Tstate>
    /// Tstate – некий стэйт, может в основном bool (0,1), int и тд
    /// </summary>
    public class SetStatesRepository<Tstate> : ISetState<Tstate> //TODO: ISetStateable ?
    {
        [SerializeField] private IEnumerable<ISetState<Tstate>> _statesList; //todo: simply serialization

        public void Setup(IEnumerable<ISetState<Tstate>> statesList)
        {
            _statesList = statesList;
        }

        public void SetState(Tstate setState, bool isForcibly = false)
        {
            foreach (var item in _statesList)
            {
                item.SetState(setState, isForcibly);
            }
        }
    }
}
using UnityEngine;

namespace LibEngine.States
{
    public class StateMachineMonobeh<T> : MonoBehaviour, IStateMachine<T>
    {
        private IStateMachine<T> _stateMachine;

        public T State
        {
            get => _stateMachine.State;
            set => _stateMachine.State = value;
        }

        public void SetStateMachine(IStateMachine<T> stateMachine)
        {
            _stateMachine = stateMachine;
        }
    }
}
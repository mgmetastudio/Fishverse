using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace LibEngine.States
{
    [System.Serializable]
    public class BaseStateMachine<T> : IStateMachine<T>
    {
        [SerializeField] protected List<UnityEvent> stateChangeEvents;

        [SerializeField] private T _state;

        public T State
        {
            get => _state;
            set
            {
                _state = value;
                OnStateChange(value);
            }
        }

        protected virtual void OnStateChange(T value)
        {
            int index = GetIndex(value);
            if (index >= 0 && index < stateChangeEvents.Count)
            {
                stateChangeEvents[index]?.Invoke();
            }
        }

        protected virtual int GetIndex(T value)
        {
            return Convert.ToInt32(value);
        }
    }
}
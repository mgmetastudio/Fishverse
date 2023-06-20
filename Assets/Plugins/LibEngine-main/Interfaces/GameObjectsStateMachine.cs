using UnityEngine;
using System.Collections.Generic;
using LibEngine.Extensions;

namespace LibEngine.States
{
    [System.Serializable]
    public class GameObjectsStateMachine<T> : BaseStateMachine<T>
    {
        [SerializeField] protected List<GameObject> gameObjects;
        [SerializeField] protected bool isInverted;

        protected override void OnStateChange(T value)
        {
            base.OnStateChange(value);

            int index = GetIndex(value);
            gameObjects.ForEach((gameObject, i) => gameObject.SetActive((i == index) ^ isInverted));
        }
    }
}
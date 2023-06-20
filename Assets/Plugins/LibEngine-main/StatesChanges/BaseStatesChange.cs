using System;
using UnityEngine;

namespace LibsEngine.States
{
    [System.Serializable]
    /// <summary>
    /// Абстрактная дженерик стэйт машина, хранит в себе некий стэйт, и может реализовывать
    /// смену стэйта на некий другой, и предварительной логикой обработки
    /// </summary>
    public class BaseStatesChange<Tstate, Ttypes> : IStateables<Tstate>, IResetStates<Ttypes>
    {
        [Tooltip("Активна ли смена для интеракция смены стэйта")]
        [SerializeField] private bool isActiveInteractableChange = true;

        [Tooltip("Нужно ли реализовывать поведение параметра isForcibly: денаить установку стэйта, если он уже установлен")]
        [SerializeField] private bool isApplyForciblyParam = false;

        /// <summary>
        /// Коллекция из возможных стэйтов (для примитивов, 0,1 /,2,3.. в зависимости от типов)
        /// Доступаються по стандарту кастом значения типа в индекс, и извлекает по нему элемент
        /// </summary>
        [Tooltip("Возможные стэйты")]
        [SerializeField] private Ttypes[] keysState = new Ttypes[2]; //"2" – стандартное количество стэйтов (0,1 / false, true)

        /// <summary>
        /// Обновить коллекцию из возможных стиэйтов
        /// </summary>
        public void UpdateKeysStates(Ttypes[] keysStates)
        {
            keysState = keysStates;
        }

        /// <summary>
        /// Последний/текущий стэйт
        /// </summary>
        protected Tstate lastState;
        /// <summary>
        /// Последний/текущий установленный стэйти
        /// </summary>
        protected Ttypes lastType;

        /// <summary>
        /// Получить последний/текущий стэйт
        /// </summary>
        public Tstate GetState()
        {
            return lastState;
        }

        /// <summary>
        /// Назначить некий стэйт
        /// Параметр isForcibly отвечает за то что будет установлен стэйт принудительно,
        /// без перепроверки относительно предыдущих состояний
        /// </summary>
        public virtual void SetState(Tstate setState, bool isForcibly = false)
        {
            if (!isActiveInteractableChange)
                return;

            if(isApplyForciblyParam)
            {
                if (!isForcibly)
                {
                    bool isEqualState = lastState.Equals(setState);
                    if (isEqualState)
                        return;
                }
            }

            int index = GetIndexFromState(setState);
            lastType = keysState[index];
            lastState = setState;
            StateChangeImplement(lastType);
        }

        /// <summary>
        /// Реализация назначения смены стэйта. 
        /// В качестве аргумента приходит тип стэйта который устанавливается
        /// </summary>
        protected virtual void StateChangeImplement(Ttypes typeSet)
        {

        }

        /// <summary>
        /// Получить индекс относительно некого устанавливаемого стэйта
        /// </summary>
        protected virtual int GetIndexFromState(Tstate setState)
        {
            // Подходит только для численных типов, которые конвертит Convert.Int
            return Convert.ToInt32(setState); //для оптимизации можно использовать другую стратегию
        }

        /// <summary>
        /// Получить индекс относительно некого устанавливаемого типа стэйта 
        /// </summary>
        protected virtual int GetIndexFromState(Ttypes setState)
        {
            // Подходит только для численных типов, которые конвертит Convert.Int
            return Convert.ToInt32(setState); //для оптимизации можно использовать другую стратегию
        }


        public virtual void ResetAllStates()
        {
            foreach (var item in keysState)
                ResetState(item);

            lastState = default;
            lastType = default;
        }

        /// <summary>
        /// Сбросить стэйт
        /// </summary>
        public virtual void ResetState(Ttypes type)
        {
            //implement
        }


#if UNITY_EDITOR
        [SerializeField] private Tstate test_setState;

        [NaughtyAttributes.Button]
        private void TestSetType()
        {
            SetState(test_setState);
        }
#endif
    }
}
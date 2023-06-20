namespace LibsEngine.States
{
    public interface IStateables<T> : IGetState<T>, ISetState<T>
    {

    }

    public interface IGetState<T>
    {
        /// <summary>
        /// Получить состояние
        /// </summary>
        T GetState();
    }

    /// <summary>
    /// Интерфейс назначения абстрактного состояния
    /// </summary>
    public interface ISetState<T>
    {
        /// <summary>
        /// Назначить состояние
        /// </summary>
        void SetState(T setState, bool isForcibly = false);
    }

    /// <summary>
    /// Интерфейс для сброса стэйтов
    /// </summary>
    public interface IResetStates<T>
    {
        /// <summary>
        /// Сбросить стэйты
        /// </summary>
        void ResetAllStates();

        /// <summary>
        /// Сбросить стэйт
        /// </summary>
        void ResetState(T state);
    }

}
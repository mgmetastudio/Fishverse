namespace LibEngine.States
{
    public interface IStateMachine<T>
    {
        T State { get; set; }
    }
}
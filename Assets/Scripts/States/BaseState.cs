public abstract class BaseState<T>
{
    public string Name { get; protected set; }

    protected T _stateMachine;

    public BaseState(string name, T stateMachine)
    {
        Name = name;
        _stateMachine = stateMachine;
    }

    public virtual void Enter() {}
    public virtual void Update() {}
    public virtual void Exit() {}
}

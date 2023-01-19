using UnityEngine;

public abstract class StateMachine<T> : MonoBehaviour
{
    BaseState<T> _currentState;

    protected virtual void Start()
    {
        _currentState = GetInitialState();
        _currentState?.Enter();
    }

    protected virtual void Update() => _currentState?.Update();

    public void ChangeState(BaseState<T> newState)
    {
        _currentState.Exit();

        _currentState = newState;
        _currentState.Enter();
    }

    protected virtual BaseState<T> GetInitialState() => null;

    protected BaseState<T> GetCurrentState() => _currentState;
}

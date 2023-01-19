using UnityEngine;

public class AgentBehaviourSM : StateMachine<AgentBehaviourSM>
{
    AgentController _agentController;

    public Move MoveState { get; private set; }

    public Attack AttackState { get; private set; }

    public Death DeathState { get; private set; }

    public Resupply ResupplyState { get; private set; }

    void Awake()
    {
        _agentController = GetComponent<AgentController>();

        MoveState = new Move(this);
        AttackState = new Attack(this);
        DeathState = new Death(this);
        ResupplyState = new Resupply(this);
    }

    public AgentController GetAgentController() => _agentController;

    protected override BaseState<AgentBehaviourSM> GetInitialState()
        => MoveState;

#if UNITY_EDITOR
    [SerializeField] private string _currentState;
    void LateUpdate() => _currentState = GetCurrentState()?.Name;
#endif
}

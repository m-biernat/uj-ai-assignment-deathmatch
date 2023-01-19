public class Resupply : BaseState<AgentBehaviourSM>
{
    AgentController _agentController;

    public Resupply(AgentBehaviourSM agentBehaviourSM) : base("Resupply", agentBehaviourSM) 
    { 
        _agentController = agentBehaviourSM.GetAgentController();
    }

    public override void Enter() => _agentController.TargetClosestAmmo();

    public override void Update() => _agentController.FollowPath(OnComplete);

    void OnComplete() => _stateMachine.ChangeState(_stateMachine.MoveState);
}
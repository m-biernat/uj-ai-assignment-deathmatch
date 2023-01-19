using UnityEngine;

public class Death : BaseState<AgentBehaviourSM>
{
    AgentController _agentController;

    float _cooldown = 0.0f;

    public Death(AgentBehaviourSM agentBehaviourSM) : base("Death", agentBehaviourSM) 
    { 
        _agentController = agentBehaviourSM.GetAgentController();
    }

    public override void Enter() => _cooldown = 0.0f;

    public override void Update() 
    {
        _cooldown += Time.deltaTime;

        if (_cooldown >= _agentController.RespawnCooldown)
        {
            _agentController.Respawn();
            _stateMachine.ChangeState(_stateMachine.MoveState);
        }
    }
}

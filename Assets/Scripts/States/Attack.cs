using System.Collections.Generic;
using UnityEngine;

public class Attack : BaseState<AgentBehaviourSM>
{
    AgentController _agentController;

    float _cooldown = 0.0f;

    public Attack(AgentBehaviourSM agentBehaviourSM) : base("Attack", agentBehaviourSM) 
    { 
        _agentController = agentBehaviourSM.GetAgentController();
    }

    public override void Enter()
    {
        if (_agentController.Ammo <= 0)
            _stateMachine.ChangeState(_stateMachine.ResupplyState);

        _cooldown = _agentController.ShootCooldown;
    }

    public override void Update() 
    {
        List<GameObject> agents;
        if (_agentController.Detect(out agents)) {
            if (_cooldown >= _agentController.ShootCooldown) {
                _agentController.Shoot(_agentController.PickRandomEnemy(agents), OutOfAmmo);
                _cooldown = 0.0f;
            }
        }
        else 
            _stateMachine.ChangeState(_stateMachine.MoveState);

        _cooldown += Time.deltaTime;
    }

    void OutOfAmmo() => _stateMachine.ChangeState(_stateMachine.ResupplyState);
}

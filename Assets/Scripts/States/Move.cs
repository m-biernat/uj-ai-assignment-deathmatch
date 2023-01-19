using System.Collections.Generic;
using UnityEngine;

public class Move : BaseState<AgentBehaviourSM>
{
    AgentController _agentController;

    public Move(AgentBehaviourSM agentBehaviourSM) : base("Move", agentBehaviourSM) 
    { 
        _agentController = agentBehaviourSM.GetAgentController();
    }

    public override void Enter() 
    {
        if (_agentController.Health < _agentController.MaxHealth) {
            if (_agentController.TargetNode.Type != NodeType.Heal)
                _agentController.TargetClosestHeal();
        }
        else if (_agentController.Ammo < _agentController.MaxAmmo) {
            if (_agentController.TargetNode.Type != NodeType.Ammo)
                _agentController.TargetClosestAmmo();
        }
        else {
            if (_agentController.TargetNode.Type != NodeType.Node)
                _agentController.TargetRandomNode();
        }
    }

    public override void Update() 
    {
        List<GameObject> agents;
        if (_agentController.Detect(out agents))
            Debug.Log("DUPA");

        _agentController.FollowPath(OnComplete);
    }

    void OnComplete() {
        Debug.Log("DUPA");
        _agentController.TargetRandomNode();
    }

    public override void Exit() {}
}
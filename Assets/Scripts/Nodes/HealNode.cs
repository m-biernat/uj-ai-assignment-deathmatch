using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class HealNode : Node
{
    public static List<Node> HealNodes { get; protected set; }

    protected override void Awake()
    {
        base.Awake();

        Type = NodeType.Heal;

        if (HealNodes is null)
            HealNodes = new List<Node>();

        HealNodes.Add(this);
    }

    protected override void OnDrawGizmosSelected() 
        => base.OnDrawGizmosSelected();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Agent"))
        {
            var agentController = other.GetComponent<AgentController>();

            if (agentController.Health < agentController.MaxHealth)
            {
                agentController.Heal();
                Debug.Log($"{other.name} healed!");
            }
        }
    }

    public static Node GetClosestNode(Vector3 position)
    {
        if (HealNodes is null || HealNodes.Count == 0)
            return null;

        if (HealNodes.Count == 1)
            return HealNodes[0];

        var closest = HealNodes[0];
        var minDist = int.MaxValue;

        foreach (var node in HealNodes)
        {
            var dist = Distance.Manhattan(position, node.Position);

            if (dist < minDist)
            {
                minDist = dist;
                closest = node;
            }
        }

        return closest;
    }
}

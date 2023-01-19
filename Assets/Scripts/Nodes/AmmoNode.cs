using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class AmmoNode : Node
{
    public static List<Node> AmmoNodes { get; protected set; }

    protected override void Awake()
    {
        base.Awake();

        Type = NodeType.Ammo;

        if (AmmoNodes is null)
            AmmoNodes = new List<Node>();

        AmmoNodes.Add(this);
    }

    protected override void OnDrawGizmosSelected() 
        => base.OnDrawGizmosSelected();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Agent"))
        {
            other.GetComponent<AgentController>()?.RefillAmmo();
            Debug.Log($"{other.name} refilled ammo!");
        }
    }

    public static Node GetClosestNode(Vector3 position)
    {
        if (AmmoNodes is null || AmmoNodes.Count == 0)
            return null;

        if (AmmoNodes.Count == 1)
            return AmmoNodes[0];

        var closest = AmmoNodes[0];
        var minDist = int.MaxValue;

        foreach (var node in AmmoNodes)
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

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Agent"))
            Debug.Log($"Heals {other.name}!");
    }
}

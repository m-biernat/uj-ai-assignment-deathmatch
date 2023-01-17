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
}

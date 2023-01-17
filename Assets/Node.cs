using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Vector3 Position { get => transform.position; }

    [field: SerializeField]
    public Node[] Vertices { get; protected set; }

    public Dictionary<Node, int> Weights { get; protected set; }

    public int G { get; set; } = 0;
    
    public int H { get; protected set; } = 0;
    
    public int F { get; set; } = int.MaxValue;
    
    public Node P { get; protected set; } = null;

    public NodeType Type { get; protected set; } = NodeType.Node;

    public static List<Node> Nodes { get; protected set; }

    [field: SerializeField]
    public bool Waypoint { get; protected set; } = true;

    protected virtual void Awake()
    {
        if (Nodes is null)
            Nodes = new List<Node>();

        if (Waypoint)
            Nodes.Add(this);

        CalculateWeights();
    }

    protected void CalculateWeights()
    {
        if (Vertices is null)
            return;

        Weights = new Dictionary<Node, int>();

        foreach (var vert in Vertices)
        {
            if (vert is null)
                continue;

            var weight = Distance.Euclidean(Position, vert.Position);

            Weights.Add(vert, weight);
        }
    }

    public int CalculateG(Node previous) 
        => previous.G + Weights[previous];

    public void CalculateH(Node end)
        => H = Distance.Manhattan(Position, end.Position);

    public void SetPrevious(Node vertex) => P = vertex; 

    protected virtual void OnDrawGizmosSelected()
    {
        if (Vertices is null)
            return;

        Gizmos.color = Color.cyan;

        foreach (var vert in Vertices)
        {
            if (vert is null)
                continue;
            
            Gizmos.DrawLine(Position, vert.Position);
        }
    }
}

public enum NodeType 
{
    Node,
    Spawn,
    Heal,
    Ammo
}

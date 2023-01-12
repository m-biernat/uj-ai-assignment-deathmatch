using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Vector3 Position { get => transform.position; }

    [field: SerializeField]
    public Node[] Vertices { get; private set; }

    public Dictionary<Node, int> Weights { get; private set; }

    public int G { get; set; } = 0;
    
    public int H { get; private set; } = 0;
    
    public int F { get; set; } = int.MaxValue;
    
    public Node P { get; private set; } = null;

    void Awake() 
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

    void OnDrawGizmosSelected()
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

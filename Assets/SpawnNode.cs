using System.Collections.Generic;

public class SpawnNode : Node
{
    public static List<Node> SpawnNodes { get; protected set; }

    private static int _currentSpawnNode = 0; 

    protected override void Awake()
    {
        base.Awake();

        Type = NodeType.Spawn;

        if (SpawnNodes is null)
            SpawnNodes = new List<Node>();

        SpawnNodes.Add(this);
    }

    protected override void OnDrawGizmosSelected() 
        => base.OnDrawGizmosSelected();

    public Node GetSpawnNode()
    {
        if (SpawnNodes is null || SpawnNodes.Count == 0)
            return null;

        var spawnNode = SpawnNodes[_currentSpawnNode];

        _currentSpawnNode = (_currentSpawnNode + 1) % SpawnNodes.Count;

        return spawnNode;
    }
}

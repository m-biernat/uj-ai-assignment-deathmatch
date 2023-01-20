using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    public static PathFinder Instance { get; private set; }

    UniqueQueue<AgentController> _requests;

    bool _active = false;

    void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
            _requests = new UniqueQueue<AgentController>();
        }
    }

    public void EnqueueRequest(AgentController agentController)
    {
        _requests.Enqueue(agentController);

        if (!_active)
            StartCoroutine(ProcessRequests());
    }

    IEnumerator ProcessRequests()
    {
        _active = true;

        while (_requests.Count > 0)
        {
            var agentController = _requests.Dequeue();
        
            agentController.SetPath(Find(agentController.CurrentNode, agentController.TargetNode));
            yield return null;
        }

        _active = false;
    }

    public static List<Node> Find(Node start, Node end)
    {
        var open = new List<Node>();
        var closed = new List<Node>();

        var curr = start;

        curr.SetPrevious(start);
        curr.G = 0;
        curr.CalculateH(end);
        curr.F = curr.G + curr.H;

        while (curr != end)
        {
            foreach (var vert in curr.Vertices)
            {
                if (closed.Contains(vert))
                    continue;

                if (!open.Contains(vert))
                    open.Add(vert);

                var g = vert.CalculateG(curr);
                vert.CalculateH(end);
                var f = g + vert.H;

                if (f < vert.F)
                {
                    vert.G = g;
                    vert.F = f;
                    vert.SetPrevious(curr);
                }
            }

            closed.Add(curr);
            open.Sort((cmpA, cmpB) => cmpA.F.CompareTo(cmpB.F));
            curr = open[0];
            open.RemoveAt(0);
        }

        open.ForEach((n) => n.F = int.MaxValue);
        closed.ForEach((n) => n.F = int.MaxValue);
        end.F = int.MaxValue;

        var path = new List<Node>();

        while (curr != start)
        {
            path.Add(curr);
            curr = curr.P;
        }

        //path.ForEach((n) => Debug.Log(n));

        return path;
    }
}

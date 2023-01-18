using System.Collections.Generic;
using UnityEngine;

public static class PathFinder
{
    static Queue<Request> _requests;

    

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
                if (closed.Contains(vert) && open.Contains(vert))
                    continue;

                open.Add(vert);

                vert.CalculateH(end);
                var g = vert.CalculateG(curr);
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

        path.ForEach((n) => Debug.Log(n));

        return path;
    }

    public struct Request
    {
        public AgentController owner;

        public Node goal;

        public Request(AgentController owner, Node goal)
        {
            this.owner = owner;
            this.goal = goal;
        }
    }
}

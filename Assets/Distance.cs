using UnityEngine;

public static class Distance
{
    public static int Euclidean(Vector3 from, Vector3 to) 
        => Mathf.RoundToInt(Vector3.Distance(from, to));

    public static int Manhattan(Vector3 from, Vector3 to) 
    {
        var x = Mathf.Abs(from.x - to.x);
        var y = Mathf.Abs(from.y - to.y);

        return Mathf.RoundToInt(x + y);
    }
}

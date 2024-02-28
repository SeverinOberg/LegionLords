using System;
using UnityEngine;

public static class ExtensionMethods
{
    public static float DistanceXZ(this Vector3 a, Vector3 b)
    {
        float num = a.x - b.x;
        float num3 = a.z - b.z;
        return (float)Math.Sqrt(num * num + num3 * num3);
    }

    public static float Distance(this Vector3 a, Vector3 b)
    {
        float num = a.x - b.x;
        float num2 = a.y - b.y;
        float num3 = a.z - b.z;
        return (float)Math.Sqrt(num * num + num2 * num2 + num3 * num3);
    }

    public static Vector3 MakeVector3(this byte value)
    {
        return new Vector3(value, value, value);
    }

}

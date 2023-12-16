using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Math
{
    public static Vector3 multiplyVector3(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    public static Vector3 offsetByY(Vector3 v3, float yOffet)
    {
        return new Vector3(v3.x, v3.y + yOffet, v3.z);
    }
}

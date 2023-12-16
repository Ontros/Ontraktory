using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Math
{
    public static Vector3 multiplyVector3(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }
}

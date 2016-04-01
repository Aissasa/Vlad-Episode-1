using UnityEngine;
using System.Collections;
using System;

public static class ExtentionMethods {

    public static Vector2 Get2DPosition(this Transform trans)
    {
        return new Vector2(trans.position.x, trans.position.y);
    }

    public static bool IsAbove(this Vector2 vect2, Vector2 other)
    {
        return vect2.y > other.y;
    }
}

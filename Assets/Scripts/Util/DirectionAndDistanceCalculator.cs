using UnityEngine;
using System.Collections;
using System;

public static class DirectionAndDistanceCalculator
{

    public static Vector2 CalculateDirection(Vector2 startPos, Vector2 targetPos)
    {
        return new Vector2(targetPos.x - startPos.x, targetPos.y - startPos.y);
    }

    public static float CalculateDistance(Vector2 startPos, Vector2 targetPos)
    {
        float dx = targetPos.x - startPos.x;
        float dy = targetPos.y - startPos.y;
        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    public static Vector2 CalculateSignedDirection(Vector2 startPos, Vector2 targetPos)
    {
        return new Vector2(Math.Sign(targetPos.x - startPos.x), Math.Sign(targetPos.y - startPos.y));
    }

    public static float CalculateAngle(Vector2 startPos, Vector2 targetPos)
    {
        return Mathf.Atan2(targetPos.y - startPos.y, targetPos.x - startPos.x);
    }

    public static Vector2 GetMiddlePosOfVector(Vector2 startPos, Vector2 targetPos)
    {
        return new Vector2((startPos.x + targetPos.x) / 2, (startPos.y + targetPos.y) / 2);
    }


}

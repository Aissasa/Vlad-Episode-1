﻿using UnityEngine;
using System.Collections;
using System;

public static class ExtentionMethods
{

    public static Vector2 Get2DPosition(this Transform trans)
    {
        return new Vector2(trans.position.x, trans.position.y);
    }

    public static bool IsAbove(this Vector2 vect, Vector2 other)
    {
        return vect.y > other.y;
    }

    public static bool IsAtLeftOf(this Vector2 vect, Vector2 other)
    {
        return vect.x < other.x;
    }

    public static Vector2 GetBoxColliderLowerPosition(this GameObject go)
    {
        BoxCollider2D boxCollider = go.GetComponent<BoxCollider2D>();
        Vector2 unitScale = new Vector2(Mathf.Abs(go.transform.localScale.x), Mathf.Abs(go.transform.localScale.y));
        Vector2 boxColliderCenter = new Vector2(go.transform.Get2DPosition().x - boxCollider.offset.x * unitScale.x, go.transform.Get2DPosition().y + boxCollider.offset.y * unitScale.y);
        float yGap = (boxCollider.size.y * unitScale.y) / 2;

        return boxColliderCenter - new Vector2(0, yGap);
    }

    //******Orthographic Camera Only******//

    public static Vector2 BoundsMin(this Camera camera)
    {
        return camera.transform.Get2DPosition() - camera.Extents();
    }

    public static Vector2 BoundsMax(this Camera camera)
    {
        return camera.transform.Get2DPosition() + camera.Extents();
    }

    public static Vector2 Extents(this Camera camera)
    {
        if (camera.orthographic)
            return new Vector2(camera.orthographicSize * Screen.width / Screen.height, camera.orthographicSize);
        else
        {
            Debug.LogError("Camera is not orthographic!", camera);
            return new Vector2();
        }
    }
    //*****End of Orthographic Only*****//
}

using UnityEngine;
using System.Collections;
using System;

public class ObstacleFinder
{

    private static ObstacleFinder instance;

    private ObstacleFinder()
    {

    }

    public static ObstacleFinder Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ObstacleFinder();
            }

            return instance;
        }
    }

    public bool CheckObstacles(GameObject go, Vector2 startPos, Vector2 targetPos, LayerMask unwalkableLayer, out Vector2 obstaclePosition)
    {
        BoxCollider2D boxCollider = go.GetComponent<BoxCollider2D>();
        Vector2 unitScale = go.transform.localScale;
        RaycastHit2D hit;
        Vector2 rayOrigin;
        Vector2 boxColliderCenter = new Vector2(startPos.x + boxCollider.offset.x * unitScale.x, startPos.y + boxCollider.offset.y * unitScale.y);
        float xGap = (boxCollider.size.x * unitScale.x) / 2;
        float yGap = (boxCollider.size.y * unitScale.y) / 2;
        Vector2 rayCastingCenter;
        Vector2 direction = DirectionAndDistanceCalculator.CalculateDirection(startPos, targetPos);
        float distance = DirectionAndDistanceCalculator.CalculateDistance(startPos, targetPos);

        if (direction.x == 0)
        {
            if (direction.y > 0)
            {
                rayCastingCenter = boxColliderCenter + new Vector2(0, yGap);
            }
            else
            {
                rayCastingCenter = boxColliderCenter - new Vector2(0, yGap);
            }

            for (int i = -1; i < 2; i++)
            {
                rayOrigin = rayCastingCenter + new Vector2(i * xGap, 0);
                hit = Physics2D.Raycast(rayOrigin, direction, distance, unwalkableLayer);
                //Debug.DrawLine(rayOrigin, rayOrigin + direction);
                if (hit.collider != null)
                {
                    //Debug.Log(go.name + " is gonna hit an obstacle !");
                    obstaclePosition = hit.point;
                    return true;
                }
            }
        }
        else
        {
            if (direction.x > 0)
            {
                rayCastingCenter = boxColliderCenter + new Vector2(xGap, 0);
            }
            else
            {
                rayCastingCenter = boxColliderCenter - new Vector2(xGap, 0);
            }
            for (int i = -1; i < 2; i++)
            {
                rayOrigin = rayCastingCenter + new Vector2(0, i * yGap);
                hit = Physics2D.Raycast(rayOrigin, direction, distance, unwalkableLayer);
                //Debug.DrawLine(rayOrigin, rayOrigin + direction);
                if (hit.collider != null)
                {
                    //Debug.Log(go.name + " is gonna hit an obstacle !");
                    obstaclePosition = hit.point;
                    return true;
                }
            }

            if (direction.y != 0)
            {
                Vector2 lastRayOrigin = boxColliderCenter + new Vector2(-xGap * Math.Sign(direction.x), yGap * Math.Sign(direction.y));
                hit = Physics2D.Raycast(lastRayOrigin, direction, distance, unwalkableLayer);
                //Debug.DrawLine(lastRayOrigin, lastRayOrigin + direction);
                if (hit.collider != null)
                {
                    //Debug.Log(go.name + " is gonna hit an obstacle !");
                    obstaclePosition = hit.point;
                    return true;
                }
            }
        }

        obstaclePosition = new Vector2(short.MinValue, short.MinValue);
        return false;
    }
}

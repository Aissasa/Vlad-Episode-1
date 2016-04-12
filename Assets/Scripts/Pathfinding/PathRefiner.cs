using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathRefiner {

    protected static PathRefiner instance;

    public static PathRefiner Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PathRefiner();
            }

            return instance;
        }
    }

    protected BezierPath bezierPath;

    public Vector2[] RefinePath(GameObject go, Vector2[] path, LayerMask unwalkableLayer)
    {
        if (path == null || path.Length <= 0)
        {
            return path;
        }

        List<Vector2> refinedPath = new List<Vector2>(path);
        Vector2 colliderPosition;

        int i = 1;
        while (i < refinedPath.Count)
        {
            Vector2 startPos = refinedPath[i - 1];
            Vector2 endPos = refinedPath[i];
            Vector2 direction = DirectionAndDistanceCalculator.CalculateSignedDirection(startPos, endPos);
            if (direction.x != 0 && direction.y != 0)
            {
                if (ObstacleFinder.Instance.CheckObstacles(go, startPos, endPos, unwalkableLayer, out colliderPosition))
                {
                    Vector2 middle = DirectionAndDistanceCalculator.GetMiddleOfVector(startPos, endPos);
                    Vector2 newPoint;
                    if (colliderPosition.IsAbove(middle))
                    {
                        if (direction.y > 0)
                        {
                            newPoint = new Vector2(endPos.x, startPos.y);
                        }
                        else
                        {
                            newPoint = new Vector2(startPos.x, endPos.y);
                        }
                    }
                    else
                    {
                        if (direction.y > 0)
                        {
                            newPoint = new Vector2(startPos.x, endPos.y);
                        }
                        else
                        {
                            newPoint = new Vector2(endPos.x, startPos.y);
                        }
                    }
                    refinedPath.Insert(i, newPoint);
                }
            }
            i++;
        }
        return refinedPath.ToArray();
    }

    public Vector2[] SmoothPath(GameObject go, Vector2[] path, LayerMask unwalkableLayer)
    {
        if (path == null || path.Length<=0)
        {
            return path ;
        }
        Vector2 checkPoint = path[0];
        Vector2 currentPoint;
        Vector2 colliderPosition;
        List<Vector2> smootherPath = new List<Vector2>();
        smootherPath.Add(checkPoint);

        for (int i = 2; i < path.Length; i++)
        {
            currentPoint = path[i];
            if (ObstacleFinder.Instance.CheckObstacles(go, checkPoint, currentPoint, unwalkableLayer, out colliderPosition))
            {
                checkPoint = path[i - 1];
                smootherPath.Add(checkPoint);
            }
        }
        smootherPath.Add(path[path.Length - 1]);
        return smootherPath.ToArray();
    }

    public Vector2[] BezierInterpolate(Vector2[] path, float bezierInterpolationRange)
    {
        if (path == null || path.Length <= 0)
        {
            return path;
        }

        bezierPath = new BezierPath();
        List<Vector2> thePath = new List<Vector2>(path);
        bezierPath.Interpolate(thePath, bezierInterpolationRange);

        return bezierPath.GetPathPoints().ToArray();
    }

    public Vector2[] RefineAndSmoothPath(GameObject gameObject, Vector2[] path, LayerMask unwalkableLayer, float bezierInterpolationRange = 0.25f)
    {
        if (path == null || path.Length<=0)
        {
            return path;
        }
        Vector2[] newPath = RefinePath(gameObject, path, unwalkableLayer);
        newPath = SmoothPath(gameObject, newPath, unwalkableLayer);
        return BezierInterpolate(newPath, bezierInterpolationRange);
    }

}

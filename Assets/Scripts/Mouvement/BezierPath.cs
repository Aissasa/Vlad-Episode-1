using UnityEngine;
using System.Collections.Generic;

/**
    Class for representing a Bezier path, and methods for getting suitable points to 
    simulate a smooth movement.
*/
public class BezierPath
{
    private const int SEGMENTS_PER_CURVE = 10;
    private const float MINIMUM_SQR_DISTANCE = 0.01f; // between points

    // This corresponds to about 172 degrees, 8 degrees from a straight line
    private const float DIVISION_THRESHOLD = -0.99f; 

    private List<Vector2> controlPoints;

    private int curveCount; //how many bezier curves in this path?

    /**
        Constructs a new empty Bezier curve. Use one of these methods
        to add points: SetControlPoints, Interpolate, SamplePoints.
    */
    public BezierPath()
    {
        controlPoints = new List<Vector2>();
    }

    /**
        Sets the control points of this Bezier path.
        Points 0-3 forms the first Bezier curve, points 
        3-6 forms the second curve, etc.
    */
    public void SetControlPoints(List<Vector2> newControlPoints)
    {
        controlPoints.Clear();
        controlPoints.AddRange(newControlPoints);
        curveCount = (controlPoints.Count - 1) / 3;
    }

    /**
        Returns the control points for this Bezier curve.
    */
    public List<Vector2> GetControlPoints()
    {
        return controlPoints;
    }

    /**
        Calculates a Bezier interpolated path for the given points.
    */
    public void Interpolate(List<Vector2> segmentPoints, float scale)
    {
        controlPoints.Clear();

        if (segmentPoints.Count < 2)
        {
            return;
        }

        for (int i = 0; i < segmentPoints.Count; i++)
        {
            if (i == 0) // is first
            {
                Vector2 p1 = segmentPoints[i];
                Vector2 p2 = segmentPoints[i + 1];                

                Vector2 tangent = (p2 - p1);
                Vector2 q1 = p1 + scale * tangent;

                controlPoints.Add(p1);
                controlPoints.Add(q1);
            }
            else if (i == segmentPoints.Count - 1) //last
            {
                Vector2 p0 = segmentPoints[i - 1];
                Vector2 p1 = segmentPoints[i];
                Vector2 tangent = (p1 - p0);
                Vector2 q0 = p1 - scale * tangent;

                controlPoints.Add(q0);
                controlPoints.Add(p1);
            }
            else
            {
                Vector2 p0 = segmentPoints[i - 1];
                Vector2 p1 = segmentPoints[i];
                Vector2 p2 = segmentPoints[i + 1];
                Vector2 tangent = (p2 - p0).normalized;
                Vector2 q0 = p1 - scale * tangent * (p1 - p0).magnitude;
                Vector2 q1 = p1 + scale * tangent * (p2 - p1).magnitude;

                controlPoints.Add(q0);
                controlPoints.Add(p1);
                controlPoints.Add(q1);
            }
        }

        curveCount = (controlPoints.Count - 1) / 3;
    }   

    /**
        Caluclates a point on the path.
        
        @param curveIndex The index of the curve that the point is on. For example, 
        the second curve (index 1) is the curve with controlpoints 3, 4, 5, and 6.
        
        @param t The paramater indicating where on the curve the point is. 0 corresponds 
        to the "left" point, 1 corresponds to the "right" end point.
    */
    public Vector2 CalculateBezierPoint(int curveIndex, float t)
    {
        int nodeIndex = curveIndex * 3;

        Vector2 p0 = controlPoints[nodeIndex];
        Vector2 p1 = controlPoints[nodeIndex + 1];
        Vector2 p2 = controlPoints[nodeIndex + 2];
        Vector2 p3 = controlPoints[nodeIndex + 3];

        return CalculateBezierPoint(t, p0, p1, p2, p3);
    }

    /**
        This gets the path points of a bezier curve, using recursive division,
        which results in less points for the same accuracy as the above implementation.
    */
    public List<Vector2> GetPathPoints()
    {
        List<Vector2> drawingPoints = new List<Vector2>();

        for (int curveIndex = 0; curveIndex < curveCount; curveIndex++)
        {
            List<Vector2> bezierCurveDrawingPoints = FindDrawingPoints(curveIndex);

            if (curveIndex != 0)
            {
                //remove the fist point, as it coincides with the last point of the previous Bezier curve.
                bezierCurveDrawingPoints.RemoveAt(0);
            }

            drawingPoints.AddRange(bezierCurveDrawingPoints);
        }

        return drawingPoints;
    }

    List<Vector2> FindDrawingPoints(int curveIndex)
    {
        List<Vector2> pointList = new List<Vector2>();

        Vector2 left = CalculateBezierPoint(curveIndex, 0);
        Vector2 right = CalculateBezierPoint(curveIndex, 1);

        pointList.Add(left);
        pointList.Add(right);

        FindDrawingPoints(curveIndex, 0, 1, pointList, 1);

        return pointList;
    }
    
    
    /**
        @returns the number of points added.
    */
    int FindDrawingPoints(int curveIndex, float t0, float t1,
        List<Vector2> pointList, int insertionIndex)
    {
        Vector2 left = CalculateBezierPoint(curveIndex, t0);
        Vector2 right = CalculateBezierPoint(curveIndex, t1);

        if ((left - right).sqrMagnitude < MINIMUM_SQR_DISTANCE)
        {
            return 0;
        }

        float tMid = (t0 + t1) / 2;
        Vector2 mid = CalculateBezierPoint(curveIndex, tMid);

        Vector2 leftDirection = (left - mid).normalized;
        Vector2 rightDirection = (right - mid).normalized;

        if (Vector2.Dot(leftDirection, rightDirection) > DIVISION_THRESHOLD || Mathf.Abs(tMid - 0.5f) < 0.0001f)
        {
            int pointsAddedCount = 0;

            pointsAddedCount += FindDrawingPoints(curveIndex, t0, tMid, pointList, insertionIndex);
            pointList.Insert(insertionIndex + pointsAddedCount, mid);
            pointsAddedCount++;
            pointsAddedCount += FindDrawingPoints(curveIndex, tMid, t1, pointList, insertionIndex + pointsAddedCount);

            return pointsAddedCount;
        }

        return 0;
    }

    /**
        Caluclates a point on the Bezier curve represented with the four controlpoints given.
    */
    private Vector2 CalculateBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector2 p = uuu * p0; //first term

        p += 3 * uu * t * p1; //second term
        p += 3 * u * tt * p2; //third term
        p += ttt * p3; //fourth term

        return p;

    }
}

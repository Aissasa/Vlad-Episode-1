using UnityEngine;
using System.Collections;
using System;

namespace EnemyAI
{
    public class EnemyMovementHandler
    {

        public GameObject gameObject { get; set; }
        public Vector2[] Path { get; set; }
        public int TargetIndex { get; set; }
        public Vector2 CurrentWayPoint { get; set; }
        public float CharSpeed { get; set; }

        public EnemyMovementHandler(GameObject go)
        {
            gameObject = go;
            TargetIndex = 0;
        }

        public void DrawPath()
        {
            if (!PathEmpty())
            {
                for (int i = 1; i < Path.Length; i++)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(new Vector3(Path[i].x, Path[i].y, 0.6f), new Vector3(0.1f, 0.1f, 0.1f));
                    if (i == TargetIndex)
                    {
                        Gizmos.DrawLine(gameObject.transform.position, Path[i]);
                    }
                    else
                    {
                        Gizmos.DrawLine(Path[i - 1], Path[i]);
                    }
                }
            }
        }

        public Vector2 GetMovementDirection()
        {
            return DirectionAndDistanceCalculator.CalculateSignedDirection(gameObject.transform.Get2DPosition(), CurrentWayPoint);
        }

        public void MoveAlongPath()
        {
            if (Path == null || Path.Length <= 0)
            {
                return;
            }

            LinearMouvement.Instance.MoveTo(gameObject, CurrentWayPoint, CharSpeed);
            // note : move along path with == or with distance
            if (gameObject.transform.Get2DPosition() == CurrentWayPoint)
            //if( DirectionAndDistanceCalculator.CalculateDistance(gameObject.transform.Get2DPosition(), currentWayPoint)<= GameManager.Instance.aiReachingPrecision)
            {
                TargetIndex++;
                if (TargetIndex >= Path.Length)
                {
                    Path = null;
                    return;
                }
                CurrentWayPoint = Path[TargetIndex];
            }
        }

        public void Reset(GameObject go, Vector2[] _path, float speed)
        {
            if (_path == null || _path.Length <= 0)
            {
                Debug.Log("path null");
                return;
            }
            gameObject = go;
            Path = _path;
            CharSpeed = speed;
            TargetIndex = 0;
            CurrentWayPoint = Path[TargetIndex];
        }

        public void ResetToZero()
        {
            Path = null;
            TargetIndex = 0;
        }

        public bool PathEmpty()
        {
            return Path == null || Path.Length <= 0;
        }

    }
}
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PathRequestManager : MonoBehaviour
{

    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static PathRequestManager instance;
    Pathfinding pathfinding;

    bool isProcessingPath;

    void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    public static void RequestPath(Vector2 pathStart, Vector2 pathEnd, GameObject go, LayerMask unwalkableLayer, Action<Vector2[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, go, unwalkableLayer, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();

    }

    public void FinishedProcessingPath(Vector2[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartPathFinding(currentPathRequest.pathStart, currentPathRequest.pathEnd,
                currentPathRequest.go, currentPathRequest.unwalkableLayer);
        }
    }

    struct PathRequest
    {
        public Vector2 pathStart;
        public Vector2 pathEnd;
        public GameObject go;
        public LayerMask unwalkableLayer;
        public Action<Vector2[], bool> callback;

        public PathRequest(Vector2 _pathStart, Vector2 _pathEnd, GameObject _go, LayerMask _layerMask, Action<Vector2[], bool> _callback)
        {
            pathStart = _pathStart;
            pathEnd = _pathEnd;
            go = _go;
            unwalkableLayer = _layerMask;
            callback = _callback;
        }
    }

}

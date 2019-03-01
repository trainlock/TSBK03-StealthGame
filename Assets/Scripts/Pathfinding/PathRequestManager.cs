using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PathRequestManager : MonoBehaviour
{

    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>(); // Queue of request
    PathRequest currentPathRequest;                                 // Store current request

    static PathRequestManager instance;
    Pathfinding pathfinding;

    bool isProcessingPath;

    void Awake(){
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback){
        // Create a new path request
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);

        //Debug.Log("REQUEST PATH MANAGER: Request path");

        // Add request to queue
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    // Check if we are currently processing the path, if not process the next one. 
    void TryProcessNext(){
        // Check if path is being processed 
        if (!isProcessingPath && pathRequestQueue.Count > 0){
            // Process request and remove from queue
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }

    // Called when path is finished
    public void FinishedProcessingPath(Vector3[] path, bool success){
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    // Datastructure that holds the needed information
    struct PathRequest{
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        // Constructor
        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback){
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
    }
}
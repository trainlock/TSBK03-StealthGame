using UnityEngine;
using System.Collections;

public class PathUnit : MonoBehaviour{
    
    public Transform target;
    public bool isPathfindingOn;
    float speed = 5;
    SeekerMovement movement;
    Vector3[] path;
    Vector3 targetPos;
    Vector3 currentTargetPos;
    int targetIndex;
    bool isSuccessful = false;

    private void Start(){
        movement = GetComponent<SeekerMovement>();
    }

    public void StartPathfinding(){
        //Debug.Log("PATHUNIT: Starting pathfinding");
        PathRequestManager.RequestPath(transform.position, targetPos, OnPathFound);
    }

    public void SetTargetPosition(Vector3 currentTargetPos){
        targetPos = currentTargetPos;
    }

    public Vector3 GetCurrentTargetPosition(){
        return currentTargetPos;
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful){
        // Check if path was found
        if(pathSuccessful){
            path = newPath;
            targetIndex = 0;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
            isSuccessful = true;
        }
        else{
            isSuccessful = false;
        }
    }

    public Vector3[] GetPath(){
        return path;
    }

    IEnumerator FollowPath(){
        //Debug.Log("PATHUNIT: Inside FollowPath!");
        if(path == null){
            //Debug.Log("PATHUNIT: No path found :( ");
            yield return null;
        }
        else if (path.Length == 0){
            //Debug.Log("PATHUNIT: Array is empty");
            yield return null;
        }

        // Check if a path exists and have at least one value
        if (path != null && path.Length > 0){
            // Start at the first point
            Vector3 currentWaypoint = path[0];
            while (true){
                // Check if we are at the current point
                if (transform.position == currentWaypoint){
                    targetIndex++;
                    if (targetIndex >= path.Length){
                        targetIndex = 0;
                        path = new Vector3[0];
                        yield break;
                    }
                    // Update waypoint to the next point
                    currentWaypoint = path[targetIndex];
                }
                // Disable smooth rotation
                //movement.isSmoothRotation = true;

                // Look towards current waypoint
                movement.LookInDirection(currentWaypoint);

                // Move towards the current waypoint
                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
                yield return null;
            }
        }
    }

    // Display path
    public void OnDrawGizmos(){
        if (path != null){
            for (int i = targetIndex; i < path.Length; i++){
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex){
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else{
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}
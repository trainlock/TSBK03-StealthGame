using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerMovement : MonoBehaviour {
    
    #region Variables
    public GameObject           Astar;

    // Private
    private Pathfinding         m_pathfinding;
    private Transform           m_meshPivot;
    private Grid                m_grid;
    private Node                m_currentNode;
    private Vector3             m_currentNodePos;
    private Vector3             m_moveDir = Vector3.zero;
    private Vector3             m_lookDir = Vector3.zero;
    private Vector3             m_lookTarget = Vector3.zero;
    private Vector3             m_nextPos = Vector3.zero;
    private float               m_speed = 3.0f;
    private float               m_turnSpeed = 3.0f;
    private float               m_viewRadius;
    #endregion

    void Start(){
        m_pathfinding = Astar.GetComponent<Pathfinding>();

        m_meshPivot = this.gameObject.transform.GetChild(1);

        // Set the first node to the seekers position
        m_grid = m_pathfinding.GetGrid();
        if(m_grid == null){
            Debug.Log("MOVEMENT: Grid == NULL");
        }
        m_currentNode = m_grid.NodeFromWorldPoint(transform.position);
        m_currentNodePos = transform.position;
        m_viewRadius = GetComponent<FieldOfView>().m_viewRadius;
    }

    #region Getters and Setters
    public Vector3 GetMoveDir(){
        return m_moveDir;
    }

    public Vector3 GetLookDir(){
        return m_lookDir;
    }

    public Vector3 GetLookTarget(){
        return m_lookTarget;
    }

    public Vector3 GetNextPos(){
        return m_nextPos;
    }
    #endregion

    // Look in the given angle
    public bool LookInDirection(float angle){
        // Convert to radians
        angle *= Mathf.Deg2Rad;

        // Calculate the new direction
        Vector3 positionOfTarget = new Vector3(m_viewRadius * Mathf.Cos(angle), 0, m_viewRadius * Mathf.Sin(angle));
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, (positionOfTarget - transform.position), m_speed * Time.deltaTime, 0);

        // Check if the rotation is done
        if(Vector3.Angle(transform.forward, newDirection) < 0.5f){
            transform.rotation = Quaternion.LookRotation(newDirection);
            return true;
        }
        transform.rotation = Quaternion.LookRotation(newDirection);
        return false;
    }

    // Look in the direction of the given position
    public bool LookInDirection(Vector3 lookTarget){
        float step = 3.0f * Time.deltaTime;

        Vector3 targetDir = lookTarget - transform.position;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);

        Debug.DrawRay(transform.position, transform.forward * 5.0f, Color.blue);
        Debug.DrawRay(transform.position, newDir * 5.0f, Color.green);

        if (Vector3.Angle(transform.forward, newDir) < 0.5){
            //Debug.Log("----------------------------!CLOSE!----------------------------");
            transform.rotation = Quaternion.LookRotation(newDir);
            return true;
        }
        transform.rotation = Quaternion.LookRotation(newDir);

        return false;
    }

    // Move to a given position (pathfinding)
    public bool MoveToPos(Vector3 targetPos){
        m_pathfinding.UpdateTarget(targetPos);
        List<Node> path = m_pathfinding.GetPath();

        if(path == null){
            if(targetPos == null){
                return false;
            }
            // Get a new path
            path = m_pathfinding.GetPath();
        }

        // At the end of the path
        if(path.Count < 1){
            //Debug.Log("Target Reached!");
            // Target is reached
            path = null;
            // Change state to idle
            return true;
        }
        // Path nodes still exist
        else {
            // The final target position is at path[path.Count-1]
            Node nextNode = path[0];
            m_nextPos = nextNode.worldPosition;
            Vector3 targetDir = m_nextPos - transform.position;
            m_moveDir = targetDir.normalized;
            Debug.DrawRay(transform.position, m_moveDir * 5.0f, Color.red);

            transform.position = Vector3.MoveTowards(transform.position, m_nextPos, m_speed * Time.deltaTime);
        }
        return false;
    }
}

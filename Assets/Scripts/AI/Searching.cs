using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Searching : MonoBehaviour {

    #region Variables
    private Transform           m_meshPivot;
    private InfluenceMap        m_influenceMap;
    private SeekerMovement      m_movement;
    private FieldOfView         m_fieldOfView;
    private float               m_viewAngle;

    private int                 m_lookAround;
    private int                 m_lookRightLeft;

    private Vector3             m_left;
    private Vector3             m_right;
    private Vector3             m_forward;
    private Vector3             m_newPos = Vector3.zero;

    private bool                m_isRotationDone = false;
    private bool                m_findNewPosition = true;
    #endregion

    void Start(){
        m_meshPivot = this.gameObject.transform.GetChild(1);
        m_influenceMap = GetComponent<InfluenceMap>();
        m_movement = GetComponent<SeekerMovement>();
        m_fieldOfView = GetComponent<FieldOfView>();
        m_viewAngle = m_fieldOfView.m_viewAngle;
        m_lookAround = 0;
        m_lookRightLeft = 0;
    }

    // Switch between looking around and moving
    public bool Run(){
        // Get new target position

        if(LookTowards()){
            return true;
        }
        //return true;

        //switch(lookAround){
        //    case 0:     // Look around
        //        if(LookTowards()){
        //            lookAround++;
        //        }
        //        lookAround++;
        //        //Debug.Log("SEARCH NEARBY: lookAround = " + lookAround);
        //        break;
        //    case 1:     // Move to a new position
        //        //if(movement.LookInDirection(right)){ // TODO: Use position of target instead of angle?
        //        //    lookAround++;
        //        //}
        //        //if (LookTowards()){
        //        //    lookAround++;
        //        //}
        //        //lookRightLeft++;
        //        //Debug.Log("forward = " + (transform.forward+transform.position));
        //        //if(movement.MoveToPos(transform.forward + transform.position)){
        //        //    lookAround++;
        //        //}
        //        //if(findNewPosition){
        //        //    Debug.Log("SEARCH NEARBY: mapPos = " + mapPos);
        //        //    newPos = GetNearbyPosition(mapPos);
        //        //    findNewPosition = false;
        //        //}
        //        //if(movement.MoveToPos(newPos)){
        //        //    lookAround++;
        //        //}
        //        lookAround++;
        //        break;
        //    case 2:     // Look around
        //        //if (LookTowards()){
        //        //    lookAround++;
        //        //}
        //        lookAround++;
        //        break;
        //    default:    // Finished looking in the nearby area, move forwards
        //        //Debug.Log("SEARCH NEARBY: Finished with movement and rotation");
        //        lookAround = 0;
        //        findNewPosition = true;
        //        return true;
        //}
        return false;
    }

    public bool LookTowards(){
        // Get rotation vector for left and right direction
        if(!m_isRotationDone){
            m_left = transform.position + Vector3.Cross(transform.forward, transform.up);
            m_right = transform.position + Vector3.Cross(transform.up, transform.forward);
            m_forward = transform.position + transform.forward;
            m_isRotationDone = true;
        }
        // Look left and right
        switch(m_lookRightLeft){
            case 0: // Look left
                if (m_movement.LookInDirection(m_left)){
                    m_lookRightLeft++;
                }
                break;
            case 1: // Look forward
                if (m_movement.LookInDirection(m_forward)){
                    m_lookRightLeft++;
                }
                break;
            case 2: // Look right
                if (m_movement.LookInDirection(m_right)){
                    m_lookRightLeft++;
                }
                break;
            case 3: // Look forward
                if (m_movement.LookInDirection(m_forward)){
                    m_lookRightLeft++;
                }
                break;
            default: // Finished looking around
                m_lookRightLeft = 0;
                m_isRotationDone = false;
                return true;
        }
        return false;
    }

    Vector3 GetNearbyPosition(Vector2 mapPos){
        float minX = Mathf.Max(1, mapPos.x - 4.0f);
        float maxX = Mathf.Min(m_influenceMap.GetSize() - 1, mapPos.x + 4.0f);
        float minZ = Mathf.Max(1, mapPos.y - 4.0f);
        float maxZ = Mathf.Min(m_influenceMap.GetSize() - 1, mapPos.y + 4.0f);

        int newX = Mathf.RoundToInt(Random.Range(minX, maxX));
        int newY = Mathf.RoundToInt(Random.Range(minZ, maxZ));

        Vector2 temp = m_influenceMap.Indices2World(newX, newY);
        return new Vector3(temp.x, transform.position.y, temp.y);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrolling : MonoBehaviour{

    #region Variables
    private InfluenceMap        m_influenceMap;
    private int                 m_mapSize;

    private SeekerMovement      m_movement;

    private int                 m_kernelSize = 3;                   // Specifices how large the search area is.
    private bool                m_searchNewTarget = true;           // To search for the first position to go to
    private Vector3             m_newTargetPosition = Vector3.zero;
    #endregion

    void Start(){
        m_influenceMap = GetComponent<InfluenceMap>();
        m_mapSize = m_influenceMap.GetSize();
        m_movement = gameObject.GetComponent<SeekerMovement>();
    }

    public bool UpdateMovement(){
        // Search for a new path if path is not found
        if(m_searchNewTarget || m_newTargetPosition == null){
            //Debug.Log("Have no path or target, finds new target and path");
            m_newTargetPosition = GetTargetPosition();
            m_searchNewTarget = false;
        }
        Debug.DrawRay(transform.position, transform.forward * 5.0f, Color.green);

        // Check if seeker has reached target position
        if(m_movement.MoveToPos(m_newTargetPosition)){
            //Debug.Log("Searching for a new position since target is reached");
            m_searchNewTarget = true;
            return true;
        }
        return false;
    }

    Vector3 GetTargetPosition(){
        float[,] map = m_influenceMap.GetInfluenceMap();
        float minPosValue = 100.0f;
        float currentPosValue = 0.0f;

        List<Vector3> positions = new List<Vector3>();
        Vector3 currentPos = transform.position;

        // Kernel of 3x3 square
        if(m_kernelSize == 3){
            // Loop through all to find the position with the smallest value
            for (int y = 1; y < m_mapSize - 1; y = y + m_kernelSize){
                for (int x = 1; x < m_mapSize - 1; x = x + m_kernelSize){
                    currentPosValue = map[x - 1, y + 1] + map[x, y + 1] + map[x + 1, y + 1]
                                    + map[x - 1, y] + map[x, y] + map[x + 1, y]
                                    + map[x - 1, y - 1] + map[x, y - 1] + map[x + 1, y - 1];

                    // Check if current value is the smallest one and replace list with the new value
                    if (currentPosValue < minPosValue){
                        minPosValue = currentPosValue;
                        positions.Clear();
                        positions.Add(m_influenceMap.Indices2World(x, y));
                    }
                    // Check if position is close enough and add to list
                    else if (Mathf.Abs(Mathf.Min(currentPosValue, minPosValue)) < 0.5f){
                        positions.Add(m_influenceMap.Indices2World(x, y));
                    }
                    // Do nothing if the position is not of interest
                }
            }
        }

        // Randomise between the values in the list of positions
        currentPos = positions[Mathf.RoundToInt((Random.Range(0, positions.Count)))];
        return currentPos;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour{

    #region State
    // State of the seeker
    public enum State{
        Idling,             // Idle
        Patrolling,         // Search the whole area
        Searching,          // Search the nearby area
        Hunting,            // Move towards target
        LocalPatrolling     // Move towards last seen position of target with distortion
    };
    #endregion

    #region Variables
    // Public
    public Light                m_spotlight;
    public float                m_timeToSpotTarget = 0.5f;

    // Private
    private FieldOfView         m_fieldOfView;
    private Transform           m_target;
    private Transform           m_meshPivot;
    private Color               m_originalSpotlightColor;
    private SeekerMovement      m_movement;
    private Vector3             m_targetPos;
    private Vector3             m_lastSeenTarget = Vector3.zero;
    private Vector3             m_movementDir = Vector3.zero;
    private Vector3             m_lookDir = Vector3.zero;
    private Vector3             m_lookTarget = Vector3.zero;

    private float               m_targetVisibleTimer;
    private float               m_turnSpeed = 3.0f;
    private float               m_noise;
    private int                 m_currentState;
    private int                 m_lookRightLeft;
    private bool                m_isRotationDone = false;
    private bool                m_isSearchingNearby = false;

    private static float        m_EPSILON = 0.01f;
    #endregion

    void Start(){
        Debug.Log("ENEMY: Start");
        m_currentState = (int)State.Searching; // Searches by default
        m_fieldOfView = GetComponent<FieldOfView>();
        m_originalSpotlightColor = m_spotlight.color;
        m_movement = GetComponent<SeekerMovement>();
        m_target = GameObject.FindGameObjectWithTag("Player").transform;

        // TODO: Add check to see which child equals to the meshpivot
        m_meshPivot = this.gameObject.transform.GetChild(1);
    }

    // Can have a day and night behaviour and state
    void Update(){
        DisplayCurrentState();
        m_fieldOfView.SetLookDir(transform.forward);
        bool hasVisibleTarget = m_fieldOfView.FindVisibleTargets();

        // Move according to state
        // If target is found, hunt it
        if(hasVisibleTarget){
            //Debug.Log("Found target!");
            // Abort current pathfinding and change target
            m_currentState = (int)State.Hunting;
            m_noise = 0.0f;
            m_targetPos = m_fieldOfView.m_visibleTargets[0].position;
            m_targetVisibleTimer += Time.deltaTime;
        }
        //else if(currentState == (int)State.Hunt){
        //    // Look at position where target was last seen
        //    currentState = (int)State.LocalPatrol;
        //}
        else {
            m_targetVisibleTimer -= Time.deltaTime;
        }

        m_targetVisibleTimer = Mathf.Clamp(m_targetVisibleTimer, 0, m_timeToSpotTarget);
        m_spotlight.color = Color.Lerp(m_originalSpotlightColor, Color.red, m_targetVisibleTimer / m_timeToSpotTarget);
        m_movementDir = m_movement.GetMoveDir();

        switch (m_currentState){
            case (int)State.Idling:
                // Can the seeker se the player whilst idle?
                break;
            case (int)State.Patrolling:
                // Move and search the area
                bool isPatrolling = GetComponent<Patrolling>().UpdateMovement(); // Update movement
                if (isPatrolling){
                    m_currentState = (int)State.Searching;
                    m_isSearchingNearby = true;

                    // TODO: Fix so that they search again if a path is not found at all
                }
                // Set viewing direction
                if(m_movementDir != Vector3.zero){
                    transform.rotation = Quaternion.LookRotation(m_movementDir);
                }
                break;
            case (int)State.Searching:
                // Look around where the seeker is standing
                // Check if you have finished looking around in the nearby are
                m_isSearchingNearby = GetComponent<Searching>().Run(); // Look Around
                if (m_isSearchingNearby){
                    // Start searching for the next place to look at
                    m_currentState = (int)State.Patrolling;
                }
                break;
            case (int)State.Hunting:
                // TODO: If player found start searching in the area around it with distortion over time
                // TODO: After a certain amount of time it goes back to normal and searches everywhere


                // TODO: If the player is caught ==> GAME OVER (Restart)
                // TODO: If the player is seen but not caught ==> Search the last seen position

                // If target is found, move towards it using pathfinding
                if(hasVisibleTarget){
                    m_lastSeenTarget = m_fieldOfView.m_visibleTargets[0].position;
                    m_movement.MoveToPos(m_lastSeenTarget);

                    // Set viewing direction
                    transform.rotation = Quaternion.LookRotation(m_movement.GetMoveDir());

                    // Check if target is within reach and if the seeker has grasped the target
                    if(Vector3.Distance(transform.position, m_lastSeenTarget) < m_EPSILON){
                        Debug.Log("Caught intruder!");
                    }

                    // Check if target has been visible for a long while (= Game Over)

                }
                else if(!hasVisibleTarget){ // has not visible target
                    m_currentState = (int)State.LocalPatrolling;
                }

                break;
            case (int)State.LocalPatrolling:
                if (!hasVisibleTarget){
                    if (m_noise < m_EPSILON){
                        m_lastSeenTarget = m_targetPos;
                    }

                    // Noise that is larger over time
                    m_noise += Time.deltaTime;

                    // If too much time has passed, the target is lost
                    if (m_noise > 0.8f){
                        // Start searching the area
                        m_currentState = (int)State.Searching;
                        m_noise = 0.0f;
                    }

                    // Move towards target position with noise
                    if (m_movement.MoveToPos(m_lastSeenTarget)){
                        float newX = Random.Range(m_targetPos.x - m_noise, m_targetPos.x + m_noise);
                        float newZ = Random.Range(m_targetPos.z - m_noise, m_targetPos.z + m_noise);

                        // Set new target with larger noise
                        m_lastSeenTarget = new Vector3(newX, m_targetPos.y, newZ);
                    }
                    // Set viewing direction
                    transform.rotation = Quaternion.LookRotation(m_movement.GetMoveDir());
                }
                else {
                    m_currentState = (int)State.Hunting;
                    m_noise = 0.0f;
                }
                break;
        }
    }

    void DisplayCurrentState(){
        if(m_currentState == (int)State.Idling){
            Debug.Log("ENEMY: Rest");
        }
        else if(m_currentState == (int)State.Patrolling){
            //Debug.Log("ENEMY: Search");
        }
        else if(m_currentState == (int)State.Searching){
            //Debug.Log("ENEMY: Search Nearby");
        }
        else if(m_currentState == (int)State.Hunting){
            //Debug.Log("ENEMY: Hunt");
        }
        else if (m_currentState == (int)State.LocalPatrolling){
            //Debug.Log("ENEMY: Lost Hunt");
        }
    }
}

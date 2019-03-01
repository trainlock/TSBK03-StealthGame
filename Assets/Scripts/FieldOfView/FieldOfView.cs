using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour{

    #region Variables
    [Range(0, 360)]
    public float                viewAngle;
    public float                viewRadius;

    public LayerMask            targetMask;
    public LayerMask            obstacleMask;

    [HideInInspector]
    public List<Transform>      m_visibleTargets = new List<Transform>();

    private Vector3             m_lookDir;
    #endregion

    void Start(){
        StartCoroutine("FindTargetsWithDelay", 0.2f);
    }

    #region Getters and Setters
    public Vector3 GetLookDir(){
        return m_lookDir;
    }

    public void SetLookDir(Vector3 lookDir){
        m_lookDir = lookDir;
    }
    #endregion

    IEnumerator FindTargetsWithDelay(float delay){
        while (true){
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    public bool FindVisibleTargets(){
        // Clear list of targets
        m_visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        // Loop through all targets
        for (int i = 0; i < targetsInViewRadius.Length; i++){
            Transform target = targetsInViewRadius[i].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            directionToTarget.y *= 0;

            // Check if target is within our view angle
            if (Vector3.Angle(m_lookDir, directionToTarget) < viewAngle / 2){
                float distToTarget = Vector3.Distance(transform.position, target.position);

                // Check if something blocks the line of sight to the target
                if (!Physics.Raycast(transform.position, directionToTarget, distToTarget, obstacleMask)){
                    m_visibleTargets.Add(target);
                    return true;
                }
            }
        }
        return false;
    }

    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal){
        // Check if angle is global ,if not convert angle to global
        if (!angleIsGlobal){
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
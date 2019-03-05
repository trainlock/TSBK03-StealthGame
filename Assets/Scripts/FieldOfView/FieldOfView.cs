using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour{

    #region Variables
    [Range(0, 360)]
    public float                m_viewAngle;
    public float                m_viewRadius;

    public LayerMask            m_targetMask;
    public LayerMask            m_obstacleMask;

    [HideInInspector]
    public List<Transform>      m_visibleTargets = new List<Transform>();

    private Transform           m_mesh;
    private Vector3             m_lookDir;
    #endregion

    void Start(){
        //m_mesh = gameObject.transform.GetChild(1);
        m_mesh = gameObject.transform.Find("MeshPivot");
        if(!m_mesh){
            Debug.Log("FOV: Start: Mesh is NOT null");
        }
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
        Collider[] targetsInViewRadius = Physics.OverlapSphere(m_mesh.transform.position, m_viewRadius, m_targetMask);

        // Loop through all targets
        for (int i = 0; i < targetsInViewRadius.Length; i++){
            Transform target = targetsInViewRadius[i].transform;
            Vector3 directionToTarget = (target.position - m_mesh.transform.position).normalized;
            directionToTarget.y *= 0;

            // Check if target is within our view angle
            if (Vector3.Angle(m_lookDir, directionToTarget) < m_viewAngle / 2){
                float distToTarget = Vector3.Distance(m_mesh.transform.position, target.position);

                // Check if something blocks the line of sight to the target
                if (!Physics.Raycast(m_mesh.transform.position, directionToTarget, distToTarget, m_obstacleMask)){
                    m_visibleTargets.Add(target);
                    return true;
                }
            }
        }
        return false;
    }

    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal){
        var m_meshPivot = gameObject.transform.Find("MeshPivot");
        if(!m_meshPivot){
            Debug.Log("FOV: MeshPivot is NOT found");
        }
        // Check if angle is global, if not convert angle to global
        if (!angleIsGlobal){
            angleInDegrees += m_meshPivot.transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
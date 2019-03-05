using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor{

    // Draw handles on the scene view
    void OnSceneGUI(){
        FieldOfView fow = (FieldOfView)target;
        //Transform meshPivot = fow.gameObject.transform.GetChild(1); // Get mesh pivot

        // Draw view radius
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.m_viewRadius);
        //Handles.DrawWireArc(fow.transform.position, Vector3.up, fow.GetLookDir(), 360, fow.viewRadius);
        Vector3 viewAngleA = fow.DirectionFromAngle(-fow.m_viewAngle / 2, false);
        Vector3 viewAngleB = fow.DirectionFromAngle(fow.m_viewAngle / 2, false);

        //Debug.Log("Look Dir = " + fow.GetLookDir());

        //Handles.DrawLine(meshPivot.transform.position, meshPivot.transform.position + viewAngleA * fow.viewRadius);
        //Handles.DrawLine(meshPivot.transform.position, meshPivot.transform.position + viewAngleB * fow.viewRadius);

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.m_viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.m_viewRadius);

        Handles.color = Color.red;
        foreach (Transform visibleTarget in fow.m_visibleTargets){
            Handles.DrawLine(fow.transform.position, visibleTarget.position);
        }
    }
}
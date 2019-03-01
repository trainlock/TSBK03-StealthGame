using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour {

    #region Variables
    // Public
    public Transform    m_target;
    public Vector2      m_pitchMinMax = new Vector2(-40, 85);
    public float        m_distanceFromTarget = 2;
    public float        m_mouseSensitivity = 10.0f;
    public bool         m_lockCursor;

    [Header("Rotation")]
    public Vector3      m_rotationSmoothVelocity;
    public Vector3      m_currentRotation;
    public float        m_rotationSmoothTime = 0.12f;

    // Private
    private float       m_yaw;
    private float       m_pitch;
    #endregion

    void Start () {
        if(m_lockCursor){
            // Hide and lock the cursor (to the center of the screen)
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
	}
	
	void Update () {
		
	}

    private void LateUpdate(){
        m_yaw += Input.GetAxis("Mouse X") * m_mouseSensitivity;
        m_pitch += Input.GetAxis("Mouse Y") * m_mouseSensitivity;
        m_pitch = Mathf.Clamp(m_pitch, m_pitchMinMax.x, m_pitchMinMax.y);

        m_currentRotation = Vector3.SmoothDamp(m_currentRotation, new Vector3(m_pitch, m_yaw), ref m_rotationSmoothVelocity, m_rotationSmoothTime);
        transform.eulerAngles = m_currentRotation;
        Vector3 eAngles = transform.eulerAngles;
        eAngles.x = 0;

        // Set target to look in same direction as camera
        m_target.eulerAngles = eAngles;
        transform.position = m_target.position - transform.forward * m_distanceFromTarget;
    }
}

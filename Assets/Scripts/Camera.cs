using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour {
    /*
    [Header("Settings")]
    public GameObject   m_target;
    public float        m_rotateSpeed = 5;
    public Transform m_target;
    public Vector2 m_pitchMinMax = new Vector2(-40, 85);
    public float m_distanceFromTarget = 2.0f;
    public float m_mouseSensitivity = 10.0f;
    public bool m_lockCursor;

    private Vector3     m_offset;

    void Start(){
        m_offset = m_target.transform.position - transform.position;
    }

    void LateUpdate(){
        float horizontal = Input.GetAxis("Mouse X") * m_rotateSpeed;
        float vertical = Input.GetAxis("Mouse Y") * m_rotateSpeed;
        m_target.transform.Rotate(0, horizontal, 0);

        float desiredAngle = m_target.transform.eulerAngles.y;
        Debug.Log("CAMERA: angle" + desiredAngle);
        Quaternion rotation = Quaternion.Euler(0, desiredAngle, 0);
        transform.position = m_target.transform.position - (rotation * m_offset);

        transform.LookAt(m_target.transform);

        m_yaw += Input.GetAxis("Mouse X") * m_mouseSensitivity;
        m_pitch += Input.GetAxis("Mouse Y") * m_mouseSensitivity;
        m_pitch = Mathf.Clamp(m_pitch, m_pitchMinMax.x, m_pitchMinMax.y);

        m_currentRotation = Vector3.Lerp(m_currentRotation, new Vector3(m_pitch, m_yaw), m_rotationSmoothTime * Time.deltaTime);

        transform.eulerAngles = m_currentRotation;
        Vector3 eAngles = transform.eulerAngles;
        eAngles.x = 0;

        // Set target to look in same direction as camera
        m_target.eulerAngles = eAngles;

    }

    */
    ///*
    #region Variables
    // Public
    [Header("Settings")]
    public Transform    m_target;
    public Vector2      m_pitchMinMax = new Vector2(-40, 85);
    public float        m_distanceFromTarget = 2.0f;
    public float        m_mouseSensitivity = 10.0f;
    public bool         m_lockCursor;

    [Header("Rotation")]
    public Vector3      m_rotationSmoothVelocity;
    public Vector3      m_currentRotation;
    public float        m_rotationSmoothTime = 8.0f;

    [Header("Collision Variables")]


    [Header("Transparancy")]
    public bool         m_changeTransparency = true;
    public MeshRenderer m_targetRenderer;

    [Header("Speeds")]
    public float        m_cameraMoveSpeed = 5.0f;
    public float        m_cameraReturnSpeed = 9.0f;
    public float        m_wallPush = 0.7f;

    [Header("Distances")]
    public float        m_closestDistToTarget = 2.0f;
    public float        m_absoluteMinDistToTarget = 1.0f;

    [Header("Masks")]
    public LayerMask    m_obstacleMask;

    // Private
    private float       m_yaw;
    private float       m_pitch;
    private bool        m_pitchLock = false;
    #endregion

    void Start(){
        if(m_lockCursor){
            // Hide and lock the cursor (to the center of the screen)
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
	}
	
	void Update(){
		
	}

    void LateUpdate(){
        // Check if camera have a collision
        CollisionCheck(m_target.position - transform.forward * m_distanceFromTarget);
        WallCheck();

        if(!m_pitchLock){
            m_yaw += Input.GetAxis("Mouse X") * m_mouseSensitivity;
            m_pitch += Input.GetAxis("Mouse Y") * m_mouseSensitivity;
            m_pitch = Mathf.Clamp(m_pitch, m_pitchMinMax.x, m_pitchMinMax.y);

            m_currentRotation = Vector3.Lerp(m_currentRotation, new Vector3(m_pitch, m_yaw), m_rotationSmoothTime * Time.deltaTime);
            //m_currentRotation = Vector3.SmoothDamp(m_currentRotation, new Vector3(m_pitch, m_yaw), ref m_rotationSmoothVelocity, m_rotationSmoothTime);
        }
        // Camera in collision
        else {
            m_yaw += Input.GetAxis("Mouse X") * m_mouseSensitivity;
            m_pitch = m_pitchMinMax.y;

            m_currentRotation = Vector3.Lerp(m_currentRotation, new Vector3(m_pitch, m_yaw), m_rotationSmoothTime * Time.deltaTime);
        }

        transform.eulerAngles = m_currentRotation;
        Vector3 eAngles = transform.eulerAngles;
        eAngles.x = 0;

        // Set target to look in same direction as camera
        m_target.eulerAngles = eAngles;
    }

    void WallCheck(){
        Ray ray = new Ray(m_target.position, -m_target.forward);
        RaycastHit hit;

        if(Physics.SphereCast(ray, 0.5f, out hit, 0.7f, m_obstacleMask)){
            m_pitchLock = true;
        }
        else {
            m_pitchLock = false;
        }
    }

    // ReturnPt is the predicted position, not the current one
    void CollisionCheck(Vector3 returnPt){
        RaycastHit hit;

        // Check if the predicted position collides with something
        if(Physics.Linecast(transform.position, returnPt, out hit, m_obstacleMask)){
            // Get the normal of the obstacle
            Vector3 normal = hit.normal * m_wallPush;
            Vector3 pt = hit.point + normal;

            TransparencyCheck();

            // Check if the predicted position is to close to the target
            if(Vector3.Distance(Vector3.Lerp(transform.position, pt, m_cameraMoveSpeed * Time.deltaTime), m_target.position) <= m_absoluteMinDistToTarget){
                
            }
            // Update position of camera
            else {
                transform.position = Vector3.Lerp(transform.position, pt, m_cameraMoveSpeed * Time.deltaTime);
            }
            return;
        }
        FullTransparency();
        transform.position = Vector3.Lerp(transform.position, returnPt, m_cameraReturnSpeed * Time.deltaTime);
        m_pitchLock = false;
    }

    void TransparencyCheck(){
        if(m_changeTransparency){
            if(Vector3.Distance(transform.position, m_target.position) <= m_closestDistToTarget){
                // Loop through all materials if several exists 
                // Change target transparency
                Color tmpColor = m_targetRenderer.sharedMaterial.color;
                tmpColor.a = Mathf.Lerp(tmpColor.a, 0.2f, m_cameraMoveSpeed * Time.deltaTime);
                m_targetRenderer.sharedMaterial.color = tmpColor;
            }
            else {
                // Check if material is transparent
                if(m_targetRenderer.sharedMaterial.color.a <= 0.99f){
                    // Change back to no transparency
                    Color tmpColor = m_targetRenderer.sharedMaterial.color;
                    tmpColor.a = Mathf.Lerp(tmpColor.a, 1.0f, m_cameraMoveSpeed * Time.deltaTime);
                    m_targetRenderer.sharedMaterial.color = tmpColor;
                }
            }
        }
    }

    void FullTransparency(){
        if(m_changeTransparency){
            if(m_targetRenderer.sharedMaterial.color.a <= 0.99f){
                // Change back to no transparency
                Color tmpColor = m_targetRenderer.sharedMaterial.color;
                tmpColor.a = Mathf.Lerp(tmpColor.a, 1.0f, m_cameraMoveSpeed * Time.deltaTime);
                m_targetRenderer.sharedMaterial.color = tmpColor;
            }
        }
    }
    //*/
}

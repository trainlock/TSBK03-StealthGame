using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    #region Variables
    // Public
    [Header("Movement")]
    public float            m_MovementSpeed = 10;
    public float            m_AimSpeedModifier = 0.5f;
    public float            m_Acceleration = 10;
    public float            m_SprintModifier = 1.5f;
    public float            m_JumpSpeed = 5;
    public float            m_TurnSpeed = 5;
    public float            m_AirControl = 0.1f;

    //[Header("Camera")]
    //public Transform        m_Camera;
    //public float            m_MouseSensitivityX = 1;
    //public float            m_MouseSensitivityY = 1;

    // Private
    private Transform       m_Mesh;
    private Rigidbody       m_Rigidbody;
    private Vector3         m_InputVec;
    private Vector3         m_CurrentInputVec;
    private Vector3         m_CurrentCameraSlotPos;
    private Vector3         m_DeathCamPos;
    private bool            m_Grounded;
    private bool            m_ShouldJump;
    private float           m_CameraYaw;
    private float           m_CameraPitch;
    #endregion

    // Use this for initialization
    void Start () {
        // Get components
        m_Rigidbody = GetComponent<Rigidbody>();

        // Find transforms
        m_Mesh = this.gameObject.transform.GetChild(1);
    }
    
    // This runs at a variable rate (depends on the framerate)
    void Update () {
        // Update input from the keyboard/mouse
        UpdateInput();
    }

    // This runs at a fixed rate (every physics timestep)
    void FixedUpdate(){
        // Update the movement of the player. Since this involves physics, we run it in FixedUpdate
        UpdateMovement();
    }

    // This runs at a variable rate (depends on the framerate), but at the very end of the frame
    //void LateUpdate(){
    //    // Update camera movement, rotation
    //    UpdateCamera();
    //}

    void UpdateInput(){
        // Movement, directional vector implementation
        m_InputVec = Vector3.zero;
        m_InputVec += Input.GetKey(KeyCode.W) ?  transform.forward : Vector3.zero;
        m_InputVec += Input.GetKey(KeyCode.S) ? -transform.forward : Vector3.zero;
        m_InputVec += Input.GetKey(KeyCode.D) ?  transform.right : Vector3.zero;
        m_InputVec += Input.GetKey(KeyCode.A) ? -transform.right : Vector3.zero;

        // Camera-relative input
        //m_InputVec = m_Camera.rotation * m_InputVec;
        //m_InputVec.y = 0; // The camera rotation might give us some movement in the y-direction (up/down) so we want to remove that

        // Normalize input vector so movement speed is the same in all directions
        m_InputVec.Normalize();

        // If we are moving, turn the mesh to face towards the moving direction
        if (m_InputVec.magnitude > Mathf.Epsilon){
            m_Mesh.rotation = Quaternion.Slerp(m_Mesh.rotation, Quaternion.LookRotation(m_InputVec), Time.deltaTime * m_TurnSpeed);
        }

        //Debug.Log("Space pressed = " + Input.GetKeyDown(KeyCode.Space));

        // Store jump input so it can be used in the fixed update
        //if (Input.GetKeyDown(KeyCode.Space) && m_Grounded){
        //    // Need to be grounded and not aiming to be able to jump
        //    m_ShouldJump = true;
        //}
        // Add Player controlled camera movement
    }

    void UpdateMovement(){
        // Move the player by pushing the rigidbody (XZ only)
        if(transform.position.y < 0.0001f){
            m_Grounded = true;
        }

        // Smooth (lerp) the input vector, giving us some acceleration/deceleration
        m_CurrentInputVec = Vector3.Lerp(m_CurrentInputVec, m_InputVec, m_Acceleration * Time.fixedDeltaTime);

        // Because the members of Rigidbody.velocity cannot be modified individually, we obtain a copy, modify it, and then assign it back
        Vector3 currentVelocity = m_Rigidbody.velocity;
        Vector3 inputVelocity = m_CurrentInputVec * m_MovementSpeed; // Calculate input velocity based on the input vector and movement speed
        Debug.DrawRay(transform.position, transform.forward * 5.0f, Color.blue);

        // On the ground
        if (m_Grounded) {
            // This will override the existing velocity, giving us absolute control of player movement on the ground
            currentVelocity.x = inputVelocity.x;
            currentVelocity.z = inputVelocity.z;
        }
        // In the air, IF the player is moving
        else if (m_InputVec.magnitude > Mathf.Epsilon && m_AirControl > Mathf.Epsilon) {
            // This will give us some amount of control in the air, but will not override the existing velocity 
            currentVelocity.x = Mathf.Lerp(currentVelocity.x, inputVelocity.x, m_AirControl);
            currentVelocity.z = Mathf.Lerp(currentVelocity.z, inputVelocity.z, m_AirControl);
        }
        else if(!m_Grounded){
            //currentVelocity -= 9.82f;
        }

        // Re-assign the velocity to the rigidbody
        //m_Rigidbody.velocity = currentVelocity;
        transform.position += currentVelocity * Time.deltaTime;
        //Debug.Log("PLAYER: Position = " + transform.position);

        // TODO: Add gravity

        // Jump by applying some velocity straight upwards
        if (m_ShouldJump){
            Debug.Log("Jumping");
            //m_Rigidbody.velocity += Vector3.up * m_JumpSpeed;
            //transform.position += Vector3.up * m_JumpSpeed;// * Time.deltaTime;
            m_ShouldJump = false;
            m_Grounded = false;
        }
    }

    void OnTriggerEnter(Collider other){
        m_Grounded = true;

        //if (other.gameObject.CompareTag("Pick Up")){
        //    other.gameObject.SetActive(false);
        //}
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    #region Variables
    public GameObject       m_gameManager; 

    // Public
    [Header("Movement")]
    public float            m_movementSpeed = 10;
    public float            m_aimSpeedModifier = 0.5f;
    public float            m_acceleration = 10;
    public float            m_sprintModifier = 1.5f;
    public float            m_jumpSpeed = 5;
    public float            m_turnSpeed = 5;
    public float            m_airControl = 0.1f;

    [Header("Gameplay")]
    public int              m_maxBooks = 0;
    public int              m_maxParchments = 0;

    //[Header("Camera")]
    //public Transform        m_camera;
    //public float            m_mouseSensitivityX = 1;
    //public float            m_mouseSensitivityY = 1;

    // Private
    private Transform       m_mesh;
    private Rigidbody       m_rigidbody;
    private Vector3         m_inputVec;
    private Vector3         m_currentInputVec;
    private Vector3         m_currentCameraSlotPos;
    private Vector3         m_deathCamPos;
    private bool            m_grounded;
    private bool            m_shouldJump;
    private bool            m_isDiscovered = false;
    private bool            m_disabled = false;
    private float           m_cameraYaw;
    private float           m_cameraPitch;
    private int             m_collectedBooks;
    private int             m_collectedParchments;
    #endregion

    public void SetDisabled(){
        m_disabled = !m_disabled;
    }

    // Use this for initialization
    void Start () {
        // Instantiate gameManager prefab
        if (GameManager.manager == null){
            Instantiate(m_gameManager);
        }

        // Get components
        m_rigidbody = GetComponent<Rigidbody>();

        // Find transforms
        m_mesh = this.gameObject.transform.GetChild(1);

        m_maxBooks = GameObject.FindGameObjectsWithTag("Book").Length;
        Debug.Log("PLAYER: m_maxBooks = " + m_maxBooks);
    }
    
    // This runs at a variable rate (depends on the framerate)
    void Update () {
        if(!m_disabled){
            // Update input from the keyboard/mouse
            UpdateInput();
        }

        UpdateUI();
    }

    // This runs at a fixed rate (every physics timestep)
    void FixedUpdate(){
        if (!m_disabled){
            // Update the movement of the player. Since this involves physics, we run it in FixedUpdate
            UpdateMovement();
        }
    }

    // This runs at a variable rate (depends on the framerate), but at the very end of the frame
    //void LateUpdate(){
    //    // Update camera movement, rotation
    //    UpdateCamera();
    //}

    void UpdateInput(){
        // Movement, directional vector implementation
        m_inputVec = Vector3.zero;
        m_inputVec += Input.GetKey(KeyCode.W) ?  transform.forward : Vector3.zero;
        m_inputVec += Input.GetKey(KeyCode.S) ? -transform.forward : Vector3.zero;
        m_inputVec += Input.GetKey(KeyCode.D) ?  transform.right : Vector3.zero;
        m_inputVec += Input.GetKey(KeyCode.A) ? -transform.right : Vector3.zero;

        // Camera-relative input
        //m_InputVec = m_Camera.rotation * m_InputVec;
        //m_InputVec.y = 0; // The camera rotation might give us some movement in the y-direction (up/down) so we want to remove that

        // Normalize input vector so movement speed is the same in all directions
        m_inputVec.Normalize();

        // If we are moving, turn the mesh to face towards the moving direction
        if (m_inputVec.magnitude > Mathf.Epsilon){
            m_mesh.rotation = Quaternion.Slerp(m_mesh.rotation, Quaternion.LookRotation(m_inputVec), Time.deltaTime * m_turnSpeed);
        }

        //Debug.Log("Space pressed = " + Input.GetKeyDown(KeyCode.Space));

        // Store jump input so it can be used in the fixed update
        //if (Input.GetKeyDown(KeyCode.Space) && m_Grounded){
        //    // Need to be grounded and not aiming to be able to jump
        //    m_ShouldJump = true;
        //}
        // Add Player controlled camera movement
    }


    void UpdateUI(){
        // Update the book counter
        UIManager.manager.SetCollectedBooks((float)m_collectedBooks, (float)m_maxBooks);

        // Update the parchment counter
        UIManager.manager.SetCollectedParchments((float)m_collectedParchments, (float)m_maxParchments);
    }

    void UpdateMovement(){
        // Move the player by pushing the rigidbody (XZ only)
        if(transform.position.y < 0.0001f){
            m_grounded = true;
        }

        // Smooth (lerp) the input vector, giving us some acceleration/deceleration
        m_currentInputVec = Vector3.Lerp(m_currentInputVec, m_inputVec, m_acceleration * Time.fixedDeltaTime);

        // Because the members of Rigidbody.velocity cannot be modified individually, we obtain a copy, modify it, and then assign it back
        Vector3 currentVelocity = m_rigidbody.velocity;
        Vector3 inputVelocity = m_currentInputVec * m_movementSpeed; // Calculate input velocity based on the input vector and movement speed
        Debug.DrawRay(transform.position, transform.forward * 5.0f, Color.blue);

        // On the ground
        if (m_grounded) {
            // This will override the existing velocity, giving us absolute control of player movement on the ground
            currentVelocity.x = inputVelocity.x;
            currentVelocity.z = inputVelocity.z;
        }
        // In the air, IF the player is moving
        else if (m_inputVec.magnitude > Mathf.Epsilon && m_airControl > Mathf.Epsilon) {
            // This will give us some amount of control in the air, but will not override the existing velocity 
            currentVelocity.x = Mathf.Lerp(currentVelocity.x, inputVelocity.x, m_airControl);
            currentVelocity.z = Mathf.Lerp(currentVelocity.z, inputVelocity.z, m_airControl);
        }
        else if(!m_grounded){
            //currentVelocity -= 9.82f;
        }

        // Re-assign the velocity to the rigidbody
        //m_Rigidbody.velocity = currentVelocity;
        transform.position += currentVelocity * Time.deltaTime;
        //Debug.Log("PLAYER: Position = " + transform.position);

        // TODO: Add gravity

        // Jump by applying some velocity straight upwards
        if (m_shouldJump){
            Debug.Log("Jumping");
            //m_Rigidbody.velocity += Vector3.up * m_JumpSpeed;
            //transform.position += Vector3.up * m_JumpSpeed;// * Time.deltaTime;
            m_shouldJump = false;
            m_grounded = false;
        }
    }

    void OnTriggerEnter(Collider other){
        m_grounded = true;
    }

    public void SetDiscovered(bool status){
        m_isDiscovered = status;
    }

    public bool IsDiscovered(){
        return m_isDiscovered;
    }

    // Add book to count
    // Returns true if book is added, else false
    public bool AddBook(int count){
        // All books are collected (Part of win condition though...)
        if(m_collectedBooks >= m_maxBooks){
            return false;
        }
        m_collectedBooks = Mathf.Min(m_collectedBooks + count, m_maxBooks); // Make sure to not add more books than is allowed
        Debug.Log("PLAYER: Collected Books  = " + m_collectedBooks);
        GameManager.manager.PickupCollected(); // Returns null
        return true;
    }

    // Add parchment to count
    // Returns true if parchment is added, else false
    public bool AddParchment(int count){
        // All books are collected (Part of win condition though...)
        if (m_collectedParchments >= m_maxParchments){
            return false;
        }
        m_collectedParchments = Mathf.Min(m_collectedParchments + count, m_maxParchments); // Make sure to not add more parchments than is allowed
        //GameManager.manager.PickupCollected();
        return true;
    }
}

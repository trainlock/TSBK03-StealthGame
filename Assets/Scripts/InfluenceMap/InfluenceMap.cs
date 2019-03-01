using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfluenceMap : MonoBehaviour {

    #region Variables
    // Public variables
    public Image                image;
    public GameObject           ground;
    public int                  nrOfPasses;

    // Private variables
    private const int           m_size = 30;
    private float[,]            m_uneditedInfluenceMap = new float[m_size, m_size];
    private float[,]            m_influenceMap = new float[m_size, m_size];
    private float[,]            m_tempMap = new float[m_size, m_size];

    private float               m_decay = 3.0f;
    private float               m_momentum = 5.0f;
    private float               m_groundSize;

    private Transform           m_meshPivot;
    private Vector3             m_pos;
    #endregion

    // Use this for initialization
    void Start () {
        InitMap();
        m_groundSize = ground.GetComponent<Collider>().bounds.size.x; // Size of plane/ground, float moveAreaX = gameObject.GetComponent<Renderer>().bounds.size.x / 2;
        m_meshPivot = this.gameObject.transform.GetChild(1);
     }


    void InitMap(){
        for (int x = 0; x < m_size; x++){
            for (int y = 0; y < m_size; y++){
                m_uneditedInfluenceMap[x, y] = 0.0f;
                m_influenceMap[x, y] = 0.0f;
                m_tempMap[x, y] = 0.0f;
            }
        }
    }

    #region Getters and Setters
    public int GetSize(){
        return m_size;
    }

    public float[,] GetInfluenceMap(){
        return m_influenceMap;
    }
    #endregion

    // Update is called once per frame
    void Update() {
        // Update map and position
        UpdateMap();

        // Copy the values to a temporary map to prepare for the low-pass filtration
        CopyMap(m_uneditedInfluenceMap, m_tempMap);

        if(nrOfPasses == 0){
            // Copy the values to the finished map
            CopyMap(m_tempMap, m_influenceMap);
        }
        else{
            // Ping-ponging
            for (int i = 0; i < nrOfPasses; i++){
                // Perform low pass filtering
                if(i % 2 == 0){
                    LowPassFilter(m_tempMap, m_influenceMap);
                }
                else{
                    // Lowpass filter the image to the filtered map
                    LowPassFilter(m_influenceMap, m_tempMap);
                }
            }
        }

        // Display the minimap of the influence map
        DisplayInfluenceMap(m_uneditedInfluenceMap);
	}

    void UpdateMap(){
        // Loop through all elements
        for (int y = 0; y < m_size; y++){
            for (int x = 0; x < m_size; x++){
                // Decrease value over time if it is not already set to 0
                if (m_uneditedInfluenceMap[x, y] > 0.0f){
                    // Set new values
                    m_uneditedInfluenceMap[x, y] = m_uneditedInfluenceMap[x, y] -1.0f * (Time.deltaTime / m_decay);
                }
                else{
                    // Set new values
                    m_uneditedInfluenceMap[x, y] = 0.0f;
                }
            }
        }

        m_pos = m_meshPivot.transform.position; // World space position
        Vector2 indices = World2Indices(m_pos);

        // Doesn't go all the way out on the map

        // Set influence value to 1.0 for the position of the current gameobject
        m_uneditedInfluenceMap[(int)indices.x, (int)indices.y] = 1.0f; // TODO: Position is slightly off when field of view is facing north


        // Field of view for seeker
        FieldOfView fieldOfView = GetComponent<FieldOfView>();

        // Check if the positions in the influence map are withing the field of view
        for (int y = 0; y < m_size; y++){
            for (int x = 0; x < m_size; x++){
                // Get position 
                Vector3 worldPos = Indices2World(x, y);

                float distToTarget = Vector3.Distance(m_meshPivot.transform.position, worldPos);
                // Check if the position is within the view radius
                if(distToTarget < fieldOfView.viewRadius){
                    Vector3 directionToPosition = (worldPos - m_meshPivot.transform.position).normalized;
                    directionToPosition.y *= 0;

                    // Check if the position is within the view angle
                    if(Vector3.Angle(m_meshPivot.transform.forward, directionToPosition) < fieldOfView.viewAngle / 2){
                        // Check if something blocks the line of sight to the position
                        if(!Physics.Raycast(m_meshPivot.transform.position, directionToPosition, distToTarget, fieldOfView.obstacleMask)){
                            // Set the position to a value > 0, where the value decreases with the length from the seeker
                            m_uneditedInfluenceMap[x, y] = Mathf.Min(m_uneditedInfluenceMap[x, y] + 1.0f - Mathf.Pow(distToTarget / fieldOfView.viewRadius, 2), 1.0f);
                        }
                    }
                }
            }
        }
    }

    // Assign the values of the inMap to the outMap
    void CopyMap(float[,] inMap, float[,] outMap){
        for (int y = 0; y < m_size; y++){
            for (int x = 0; x < m_size; x++){
                outMap[x, y] = inMap[x, y];
            }
        }
    }

    void LowPassFilter(float[,] inMap, float[,] outMap){
        float noiseSmoothing = 1.0f / 8.0f;
        float centerValue = 0.0f, crossValue = 0.0f, cornerValue = 0.0f;
        for (int y = 1; y < m_size-1; y++){
            for (int x = 1; x < m_size-1; x++){
                centerValue = (1.0f / 2.0f) * inMap[x, y];
                crossValue = noiseSmoothing * (inMap[x, y - 1] + inMap[x - 1, y] + inMap[x + 1, y] + inMap[x, y + 1]);
                cornerValue = inMap[x - 1, y - 1] + inMap[x + 1, y] + inMap[x - 1, y + 1] + inMap[x + 1, y + 1];
                outMap[x, y] = centerValue + crossValue + cornerValue;
            }
        }
    }

    // Show the influence map
    void DisplayInfluenceMap(float[,] iMap){
        // Create texture
        Texture2D texture = new Texture2D(m_size, m_size);

        // Create sprite
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, m_size, m_size), Vector2.zero);

        // Write sprite to image
        image.sprite = sprite;

        // Go through all elements and write as pixels
        for (int y = 0; y < texture.height; y++){
            for (int x = 0; x < texture.width; x++){
                Color colour = new Color(iMap[x, y], iMap[x, y], iMap[x, y], 1);
                texture.SetPixel(x, y, colour);
            }
        }
        texture.Apply();
    }

    // Move the world position with an origo at the bottom left corner
    // Returns the normal so should have a value between 0 and 1
    public Vector2 World2Normal(Vector3 pos){
        Vector2 normalPos;
        // Should not contain negative values
        normalPos.x = (pos.x + (m_groundSize / 2.0f)) / m_groundSize;
        normalPos.y = (pos.z + (m_groundSize / 2.0f)) / m_groundSize;
        return normalPos;
    }

    public Vector2 World2Indices(Vector3 worldPos){
        Vector2 indices;
        Vector2 normalPos = World2Normal(worldPos);
        indices.x = (int)(normalPos.x * m_size);
        indices.y = (int)(normalPos.y * m_size);

        return indices;
    }

    public Vector3 Indices2World(int x, int y){
        // Percentage of positions in local coordinate and then converted to world
        Vector2 newPos;
        newPos.x = ((float)x / m_size) * m_groundSize - m_groundSize / 2.0f;
        newPos.y = ((float)y / m_size) * m_groundSize - m_groundSize / 2.0f;

        return new Vector3(newPos.x, transform.position.y, newPos.y);
    }
}

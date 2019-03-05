using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Needed to reload the scene

public class GameManager : MonoBehaviour {

    #region Variables
    public static GameManager   manager;

    // Public
    public int                  m_totalCollectiblesCount = 3;

    // Private
    private Player              m_player;
    private int                 m_currentPickupCount = 0;
    private bool                m_gameWon = false;
    private bool                m_isDiscovered = false;
    #endregion

    void Start () {
		// Singleton pattern
        if(manager){
            Destroy(manager);
        }
        manager = this;

        // Get a reference to the player
        m_player = FindObjectOfType<Player>();
        int maxBooks = GameObject.FindGameObjectsWithTag("Book").Length;
        int maxParchment = GameObject.FindGameObjectsWithTag("Parchment").Length;
        Debug.Log("GAME MANAGER: Total pickup count = " + m_currentPickupCount);
        m_currentPickupCount = maxBooks + maxParchment;
	}

    // TODO: Add win and lose condition!
    void Update(){
        Debug.Log("GM: isdiscovered? " + m_player.IsDiscovered() + ", is game won? " + m_gameWon);
        // Check if the game should be restarted
        if((!m_player.IsDiscovered() || m_gameWon) && Input.GetKeyDown(KeyCode.R)){
            // Reload the scene by telling the SceneManager to load the current (active) scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    // Call this if the player has been discovered
    public void PickupCollected(){
        m_currentPickupCount--;
        Debug.Log("GM: Pickup collected, currently left pickups are " + m_currentPickupCount);

        // Check if the game is won
        if(!IsGameWon() && !m_gameWon){
            UIManager.manager.GameWon();
            m_gameWon = true;
        }
    }

    // Call this when all artefacts are collected
    public bool IsGameWon(){
        return m_currentPickupCount <= 0;
    }
}

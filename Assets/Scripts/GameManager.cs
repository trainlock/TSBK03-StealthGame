using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    #region Variables
    public static GameManager   manager;

    // Public
    public int                  m_totalCollectiblesCount = 3;

    // Private
    private Player              m_Player;
    private bool                m_gameWon = false;
    #endregion

    void Start () {
		// Singleton pattern
        if(manager){
            Destroy(manager);
        }
        manager = this;

        // Get a reference to the player
        m_Player = FindObjectOfType<Player>();
	}

    public bool IsGameWon(){
        return false;
    }
}

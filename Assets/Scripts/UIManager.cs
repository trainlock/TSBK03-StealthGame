using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    #region Variables
    public static UIManager manager;

    // Public
    public GameObject m_gameOverText;
    public GameObject m_gameWonText;
    #endregion

    private void Start(){
        // Singleton pattern
        if(manager){
            Destroy(manager);
        }
        manager = this;

        // Hide end texts
        m_gameOverText.SetActive(false);
        m_gameWonText.SetActive(false);
    }

    // We call this to tell the UI the player lost
    public void GameLost(){
        // Show the death text
        m_gameOverText.SetActive(true);
    }

    // We call this to tell the UI the game is won
    public void GameWon(){
        m_gameWonText.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    #region Variables
    public static UIManager manager = null;

    // Public
    //public RectTransform    m_bookCount;
    //public RectTransform    m_parchmentCount;
    public GameObject       m_collectedBooksText;
    public GameObject       m_gameOverText;
    public GameObject       m_gameWonText;
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

    // We call this to set the size of the health bar
    public void SetCollectedBooks(float nrBooks = 0, float maxNrBooks = 3){
        // Show number of collected books
        m_collectedBooksText.transform.GetChild(1).GetComponent<Text>().text = nrBooks + "/" + maxNrBooks;
    }

    // We call this to set the size of the ammo bar
    public void SetCollectedParchments(float nrParchments, float maxNrParchments)
    {
        // Show number of collected parchments

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

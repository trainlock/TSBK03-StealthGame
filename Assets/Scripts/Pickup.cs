using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Add pickup type (so that there are several different things that you have to collect
[System.Serializable] // Without this, the enum will not be visible in the inspector
public enum PickupType {
    Book,
    Parchment
}

public class Pickup : MonoBehaviour {
	#region Variables
	public PickupType   m_type;
	public int          m_count = 1;
    #endregion

	// Entered when our ground collider is triggered
	void OnTriggerEnter(Collider other) {
		// Check if a player entered the trigger
		var player = other.transform.parent.gameObject.transform.GetComponent<Player>();
		if(player){
			bool pickedUp = false;

			switch(m_type){
				case PickupType.Book:
                    //Debug.Log("PICKUP: Picked up book");
					pickedUp = player.AddBook(m_count);
					break;
				case PickupType.Parchment:
                    //Debug.Log("PICKUP: Picked up parchment");
					pickedUp = player.AddParchment(m_count);
					break;
				default:
					break;
			}
			// Destroy the game object when picked up
            if (pickedUp){
                Destroy(gameObject);
			}
		}
	}

    // Add spinning motion or light to object
}

using UnityEngine;
using System.Collections;

public class ActorHealth : MonoBehaviour {

	public delegate void Death();
	public event Death onDeath;
	
	// how much health the object should start with
	public float startHealth = 100;
	
	public float Amount { get { return health; } }
	private float health;
	
	public void Start() {
		Actor actor = gameObject.GetComponent<Actor>();
		if(actor)
			startHealth = actor.combatStats.health.combatValue;
		health = startHealth;
	}
	
	// the attached object has been damaged
	public void takeDamage(float amount) {
		health -= amount;
		
		// don't let the health go below zero
		if (health <= 0) {
			health = 0;
			// fire an event when the attached object is dead
			if (onDeath != null) {
				onDeath();
				onDeath = null;
			}
		}
	}
	
	// reset the variables back to their starting variables
	public void reset() {
		health = startHealth;
		onDeath = null;
	}
}

using UnityEngine;
using System.Collections;

public class ActorHealth : MonoBehaviour {

	public delegate void Death();
	public event Death onDeath;
	
	// how much health the object should start with
	public float startHealth = 100;
	public float attackImmunityTime = .5f;
	public float Amount { get { return health; } }
	public float criticalHealthThreshold = .15f;
	public float health;
	private float lastHitTime;


	public void Start() {
		Actor actor = gameObject.GetComponent<Actor>();
		if(actor)
			startHealth = actor.combatStats.health.combatValue;
		health = startHealth;
	}

	
	// the attached object has been damaged
	public void TakeDamage(float amount) {
		if(Time.time <= lastHitTime + attackImmunityTime)
			return;

		lastHitTime = Time.time;
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

	public bool IsAlive() { return health > 0; }

	public float GetHealthPercentage() { return health / startHealth; }
}

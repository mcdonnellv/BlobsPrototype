using UnityEngine;
using System.Collections;

public class ActorHealth : MonoBehaviour {

	public delegate void Death();
	public event Death onDeath;
	public delegate void Flinch();
	public event Flinch onFlinch;
	
	// how much health the object should start with
	public float startHealth = 100;
	public float attackImmunityTime = .5f;
	public float HealthAmount { get { return health; } }
	public float criticalHealthThreshold = .15f;
	public float health;
	private float lastHitTime;

	public float flinchLimit = 100f;
	public float FlinchAmount { get { return flinchPoints; } }
	private float flinchPoints;



	public void Start() {
		Actor actor = GetComponent<Actor>();
		if(actor)
			startHealth = actor.combatStats.health.combatValue;

		if(actor.monsterData != null)
			flinchLimit = actor.data.flinchLimit;

		flinchPoints = 0f;
		health = startHealth;
	}

	public void Update() {
		if (health <= 0) {
			health = 0;
			// fire an event when the attached object is dead
			if (onDeath != null) {
				onDeath();
				onDeath = null;
			}
		}
	}

	
	// the attached object has been damaged
	public void TakeDamage(float amount) {
		if(Immune())
			return;
		lastHitTime = Time.time;
		TakePhysicalDamage(amount);
		TakeFlinchDamage(amount);
	}


	public void TakePhysicalDamage(float amount) {
		health -= amount;
		// don't let the health go below zero
	}

	public void TakeFlinchDamage(float amount) {
		flinchPoints += amount;
		if (flinchPoints >= flinchLimit) {
			flinchPoints = 0f;
			// fire an event when the attached object flinches
			if (onFlinch != null && IsAlive()) {
				onFlinch();
			}
		}
	}

	public bool IsAlive() { return health > 0; }

	public bool Immune() { return (Time.time <= lastHitTime + attackImmunityTime); }
}

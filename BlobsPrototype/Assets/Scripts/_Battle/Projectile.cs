using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public GameObject explosion;		// Prefab of explosion effect.
	public Actor owner;
	public float damage = 1f;
	private float actorAttackValue = 100;


	void Start ()  {
		// Destroy the rocket after 2 seconds if it doesn't get destroyed before then.
		Destroy(gameObject, 3);

		if(owner.monsterData != null)
			actorAttackValue = owner.data.attack;
		else
			actorAttackValue = owner.combatStats.attack.combatValue;
	}

	public void Awake() {
	}


	void OnExplode() {
		// Create a quaternion with a random rotation in the z-axis.
		Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

		// Instantiate the explosion where the rocket is with the random rotation.
		Instantiate(explosion, transform.position, randomRotation);
	}

	void OnTriggerEnter (Collider col) {
		if(owner != null && col.tag == owner.tag)
			return;
		
		ActorHealth health = col.gameObject.GetComponent<ActorHealth>();
		if(health == null)
			return;

		float modifiedDamage = damage * actorAttackValue *.1f; 
		if(modifiedDamage > 0 && health != null)
			health.TakeDamage(modifiedDamage);


		// Call the explosion instantiation.
		if(explosion != null)
			OnExplode();

		// Destroy the rocket.
		Destroy (gameObject);
	}
}

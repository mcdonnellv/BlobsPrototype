using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ActorAttack : MonoBehaviour {

	public float damage = 10f;
	public float flinchPoints = 10f;
	public float staminaConsumption = 10f;
	public List<string> validTargetTags;
	public Actor actor;
	public float force = 100f;
	private float actorAttackValue = 100;

	public void Start() {
	}

	public void Awake() {
		if(actor == null)
			actor = GetComponentInParent<Actor>();
		if(actor.monsterData != null)
			actorAttackValue = actor.data.attack;
		else
			actorAttackValue = actor.combatStats.attack.combatValue;
	}

	void OnTriggerStay (Collider col) {
		foreach(string tag in validTargetTags) {
			if(col.gameObject.tag == tag) {
				ActorHealth health = col.gameObject.GetComponent<ActorHealth>();

				if(health.Immune() == false) {
					Actor victim = col.gameObject.GetComponent<Actor>();
					victim.AddForce((actor.IsFacingRight() ? new Vector2(.5f, .5f) : new Vector2(-.5f, .5f)) * force);
				}

				float modifiedDamage = (damage / 10f) * actorAttackValue; 
				if(modifiedDamage > 0 && health != null)
					health.TakeDamage(modifiedDamage);
				return;
			}
		}
	}
}

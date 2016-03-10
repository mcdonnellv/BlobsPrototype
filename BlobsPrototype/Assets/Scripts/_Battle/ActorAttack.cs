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
	public bool applyDamageOncePerAttack = false;

	private bool damageApplied = false;
	private float actorAttackValue = 100;

	public void Start() {
	}

	void OnEnable() {
		damageApplied = false;
	}

	public void Awake() {
		if(actor == null)
			actor = GetComponentInParent<Actor>();
		if(actor.monsterData != null)
			actorAttackValue = actor.data.attack;
		else
			actorAttackValue = actor.combatStats.attack.combatValue;
	}


	void OnCollisionStay(Collision collision) {
		Collider col = collision.collider;
		foreach(string tag in validTargetTags)
			if(col.gameObject.tag == tag)
				ApplyDamage(col);
	}

		void OnTriggerStay (Collider col) {
		foreach(string tag in validTargetTags) 
			if(col.gameObject.tag == tag)
				ApplyDamage(col);
	}

	void ApplyDamage(Collider col) {
		if(applyDamageOncePerAttack && damageApplied)
			return;

		ActorHealth health = col.gameObject.GetComponent<ActorHealth>();
		if(health == null)
			return;
		
		damageApplied = true;
		float modifiedDamage = (damage / 10f) * actorAttackValue; 
		if(modifiedDamage > 0 && health != null)
			health.TakeDamage(modifiedDamage);
	}
}

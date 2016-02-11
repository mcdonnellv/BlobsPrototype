using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ActorAttack : MonoBehaviour {

	public float damage = 10f;
	public float flinchPoints = 10f;
	public float staminaConsumption = 10f;
	public List<string> validTargetTags;
	public float hitboxDuration = .3f;

	private float hitboxSpawnTime;

	public void Start() {
		hitboxSpawnTime = Time.time;
	}

	public void Update() {
		if(Time.time >= hitboxSpawnTime + hitboxDuration)
			Destroy(gameObject);
	}

	void OnTriggerEnter2D (Collider2D col) {
	}

	void OnTriggerStay2D (Collider2D col) {
		foreach(string tag in validTargetTags) {
			if(col.gameObject.tag == tag) {
				ActorHealth health = col.gameObject.GetComponent<ActorHealth>();
				if(damage > 0 && health != null)
					health.TakeDamage(damage);
				return;
			}
		}
	}

	void OnTriggerExit2D (Collider2D col) {
	}
}

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ActorAttack : MonoBehaviour {

	public float damage;
	public float lifetime;
	public List<string> opposingFactionTags;
	private float spawntime;

	public void Start() {
		spawntime = Time.time;
	}

	public void Update() {
		if(Time.time >= spawntime + lifetime)
			Destroy(gameObject);
	}

	void OnTriggerEnter2D (Collider2D col) {
	}

	void OnTriggerStay2D (Collider2D col) {
		foreach(string tag in opposingFactionTags) {
			if(col.gameObject.tag == tag) {
				col.gameObject.GetComponent<ActorHealth>().TakeDamage(damage);
				return;
			}
		}
	}

	void OnTriggerExit2D (Collider2D col) {
	}
}

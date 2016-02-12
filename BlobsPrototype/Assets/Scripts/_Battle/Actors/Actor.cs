using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;

public class Actor : MonoBehaviour {



	public CombatStats combatStats;
	public List<ActorAttack> attackPrefabs;
	public int attackIndex = 0;
	protected BehaviorTree behaviorTree;
	protected ActorHealth health;
	protected Animator anim;

	// Use this for initialization
	public virtual void Start () {
		behaviorTree = GetComponent<BehaviorTree>();
		health = GetComponent<ActorHealth>();
		anim = GetComponent<Animator>();

		if(health != null) {
			health.onDeath += Death;
			health.onFlinch += Flinch;
		}
	}
	
	// Update is called once per frame
	public virtual void Update () {
	}
		
	// called via animation event
	public virtual ActorAttack SpawnAttackBox() {
		ActorAttack prefab = attackPrefabs[attackIndex];
		ActorAttack attackBox = (ActorAttack)Instantiate(prefab, transform.position, Quaternion.identity);
		attackBox.transform.parent = transform;
		return attackBox;
	}

	public void Death() {
		if(behaviorTree)
			behaviorTree.DisableBehavior();

		// trigger the death animation
		anim.SetTrigger("Death");
	}

	public void Flinch() {
		// trigger the flinch animation
		anim.SetTrigger("Flinch");
	}

	public bool IsAlive() {
		ActorHealth ac = GetComponent<ActorHealth>();
		if(ac == null) 
			return false;
		return ac.IsAlive();
	}

	// called by death animation
	public void DestroySelf() {
		Destroy(gameObject);
	}
}

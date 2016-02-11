using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;

public class Actor : MonoBehaviour {



	public CombatStats combatStats;
	public ActorAttack attackPrefab;
	protected BehaviorTree behaviorTree;

	// Use this for initialization
	public virtual void Start () {
		behaviorTree = GetComponent<BehaviorTree>();
	}
	
	// Update is called once per frame
	public virtual void Update () {
	}

	public ActorAttack SpawnAttackBox(float size){
		ActorAttack attackBox = (ActorAttack)Instantiate(attackPrefab, transform.position, Quaternion.identity);
		BoxCollider2D b2d = attackBox.GetComponent<BoxCollider2D>();
		b2d.size = new Vector2(size, size);
		attackBox.transform.parent = transform;
		return attackBox;
	}

	public bool IsAlive() {
		ActorHealth ac = GetComponent<ActorHealth>();
		if(ac == null) 
			return false;
		return ac.IsAlive();
	}
}

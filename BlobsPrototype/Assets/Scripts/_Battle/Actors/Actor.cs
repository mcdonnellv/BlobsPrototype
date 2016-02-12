using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;

public class Actor : MonoBehaviour {



	public CombatStats combatStats;
	public int monsterId = -1;
	public BaseMonster monsterData { get; protected set; }
	protected BehaviorTree behaviorTree;
	protected ActorHealth health;
	protected Animator anim;
	private Dictionary<string, ActorAttack> attacks;

	public void Awake () {
		if(monsterId >= 0)
			monsterData = MonsterManager.monsterManager.GetBaseMonsterByID(monsterId);
		else
			monsterData = null;
	}

	// Use this for initialization
	public virtual void Start () {
		behaviorTree = GetComponent<BehaviorTree>();
		health = GetComponent<ActorHealth>();
		anim = GetComponent<Animator>();
		attacks = new Dictionary<string, ActorAttack>();

		if(health != null) {
			health.onDeath += Death;
			health.onFlinch += Flinch;
		}
	}
	
	// Update is called once per frame
	public virtual void Update () {
		anim.SetBool("Walk", false);
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

	public void ApplyForceY(float y) {
		Rigidbody2D myRigidBody = GetComponent<Rigidbody2D>();
		myRigidBody.AddForce(new Vector2(0,y));
	}

	public void ApplyForceX(float x) {
		bool facingRight = (transform.localRotation.y == 0);
		if(!facingRight)
			x *= -1;
		Rigidbody2D myRigidBody = GetComponent<Rigidbody2D>();
		myRigidBody.AddForce(new Vector2(x,0));
	}
}

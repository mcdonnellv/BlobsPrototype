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
	protected Rigidbody2D rigidBody;
	protected float groundDistance = 0.7f;

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

		if(health != null) {
			health.onDeath += Death;
			health.onFlinch += Flinch;
		}

		rigidBody = GetComponent<Rigidbody2D>();
		if(rigidBody == null) {
			UnityJellySprite ujs = GetComponent<UnityJellySprite>();
			rigidBody = ujs.CentralPoint.Body2D;
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
		rigidBody.AddForce(new Vector2(0, y));
	}

	public void ApplyForceX(float x) {
		bool facingRight = (transform.localRotation.y == 0);
		if(!facingRight)
			x *= -1;
		rigidBody.AddForce(new Vector2(x, 0));
	}


	public bool IsGrounded () {
		Ray2D ray = new Ray2D(transform.position, -Vector3.up * (groundDistance + 0.1f));
		Debug.DrawRay(ray.origin, ray.direction, Color.red);
		RaycastHit2D[] hit = Physics2D.RaycastAll(ray.origin, ray.direction);

		for(int i=0; i < hit.Length; i++) 
			if(hit[i].collider.tag == "Floor")
				return true;

		return false;
	}
}

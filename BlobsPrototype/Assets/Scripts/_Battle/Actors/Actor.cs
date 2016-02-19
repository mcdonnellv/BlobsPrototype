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
	protected bool groundCheckDone = false;
	protected bool isGrounded = false;
	public Rigidbody rigidBody { get; protected set; }



	public virtual void Awake () {
		behaviorTree = GetComponent<BehaviorTree>();
		health = GetComponent<ActorHealth>();
		anim = GetComponent<Animator>();
		rigidBody = GetComponent<Rigidbody>();
		
		if(monsterId >= 0)
			monsterData = MonsterManager.monsterManager.GetBaseMonsterByID(monsterId);
		else
			monsterData = null;
	}

	// Use this for initialization
	public virtual void Start () {
		if(health != null) {
			health.onDeath += Death;
			health.onFlinch += Flinch;
		}
	}
	
	// Update is called once per frame
	public virtual void Update () {
		//anim.SetBool("Walk", false);
		groundCheckDone = false;
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

	public virtual void AddForce(Vector2 v) {
		rigidBody.AddForce(v);
	}


	public virtual bool IsGrounded () {
		if(groundCheckDone)
			return isGrounded;
		
		groundCheckDone = true;
		isGrounded = false;
		Ray ray = new Ray(transform.position, -Vector3.up);
		int groundLayer = 11;
		int layerMask = 1 << groundLayer;
		float groundDistance = 0.7f;
		RaycastHit[] hit = Physics.RaycastAll(ray.origin, ray.direction, groundDistance + 0.1f, layerMask);
		isGrounded = (hit.Length > 0);
		return isGrounded;
	}


	public virtual bool IsFacingRight() {
		return transform.rotation.y == 0;
	}

	public virtual void FaceOpposite() {
		transform.localRotation = Quaternion.Euler(0, IsFacingRight() ? 180 : 0, 0);
	}
}

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;


public class Actor : MonoBehaviour {
	public delegate void Collided(Collider other);
	public event Collided onCollided;

	public CombatStats combatStats;
	public int monsterId = -1;
	public BaseMonster monsterData { get; protected set; }
	public Rigidbody rigidBody { get; protected set; }
	public ActorData data;
	public ActorHealth health { get; protected set; }

	protected BehaviorTree behaviorTree;
	protected Animator anim;
	protected bool groundCheckDone = false;
	protected bool isGrounded = false;
	public Puppet2D_GlobalControl puppet2d;
	public JellySprite jellySprite;
	protected bool alwaysUpdateJoints = false;
	protected bool reparantedRefPoints = false;
	protected bool dead = false;

	public void SetBehaviorSharedVariable(string name, object value) {
		var sharedVariable = (SharedVariable)behaviorTree.GetVariable(name);
		sharedVariable.SetValue(value);
	}

	public object GetBehaviorSharedVariable(string name) {
		var sharedVariable = (SharedVariable)behaviorTree.GetVariable(name);
		return sharedVariable.GetValue();
	}

	public virtual void Awake () {
		behaviorTree = GetComponent<BehaviorTree>();
		health = GetComponent<ActorHealth>();
		anim = GetComponent<Animator>();
		rigidBody = GetComponent<Rigidbody>();

		monsterData = null;
		if(monsterId != -1) {
			monsterData = MonsterManager.monsterManager.GetBaseMonsterByID(monsterId);
			data = monsterData.data;
		}
		else
			data.opposingFaction = "Enemy";
	}

	// Use this for initialization
	public virtual void Start () {
		if(health != null) {
			health.onDeath += Death;
			health.onFlinch += Flinch;
		}
	}

	void FixedUpdate () {
		if(dead)
			return;
		float speed = Mathf.Abs(rigidBody.velocity.x);
		anim.SetFloat("Speed", speed);
		anim.SetBool("Grounded", IsGrounded());
	}

	// Update is called once per frame
	public void Update () {
		groundCheckDone = false;
		if(alwaysUpdateJoints)
			jellySprite.UpdateJoints();
	}


	public void Death() {
		if(behaviorTree)
			behaviorTree.DisableBehavior();
		dead = true;
		// trigger the death animation
		anim.SetTrigger("Death");
	}


	public void Flinch() {
		// trigger the flinch animation
		anim.SetTrigger("Flinch");
	}


	// called by death animation
	public void DestroySelf() {
		Destroy(gameObject);
	}


	public void AddForce(Vector2 v) {
 		rigidBody.AddForce(v);
	}

	public void AddVerticalForce(float f) {
		AddForce(Vector2.up * f);
	}

	public void AnimJumpEvent() {
		rigidBody.AddForce(Vector3.up * data.jumpForce);
	}


	public void OnCollisionStay(Collision collision) {
		if (onCollided != null && collision.collider.tag != "Ground") {
			onCollided(collision.collider);
		}
	}

	public void OnTriggerStay(Collider other) {
		if (onCollided != null && GetComponent<Collider>().tag != "Ground") {
			onCollided(other);
		}
	}

	public bool IsGrounded () {
		if(groundCheckDone)
			return isGrounded;
		
		groundCheckDone = true;
		isGrounded = false;
		Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
		float groundDistance = 1f;
		RaycastHit[] hit = Physics.RaycastAll(ray.origin, ray.direction, groundDistance + 0.1f);
		for(int i = 0; i < hit.Length; i++) {
			if( hit[i].collider.tag == "Ground")
				isGrounded = true;		
		}
		//Debug.DrawLine(ray.origin, ray.origin + (ray.direction * (groundDistance + 0.1f)), Color.red);
		return isGrounded;
	}


	public virtual bool IsFacingRight() {
		if(puppet2d != null) 
			return puppet2d.flip == true;
		if(jellySprite != null)
			return jellySprite.m_FlipX == false;
		return transform.rotation.y == 0;
	}


	public virtual void FaceOpposite() {
		if(puppet2d != null) 
			puppet2d.flip = !puppet2d.flip;
		else if(jellySprite != null)
			jellySprite.m_FlipX = !jellySprite.m_FlipX;
		else
			transform.localRotation = Quaternion.Euler(0, IsFacingRight() ? 180 : 0, 0);
	}
}

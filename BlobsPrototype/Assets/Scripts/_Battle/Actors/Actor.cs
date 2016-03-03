using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;


public class Actor : MonoBehaviour {
	public CombatStats combatStats;
	public int monsterId = -1;
	public BaseMonster monsterData { get; protected set; }
	public Rigidbody rigidBody { get; protected set; }
	public ActorData data;

	protected BehaviorTree behaviorTree;
	protected ActorHealth health;
	protected Animator anim;
	protected bool groundCheckDone = false;
	protected bool isGrounded = false;

	public void SetTarget(GameObject targetObject) {
		var targetVariable = (SharedGameObject)behaviorTree.GetVariable("Target");
		targetVariable.Value = targetObject;
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
		anim.SetFloat("Speed", GetCurSpeed());
		anim.SetBool("Grounded", IsGrounded());
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

	public void AnimJumpEvent() {
		rigidBody.AddForce(Vector3.up * data.jumpForce);
	}


	public virtual bool IsGrounded () {
		if(groundCheckDone)
			return isGrounded;
		
		groundCheckDone = true;
		isGrounded = false;
		Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
		int groundLayer = 14;
		int layerMask = 1 << groundLayer;
		float groundDistance = 1.7f;
		RaycastHit[] hit = Physics.RaycastAll(ray.origin, ray.direction, groundDistance + 0.1f);
		isGrounded = (hit.Length > 0);
		Debug.DrawLine(ray.origin, ray.origin + (ray.direction * (groundDistance + 0.1f)), Color.red);
		return isGrounded;
	}


	public virtual bool IsFacingRight() {
		Puppet2D_GlobalControl p2dCtrl = GetComponent<Puppet2D_GlobalControl>();
		if(p2dCtrl != null) 
			return p2dCtrl.flip == true;
		else
			return transform.rotation.y == 0;
	}


	public virtual void FaceOpposite() {
		Puppet2D_GlobalControl p2dCtrl = GetComponent<Puppet2D_GlobalControl>();
		if(p2dCtrl != null) 
			p2dCtrl.flip = !p2dCtrl.flip;
		else
			transform.localRotation = Quaternion.Euler(0, IsFacingRight() ? 180 : 0, 0);
	}


	public float GetCurSpeed() {
		return rigidBody.velocity.magnitude;
	}
}

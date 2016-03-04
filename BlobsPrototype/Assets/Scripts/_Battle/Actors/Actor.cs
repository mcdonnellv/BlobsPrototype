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
	protected Puppet2D_GlobalControl p2dCtrl;
	protected JellySprite jellySprite;
	protected bool alwaysUpdateJoints = false;
	protected bool reparantedRefPoints = false;


	public void SetTarget(GameObject targetObject) {
		var targetVariable = (SharedGameObject)behaviorTree.GetVariable("Target");
		targetVariable.Value = targetObject;
	}

	public virtual void Awake () {
		behaviorTree = GetComponent<BehaviorTree>();
		health = GetComponent<ActorHealth>();
		anim = GetComponent<Animator>();
		rigidBody = GetComponent<Rigidbody>();
		p2dCtrl = GetComponent<Puppet2D_GlobalControl>();
		jellySprite = GetComponentInChildren<JellySprite>();

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

	
	// Update is called once per frame
	public virtual void Update () {
		anim.SetFloat("Speed", GetCurSpeed());
		anim.SetBool("Grounded", IsGrounded());
		groundCheckDone = false;

		if(jellySprite != null && reparantedRefPoints == false && jellySprite.CentralPoint != null) {
			reparantedRefPoints = true;
			jellySprite.CentralPoint.Body3D.isKinematic = true;
			Transform refpoints = jellySprite.CentralPoint.transform.parent;
			refpoints.parent = jellySprite.transform;

			// tag all colliders as blob
			Collider[] cols = gameObject.GetComponentsInChildren<Collider>();
			for(int i = 0; i < cols.Length; i++)
				cols[i].gameObject.tag = "Blob";
		}

		if(alwaysUpdateJoints)
			jellySprite.UpdateJoints();
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


	public void OnCollisionEnter(Collision collision) {
		if (onCollided != null) {
			onCollided(collision.collider);
		}
	}

	public void OnTriggerEnter(Collider other) {
		if (onCollided != null) {
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
		if(p2dCtrl != null) 
			return p2dCtrl.flip == true;
		if(jellySprite != null)
			return jellySprite.m_FlipX == false;
		return transform.rotation.y == 0;
	}


	public virtual void FaceOpposite() {
		if(p2dCtrl != null) 
			p2dCtrl.flip = !p2dCtrl.flip;
		else if(jellySprite != null)
			jellySprite.m_FlipX = !jellySprite.m_FlipX;
		else
			transform.localRotation = Quaternion.Euler(0, IsFacingRight() ? 180 : 0, 0);
	}


	public float GetCurSpeed() {
		return rigidBody.velocity.magnitude;
	}
}

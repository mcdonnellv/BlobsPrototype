using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;

[Serializable]
public struct BoneReferencePoint {
	public string name;
	public Transform transform;
}

public class Actor : MonoBehaviour {
	public delegate void Collided(Collider other);
	public event Collided onCollided;

	public CombatStats combatStats;
	public int monsterId = -1;
	public BaseMonster monsterData { get; protected set; }
	public Rigidbody rigidBody { get; protected set; }
	public ActorData data;
	public ActorHealth health { get; protected set; }
	public BattleRole battleRole { get; protected set; }
	public BoneReferencePoint[] boneReferencePoints;

	protected BehaviorTree behaviorTree;
	protected Animator anim;
	protected bool groundCheckDone = false;
	protected bool isGrounded = false;
	public Puppet2D_GlobalControl puppet2d;
	public JellySprite jellySprite;
	protected bool alwaysUpdateJoints = false;
	protected bool reparantedRefPoints = false;
	protected bool dead = false;

	public Rigidbody projectile;				// Prefab of the rocket.

	public void ShootProjectile(float angle) {
		GameObject target = (GameObject)GetBehaviorSharedVariable("Target");
		Vector3 startPos = transform.position + Vector3.up * 2f;
		for (int i = 0; i < boneReferencePoints.Length; i++) {
			BoneReferencePoint brp = boneReferencePoints[i];
				if(brp.name == "mouth")
					startPos = brp.transform.position;
		}

		Vector3 targetPos = transform.position + Vector3.right * 10f;

		if(target != null) {
			Collider[] cols = target.GetComponents<Collider>();
			Collider col = cols[0];
			for(int i = 1; i < cols.Length; i++) {
				if(col.bounds.size.sqrMagnitude < cols[i].bounds.size.sqrMagnitude)
					col = cols[i];
			}
			targetPos = col.bounds.center;
		}
		Projectile p = AiManager.ShootProjectile(projectile, startPos, targetPos, angle, 30f);
		p.owner = this;
	}


	public void SetBattleRole(BattleRole role) {
		battleRole = role;
		SetBehaviorSharedVariable("role", (int)role);
	}

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
		CombatManager.combatManager.onBeat += Beat;
	}
		

	void Beat() {
		if(anim!= null)
			anim.SetTrigger("Beat");
	}

	void FixedUpdate () {
		if(dead)
			return;
		float speed = Mathf.Abs(rigidBody.velocity.x);
		anim.SetFloat("Speed", speed);
		anim.SetBool("Grounded", IsGrounded());
		anim.ResetTrigger("Beat");
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
		CombatManager.combatManager.onBeat -= Beat;
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
		Ray ray = new Ray(transform.position + (Vector3.up * .1f), -Vector3.up);
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

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;

public class Actor : MonoBehaviour {



	public CombatStats combatStats;
	public int monsterId = -1;
	public BaseMonster monsterData { get; protected set; }
	public UnityJellySprite jellySprite { get; protected set; }
	private Rigidbody2D _rigidBody;
	public Rigidbody2D rigidBody { 
		get { 
			if(jellySprite != null && jellySprite.CentralPoint != null) 
				return jellySprite.CentralPoint.Body2D;
			else
				return _rigidBody;
		} 
		protected set { _rigidBody = value; } 
	}
	protected BehaviorTree behaviorTree;
	protected ActorHealth health;
	protected Animator anim;
	protected float groundDistance = 0.7f;
	protected bool groundCheckDone = false;
	protected bool isGrounded = false;



	public void Awake () {
		jellySprite = GetComponent<UnityJellySprite>();
		behaviorTree = GetComponent<BehaviorTree>();
		health = GetComponent<ActorHealth>();
		anim = GetComponent<Animator>();
		rigidBody = GetComponent<Rigidbody2D>();
		
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
		anim.SetBool("Walk", false);
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

	public void ApplyForceY(float y) {
		rigidBody.AddForce(new Vector2(0, y));
	}

	public void ApplyForceX(float x) {
		bool facingRight = AiManager.IsFacingRight(this);
		if(!facingRight)
			x *= -1;
		rigidBody.AddForce(new Vector2(x, 0));
	}


	public bool IsGrounded () {
		if(groundCheckDone)
			return isGrounded;
		
		groundCheckDone = true;
		Ray2D ray = new Ray2D(transform.position, -Vector3.up * (groundDistance + 0.1f));
		//Debug.DrawRay(ray.origin, ray.direction, Color.red);
		RaycastHit2D[] hit = Physics2D.RaycastAll(ray.origin, ray.direction);
		isGrounded = false;
		for(int i=0; i < hit.Length; i++) {
				if(hit[i].collider.tag == "Floor") {
					isGrounded = true;
				}
			}
		return isGrounded;
	}

	public void Jump(float force) {
		if(jellySprite != null && jellySprite.ReferencePoints.Count >= 3) {
			jellySprite.ReferencePoints[1].Body2D.AddForce(new Vector2(0, force/2));
			jellySprite.ReferencePoints[2].Body2D.AddForce(new Vector2(0, force/2));
		}
	}

	public void Deflate() {
		if(jellySprite != null) {
			jellySprite.ReferencePoints[1].InitialOffset = new Vector2(1,0);
			jellySprite.ReferencePoints[2].InitialOffset = new Vector2(-1,0);
			jellySprite.m_Stiffness = 0f;
			jellySprite.UpdateJoints();

		}
	}

	public void Inflate() {
		if(jellySprite != null) {
			jellySprite.ReferencePoints[1].InitialOffset = new Vector2(.5f,0);
			jellySprite.ReferencePoints[2].InitialOffset = new Vector2(-.5f,0);
			jellySprite.m_Stiffness = 2.50f;
			jellySprite.UpdateJoints();

		}
	}
}

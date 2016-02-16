using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlobActor : Actor {
	public float jumpForce = 50;
	public float moveForce = 50;
	public float walkSpeed = 5;
	private UnityJellySprite jellySprite;
	public override Rigidbody2D rigidBody { 
		get { if (jellySprite != null && jellySprite.CentralPoint != null) return jellySprite.CentralPoint.Body2D; return null; }
	}
	private float lastJump;
	public float jumpWait = .5f;

	public override void Awake () {
		jellySprite = GetComponent<UnityJellySprite>();
		base.Awake();
	}

	// Use this for initialization
	public override void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update();
	}


	void FixedUpdate () {
		float h = Input.GetAxis("Horizontal");
		AiManager.MoveDirection((Actor)this, h, moveForce, walkSpeed);

		float v = Input.GetAxis("Vertical");
		if(v > 0)
			Jump(jumpForce);

	}


	public override bool IsGrounded () {
		int groundLayer = 11;
		int layerMask = 1 << groundLayer;
		return jellySprite.IsGrounded(layerMask,2);
	}

	public override bool IsFacingRight() {
		return !jellySprite.m_FlipX;
	}

	public override void FaceOpposite() {
		jellySprite.m_FlipX = !jellySprite.m_FlipX;
	}

	public void Jump(float force) {
		if(force <= 0 || IsGrounded() == false || (lastJump + jumpWait) > Time.time)
			return;
		lastJump = Time.time;
		//anim.SetTrigger("Jump");
		//return;

		List<JellySprite.ReferencePoint> refPointsTouchingGround = new List<JellySprite.ReferencePoint>();
		int groundLayer = 11;
		foreach(JellySprite.ReferencePoint referencePoint in jellySprite.ReferencePoints) {
			if(referencePoint.Collider2D) {		
				CircleCollider2D circleCollider = referencePoint.Collider2D;
				Vector2 bodyPosition = referencePoint.transform.position;
				if(Physics2D.OverlapCircle(bodyPosition + new Vector2(0, -circleCollider.radius * 0.1f), circleCollider.radius, 1 << groundLayer)) {
					refPointsTouchingGround.Add(referencePoint);
				}
			}
		}

		if(refPointsTouchingGround.Count <= 0)
			return;


		float distributedForce = force / (jellySprite.ReferencePoints.Count - refPointsTouchingGround.Count);
		foreach(JellySprite.ReferencePoint referencePoint in jellySprite.ReferencePoints) {
			if(refPointsTouchingGround.Contains(referencePoint))
				continue;
			referencePoint.Body2D.AddForce(new Vector2(0, distributedForce));
		}
	}

	
	public void SetSpringStiffness(float val) {
		jellySprite.m_Stiffness = val;
		jellySprite.UpdateJoints();
	}

	public void SetGravityScale(float val) {
		jellySprite.m_GravityScale = val;
		jellySprite.UpdateJoints();
	}
}

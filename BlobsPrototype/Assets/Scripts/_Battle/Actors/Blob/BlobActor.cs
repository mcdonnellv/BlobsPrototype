using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlobActor : Actor {
	public bool alwaysUpdateJoints = false;
	public JellySprite jellySprite;
	private bool reparantedRefPoints = false;
	private Rigidbody body;


	public override void Awake () {
		base.Awake();
		data.opposingFaction = "Enemy";
	}

	// Use this for initialization
	public override void Start () {
		base.Start();
		body = GetComponent<Rigidbody>();
	}

	
	// Update is called once per frame
	public override void Update () {
		
		if(reparantedRefPoints == false && jellySprite.CentralPoint != null) {
			reparantedRefPoints = true;
			jellySprite.CentralPoint.Body3D.isKinematic = true;
			Transform refpoints = jellySprite.CentralPoint.transform.parent;
			refpoints.parent = jellySprite.transform;
		}

		if(alwaysUpdateJoints)
			jellySprite.UpdateJoints();
		base.Update();
	}


	public override bool IsGrounded () {
		int groundLayer = 11;
		int layerMask = (1 << groundLayer);
		return jellySprite.IsGrounded(layerMask,1);
	}


	public override bool IsFacingRight() {
		return !jellySprite.m_FlipX;
	}


	public override void FaceOpposite() {
		jellySprite.m_FlipX = !jellySprite.m_FlipX;
	}


	public override void AddForce(Vector2 f) {
		//jellySprite.AddForce(f);
		body.AddForce(f);
	}


	public void AddVerticalForce(float f) {
		AddForce(Vector2.up * f);
	}


	private List<JellySprite.ReferencePoint> GetRefPointsTouchingGround() {
		List<JellySprite.ReferencePoint> refPointsTouchingGround = new List<JellySprite.ReferencePoint>();
		int groundLayer = 11;
		foreach(JellySprite.ReferencePoint referencePoint in jellySprite.ReferencePoints) {
			if(referencePoint.Collider) {		
				SphereCollider sphereCollider = referencePoint.Collider;
				Vector2 bodyPosition = referencePoint.transform.position;
				Collider[] col = Physics.OverlapSphere(bodyPosition + new Vector2(0, -sphereCollider.radius * 0.1f), sphereCollider.radius, 1 << groundLayer);
				if(col.Length > 0) 
					refPointsTouchingGround.Add(referencePoint);
			}
		}
		return refPointsTouchingGround;
	}
		
	


}

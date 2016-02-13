using UnityEngine;
using System.Collections;

public class BlobActor : Actor {
	float moveForce = 100;
	float walkSpeed = 2f;
	private Rigidbody2D rb2d;

	// Use this for initialization
	public override void Start () {
		UnityJellySprite js = GetComponent<UnityJellySprite>();
		rb2d = js.CentralPoint.Body2D;
		base.Start();
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update();
	}


	void FixedUpdate () {
		float h = Input.GetAxis("Horizontal");
		if(h != 0)
			AiManager.MoveDirection(h, rb2d, moveForce, walkSpeed, IsGrounded());
	}
}

using UnityEngine;
using System.Collections;

public class BlobActor : Actor {
	float moveForce = 100;
	float walkSpeed = 2f;

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
	}
}

using UnityEngine;
using System.Collections;

public class BlobActor : Actor {
	float moveForce = 100;

	// Use this for initialization
	public override void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update();
	}

	// This function is called by the animation system on the frame of the attack 
	public ActorAttack SpawnAttackBox(float size) {
		ActorAttack aa = base.SpawnAttackBox(size);
		aa.damage = combatStats.attack.combatValue / 10f;
		aa.opposingFactionTags.Add("Enemy");
		aa.transform.position += new Vector3(0f, .5f, 0f) + (transform.right * 1);
		return aa;
	}

	void FixedUpdate ()
	{
		// Cache the horizontal input.
		float h = Input.GetAxis("Horizontal");

		// The Speed animator parameter is set to the absolute value of the horizontal input.
		//anim.SetFloat("Speed", Mathf.Abs(h));

		// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
		if(h * GetComponent<Rigidbody2D>().velocity.x < walkSpeed)
			// ... add a force to the player.
			GetComponent<Rigidbody2D>().AddForce(Vector2.right * h * moveForce);

		// If the player's horizontal velocity is greater than the maxSpeed...
		if(Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x) > walkSpeed)
			// ... set the player's velocity to the maxSpeed in the x axis.
			GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Sign(GetComponent<Rigidbody2D>().velocity.x) * walkSpeed, GetComponent<Rigidbody2D>().velocity.y);
	}
}

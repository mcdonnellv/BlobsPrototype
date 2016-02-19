using UnityEngine;
using System.Collections;

public class PlayerControlable : MonoBehaviour {
	public float jumpForce = 50;
	public float moveForce = 50;
	public float walkSpeed = 5;
	public float jumpWait = 0;

	private Actor actor;
	private Animator anim;
	private Rigidbody body;
	private float lastJump = 0;
	private bool reparantedRefPoints = false;


	public void Start () {
		actor = GetComponent<Actor>();
		anim = GetComponent<Animator>();
		body = GetComponent<Rigidbody>();
	}


	void FixedUpdate () {

		float h = Input.GetAxis("Horizontal");
		if(h != 0) {
			AiManager.MoveDirection(body, h, actor.IsGrounded() ? moveForce : moveForce / 2f, walkSpeed);
			anim.SetBool("Walk", true);
		}
		else anim.SetBool("Walk", false);

		if(Input.GetButton("Jump"))
			TryJump();
	}


	void TryJump() {
		if(actor.IsGrounded() == false || anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "BlobJump")
			return;
 		anim.SetTrigger("Jump");
	} 

	public void AnimJumpEvent() {
		body.AddForce(Vector3.up * jumpForce);
	}
}

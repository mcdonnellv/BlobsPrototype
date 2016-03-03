using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;

public class AiJumpTo : Action {
	public SharedVector3 targetPosition;
	public SharedFloat initialAngle;

	private Actor actor;
	private float jumpTime;
	private float pauseTime = 1f;

	// Use this for initialization
	public override void OnAwake() {
		actor = GetComponent<Actor>();
	}

	public override void OnStart() {
		TriggerUsingForce();
		jumpTime = Time.time;
	}
	
	// Update is called once per frame
	public override TaskStatus OnUpdate() {
		if(Time.time > jumpTime + pauseTime && actor.IsGrounded()) {
			// simply snap to exact destination once we land 
			actor.transform.position = targetPosition.Value;
			return TaskStatus.Success;
		}

		return TaskStatus.Running;
	}


	public void TriggerUsingForce() {
		var rigid = actor.rigidBody;

		Vector3 p = targetPosition.Value;
		float gravity = Physics.gravity.magnitude;

		// Selected angle in radians
		float angle = initialAngle.Value * Mathf.Deg2Rad;

		// Positions of this object and the target on the same plane
		Vector3 planarTarget = new Vector3(p.x, 0, p.z);
		Vector3 planarPostion = new Vector3(transform.position.x, 0, transform.position.z);

		// Planar distance between objects
		float distance = Vector3.Distance(planarTarget, planarPostion);
		
		// Distance along the y axis between objects
		float yOffset = transform.position.y - p.y;

		float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));

		Vector3 velocity = new Vector3(initialVelocity * Mathf.Cos(angle), initialVelocity * Mathf.Sin(angle), 0);

		// Rotate our velocity to match the direction between the two objects
		float angleBetweenObjects = Vector3.Angle(Vector3.right, planarTarget - planarPostion);
		Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

		// Fire!
		//rigid.velocity = finalVelocity;

		// Alternative way:
		rigid.AddForce(finalVelocity * rigid.mass, ForceMode.Impulse);
	}
}

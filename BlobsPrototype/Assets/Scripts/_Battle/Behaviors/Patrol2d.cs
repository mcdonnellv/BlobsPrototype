using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;

[TaskDescription("Patrol around the specified waypoints using force on a RrigidBody2D")]
[TaskCategory("Movement")]
[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}PatrolIcon.png")]
public class Patrol2d : Patrol {

	private Rigidbody2D rigidBody;
	private Vector2 destTarget;
	public SharedFloat moveForce = 100;
	public SharedFloat slowDownDistance = 10;
	private Vector2 originalDirection;
	private Vector2 curDirection;


	public override TaskStatus OnUpdate() {
		if(destTarget != (Vector2)transform.position && !HasArrived()) {
			bool grounded = Mathf.Abs(rigidBody.velocity.y) <= 0.010f;

			if(grounded) {
				Vector2 direction = destTarget - (Vector2)transform.position;
				direction.y = 0;
				float distance = direction.magnitude;
				curDirection = direction.normalized;
				rigidBody.AddForce(curDirection * moveForce.Value);
				float curSpeed = rigidBody.velocity.magnitude;
				if(distance < slowDownDistance.Value && curSpeed > 1f)
					rigidBody.AddForce(-curDirection * moveForce.Value * (1 - (distance / slowDownDistance.Value)));

				if(curSpeed > speed.Value)
					rigidBody.velocity = curDirection * speed.Value;
			}
		}

		return base.OnUpdate();
	}

	public override void OnAwake() {
		rigidBody = gameObject.GetComponent<Rigidbody2D>();
	}

	public override void OnStart() {
		destTarget = transform.position;
	}

	protected override bool SetDestination(Vector3 target) {
		destTarget = (Vector2)target;
		return true;
	}

	protected override bool HasArrived() {
		bool retval =  (((Vector2)transform.position - destTarget).magnitude < .5f);
		return retval;
	}

	protected override Vector3 Velocity() {
		return rigidBody.velocity;
	}

	public override void OnEnd() {
		rigidBody.velocity = Vector3.zero;
		destTarget = transform.position;
	}
}

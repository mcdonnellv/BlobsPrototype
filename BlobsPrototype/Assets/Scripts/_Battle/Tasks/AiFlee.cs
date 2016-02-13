using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;

public class AiFlee : Movement {

	public SharedGameObject target;
	public SharedFloat fleeSpeed;
	public SharedFloat fleeDistance;
	public SharedFloat moveForce;

	private Vector2 destTarget;
	private Rigidbody2D rigidBody;
	private Actor actor;


	public override void OnAwake() {
		rigidBody = gameObject.GetComponent<Rigidbody2D>();
		if(rigidBody == null)
			rigidBody = gameObject.GetComponent<UnityJellySprite>().CentralPoint.Body2D;
		actor = gameObject.GetComponent<Actor>();
	}

	public override void OnStart() {
		if(target.Value != null) {
			Vector3 dir = target.Value.transform.position - transform.position;
			Vector3 fleeDirection = -dir.normalized;
			SetDestination(transform.position + (fleeDirection * fleeDistance.Value));
		}
	}

	public override TaskStatus OnUpdate() {
		if(target.Value == null || HasArrived()) {
			return TaskStatus.Success;
		}

		AiManager.MoveToDestination(transform, rigidBody, destTarget, moveForce.Value, fleeSpeed.Value, true, "Walk", actor.IsGrounded());
		return TaskStatus.Running;
	}

	protected override bool HasArrived() {
		bool retval =  Mathf.Abs(transform.position.x - destTarget.x) < .5f;
		return retval;
	}

	protected override bool SetDestination(Vector3 target){
		destTarget = (Vector2)target;
		return true;
	}
	
	protected override Vector3 Velocity() {
		return rigidBody.velocity;
	}
}

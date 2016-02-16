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
	private Actor actor;


	public override void OnAwake() {
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
			
		AiManager.MoveToDestination(actor, destTarget, moveForce.Value, fleeSpeed.Value, true, "Walk");
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
		return actor.rigidBody.velocity;
	}
}

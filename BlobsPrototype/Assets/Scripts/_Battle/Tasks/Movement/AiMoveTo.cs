using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;


[TaskDescription("Move towards the specified position. The position can either be specified by a transform or position. If the transform " +
	"is used then the position will not be used.")]
[TaskCategory("Movement")]
[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}MoveTowardsIcon.png")]

public class AiMoveTo : Action {

	public SharedFloat toVel = 5f;
	public SharedFloat maxSpeed = 8f;
	public SharedFloat maxForce = 30f;
	public SharedFloat minForce = 5f;
	public SharedFloat gain = 10f;
	public SharedFloat arriveDistance = 0.01f;
	public SharedGameObject target;
	public SharedVector3 targetPosition;
	protected Actor actor;
	protected bool collided = false;


	public override void OnAwake() {
		actor = gameObject.GetComponent<Actor>();
	}

	public override void OnStart() {
		actor.onCollided += Collided;
	}

	public void Collided(Collider other) {
		if (target != null && target.Value != null) {
			if(other.gameObject == target.Value) {
				collided = true;
			}
		}
	}

	public override TaskStatus OnUpdate() {
		var position = Target();
		if (HasArrived()) {
			actor.onCollided -= Collided;
			if(!collided)
				transform.position = position;
			return TaskStatus.Success;
		}

		AiManager.MoveToPoint(actor, position, toVel.Value, maxSpeed.Value, maxForce.Value, minForce.Value, gain.Value);
		return TaskStatus.Running;
	}


	protected bool HasArrived() {
		Vector3 dist = Target() - transform.position;
		if (Mathf.Abs(dist.x) < arriveDistance.Value)
			return true;

		// also check if we have physical contact with target,
		if (collided == true) 
			return true;
		
		return false;
	}

	protected virtual Vector3 Target() {
		if (target == null || target.Value == null) {
			return targetPosition.Value;
		}
		return target.Value.transform.position;
	}
}



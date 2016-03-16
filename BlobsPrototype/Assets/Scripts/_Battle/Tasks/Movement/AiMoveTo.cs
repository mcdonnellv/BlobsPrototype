using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;


public class AiMoveTo : Action {

	public SharedFloat toVel = 5f;
	public SharedFloat maxSpeed = 8f;
	public SharedFloat maxForce = 30f;
	public SharedFloat minForce = 5f;
	public SharedFloat gain = 10f;
	public SharedFloat arriveDistance = 0.01f;
	public SharedGameObject target;
	public SharedVector3 targetPosition;
	public SharedVector3 targetOffset;
	public SharedBool snapToDestination = true;
	public SharedBool stopOnCollision = true;
	protected Actor actor;
	protected Collider collidedWith;
	protected bool collidedWithTarget = false;


	public override void OnAwake() {
		actor = gameObject.GetComponent<Actor>();
	}

	public override void OnStart() {
		collidedWith = null;
		collidedWithTarget = false;
		actor.onCollided += Collided;
	}

	public void Collided(Collider other) {
		collidedWith = other;
		if (target != null && target.Value != null) {
			if(other.gameObject == target.Value) {
				collidedWithTarget = true;
			}
		}
	}

	public override TaskStatus OnUpdate() {
		var position = Target();
		if (HasArrived()) {
			if(snapToDestination.Value && !collidedWithTarget)
				transform.position = position;
			Cleanup();
			return TaskStatus.Success;
		}

		if(stopOnCollision.Value && collidedWith != null) {
			collidedWith = null;
			return TaskStatus.Failure; //something is blocking my way
		}

		if(!actor.IsGrounded()) {
			return TaskStatus.Running;
		}

		AiManager.MoveToPoint(actor, position, toVel.Value, maxSpeed.Value, maxForce.Value, minForce.Value, gain.Value);
		return TaskStatus.Running;
	}


	protected bool HasArrived() {
		Vector3 dist = Target() - transform.position;
		if (Mathf.Abs(dist.x) < arriveDistance.Value)
			return true;

		// also check if we have physical contact with target,
		if (collidedWithTarget == true) 
			return true;
		
		return false;
	}

	protected virtual Vector3 Target() {
		Vector3 ret;
		if (target == null || target.Value == null)
			ret =  targetPosition.Value;
		else
			ret =  target.Value.transform.position;
		ret += targetOffset.Value;
		ret = new Vector3(ret.x, 0f, 0f); //prune y and z
		return ret;
	}

	protected void Cleanup() {
		actor.onCollided -= Collided;
	}
}



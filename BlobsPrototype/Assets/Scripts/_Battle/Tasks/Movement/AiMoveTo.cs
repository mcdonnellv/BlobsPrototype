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
	public SharedBool snapToDestination = true;
	public SharedBool stopOnCollision = true;
	protected Actor actor;
	protected bool collidedWithSomething = false;
	protected bool collidedWithTarget = false;


	public override void OnAwake() {
		actor = gameObject.GetComponent<Actor>();
	}

	public override void OnStart() {
		collidedWithSomething = false;
		collidedWithTarget = false;
		actor.onCollided += Collided;
	}

	public void Collided(Collider other) {
		collidedWithSomething = true;
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

		if(stopOnCollision.Value && collidedWithSomething) {
			Ray ray = new Ray(transform.position + Vector3.up * .5f, (position - transform.position).normalized * 1.5f);
			Debug.DrawRay(ray.origin, ray.direction);
			var layerMask = ~((1 << 8) | (1 << 9) | (1 << 10) | Physics.IgnoreRaycastLayer);
			if(Physics.Raycast(ray, 5f, layerMask))
				return TaskStatus.Running; //somehting is blocking my way
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

		ret = new Vector3(ret.x, 0f, 0f); //prune y and z
		return ret;
	}

	protected void Cleanup() {
		actor.onCollided -= Collided;
	}
}



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;

public class AiJumpTo : Action {
	public SharedVector3 targetPosition;
	public SharedGameObject target;
	public SharedFloat initialAngle;

	private Actor actor;
	private float jumpTime;
	private float pauseTime = .3f;
	private Collider myCol;

	// Use this for initialization
	public override void OnAwake() {
		actor = GetComponent<Actor>();
		myCol = transform.GetComponent<Collider>();
	}

	public override void OnStart() {
		AiManager.JumpToPoint(actor, Target(), initialAngle.Value);
		jumpTime = Time.time;
	}
	
	// Update is called once per frame
	public override TaskStatus OnUpdate() {
		if(Time.time > jumpTime + pauseTime && actor.IsGrounded()) {
			// simply snap to exact destination once we land 
			//var position = Target();
			//actor.transform.position = position;
			return TaskStatus.Success;
		}

		return TaskStatus.Running;
	}

	protected virtual Vector3 Target() {
		Vector3 ret;
		if (target == null || target.Value == null)
			ret = targetPosition.Value;
		else {
			Collider targetCol = target.Value.GetComponent<Collider>();
			if (targetCol == null || myCol == null)
				ret = target.Value.transform.position;
			else {
				ret = targetCol.ClosestPointOnBounds(transform.position); 
				ret += (transform.position.x > ret.x) ? myCol.bounds.extents : - myCol.bounds.extents;
			}
		}
			
		ret = new Vector3(ret.x, 0f, 0f); //prune y and z
		return ret;
	}
}

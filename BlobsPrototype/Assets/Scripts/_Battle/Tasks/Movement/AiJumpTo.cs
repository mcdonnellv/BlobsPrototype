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
	private float pauseTime = 1f;

	// Use this for initialization
	public override void OnAwake() {
		actor = GetComponent<Actor>();
	}

	public override void OnStart() {
		AiManager.JumpToPoint(actor, Target(), initialAngle.Value);
		jumpTime = Time.time;
	}
	
	// Update is called once per frame
	public override TaskStatus OnUpdate() {
		if(Time.time > jumpTime + pauseTime && actor.IsGrounded()) {
			// simply snap to exact destination once we land 
			var position = Target();
			actor.transform.position = position;
			return TaskStatus.Success;
		}

		return TaskStatus.Running;
	}

	protected virtual Vector3 Target() {
		if (target == null || target.Value == null) {
			return targetPosition.Value;
		}
		return target.Value.transform.position;
	}
}

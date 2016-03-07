using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;


[TaskCategory("Movement")]

public class AiAtDestination : Conditional {

	public SharedFloat arriveDistance = 0.01f;
	public SharedGameObject target;

	public override TaskStatus OnUpdate() {
		Vector3 dist = Target() - transform.position;
		if (Mathf.Abs(dist.x) < arriveDistance.Value)
			return TaskStatus.Success;
		return TaskStatus.Failure;
	}

	protected virtual Vector3 Target() {
		Vector3 ret = target.Value.transform.position;
		ret = new Vector3(ret.x, 0f, 0f); //prune y and z
		return ret;
	}
}



using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;

public class AiFlee : AiMoveTo {

	public SharedFloat fleeDistance;


	public override void OnAwake() {
		base.OnAwake();
		if (target == null || target.Value == null) {
			Vector3 dir = target.Value.transform.position - transform.position;
			Vector3 fleeDirection = -dir.normalized;
			targetPosition.Value = transform.position + (fleeDirection * fleeDistance.Value);
		}
	}

	public override TaskStatus OnUpdate() {
		return base.OnUpdate();
	}
}

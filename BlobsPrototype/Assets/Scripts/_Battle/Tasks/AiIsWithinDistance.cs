using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;

[TaskCategory("Movement")]
[TaskDescription("Simple Distance check")]
[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}WithinDistanceIcon.png")]
public class AiIsWithinDistance : Conditional {

	public SharedGameObject target;
	public SharedFloat distance;
	public SharedVector3 sourceOffset;
	public SharedVector3 destinationOffset;

	private Actor actor;

	// distance * distance, optimization so we don't have to take the square root
	private float sqrMagnitude;

	public override void OnStart() {
		actor = GetComponent<Actor>();
		sqrMagnitude = distance.Value * distance.Value;
	}


	// returns success if any object is within distance of the current object. Otherwise it will return failure
	public override TaskStatus OnUpdate() {
		if (transform == null || target == null || target.Value == null)
			return TaskStatus.Failure;

		Vector3 direction = (target.Value.transform.position + destinationOffset.Value) - (transform.position + sourceOffset.Value);
		// check to see if the square magnitude is less than what is specified
		if (Vector3.SqrMagnitude(direction) < sqrMagnitude)
			return TaskStatus.Success;

		return TaskStatus.Failure;
	}
}

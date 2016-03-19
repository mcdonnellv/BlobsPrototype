using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;

[TaskCategory("Movement")]
[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}WithinDistanceIcon.png")]
public class AiCanSeeObject : Conditional {
	public SharedBool showGizmo;
	public SharedGameObject target;
	public SharedFloat viewDistance;
	public SharedVector3 offset;
	public SharedFloat fieldOfViewAngle;
	public LayerMask layerMask = -1;

	private Actor actor;

	public override void OnStart() {
		actor = GetComponent<Actor>();
	}

	public override TaskStatus OnUpdate() {
		GameObject retObj = AiManager.WithinSight2D(actor, target.Value, offset.Value, Vector3.zero, fieldOfViewAngle.Value, viewDistance.Value, layerMask);
		if (retObj != null)
			return TaskStatus.Success;
		return TaskStatus.Failure;
	}

	public override void OnDrawGizmos(){
		if(showGizmo.Value && Owner != null) {
			Actor a = Owner.GetComponent<Actor>();
			if(a != null)
				AiManager.DrawLineOfSight2D(a, offset.Value, fieldOfViewAngle.Value, viewDistance.Value);
		}
	}
}

using UnityEngine;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;


public class AiSimAllBlobsWithinDistance : Conditional {

	public Transform anchor;
	public float distance;
	List<Actor> actorList;
	float d;

	public override void OnStart() {
		actorList = CombatManager.combatManager.GetActorsOfTag("Blob", true);
		d = distance * distance;
	}

	public override TaskStatus OnUpdate() {
		foreach(Actor actor in actorList) {
			if ((actor.transform.position - anchor.position).sqrMagnitude > d)
				return TaskStatus.Failure;
		}
		return TaskStatus.Success;
	}
}

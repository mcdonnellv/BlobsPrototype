using UnityEngine;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;


public class AiSimAllBlobsWithinDistance : Conditional {

	public Transform anchor;
	public float distance;
	List<Actor> actorList;

	public override void OnStart() {
		actorList = CombatManager.combatManager.GetActorsOfTag("Blob", true);
	}

	public override TaskStatus OnUpdate() {
		float anchorXpos = anchor.position.x;
		foreach(Actor actor in actorList) {
			if (Mathf.Abs(anchorXpos - actor.transform.position.x) > distance)
				return TaskStatus.Failure;
		}
		return TaskStatus.Success;
	}
}

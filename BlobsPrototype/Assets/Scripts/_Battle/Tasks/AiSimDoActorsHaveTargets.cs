using UnityEngine;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class AiSimDoActorsHaveTargets : Conditional {
	public string actorTag;
	public bool allMustHaveTargets = true;
	public bool onScreenActorsOnly = true;
	List<Actor> actorList;

	public override void OnStart() {
		actorList = CombatManager.combatManager.GetActorsOfTag(actorTag, true);

		if(onScreenActorsOnly) {
			foreach(Actor actor in actorList.ToArray())
				if(AiManager.IsObjectOnScreen(actor.gameObject) == false)
					actorList.Remove(actor);
		}
	}

	public override TaskStatus OnUpdate() {
		int targetCt = 0;
		foreach(Actor actor in actorList) {
			GameObject target = (GameObject)actor.GetBehaviorSharedVariable("Target");
			if(target != null) 
				targetCt++;
		}

		if(targetCt > 0) {
			if(!allMustHaveTargets)
				return TaskStatus.Success;
			
			if(targetCt == actorList.Count)
				return TaskStatus.Success;
		}

		return TaskStatus.Failure;
	}
}

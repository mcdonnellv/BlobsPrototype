using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;

public class AiMoveToNearestTarget : AiMoveTo {
	private List<GameObject> objects;

	private GameObject GetNewTarget() {
		objects = new List<GameObject>();

		// if target is null then find all of the objects using the objectTag
		if (!string.IsNullOrEmpty(actor.data.opposingFaction)) {
			var gameObjects = GameObject.FindGameObjectsWithTag(actor.data.opposingFaction);
			for (int i = 0; i < gameObjects.Length; ++i) {
				ActorHealth ac = gameObjects[i].GetComponent<ActorHealth>();
				if(ac != null && ac.IsAlive())
					objects.Add(gameObjects[i]);
			}
		}

		if(objects.Count > 0) {
			// order closest objects to be first in list
			objects = objects.OrderBy( x => (transform.position - x.transform.position).sqrMagnitude ).ToList();
			return objects[0];
		}
		return null;
	}

	public override TaskStatus OnUpdate() {
		target.Value =  GetNewTarget();
		return base.OnUpdate();
	}
}

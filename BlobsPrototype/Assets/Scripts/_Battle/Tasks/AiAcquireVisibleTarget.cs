using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;

[TaskCategory("Movement")]
[TaskDescription("Check if we can see a valid target and set it as our target")]
[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}WithinDistanceIcon.png")]

public class AiAcquireVisibleTarget : Conditional {
	
	public SharedGameObject currentTarget;
	public SharedGameObject acquiredTarget;
	public SharedFloat viewDistance;
	public SharedBool checkLineOfSight;
	public SharedBool checkAliveOnly;
	public SharedBool showGizmo;
	public SharedVector3 sourceOffset;
	public SharedVector3 destinationOffset;
	public SharedFloat fieldOfViewAngle;
	public SharedBool requireOnScreen;
	public LayerMask layerMask = -1;

	private Actor actor;
	private List<GameObject> objects;

	public override void OnAwake(){
		actor = GetComponent<Actor>();
		objects = new List<GameObject>();
	}

	public override void OnStart() {
		objects.Clear();

		// if target object is spcified then we only care about this current target (sticky targetting)
		if(currentTarget.Value != null)
			objects.Add(currentTarget.Value);
			return;

		// if target is null then find all of the objects using the objectTag
		if (!string.IsNullOrEmpty(actor.data.opposingFaction)) {
			var gameObjects = GameObject.FindGameObjectsWithTag(actor.data.opposingFaction);
			for (int i = 0; i < gameObjects.Length; ++i) {
				if(checkAliveOnly.Value){
					ActorHealth ac = gameObjects[i].GetComponent<ActorHealth>();
					if(ac != null && ac.IsAlive())
						objects.Add(gameObjects[i]);
				}
				else 
					objects.Add(gameObjects[i]);
			}
		}

		// order closest objects to be first in list
		objects = objects.OrderBy( x => (transform.position - x.transform.position).sqrMagnitude ).ToList();
	}
	

	public override TaskStatus OnUpdate() {
		// clear returnedObject, the purpose of this task is to set it to something valid given the rules
		acquiredTarget.Value = null;

		if(objects.Count == 0)
			objects.Add(currentTarget.Value);

		for (int i = 0; i < objects.Count; ++i) {
			GameObject candidate = AiManager.WithinSight2D(actor, objects[i], sourceOffset.Value, destinationOffset.Value, fieldOfViewAngle.Value, viewDistance.Value, layerMask);

			if(candidate == objects[i])
				acquiredTarget.Value = candidate;
			
			if(requireOnScreen.Value && !AiManager.IsObjectOnScreen(candidate))
				acquiredTarget.Value = null;
				
			if(acquiredTarget.Value != null)
				return TaskStatus.Success;
		}

		return TaskStatus.Failure;
	}


	public override void OnDrawGizmos(){
		if(showGizmo.Value && Owner != null) {
			Actor actor = Owner.GetComponent<Actor>();
			if(actor != null)
				AiManager.DrawLineOfSight2D(actor, sourceOffset.Value, fieldOfViewAngle.Value, viewDistance.Value);
		}
	}
}

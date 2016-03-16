using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;

[TaskCategory("Movement")]
[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}WithinDistanceIcon.png")]

public class AiAcquireVisibleTarget : Conditional {
	public SharedBool showGizmo;
	public SharedGameObject targetObject;
	public SharedGameObject returnedObject;
	public SharedFloat viewDistance;
	public SharedVector3 offset;
	public SharedFloat fieldOfViewAngle;
	public SharedBool requireOnScreen;

	private Actor actor;
	private List<GameObject> objects;

	public override void OnStart() {
		actor = GetComponent<Actor>();
		objects = new List<GameObject>();

		// if target object is spcified then we only care about this current target (sticky targetting)
		if(targetObject.Value != null) {
			objects.Add(targetObject.Value);
			return;
		}

		// if target is null then find all of the objects using the objectTag
		if (!string.IsNullOrEmpty(actor.data.opposingFaction)) {
			var gameObjects = GameObject.FindGameObjectsWithTag(actor.data.opposingFaction);
			for (int i = 0; i < gameObjects.Length; ++i) {
				ActorHealth ac = gameObjects[i].GetComponent<ActorHealth>();
				if(ac != null && ac.IsAlive())
					objects.Add(gameObjects[i]);
			}
		}

		// order closest objects to be first in list
		objects = objects.OrderBy( x => (transform.position - x.transform.position).sqrMagnitude ).ToList();
	}

	public override TaskStatus OnUpdate() {
		// clear returnedObject, the purpose of this task is to set it to something valid given the rules
		returnedObject.Value = null;

		for (int i = 0; i < objects.Count; ++i) {
			GameObject candidate = AiManager.WithinSight2D(actor, offset.Value, fieldOfViewAngle.Value, viewDistance.Value, objects[i]);
			if(candidate != null) {
				if(requireOnScreen.Value) {
					Vector3 cameraViwablePosition = Camera.main.WorldToViewportPoint(candidate.transform.position);
					float x = cameraViwablePosition.x;
					if(0f <= x && x <= 1f)
						returnedObject.Value = candidate;
				}
				else 
					returnedObject.Value = candidate;
				
				if(returnedObject.Value != null)
					return TaskStatus.Success;
			}
		}

		return TaskStatus.Failure;
	}

	public override void OnDrawGizmos(){
		if(showGizmo.Value && Owner != null) {
			Actor actor = Owner.GetComponent<Actor>();
			if(actor != null)
				AiManager.DrawLineOfSight2D(actor, offset.Value, fieldOfViewAngle.Value, viewDistance.Value);
		}
	}
}

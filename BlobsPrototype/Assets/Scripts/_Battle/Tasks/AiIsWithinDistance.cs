using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;

[TaskCategory("Movement")]
[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}WithinDistanceIcon.png")]
public class AiIsWithinDistance : Conditional {

	public SharedFloat perception;
	public SharedBool lineOfSight;
	public SharedBool aliveOnly;
	public SharedString objectTag;
	public SharedGameObject returnedObject;

	private List<GameObject> objects;

	// distance * distance, optimization so we don't have to take the square root
	private float sqrMagnitude;

	public override void OnAwake(){
	}

	public override void OnStart()
	{
		sqrMagnitude = perception.Value * perception.Value;

		// if objects is null then find all of the objects using the objectTag
		if (!string.IsNullOrEmpty(objectTag.Value)) {
			var gameObjects = GameObject.FindGameObjectsWithTag(objectTag.Value);
			objects = new List<GameObject>();
			for (int i = 0; i < gameObjects.Length; ++i) {
				if(aliveOnly.Value){
					ActorHealth ac = gameObjects[i].GetComponent<ActorHealth>();
					if(ac != null && ac.IsAlive())
						objects.Add(gameObjects[i]);
				}
				else 
					objects.Add(gameObjects[i]);
			}
		}
	}

	// returns success if any object is within distance of the current object. Otherwise it will return failure
	public override TaskStatus OnUpdate()
	{
		if (transform == null || objects == null)
			return TaskStatus.Failure;

		Vector3 direction;
		// check each object. All it takes is one object to be able to return success
		for (int i = 0; i < objects.Count; ++i) {
			direction = objects[i].transform.position - transform.position;
			// check to see if the square magnitude is less than what is specified
			if (Vector3.SqrMagnitude(direction) < sqrMagnitude) {
				// the magnitude is less. If lineOfSight is true do one more check
				if (lineOfSight.Value) {
					bool facingObject = transform.right.x * direction.x > 0;
					if(facingObject) {
						returnedObject.Value = objects[i];
						return TaskStatus.Success;
					}

				} else { //dont care about line of sight
					returnedObject.Value = objects[i];
					return TaskStatus.Success;
				}
			}
		}
		// no objects are within distance. Return failure
		return TaskStatus.Failure;
	}

	// Draw the seeing radius
	public override void OnDrawGizmos()
	{
		#if UNITY_EDITOR
		if (Owner == null || perception == null) {
			return;
		}
		var oldColor = UnityEditor.Handles.color;
		UnityEditor.Handles.color = Color.yellow;
		UnityEditor.Handles.DrawWireDisc(Owner.transform.position, Owner.transform.forward, perception.Value);
		UnityEditor.Handles.color = oldColor;
		#endif
	}
}

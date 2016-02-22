﻿using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;

[TaskDescription("Patrol around the specified waypoints using force on a Rigidbody")]
[TaskCategory("Movement")]
[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}PatrolIcon.png")]
public class Patrol2d : Patrol {

	public SharedFloat moveForce;

	private Vector2 destTarget;
	private Vector2 originalDirection;
	private Vector2 curDirection;
	private Actor actor;

	public override void OnAwake() {
		actor = GetComponent<Actor>();
	}
	
	public override void OnStart() {
		// initially move towards the closest waypoint
		float distance = Mathf.Infinity;
		float localDistance;
		for (int i = 0; i < waypoints.Value.Count; ++i) {
			if ((localDistance = Vector3.Magnitude(transform.position - waypoints.Value[i].transform.position)) < distance) {
				distance = localDistance;
				base.waypointIndex = i;
			}
		}
		waypointReachedTime = -waypointPauseDuration.Value;
		SetDestination(Target());
	}


	public override TaskStatus OnUpdate() {
		if( base.waypoints.Value.Count <= 0 )
			return TaskStatus.Success;

		if(!HasArrived()) 
			AiManager.AiMoveToDestination(actor, destTarget, moveForce.Value, speed.Value, true, "Walk");
		return base.OnUpdate();
	}



	// Return the current waypoint index position
	private Vector3 Target() {
		if (waypointIndex >= waypoints.Value.Count) {
			return transform.position;
		}
		return waypoints.Value[waypointIndex].transform.position;
	}

	protected override bool SetDestination(Vector3 target) {
		destTarget = (Vector2)target;
		return true;
	}

	protected override bool HasArrived() {
		bool retval =  Mathf.Abs(transform.position.x - destTarget.x) < .5f;
		return retval;
	}

	protected override Vector3 Velocity() {
		return actor.rigidBody.velocity;
	}

	public override void OnEnd() {
		//rigidBody.velocity = Vector3.zero;
		//destTarget = transform.position;
	}
}
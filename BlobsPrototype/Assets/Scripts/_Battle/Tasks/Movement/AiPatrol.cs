using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;

[TaskDescription("Patrol around the specified waypoints using force on a Rigidbody")]
[TaskCategory("Movement")]
[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}PatrolIcon.png")]

public class AiPatrol : AiMoveTo {
	public SharedBool randomPatrol = false;
	public SharedFloat waypointPauseDuration = 0;
	public SharedGameObjectList waypoints;

	protected int waypointIndex;
	protected float waypointReachedTime;


	public override void OnAwake() {
		base.OnAwake();
		SetClosestWaypoint();
	}

	public override TaskStatus OnUpdate() {
		if( waypoints.Value.Count <= 0 )
			return TaskStatus.Success;
		
		var position = Target();

		if (HasArrived()) {
			if (waypointReachedTime == -waypointPauseDuration.Value)
				waypointReachedTime = Time.time;
			
			// wait the required duration before switching waypoints.
			if (waypointReachedTime + waypointPauseDuration.Value <= Time.time) 
				SetNextWaypoint();
		}
		else
			AiManager.MoveToPoint(actor, position, toVel.Value, maxSpeed.Value, maxForce.Value, minForce.Value, gain.Value);

		return TaskStatus.Running;
	}

	// Return the current waypoint index position
	protected override Vector3 Target() {
		if (waypointIndex >= waypoints.Value.Count) {
			return transform.position;
		}
		return waypoints.Value[waypointIndex].transform.position;
	}

	private void SetClosestWaypoint() {
		float distance = Mathf.Infinity;
		float localDistance;
		for (int i = 0; i < waypoints.Value.Count; ++i) {
			if ((localDistance = Vector3.Magnitude(transform.position - waypoints.Value[i].transform.position)) < distance) {
				distance = localDistance;
				waypointIndex = i;
			}
		}
		waypointReachedTime = -waypointPauseDuration.Value;
	}

	private void SetNextWaypoint() {
		if (randomPatrol.Value) {
			if (waypoints.Value.Count == 1)
				waypointIndex = 0;
			else {
				// prevent the same waypoint from being selected
				var newWaypointIndex = waypointIndex;
				while (waypoints.Value.Count > 0 && newWaypointIndex == waypointIndex)
					newWaypointIndex = Random.Range(0, waypoints.Value.Count - 1);
				waypointIndex = newWaypointIndex;
			}
		} else 
			waypointIndex = (waypointIndex + 1) % waypoints.Value.Count;
		waypointReachedTime = -waypointPauseDuration.Value;
	}
}

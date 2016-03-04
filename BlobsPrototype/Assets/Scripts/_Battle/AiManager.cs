using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AiManager : MonoBehaviour {
	private static AiManager _aiManager;
	public static AiManager aiManager { get {if(_aiManager == null) _aiManager = GameObject.Find("AiManager").GetComponent<AiManager>(); return _aiManager; } }

	public List<GameObject> patrolPoints;


	public static void MoveToPoint(Actor actor, Vector3 targetPos, float toVel = 5f, float maxVel = 8f, float maxForce = 30f, float minForce = 5f, float gain = 10f) {
		//toVel - converts the distance remaining to the target velocity - if too low, the rigidbody slows down early and takes a long time to stop; if too high, it may overshoot
		//maxVel - the max speed the rigidbody will reach when moving; 
		//maxForce - limits the force applied to the rigidbody in order to avoid excessive acceleration (and instability);
		//gain - sets the feedback amount: if too low, the rigidbody stops before the target point; if too high, it may overshoot and oscillate 
		Transform transform = actor.transform;
		Rigidbody rigidbody = actor.rigidBody;
		Vector3 dist = targetPos - transform.position;
		dist.y = 0f;
		dist.z = 0f;
		Vector3 tgtVel = Vector3.ClampMagnitude(toVel * dist, maxVel);
		Vector3 error = tgtVel - rigidbody.velocity;
		Vector3 force = Vector3.ClampMagnitude(gain * error, maxForce);
		if(force.magnitude < minForce)
			force = force.normalized * minForce;
		rigidbody.AddForce(force, ForceMode.Acceleration); //acceleration ignores mass
	}

	public static bool IsTouching(Actor actor, GameObject gameobject) {
		return false;
		//Collider[] col = actor.GetComponentsInChildren<Collider>();
		//for(int i = 0; i < col.Length; i++) {
		//	if(col[i].)
		//}
	}


	public static void LookAtTarget(Actor actor, Vector2 target) {
		if(!IsLookingAt(actor, target))
			actor.FaceOpposite();
	}


	public static bool IsLookingAt(Actor actor, Vector2 target) {
		Vector2 dir = target - (Vector2)actor.transform.position;
		bool facingRight = actor.IsFacingRight();
		if(dir.x < 0 && facingRight)
			return false;
		if(dir.x > 0 && !facingRight)
			return false;
		return true;
	}


	public static void DrawLineOfSight2D(Actor actor, Vector3 positionOffset, float fieldOfViewAngle, float viewDistance)	{
#if UNITY_EDITOR
		if(actor == null)
			return;
		Transform transform = actor.transform;
		var oldColor = UnityEditor.Handles.color;
		var color = Color.yellow;
		color.a = 0.05f;
		UnityEditor.Handles.color = color;

		var halfFOV = fieldOfViewAngle * 0.5f;
		var beginDirection = Quaternion.AngleAxis(-halfFOV, transform.forward) * (actor.IsFacingRight() ? transform.right : -transform.right);
		UnityEditor.Handles.DrawSolidArc(transform.TransformPoint(positionOffset), transform.forward, beginDirection, fieldOfViewAngle, viewDistance);

		UnityEditor.Handles.color = oldColor;
#endif
	}


	// Determines if the targetObject is within sight of the transform. It will set the angle regardless of whether or not the object is within sight
	public static GameObject WithinSight2D(Actor actor, Vector3 positionOffset, float fieldOfViewAngle, float viewDistance, GameObject targetObject) {
		if(targetObject == null)
			return null;
		Transform transform = actor.transform;
		// The target object needs to be within the field of view of the current object
		var direction = targetObject.transform.position - transform.TransformPoint(positionOffset);
		float angle = Vector3.Angle(direction, (actor.IsFacingRight() ? transform.right : -transform.right));
		direction.z = 0;

		if (direction.magnitude < viewDistance && angle < fieldOfViewAngle * 0.5f) {
			if (targetObject.gameObject.activeSelf)
				return targetObject;
			
		}
		return null;
	}
}


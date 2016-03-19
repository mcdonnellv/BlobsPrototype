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


	public static void LookAtTarget(Actor actor, Vector2 target) {
		if(!IsActorFacing(actor, target))
			actor.FaceOpposite();
	}


	public static bool IsActorFacing(Actor actor, Vector2 target) {
		Vector2 dir = target - (Vector2)actor.transform.position;
		bool facingRight = actor.IsFacingRight();
		if(dir.x < 0 && facingRight)
			return false;
		if(dir.x > 0 && !facingRight)
			return false;
		return true;
	}

	public static bool CastRay(Vector3 source, Vector3 destination, float checkDistance, int layerMask, out RaycastHit hit) {
		Ray ray = new Ray(source, destination - source);
		if(Physics.Raycast(ray.origin, ray.direction, out hit, checkDistance, layerMask)) {
			Debug.DrawLine(ray.origin, hit.point, Color.red);
			return true;
		}
			
		Debug.DrawLine(ray.origin, ray.origin + (ray.direction * checkDistance), Color.green);
		return false;
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


	public static bool IsObjectOnScreen(GameObject go) {
		Vector3 cameraViwablePosition = Camera.main.WorldToViewportPoint(go.transform.position);
		float x = cameraViwablePosition.x;
		if(0f <= x && x <= 1f)
			return true;
		return false;
	}


	// Determines if the targetObject is within sight of the transform. It will set the angle regardless of whether or not the object is within sight
	public static GameObject WithinSight2D(Actor actor, GameObject targetObject, Vector3 sourceOffset, Vector3 destinationOffset, float fieldOfViewAngle, float viewDistance, int layerMask) {
		if(targetObject == null)
			return null;
		
		if(IsActorFacing(actor, targetObject.transform.position) == false)
			return null;
		
		Transform transform = actor.transform;
		// The target object needs to be within the field of view of the current object
		var direction = targetObject.transform.TransformPoint(destinationOffset) - transform.TransformPoint(sourceOffset);
		float angle = Vector3.Angle(direction, (actor.IsFacingRight() ? transform.right : -transform.right));
		direction.z = 0;

		// is it within the field of vision?
		if (direction.magnitude < viewDistance && angle < fieldOfViewAngle * 0.5f) {
			if (targetObject.gameObject.activeSelf) {
				// is there anything blocking our view?
				RaycastHit hit;
				if(CastRay(transform.TransformPoint(sourceOffset), targetObject.transform.TransformPoint(destinationOffset), viewDistance, layerMask, out hit)) {
					// the first thing we saw was the object. success!
					if(hit.collider == targetObject)
						return targetObject;
				}
			}
			
		}
		return null;
	}
}


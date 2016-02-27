using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum Faction {
	blob,
	enemy,
};


public class AiManager : MonoBehaviour {
	private static AiManager _aiManager;
	public static AiManager aiManager { get {if(_aiManager == null) _aiManager = GameObject.Find("AiManager").GetComponent<AiManager>(); return _aiManager; } }

	public List<GameObject> patrolPoints;

	public T FindClosest<T>(Vector3 myPosition) where T : MonoBehaviour {
		float minDistance = float.MaxValue;
		T ret = null;
		foreach (T obj in FindObjectsOfType(typeof(T))) {
			float dist = (obj.gameObject.transform.position - myPosition).magnitude;
			if(dist < minDistance) {
				minDistance = dist;
				ret = obj;
			}
		}
		return ret;
	}


	public static void MoveDirection(Rigidbody rb, float h, float moveForce, float maxSpeed) {
		if(h == 0 || rb == null)
			return;
		
		// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
		if(h * rb.velocity.x < maxSpeed)
			// ... add a force to the player.
			rb.AddForce(Vector2.right * h * moveForce);

		// If the player's horizontal velocity is greater than the maxSpeed...
		if(Mathf.Abs(rb.velocity.x) > maxSpeed)
			// ... set the player's velocity to the maxSpeed in the x axis.
			rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
	}


	public static void MoveDirection(Actor actor, float h, float moveForce, float maxSpeed) {
		MoveDirection(actor.rigidBody, h, moveForce, maxSpeed);
	}
	

	public static void AiMoveToDestination(Actor actor, Vector2 destTarget, float moveForce, float maxSpeed, bool lookAtTarget) {
		if(!actor.IsGrounded() || destTarget == Vector2.zero || actor.rigidBody == null)
			return;

		Vector2 direction = destTarget - (Vector2)actor.transform.position;
		direction.y = 0;
		float distanceLeft = direction.magnitude;
		Vector2 dNorm = direction.normalized;
		float curSpeed = actor.GetCurSpeed();
		
		// distance =  velocity * time (stop moving if it looks like we will arrive with current speed)
		if(curSpeed < distanceLeft)
			MoveDirection(actor, dNorm.x, moveForce, maxSpeed);
		
		if(lookAtTarget)
			LookAtTarget(actor, destTarget);
	}


	public static void LookAtTarget(Actor actor, Vector2 target) {
		if(IsLookingAt(actor, target))
			return;
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

}

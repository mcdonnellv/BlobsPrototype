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


	public static void MoveDirection(float h, Rigidbody2D rb2d, float moveForce, float maxSpeed, bool grounded) {
		if(!grounded || h == 0)
			return;

		// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
		if(h * rb2d.velocity.x < maxSpeed)
			// ... add a force to the player.
			rb2d.AddForce(Vector2.right * h * moveForce);
		
		// If the player's horizontal velocity is greater than the maxSpeed...
		if(Mathf.Abs(rb2d.velocity.x) > maxSpeed)
			// ... set the player's velocity to the maxSpeed in the x axis.
			rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * maxSpeed, rb2d.velocity.y);
	}
	

	public static void MoveToDestination(Transform myTransform, Rigidbody2D myRigidBody, 
	                                     Vector2 destTarget, float moveForce, float maxSpeed, 
	                                     bool lootAtTarget, string animBoolStr, bool grounded) {
		if(!grounded || destTarget == Vector2.zero)
			return;
		
		Vector2 direction = destTarget - (Vector2)myTransform.position;
		direction.y = 0;
		float distanceLeft = direction.magnitude;
		Vector2 dNorm = direction.normalized;
		float curSpeed = myRigidBody.velocity.magnitude;
		
		// distance =  velocity * time (stop moving if it looks like we will arrive with current speed)
		if(curSpeed < distanceLeft)
			MoveDirection(dNorm.x, myRigidBody, moveForce, maxSpeed, grounded);
		
		if(lootAtTarget)
			LookAtTarget(myTransform, destTarget);
		
		// trigger traverse animation
		Animator anim = myTransform.gameObject.GetComponent<Animator>();
		if(anim != null && !string.IsNullOrEmpty(animBoolStr))
			anim.SetBool(animBoolStr, true);
	}

	public static void LookAtTarget(Transform myTransform, Vector2 target) {
		bool facingRight = (myTransform.localRotation.y == 0);
		Vector2 dir = target - (Vector2)myTransform.position;
		if(dir.x < 0 && facingRight)
			myTransform.localRotation = Quaternion.Euler(0, 180, 0);
		
		if(dir.x > 0 && !facingRight)
			myTransform.localRotation = Quaternion.Euler(0, 0, 0);
	}

}

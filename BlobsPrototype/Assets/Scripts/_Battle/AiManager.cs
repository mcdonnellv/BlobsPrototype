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


	public static void MoveToDestination(Transform myTransform, Rigidbody2D myRigidBody, Vector2 destTarget, float moveForce, float maxSpeed, bool lootAtTarget, string animBoolStr) {
		if(destTarget == Vector2.zero)
			return;

		bool grounded = Mathf.Abs(myRigidBody.velocity.y) <= 0.010f;
		
		if(grounded) {
			Vector2 direction = destTarget - (Vector2)myTransform.position;
			direction.y = 0;
			float distanceLeft = direction.magnitude;
			Vector2 dNorm = direction.normalized;
			float curSpeed = myRigidBody.velocity.magnitude;
			
			// distance =  velocity * time
			if(curSpeed < distanceLeft)
				myRigidBody.AddForce(dNorm * moveForce);
			
			if(curSpeed > maxSpeed)
				myRigidBody.velocity = dNorm * maxSpeed;

			if(lootAtTarget)
				LookAtTarget(myTransform, destTarget);

			// trigger traverse animation
			Animator anim = myTransform.gameObject.GetComponent<Animator>();
			if(anim != null && !string.IsNullOrEmpty(animBoolStr))
				anim.SetBool(animBoolStr, true);
		}
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

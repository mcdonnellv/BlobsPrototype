using UnityEngine;
using System.Collections;
using System;

public class Actor : MonoBehaviour {

	// how fast do we walk
	public float walkSpeed = 10f;

	public Faction faction;

	public Rigidbody2D rigidBody;

	[HideInInspector] public float health = 0f;

	[HideInInspector] public CombatStats combatStats;

	// when set will try to go to its position
	public GameObject moveToTarget;

	// reference to an opponent
	protected GameObject aggroTarget;
	
	// working health througout this fight



	public Vector2 myPosition { 
		get { return (Vector2)transform.position; }  
		set { transform.position = (Vector3)value; } 
	}
	

	void OnCollisionEnter2D (Collision2D col) {
		if(col.gameObject == moveToTarget) {
			moveToTarget = gameObject;
			rigidBody.velocity = Vector2.zero;
		}
	}

	public void Move(Vector2 destination) {
		if(myPosition == destination)
			return;

		Vector2 a = destination - myPosition;
		Vector2 direction = a.normalized;
		float maxDistanceDelta = walkSpeed;// * Time.deltaTime;
		rigidBody.velocity = direction * maxDistanceDelta;
	}


	public void Initialize(CombatStats cs) { 
		combatStats = cs;
		health = combatStats.health.combatValue;
	}


	// Use this for initialization
	public virtual void Start () {
		moveToTarget = gameObject;
	}

	
	// Update is called once per frame
	public virtual void Update () {
		if(moveToTarget != null)
			Move((Vector2)moveToTarget.transform.position);
	}
}
